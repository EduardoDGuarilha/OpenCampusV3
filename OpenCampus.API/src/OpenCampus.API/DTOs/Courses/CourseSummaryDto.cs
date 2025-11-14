using System;

namespace OpenCampus.API.DTOs.Courses;

public record CourseSummaryDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public Guid InstitutionId { get; init; }

    public string InstitutionName { get; init; } = string.Empty;
}
