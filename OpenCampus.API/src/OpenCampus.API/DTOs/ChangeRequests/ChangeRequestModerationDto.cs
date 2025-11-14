using System;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.ChangeRequests;

public record ChangeRequestModerationDto
{
    public Guid Id { get; init; }

    public ChangeRequestTargetType TargetType { get; init; }

    public string SuggestedData { get; init; } = string.Empty;

    public ChangeRequestStatus Status { get; init; }

    public DateTime CreatedAt { get; init; }

    public Guid CreatedById { get; init; }

    public string? CreatedByName { get; init; }

    public Guid? ResolvedById { get; init; }

    public string? ResolvedByName { get; init; }

    public DateTime? ResolvedAt { get; init; }

    public string? ResolutionNotes { get; init; }

    public Guid? InstitutionId { get; init; }

    public Guid? CourseId { get; init; }

    public Guid? ProfessorId { get; init; }

    public Guid? SubjectId { get; init; }
}
