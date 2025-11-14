using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Common.Extensions;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.DTOs.Reviews;
using OpenCampus.API.Entities;
using OpenCampus.API.Filters;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicyNames.RequireStudent)]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> CreateAsync([FromBody] ReviewCreateDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var review = await _reviewService.CreateAsync(userId.Value, request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.CreatedAtAction(this, nameof(GetByIdAsync), new { reviewId = review.Id }, review);
    }

    [HttpGet("{reviewId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        var requesterRole = User.GetUserRole();
        var review = await _reviewService.GetByIdAsync(reviewId, requesterId, requesterRole, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(review);
    }

    [HttpGet("target/{targetType}/{targetId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByTargetAsync(ReviewTargetType targetType, Guid targetId, [FromQuery] bool includePending = false, CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        var requesterRole = User.GetUserRole();
        var reviews = await _reviewService.GetByTargetAsync(targetType, targetId, requesterId, requesterRole, includePending, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(reviews);
    }
}
