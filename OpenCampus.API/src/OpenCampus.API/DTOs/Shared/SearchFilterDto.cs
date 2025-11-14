namespace OpenCampus.API.DTOs.Shared;

public record SearchFilterDto
{
    public string? Query { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }
}
