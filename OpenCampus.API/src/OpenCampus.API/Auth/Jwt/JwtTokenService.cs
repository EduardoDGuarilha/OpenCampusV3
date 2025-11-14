using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenCampus.API.Auth.Roles;
using OpenCampus.API.Configuration.Options;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Auth.Jwt;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtTokenFactory _jwtTokenFactory;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenService(JwtTokenFactory jwtTokenFactory, IOptions<JwtOptions> jwtOptions)
    {
        _jwtTokenFactory = jwtTokenFactory ?? throw new ArgumentNullException(nameof(jwtTokenFactory));
        if (jwtOptions == null)
        {
            throw new ArgumentNullException(nameof(jwtOptions));
        }

        _jwtOptions = jwtOptions.Value ?? throw new ArgumentException("JWT options must be provided.", nameof(jwtOptions));
        _jwtOptions.Validate();
    }

    public Task<JwtTokenResult> CreateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes);

        var claims = BuildClaims(user, now);
        var token = _jwtTokenFactory.CreateToken(claims, now, expiresAt);

        return Task.FromResult(new JwtTokenResult(token, expiresAt));
    }

    private static IEnumerable<Claim> BuildClaims(User user, DateTimeOffset issuedAt)
    {
        yield return new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString());
        yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        yield return new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            yield return new Claim(JwtRegisteredClaimNames.Email, user.Email);
        }

        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            yield return new Claim(JwtRegisteredClaimNames.Name, user.FullName);
        }

        yield return new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
        yield return new Claim(ClaimTypes.Role, RoleNames.FromUserRole(user.Role));
    }
}
