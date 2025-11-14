using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Auth.Roles;

public static class RoleNames
{
    public const string Student = "STUDENT";
    public const string Professor = "PROFESSOR";
    public const string Institution = "INSTITUTION";
    public const string Moderator = "MODERATOR";

    private static readonly IReadOnlyDictionary<UserRole, string> RoleMap = new Dictionary<UserRole, string>
    {
        [UserRole.Student] = Student,
        [UserRole.Professor] = Professor,
        [UserRole.Institution] = Institution,
        [UserRole.Moderator] = Moderator
    };

    private static readonly IReadOnlyDictionary<string, string> NormalizedRoleNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [Student] = Student,
        [Professor] = Professor,
        [Institution] = Institution,
        [Moderator] = Moderator,
        [nameof(UserRole.Student)] = Student,
        [nameof(UserRole.Professor)] = Professor,
        [nameof(UserRole.Institution)] = Institution,
        [nameof(UserRole.Moderator)] = Moderator
    };

    public static string FromUserRole(UserRole role)
    {
        if (!RoleMap.TryGetValue(role, out var name))
        {
            throw new ArgumentOutOfRangeException(nameof(role), role, "Unsupported role.");
        }

        return name;
    }

    public static bool TryNormalize(string? value, [NotNullWhen(true)] out string? normalized)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            normalized = null;
            return false;
        }

        if (NormalizedRoleNames.TryGetValue(value.Trim(), out var canonical))
        {
            normalized = canonical;
            return true;
        }

        normalized = null;
        return false;
    }

    public static IEnumerable<string> AllRoles => RoleMap.Values;
}
