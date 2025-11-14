using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using OpenCampus.API.Configuration.Options;

namespace OpenCampus.API.Auth.Jwt;

public sealed class JwtTokenFactory
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenFactory(IOptions<JwtOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _options = options.Value ?? throw new ArgumentException("JWT options must be provided.", nameof(options));
        _options.Validate();

        var signingKeyBytes = _options.GetSigningKeyBytes();
        var securityKey = new SymmetricSecurityKey(signingKeyBytes);
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    public string CreateToken(IEnumerable<Claim> claims, DateTimeOffset issuedAt, DateTimeOffset expiresAt)
    {
        if (claims == null)
        {
            throw new ArgumentNullException(nameof(claims));
        }

        if (expiresAt <= issuedAt)
        {
            throw new ArgumentException("The expiration time must be after the issued time.", nameof(expiresAt));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: _signingCredentials);

        return _tokenHandler.WriteToken(token);
    }
}
