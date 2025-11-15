using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Auth;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Interfaces;

public interface IUserService
{
    Task<User> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> VerifyPasswordAsync(User user, string password, CancellationToken cancellationToken = default);
}
