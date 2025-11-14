using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Auth.Roles;

namespace OpenCampus.API.Configuration.DependencyInjection;

public static class AuthorizationSetup
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(AuthorizationPolicyNames.RequireAuthenticatedUser, policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(AuthorizationPolicyNames.RequireStudent, policy =>
                policy.RequireRole(RoleNames.Student));

            options.AddPolicy(AuthorizationPolicyNames.RequireProfessor, policy =>
                policy.RequireRole(RoleNames.Professor));

            options.AddPolicy(AuthorizationPolicyNames.RequireInstitution, policy =>
                policy.RequireRole(RoleNames.Institution));

            options.AddPolicy(AuthorizationPolicyNames.RequireModerator, policy =>
                policy.RequireRole(RoleNames.Moderator));

            options.AddPolicy(AuthorizationPolicyNames.RequireOfficialCommenter, policy =>
                policy.RequireRole(RoleNames.Professor, RoleNames.Institution));
        });

        services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

        return services;
    }
}
