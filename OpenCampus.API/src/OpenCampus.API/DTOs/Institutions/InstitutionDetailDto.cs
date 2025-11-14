using System;
using System.Collections.Generic;
using OpenCampus.API.DTOs.Courses;
using OpenCampus.API.DTOs.Professors;

namespace OpenCampus.API.DTOs.Institutions;

public record InstitutionDetailDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Acronym { get; init; }

    public string? Description { get; init; }

    public string? WebsiteUrl { get; init; }

    public double AverageScore { get; init; }

    public int ReviewCount { get; init; }

    public IReadOnlyCollection<CourseSummaryDto> Courses { get; init; } = Array.Empty<CourseSummaryDto>();

    public IReadOnlyCollection<ProfessorSummaryDto> Professors { get; init; } = Array.Empty<ProfessorSummaryDto>();
}
