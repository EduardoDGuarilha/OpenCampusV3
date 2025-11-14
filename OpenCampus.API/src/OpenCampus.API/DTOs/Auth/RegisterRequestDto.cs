using System.ComponentModel.DataAnnotations;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.Auth;

public record RegisterRequestDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;

    [EmailAddress]
    public string? StudentEmail { get; init; }

    [StringLength(14)]
    public string? Cpf { get; init; }

    [Required]
    public UserRole Role { get; init; } = UserRole.Student;
}
