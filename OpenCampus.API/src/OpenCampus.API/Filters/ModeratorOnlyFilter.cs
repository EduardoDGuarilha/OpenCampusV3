using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OpenCampus.API.Auth.Roles;

namespace OpenCampus.API.Filters;

public sealed class ModeratorOnlyFilter : IAuthorizationFilter
{
    private readonly ILogger<ModeratorOnlyFilter> _logger;

    public ModeratorOnlyFilter(ILogger<ModeratorOnlyFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var user = context.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            _logger.LogWarning("Unauthenticated request blocked by ModeratorOnlyFilter for {Path}.", context.HttpContext.Request.Path);
            context.Result = new UnauthorizedResult();
            return;
        }

        if (user.IsInRole(RoleNames.Moderator))
        {
            return;
        }

        _logger.LogWarning("Non-moderator user blocked by ModeratorOnlyFilter for {Path}.", context.HttpContext.Request.Path);
        context.Result = new ForbidResult();
    }
}
