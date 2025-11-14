using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Common.Extensions;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.Entities;
using OpenCampus.API.Filters;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.RequireAuthenticatedUser)]
public class ChangeRequestsController : ControllerBase
{
    private readonly IChangeRequestService _changeRequestService;

    public ChangeRequestsController(IChangeRequestService changeRequestService)
    {
        _changeRequestService = changeRequestService ?? throw new ArgumentNullException(nameof(changeRequestService));
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> CreateAsync([FromBody] CreateChangeRequestDto request, CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        if (!requesterId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var changeRequest = await _changeRequestService.CreateAsync(requesterId.Value, request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.CreatedAtAction(this, nameof(GetByIdAsync), new { changeRequestId = changeRequest.Id }, changeRequest);
    }

    [HttpGet("{changeRequestId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid changeRequestId, CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        var requesterRole = User.GetUserRole();
        if (!requesterId.HasValue || !requesterRole.HasValue)
        {
            return ApiResponseFactory.Forbidden();
        }

        var changeRequest = await _changeRequestService.GetByIdAsync(changeRequestId, requesterId.Value, requesterRole.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(changeRequest);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMineAsync(CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        if (!requesterId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var changeRequests = await _changeRequestService.GetForUserAsync(requesterId.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(changeRequests);
    }
}
