using System.ComponentModel.DataAnnotations;

namespace OpenCampus.API.DTOs.Auth;

public record TokenRefreshRequestDto
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}
