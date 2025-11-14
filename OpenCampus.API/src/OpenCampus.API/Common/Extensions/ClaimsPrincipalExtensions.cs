using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using OpenCampus.API.Auth.Roles;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null || principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var identifierClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);
        if (identifierClaim == null)
        {
            return null;
        }

        return Guid.TryParse(identifierClaim.Value, out var userId) ? userId : null;
    }

    public static bool TryGetUserId(this ClaimsPrincipal principal, out Guid userId)
    {
        var value = principal.GetUserId();
        if (value.HasValue)
        {
            userId = value.Value;
            return true;
        }

        userId = Guid.Empty;
        return false;
    }

    public static UserRole? GetUserRole(this ClaimsPrincipal principal)
    {
        if (principal == null || principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var roleClaim = principal.Claims
            .FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));
        if (roleClaim == null)
        {
            return null;
        }

        if (!RoleNames.TryNormalize(roleClaim.Value, out var normalized))
        {
            return null;
        }

        return normalized switch
        {
            RoleNames.Student => UserRole.Student,
            RoleNames.Professor => UserRole.Professor,
            RoleNames.Institution => UserRole.Institution,
            RoleNames.Moderator => UserRole.Moderator,
            _ => null,
        };
    }
}
