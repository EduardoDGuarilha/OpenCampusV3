using System;

namespace OpenCampus.API.DTOs.Subjects;

public record SubjectListDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public Guid CourseId { get; init; }

    public string CourseName { get; init; } = string.Empty;

    public Guid InstitutionId { get; init; }

    public string InstitutionName { get; init; } = string.Empty;

    public double AverageScore { get; init; }

    public int ReviewCount { get; init; }
}
