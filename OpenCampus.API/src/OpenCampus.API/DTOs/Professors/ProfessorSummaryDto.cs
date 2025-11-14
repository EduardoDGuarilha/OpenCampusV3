using System;

namespace OpenCampus.API.DTOs.Professors;

public record ProfessorSummaryDto
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public Guid InstitutionId { get; init; }

    public string InstitutionName { get; init; } = string.Empty;
}
