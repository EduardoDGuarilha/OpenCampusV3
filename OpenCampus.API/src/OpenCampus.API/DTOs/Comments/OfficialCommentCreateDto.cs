using System;
using System.ComponentModel.DataAnnotations;

namespace OpenCampus.API.DTOs.Comments;

public record OfficialCommentCreateDto
{
    [Required]
    public Guid ReviewId { get; init; }

    [Required]
    [MaxLength(2000)]
    public string Text { get; init; } = string.Empty;
}
