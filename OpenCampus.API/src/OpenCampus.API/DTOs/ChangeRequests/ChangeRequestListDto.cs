using System;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.ChangeRequests;

public record ChangeRequestListDto
{
    public Guid Id { get; init; }

    public ChangeRequestTargetType TargetType { get; init; }

    public ChangeRequestStatus Status { get; init; }

    public DateTime CreatedAt { get; init; }

    public Guid CreatedById { get; init; }

    public Guid? InstitutionId { get; init; }

    public Guid? CourseId { get; init; }

    public Guid? ProfessorId { get; init; }

    public Guid? SubjectId { get; init; }
}
