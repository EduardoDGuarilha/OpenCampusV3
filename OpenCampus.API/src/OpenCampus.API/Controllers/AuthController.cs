using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.DTOs.Auth;
using OpenCampus.API.Filters;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.CreatedAtRoute(this, UsersController.GetCurrentUserRouteName, null, result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.LoginAsync(request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRefreshRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.RefreshTokenAsync(request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(result);
    }
}
