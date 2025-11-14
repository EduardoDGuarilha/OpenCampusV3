using System;
using System.Collections.Generic;
using OpenCampus.API.DTOs.Comments;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.Reviews;

public record ReviewListDto
{
    public Guid Id { get; init; }

    public ReviewTargetType TargetType { get; init; }

    public Guid? InstitutionId { get; init; }

    public Guid? CourseId { get; init; }

    public Guid? ProfessorId { get; init; }

    public Guid? SubjectId { get; init; }

    public int ScoreClarity { get; init; }

    public int ScoreRelevance { get; init; }

    public int ScoreSupport { get; init; }

    public int ScoreInfrastructure { get; init; }

    public string Text { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public IReadOnlyCollection<CommentListDto> Comments { get; init; } = Array.Empty<CommentListDto>();
}
