using System;
using System.ComponentModel.DataAnnotations;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.ChangeRequests;

public record CreateChangeRequestDto
{
    [Required]
    public ChangeRequestTargetType TargetType { get; init; }

    [Required]
    [MaxLength(4000)]
    public string SuggestedData { get; init; } = string.Empty;

    public Guid? InstitutionId { get; init; }

    public Guid? CourseId { get; init; }

    public Guid? ProfessorId { get; init; }

    public Guid? SubjectId { get; init; }
}
