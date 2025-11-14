using System.ComponentModel.DataAnnotations;

namespace OpenCampus.API.DTOs.Auth;

public record LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
