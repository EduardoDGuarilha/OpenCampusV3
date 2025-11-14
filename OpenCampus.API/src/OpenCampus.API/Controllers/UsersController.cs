using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Common.Extensions;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.RequireAuthenticatedUser)]
public class UsersController : ControllerBase
{
    public const string GetCurrentUserRouteName = "GetCurrentUser";

    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("me", Name = GetCurrentUserRouteName)]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var user = await _userService.GetByIdAsync(userId.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(user);
    }
}
