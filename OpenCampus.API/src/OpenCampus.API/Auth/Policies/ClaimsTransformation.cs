using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using OpenCampus.API.Auth.Roles;

namespace OpenCampus.API.Auth.Policies;

public sealed class ClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        foreach (var identity in principal.Identities.Where(identity => identity.IsAuthenticated))
        {
            NormalizeRoleClaims(identity);
        }

        return Task.FromResult(principal);
    }

    private static void NormalizeRoleClaims(ClaimsIdentity identity)
    {
        var roleClaims = identity.Claims
            .Where(claim => string.Equals(claim.Type, identity.RoleClaimType, StringComparison.OrdinalIgnoreCase) || string.Equals(claim.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) || string.Equals(claim.Type, "role", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (roleClaims.Count == 0)
        {
            return;
        }

        var canonicalRoles = new HashSet<string>(StringComparer.Ordinal);
        var claimsToRemove = new List<Claim>();

        foreach (var claim in roleClaims)
        {
            if (RoleNames.TryNormalize(claim.Value, out var normalized))
            {
                canonicalRoles.Add(normalized);
                claimsToRemove.Add(claim);
            }
        }

        foreach (var claim in claimsToRemove)
        {
            identity.TryRemoveClaim(claim);
        }

        if (canonicalRoles.Count == 0)
        {
            return;
        }

        foreach (var role in canonicalRoles)
        {
            if (!identity.HasClaim(ClaimTypes.Role, role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            if (!string.Equals(identity.RoleClaimType, ClaimTypes.Role, StringComparison.Ordinal) && !identity.HasClaim(identity.RoleClaimType, role))
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, role));
            }
        }
    }
}
