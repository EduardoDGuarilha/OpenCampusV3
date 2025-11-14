using System;
using System.ComponentModel.DataAnnotations;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.Reviews;

public record ReviewCreateDto
{
    [Required]
    public ReviewTargetType TargetType { get; init; }

    public Guid? InstitutionId { get; init; }

    public Guid? CourseId { get; init; }

    public Guid? ProfessorId { get; init; }

    public Guid? SubjectId { get; init; }

    [Range(1, 5)]
    public int ScoreClarity { get; init; }

    [Range(1, 5)]
    public int ScoreRelevance { get; init; }

    [Range(1, 5)]
    public int ScoreSupport { get; init; }

    [Range(1, 5)]
    public int ScoreInfrastructure { get; init; }

    [Required]
    [MaxLength(4000)]
    public string Text { get; init; } = string.Empty;
}
