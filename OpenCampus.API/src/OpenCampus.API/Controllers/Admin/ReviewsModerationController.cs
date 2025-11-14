using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Common.Extensions;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers.Admin;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Policy = AuthorizationPolicyNames.RequireModerator)]
public class ReviewsModerationController : ControllerBase
{
    private readonly IModerationService _moderationService;

    public ReviewsModerationController(IModerationService moderationService)
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

        var reviews = await _moderationService.GetPendingReviewsAsync(moderatorId.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(reviews);
    }

    [HttpPost("{reviewId:guid}/approve")]
    public async Task<IActionResult> ApproveAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var moderatorId = User.GetUserId();
        if (!moderatorId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var review = await _moderationService.ApproveReviewAsync(reviewId, moderatorId.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(review);
    }

    [HttpPost("{reviewId:guid}/reject")]
    public async Task<IActionResult> RejectAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var moderatorId = User.GetUserId();
        if (!moderatorId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        await _moderationService.RejectReviewAsync(reviewId, moderatorId.Value, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.NoContent();
    }
}
