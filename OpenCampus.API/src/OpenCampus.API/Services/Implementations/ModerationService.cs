using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.DTOs.Reviews;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Services.Implementations;

public class ModerationService : IModerationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ModerationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IReadOnlyCollection<ReviewModerationDto>> GetPendingReviewsAsync(Guid moderatorId, CancellationToken cancellationToken = default)
    {
        await EnsureModeratorAsync(moderatorId, cancellationToken).ConfigureAwait(false);

        var specification = new PendingReviewsSpecification();
        var reviews = await _unitOfWork.Reviews.ListAsync(specification, cancellationToken).ConfigureAwait(false);
        return _mapper.Map<IReadOnlyCollection<ReviewModerationDto>>(reviews);
    }

    public async Task<ReviewModerationDto> ApproveReviewAsync(Guid reviewId, Guid moderatorId, CancellationToken cancellationToken = default)
    {
        await EnsureModeratorAsync(moderatorId, cancellationToken).ConfigureAwait(false);

        var specification = new ReviewWithCommentsSpecification(reviewId);
        var review = await _unitOfWork.Reviews.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (review == null)
        {
            throw new NotFoundException("Review was not found.", ErrorCodes.ReviewNotFound);
        }

        if (review.Approved)
        {
            return _mapper.Map<ReviewModerationDto>(review);
        }

        review.Approved = true;
        review.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return _mapper.Map<ReviewModerationDto>(review);
    }

    public async Task RejectReviewAsync(Guid reviewId, Guid moderatorId, CancellationToken cancellationToken = default)
    {
        await EnsureModeratorAsync(moderatorId, cancellationToken).ConfigureAwait(false);

        var specification = new ReviewWithCommentsSpecification(reviewId);
        var review = await _unitOfWork.Reviews.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (review == null)
        {
            throw new NotFoundException("Review was not found.", ErrorCodes.ReviewNotFound);
        }

        foreach (var comment in review.Comments.ToList())
        {
            _unitOfWork.Comments.Delete(comment);
        }

        _unitOfWork.Reviews.Delete(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ChangeRequestModerationDto>> GetPendingChangeRequestsAsync(Guid moderatorId, CancellationToken cancellationToken = default)
    {
        await EnsureModeratorAsync(moderatorId, cancellationToken).ConfigureAwait(false);

        var specification = new PendingChangeRequestsSpecification();
        var changeRequests = await _unitOfWork.ChangeRequests.ListAsync(specification, cancellationToken).ConfigureAwait(false);
        return _mapper.Map<IReadOnlyCollection<ChangeRequestModerationDto>>(changeRequests);
    }

    public async Task<ChangeRequestModerationDto> UpdateChangeRequestStatusAsync(Guid changeRequestId, Guid moderatorId, UpdateChangeRequestStatusDto request, CancellationToken cancellationToken = default)
    {
        var moderator = await EnsureModeratorAsync(moderatorId, cancellationToken).ConfigureAwait(false);

        if (request.Status == ChangeRequestStatus.Pending)
        {
            throw new InvalidOperationException("Change requests cannot be returned to the pending state once reviewed.");
        }

        var specification = new ChangeRequestWithUsersSpecification(changeRequestId);
        var changeRequest = await _unitOfWork.ChangeRequests.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (changeRequest == null)
        {
            throw new NotFoundException("Change request was not found.", ErrorCodes.ChangeRequestNotFound);
        }

        if (changeRequest.Status != ChangeRequestStatus.Pending)
        {
            throw new ConflictException("The change request has already been resolved.", ErrorCodes.ChangeRequestAlreadyResolved);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (request.Status == ChangeRequestStatus.Approved)
            {
                await ApplyChangeRequestAsync(changeRequest, cancellationToken).ConfigureAwait(false);
            }

            changeRequest.Status = request.Status;
            changeRequest.ResolvedById = moderator.Id;
            changeRequest.ResolvedBy = moderator;
            changeRequest.ResolvedAt = DateTime.UtcNow;
            changeRequest.ResolutionNotes = string.IsNullOrWhiteSpace(request.ResolutionNotes)
                ? null
                : request.ResolutionNotes.Trim();

            _unitOfWork.ChangeRequests.Update(changeRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _unitOfWork.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }

        return _mapper.Map<ChangeRequestModerationDto>(changeRequest);
    }

    private async Task<User> EnsureModeratorAsync(Guid moderatorId, CancellationToken cancellationToken)
    {
        var moderator = await _unitOfWork.Users.GetByIdAsync(moderatorId, cancellationToken).ConfigureAwait(false);
        if (moderator == null)
        {
            throw new NotFoundException("User was not found.", ErrorCodes.UserNotFound);
        }

        if (!moderator.IsActive || moderator.Role != UserRole.Moderator)
        {
            throw new ForbiddenException("Only moderators may perform this action.", ErrorCodes.UnauthorizedAction);
        }

        return moderator;
    }

    private async Task ApplyChangeRequestAsync(ChangeRequest changeRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(changeRequest.SuggestedData))
        {
            throw new InvalidOperationException("The change request does not contain any suggested data to apply.");
        }

        try
        {
            using var document = JsonDocument.Parse(changeRequest.SuggestedData);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("Suggested data must be a JSON object containing the fields to update.");
            }

            var values = document.RootElement
                .EnumerateObject()
                .ToDictionary(property => property.Name, property => property.Value, StringComparer.OrdinalIgnoreCase);

            switch (changeRequest.TargetType)
            {
                case ChangeRequestTargetType.Institution:
                    await ApplyInstitutionChangesAsync(changeRequest, values, cancellationToken).ConfigureAwait(false);
                    break;
                case ChangeRequestTargetType.Course:
                    await ApplyCourseChangesAsync(changeRequest, values, cancellationToken).ConfigureAwait(false);
                    break;
                case ChangeRequestTargetType.Professor:
                    await ApplyProfessorChangesAsync(changeRequest, values, cancellationToken).ConfigureAwait(false);
                    break;
                case ChangeRequestTargetType.Subject:
                    await ApplySubjectChangesAsync(changeRequest, values, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported change request target type.");
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("The suggested data for this change request is not valid JSON.", ex);
        }
    }

    private async Task ApplyInstitutionChangesAsync(ChangeRequest changeRequest, IReadOnlyDictionary<string, JsonElement> values, CancellationToken cancellationToken)
    {
        var institutionId = changeRequest.InstitutionId ?? throw new InvalidOperationException("Institution change requests must reference an institution identifier.");
        var institution = await _unitOfWork.Institutions.GetByIdAsync(institutionId, cancellationToken).ConfigureAwait(false);
        if (institution == null)
        {
            throw new NotFoundException("Institution was not found.", ErrorCodes.TargetEntityNotFound);
        }

        if (TryGetNonEmptyString(values, "name", out var name))
        {
            institution.Name = name;
        }

        if (TryGetOptionalString(values, "acronym", out var acronym))
        {
            institution.Acronym = acronym;
        }

        if (TryGetOptionalString(values, "description", out var description))
        {
            institution.Description = description;
        }

        if (TryGetOptionalString(values, "websiteUrl", out var website))
        {
            institution.WebsiteUrl = website;
        }

        institution.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Institutions.Update(institution);
    }

    private async Task ApplyCourseChangesAsync(ChangeRequest changeRequest, IReadOnlyDictionary<string, JsonElement> values, CancellationToken cancellationToken)
    {
        var courseId = changeRequest.CourseId ?? throw new InvalidOperationException("Course change requests must reference a course identifier.");
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId, cancellationToken).ConfigureAwait(false);
        if (course == null)
        {
            throw new NotFoundException("Course was not found.", ErrorCodes.TargetEntityNotFound);
        }

        if (TryGetNonEmptyString(values, "name", out var name))
        {
            course.Name = name;
        }

        if (TryGetOptionalString(values, "description", out var description))
        {
            course.Description = description;
        }

        course.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Courses.Update(course);
    }

    private async Task ApplyProfessorChangesAsync(ChangeRequest changeRequest, IReadOnlyDictionary<string, JsonElement> values, CancellationToken cancellationToken)
    {
        var professorId = changeRequest.ProfessorId ?? throw new InvalidOperationException("Professor change requests must reference a professor identifier.");
        var professor = await _unitOfWork.Professors.GetByIdAsync(professorId, cancellationToken).ConfigureAwait(false);
        if (professor == null)
        {
            throw new NotFoundException("Professor was not found.", ErrorCodes.TargetEntityNotFound);
        }

        if (TryGetNonEmptyString(values, "fullName", out var fullName))
        {
            professor.FullName = fullName;
        }

        if (TryGetOptionalString(values, "bio", out var bio))
        {
            professor.Bio = bio;
        }

        professor.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Professors.Update(professor);
    }

    private async Task ApplySubjectChangesAsync(ChangeRequest changeRequest, IReadOnlyDictionary<string, JsonElement> values, CancellationToken cancellationToken)
    {
        var subjectId = changeRequest.SubjectId ?? throw new InvalidOperationException("Subject change requests must reference a subject identifier.");
        var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId, cancellationToken).ConfigureAwait(false);
        if (subject == null)
        {
            throw new NotFoundException("Subject was not found.", ErrorCodes.TargetEntityNotFound);
        }

        if (TryGetNonEmptyString(values, "name", out var name))
        {
            subject.Name = name;
        }

        if (TryGetOptionalString(values, "description", out var description))
        {
            subject.Description = description;
        }

        subject.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Subjects.Update(subject);
    }

    private static bool TryGetNonEmptyString(IReadOnlyDictionary<string, JsonElement> values, string key, out string result)
    {
        if (TryGetOptionalString(values, key, out var optional) && !string.IsNullOrWhiteSpace(optional))
        {
            result = optional.Trim();
            return true;
        }

        result = string.Empty;
        return false;
    }

    private static bool TryGetOptionalString(IReadOnlyDictionary<string, JsonElement> values, string key, out string? result)
    {
        if (!values.TryGetValue(key, out var element))
        {
            result = null;
            return false;
        }

        if (element.ValueKind == JsonValueKind.Null)
        {
            result = null;
            return true;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            var value = element.GetString();
            result = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            return true;
        }

        throw new InvalidOperationException("Suggested data contains an unsupported value type for a text field.");
    }

    private sealed class PendingReviewsSpecification : Specification<Review>
    {
        public PendingReviewsSpecification()
            : base(review => !review.Approved)
        {
            AddInclude(review => review.Comments);
            ApplyOrderBy(review => review.CreatedAt);
            DisableTracking();
        }
    }

    private sealed class ReviewWithCommentsSpecification : Specification<Review>
    {
        public ReviewWithCommentsSpecification(Guid reviewId)
            : base(review => review.Id == reviewId)
        {
            AddInclude(review => review.Comments);
            DisableTracking();
        }
    }

    private sealed class PendingChangeRequestsSpecification : Specification<ChangeRequest>
    {
        public PendingChangeRequestsSpecification()
            : base(changeRequest => changeRequest.Status == ChangeRequestStatus.Pending)
        {
            AddInclude(changeRequest => changeRequest.CreatedBy);
            ApplyOrderBy(changeRequest => changeRequest.CreatedAt);
            DisableTracking();
        }
    }

    private sealed class ChangeRequestWithUsersSpecification : Specification<ChangeRequest>
    {
        public ChangeRequestWithUsersSpecification(Guid changeRequestId)
            : base(changeRequest => changeRequest.Id == changeRequestId)
        {
            AddInclude(changeRequest => changeRequest.CreatedBy);
            AddInclude(changeRequest => changeRequest.ResolvedBy);
            DisableTracking();
        }
    }
}
