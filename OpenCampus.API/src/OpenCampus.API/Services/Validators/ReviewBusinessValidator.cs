using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.Reviews;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Validators;

public class ReviewBusinessValidator
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewBusinessValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task ValidateCreationAsync(User author, ReviewCreateDto request, CancellationToken cancellationToken = default)
    {
        if (author == null)
        {
            throw new ArgumentNullException(nameof(author));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!author.IsActive)
        {
            throw new ForbiddenException("Inactive users cannot submit reviews.", ErrorCodes.UnauthorizedAction);
        }

        if (author.Role != UserRole.Student)
        {
            throw new ForbiddenException("Only students can submit reviews.", ErrorCodes.UnauthorizedAction);
        }

        ValidateScore(request.ScoreClarity);
        ValidateScore(request.ScoreRelevance);
        ValidateScore(request.ScoreSupport);
        ValidateScore(request.ScoreInfrastructure);

        switch (request.TargetType)
        {
            case ReviewTargetType.Institution:
                await ValidateInstitutionTargetAsync(author, request, cancellationToken).ConfigureAwait(false);
                break;
            case ReviewTargetType.Course:
                await ValidateCourseTargetAsync(author, request, cancellationToken).ConfigureAwait(false);
                break;
            case ReviewTargetType.Professor:
                await ValidateProfessorTargetAsync(author, request, cancellationToken).ConfigureAwait(false);
                break;
            case ReviewTargetType.Subject:
                await ValidateSubjectTargetAsync(author, request, cancellationToken).ConfigureAwait(false);
                break;
            default:
                throw new InvalidOperationException("Unsupported review target type.");
        }
    }

    private static void ValidateScore(int score)
    {
        if (score < 1 || score > 5)
        {
            throw new InvalidOperationException("Scores must be between 1 and 5.");
        }
    }

    private async Task ValidateInstitutionTargetAsync(User author, ReviewCreateDto request, CancellationToken cancellationToken)
    {
        var institutionId = EnsureIdentifier(request.InstitutionId, "institution");
        var institution = await _unitOfWork.Institutions.GetByIdAsync(institutionId, cancellationToken).ConfigureAwait(false);
        if (institution == null)
        {
            throw new NotFoundException("Institution was not found.", ErrorCodes.TargetEntityNotFound);
        }

        var duplicateExists = await _unitOfWork.Reviews.AnyAsync(
                review => review.AuthorId == author.Id && review.TargetType == ReviewTargetType.Institution && review.InstitutionId == institutionId,
                cancellationToken)
            .ConfigureAwait(false);

        if (duplicateExists)
        {
            throw new ConflictException("A review has already been submitted for this target by the current user.", ErrorCodes.ReviewAlreadyExists);
        }
    }

    private async Task ValidateCourseTargetAsync(User author, ReviewCreateDto request, CancellationToken cancellationToken)
    {
        var courseId = EnsureIdentifier(request.CourseId, "course");
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId, cancellationToken).ConfigureAwait(false);
        if (course == null)
        {
            throw new NotFoundException("Course was not found.", ErrorCodes.TargetEntityNotFound);
        }

        var duplicateExists = await _unitOfWork.Reviews.AnyAsync(
                review => review.AuthorId == author.Id && review.TargetType == ReviewTargetType.Course && review.CourseId == courseId,
                cancellationToken)
            .ConfigureAwait(false);

        if (duplicateExists)
        {
            throw new ConflictException("A review has already been submitted for this target by the current user.", ErrorCodes.ReviewAlreadyExists);
        }
    }

    private async Task ValidateProfessorTargetAsync(User author, ReviewCreateDto request, CancellationToken cancellationToken)
    {
        var professorId = EnsureIdentifier(request.ProfessorId, "professor");
        var professor = await _unitOfWork.Professors.GetByIdAsync(professorId, cancellationToken).ConfigureAwait(false);
        if (professor == null)
        {
            throw new NotFoundException("Professor was not found.", ErrorCodes.TargetEntityNotFound);
        }

        var duplicateExists = await _unitOfWork.Reviews.AnyAsync(
                review => review.AuthorId == author.Id && review.TargetType == ReviewTargetType.Professor && review.ProfessorId == professorId,
                cancellationToken)
            .ConfigureAwait(false);

        if (duplicateExists)
        {
            throw new ConflictException("A review has already been submitted for this target by the current user.", ErrorCodes.ReviewAlreadyExists);
        }
    }

    private async Task ValidateSubjectTargetAsync(User author, ReviewCreateDto request, CancellationToken cancellationToken)
    {
        var subjectId = EnsureIdentifier(request.SubjectId, "subject");
        var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId, cancellationToken).ConfigureAwait(false);
        if (subject == null)
        {
            throw new NotFoundException("Subject was not found.", ErrorCodes.TargetEntityNotFound);
        }

        var duplicateExists = await _unitOfWork.Reviews.AnyAsync(
                review => review.AuthorId == author.Id && review.TargetType == ReviewTargetType.Subject && review.SubjectId == subjectId,
                cancellationToken)
            .ConfigureAwait(false);

        if (duplicateExists)
        {
            throw new ConflictException("A review has already been submitted for this target by the current user.", ErrorCodes.ReviewAlreadyExists);
        }
    }

    private static Guid EnsureIdentifier(Guid? identifier, string targetName)
    {
        if (!identifier.HasValue || identifier == Guid.Empty)
        {
            throw new InvalidOperationException($"A valid {targetName} identifier must be provided.");
        }

        return identifier.Value;
    }
}

