using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace OpenCampus.API.Configuration.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Security:Jwt";

    private string _issuer = string.Empty;
    private string _audience = string.Empty;
    private string _signingKey = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenLifetimeMinutes { get; set; } = 120;

    [Required]
    public string Issuer
    {
        get => _issuer;
        set => _issuer = value?.Trim() ?? string.Empty;
    }

    [Required]
    public string Audience
    {
        get => _audience;
        set => _audience = value?.Trim() ?? string.Empty;
    }

    [Required]
    public string SigningKey
    {
        get => _signingKey;
        set => _signingKey = value?.Trim() ?? string.Empty;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new CryptographicException("JWT issuer must be provided.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new CryptographicException("JWT audience must be provided.");
        }

        var keyBytes = GetSigningKeyBytes();
        if (keyBytes.Length < 32)
        {
            throw new CryptographicException("JWT signing key must be at least 256 bits.");
        }

        if (AccessTokenLifetimeMinutes < 1)
        {
            throw new CryptographicException("Access token lifetime must be at least one minute.");
        }
    }

    public byte[] GetSigningKeyBytes()
    {
        if (string.IsNullOrWhiteSpace(SigningKey))
        {
            throw new CryptographicException("JWT signing key cannot be empty.");
        }

        var trimmed = SigningKey.Trim();

        if (TryBase64Decode(trimmed, out var bytes))
        {
            return bytes;
        }

        return Encoding.UTF8.GetBytes(trimmed);
    }

    private static bool TryBase64Decode(string value, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(value);
            return bytes.Length > 0;
        }
        catch (FormatException)
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }
}
