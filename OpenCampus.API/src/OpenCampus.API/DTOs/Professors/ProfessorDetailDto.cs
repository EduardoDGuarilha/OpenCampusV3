using System;

namespace OpenCampus.API.DTOs.Professors;

public record ProfessorDetailDto
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string? Bio { get; init; }

    public Guid InstitutionId { get; init; }

    public string InstitutionName { get; init; } = string.Empty;

    public double AverageScore { get; init; }

    public int ReviewCount { get; init; }
}
