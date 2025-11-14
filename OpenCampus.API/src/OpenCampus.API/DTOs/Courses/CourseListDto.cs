using System;

namespace OpenCampus.API.DTOs.Courses;

public record CourseListDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public Guid InstitutionId { get; init; }

    public string InstitutionName { get; init; } = string.Empty;

    public double AverageScore { get; init; }

    public int ReviewCount { get; init; }
}
