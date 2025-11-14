using System;

namespace OpenCampus.API.DTOs.Institutions;

public record InstitutionSummaryDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Acronym { get; init; }
}
