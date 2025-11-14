using System;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.Reviews;

public record ReviewSummaryDto
{
    public Guid TargetId { get; init; }

    public ReviewTargetType TargetType { get; init; }

    public double AverageClarity { get; init; }

    public double AverageRelevance { get; init; }

    public double AverageSupport { get; init; }

    public double AverageInfrastructure { get; init; }

    public double AverageOverall { get; init; }

    public int ReviewCount { get; init; }
}
