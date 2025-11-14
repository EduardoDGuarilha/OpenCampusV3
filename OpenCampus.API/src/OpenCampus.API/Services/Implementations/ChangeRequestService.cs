using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Services.Implementations;

public class ChangeRequestService : IChangeRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChangeRequestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ChangeRequestDetailDto> CreateAsync(Guid requesterId, CreateChangeRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.SuggestedData))
        {
            throw new InvalidOperationException("Suggested data must be provided for a change request.");
        }

        var requester = await _unitOfWork.Users.GetByIdAsync(requesterId, cancellationToken).ConfigureAwait(false);
        if (requester == null)
        {
            throw new NotFoundException("User was not found.", ErrorCodes.UserNotFound);
        }

        if (!requester.IsActive)
        {
            throw new ForbiddenException("Inactive users cannot create change requests.", ErrorCodes.UnauthorizedAction);
        }

        var changeRequest = _mapper.Map<ChangeRequest>(request);
        changeRequest.Id = Guid.NewGuid();
        changeRequest.CreatedById = requester.Id;
        changeRequest.CreatedAt = DateTime.UtcNow;
        changeRequest.UpdatedAt = null;

        await EnsureTargetExistsAsync(changeRequest, cancellationToken).ConfigureAwait(false);

        await _unitOfWork.ChangeRequests.AddAsync(changeRequest, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return _mapper.Map<ChangeRequestDetailDto>(changeRequest);
    }

    public async Task<ChangeRequestDetailDto> GetByIdAsync(Guid changeRequestId, Guid requesterId, UserRole requesterRole, CancellationToken cancellationToken = default)
    {
        var specification = new ChangeRequestWithUsersSpecification(changeRequestId);
        var changeRequest = await _unitOfWork.ChangeRequests.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (changeRequest == null)
        {
            throw new NotFoundException("Change request was not found.", ErrorCodes.ChangeRequestNotFound);
        }

        if (requesterRole != UserRole.Moderator && changeRequest.CreatedById != requesterId)
        {
            throw new ForbiddenException("You do not have access to this change request.", ErrorCodes.UnauthorizedAction);
        }

        return _mapper.Map<ChangeRequestDetailDto>(changeRequest);
    }

    public async Task<IReadOnlyCollection<ChangeRequestListDto>> GetForUserAsync(Guid requesterId, CancellationToken cancellationToken = default)
    {
        var specification = new ChangeRequestsByUserSpecification(requesterId);
        var changeRequests = await _unitOfWork.ChangeRequests.ListAsync(specification, cancellationToken).ConfigureAwait(false);
        return _mapper.Map<IReadOnlyCollection<ChangeRequestListDto>>(changeRequests);
    }

    private async Task EnsureTargetExistsAsync(ChangeRequest changeRequest, CancellationToken cancellationToken)
    {
        switch (changeRequest.TargetType)
        {
            case ChangeRequestTargetType.Institution:
                changeRequest.InstitutionId = EnsureIdentifier(changeRequest.InstitutionId, "institution");
                var institution = await _unitOfWork.Institutions.GetByIdAsync(changeRequest.InstitutionId.Value, cancellationToken).ConfigureAwait(false);
                if (institution == null)
                {
                    throw new NotFoundException("Institution was not found.", ErrorCodes.TargetEntityNotFound);
                }

                break;
            case ChangeRequestTargetType.Course:
                changeRequest.CourseId = EnsureIdentifier(changeRequest.CourseId, "course");
                var course = await _unitOfWork.Courses.GetByIdAsync(changeRequest.CourseId.Value, cancellationToken).ConfigureAwait(false);
                if (course == null)
                {
                    throw new NotFoundException("Course was not found.", ErrorCodes.TargetEntityNotFound);
                }

                break;
            case ChangeRequestTargetType.Professor:
                changeRequest.ProfessorId = EnsureIdentifier(changeRequest.ProfessorId, "professor");
                var professor = await _unitOfWork.Professors.GetByIdAsync(changeRequest.ProfessorId.Value, cancellationToken).ConfigureAwait(false);
                if (professor == null)
                {
                    throw new NotFoundException("Professor was not found.", ErrorCodes.TargetEntityNotFound);
                }

                break;
            case ChangeRequestTargetType.Subject:
                changeRequest.SubjectId = EnsureIdentifier(changeRequest.SubjectId, "subject");
                var subject = await _unitOfWork.Subjects.GetByIdAsync(changeRequest.SubjectId.Value, cancellationToken).ConfigureAwait(false);
                if (subject == null)
                {
                    throw new NotFoundException("Subject was not found.", ErrorCodes.TargetEntityNotFound);
                }

                break;
            default:
                throw new InvalidOperationException("Unsupported change request target type.");
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

    private sealed class ChangeRequestsByUserSpecification : Specification<ChangeRequest>
    {
        public ChangeRequestsByUserSpecification(Guid requesterId)
            : base(changeRequest => changeRequest.CreatedById == requesterId)
        {
            ApplyOrderByDescending(changeRequest => changeRequest.CreatedAt);
            DisableTracking();
        }
    }
}
