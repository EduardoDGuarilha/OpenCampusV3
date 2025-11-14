using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Auth;
using OpenCampus.API.DTOs.Users;

namespace OpenCampus.API.Services.Interfaces;

public interface IUserService
{
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<LoginResponseDto> RefreshTokenAsync(TokenRefreshRequestDto request, CancellationToken cancellationToken = default);

    Task<UserDetailDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
