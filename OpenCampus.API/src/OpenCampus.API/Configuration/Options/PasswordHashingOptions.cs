using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace OpenCampus.API.Configuration.Options;

public sealed class PasswordHashingOptions
{
    public const string SectionName = "Security:PasswordHashing";

    private string _prf = KeyDerivationPrf.HMACSHA256.ToString();
    private string _version = "v1";

    [Range(8, 128)]
    public int SaltSize { get; set; } = 16;

    [Range(16, 256)]
    public int HashSize { get; set; } = 32;

    [Range(10_000, int.MaxValue)]
    public int IterationCount { get; set; } = 100_000;

    public string Prf
    {
        get => _prf;
        set => _prf = string.IsNullOrWhiteSpace(value) ? KeyDerivationPrf.HMACSHA256.ToString() : value.Trim();
    }

    public string Version
    {
        get => _version;
        set => _version = string.IsNullOrWhiteSpace(value) ? "v1" : value.Trim();
    }

    public void Validate()
    {
        if (SaltSize < 8)
        {
            throw new CryptographicException("Salt size must be at least 8 bytes.");
        }

        if (HashSize < 16)
        {
            throw new CryptographicException("Hash size must be at least 16 bytes.");
        }

        if (IterationCount < 10_000)
        {
            throw new CryptographicException("Iteration count is too low for secure hashing.");
        }

        _ = ParsePrf(Prf);

        if (string.IsNullOrWhiteSpace(Version))
        {
            throw new CryptographicException("A hash format version must be provided.");
        }
    }

    internal static KeyDerivationPrf ParsePrf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return KeyDerivationPrf.HMACSHA256;
        }

        if (Enum.TryParse<KeyDerivationPrf>(value, ignoreCase: true, out var prf))
        {
            return prf;
        }

        throw new CryptographicException($"Unsupported PBKDF2 PRF '{value}'.");
    }
}
