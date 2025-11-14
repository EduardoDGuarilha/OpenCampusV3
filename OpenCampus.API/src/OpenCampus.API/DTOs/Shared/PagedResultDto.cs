using System;
using System.Collections.Generic;

namespace OpenCampus.API.DTOs.Shared;

public record PagedResultDto<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}
