using System;

namespace OpenCampus.API.DTOs.Comments;

public record CommentListDto
{
    public Guid Id { get; init; }

    public Guid ReviewId { get; init; }

    public string Text { get; init; } = string.Empty;

    public bool IsOfficial { get; init; }

    public DateTime CreatedAt { get; init; }
}
