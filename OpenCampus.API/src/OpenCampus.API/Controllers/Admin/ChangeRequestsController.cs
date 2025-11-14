using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Common.Extensions;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.Filters;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers.Admin;

[ApiController]
[Route("api/admin/change-requests")]
[Authorize(Policy = AuthorizationPolicyNames.RequireModerator)]
public class ChangeRequestsController : ControllerBase
{
    private readonly IModerationService _moderationService;

    public ChangeRequestsController(IModerationService moderationService)
    {
        _moderationService = moderationService ?? throw new ArgumentNullException(nameof(moderationService));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingAsync(CancellationToken cancellationToken)
    {
        var moderatorId = User.GetUserId();
        if (!moderatorId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var changeRequests = await _moderationService.GetPendingChangeRequestsAsync(moderatorId.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(changeRequests);
    }

    [HttpPut("{changeRequestId:guid}")]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> UpdateStatusAsync(Guid changeRequestId, [FromBody] UpdateChangeRequestStatusDto request, CancellationToken cancellationToken)
    {
        var moderatorId = User.GetUserId();
        if (!moderatorId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var changeRequest = await _moderationService.UpdateChangeRequestStatusAsync(changeRequestId, moderatorId.Value, request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(changeRequest);
    }
}
