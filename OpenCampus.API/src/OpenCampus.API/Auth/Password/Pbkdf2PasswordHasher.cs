using System;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using OpenCampus.API.Configuration.Options;

namespace OpenCampus.API.Auth.Password;

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const char SegmentDelimiter = '.';
    private readonly PasswordHashingOptions _options;

    public Pbkdf2PasswordHasher(IOptions<PasswordHashingOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _options = options.Value ?? throw new ArgumentException("Password hashing options must be provided.", nameof(options));
        _options.Validate();
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password must not be empty.", nameof(password));
        }

        var prf = PasswordHashingOptions.ParsePrf(_options.Prf);
        var salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
        var hash = KeyDerivation.Pbkdf2(password, salt, prf, _options.IterationCount, _options.HashSize);

        return string.Join(
            SegmentDelimiter,
            _options.Version,
            _options.IterationCount.ToString(CultureInfo.InvariantCulture),
            prf,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
        {
            return false;
        }

        if (!TryParseHash(hashedPassword, out var descriptor))
        {
            return false;
        }

        var derived = KeyDerivation.Pbkdf2(
            providedPassword,
            descriptor.Salt,
            descriptor.Prf,
            descriptor.IterationCount,
            descriptor.HashSize);

        return CryptographicOperations.FixedTimeEquals(derived, descriptor.Hash);
    }

    public bool NeedsRehash(string hashedPassword)
    {
        if (!TryParseHash(hashedPassword, out var descriptor))
        {
            return true;
        }

        if (!string.Equals(descriptor.Version, _options.Version, StringComparison.Ordinal))
        {
            return true;
        }

        if (descriptor.IterationCount < _options.IterationCount)
        {
            return true;
        }

        if (descriptor.HashSize != _options.HashSize)
        {
            return true;
        }

        if (descriptor.Prf != PasswordHashingOptions.ParsePrf(_options.Prf))
        {
            return true;
        }

        return false;
    }

    private static bool TryParseHash(string hashedPassword, out HashDescriptor descriptor)
    {
        descriptor = default;

        var segments = hashedPassword.Split(SegmentDelimiter);
        if (segments.Length != 5)
        {
            return false;
        }

        var version = segments[0];

        if (!int.TryParse(segments[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var iterations) || iterations <= 0)
        {
            return false;
        }

        KeyDerivationPrf prf;
        try
        {
            prf = PasswordHashingOptions.ParsePrf(segments[2]);
        }
        catch (CryptographicException)
        {
            return false;
        }

        if (!TryFromBase64(segments[3], out var salt))
        {
            return false;
        }

        if (!TryFromBase64(segments[4], out var hash))
        {
            return false;
        }

        descriptor = new HashDescriptor(version, iterations, prf, salt, hash);
        return true;
    }

    private static bool TryFromBase64(string value, out byte[] data)
    {
        data = Array.Empty<byte>();
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            data = Convert.FromBase64String(value);
            return data.Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private readonly record struct HashDescriptor(
        string Version,
        int IterationCount,
        KeyDerivationPrf Prf,
        byte[] Salt,
        byte[] Hash)
    {
        public readonly int HashSize => Hash.Length;
    }
}
