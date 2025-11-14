using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Auth.Jwt;

public interface IJwtTokenService
{
    Task<JwtTokenResult> CreateTokenAsync(User user, CancellationToken cancellationToken = default);
}

public sealed record JwtTokenResult(string Token, System.DateTimeOffset ExpiresAt);
