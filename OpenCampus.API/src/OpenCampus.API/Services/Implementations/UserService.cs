using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Auth.Password;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.Auth;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Services.Implementations;

public class UserService : BaseService<User>, IUserService
{
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
        : base(unitOfWork, mapper)
    {
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<User> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var normalizedEmail = NormalizeEmail(request.Email);
        var emailAlreadyExists = await Repository.AnyAsync(user => user.Email == normalizedEmail, cancellationToken).ConfigureAwait(false);
        if (emailAlreadyExists)
        {
            throw new ConflictException("A user with the provided email already exists.", ErrorCodes.UserAlreadyExists);
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = NormalizeRequiredText(request.FullName),
            Email = normalizedEmail,
            StudentEmail = NormalizeOptionalEmail(request.StudentEmail),
            Cpf = NormalizeOptional(request.Cpf),
            Role = request.Role,
            IsActive = true,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = null
        };

        await Repository.AddAsync(user, cancellationToken).ConfigureAwait(false);
        await UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return user;
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User identifier must be provided.", nameof(userId));
        }

        return await Repository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var normalized = NormalizeEmail(email);
        var specification = new UserByEmailSpecification(normalized);
        return await Repository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> VerifyPasswordAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (string.IsNullOrEmpty(password) || !user.IsActive)
        {
            return false;
        }

        var isValid = _passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
        if (!isValid)
        {
            return false;
        }

        if (_passwordHasher.NeedsRehash(user.PasswordHash))
        {
            user.PasswordHash = _passwordHasher.HashPassword(password);
            user.UpdatedAt = DateTime.UtcNow;
            Repository.Update(user);
            await UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email must be provided.", nameof(email));
        }

        return email.Trim().ToLowerInvariant();
    }

    private static string NormalizeRequiredText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value must be provided.", nameof(value));
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalEmail(string? email)
    {
        var normalized = NormalizeOptional(email);
        return normalized?.ToLowerInvariant();
    }

    private sealed class UserByEmailSpecification : Specification<User>
    {
        public UserByEmailSpecification(string email)
            : base(user => user.Email == email)
        {
            DisableTracking();
        }
    }
}
