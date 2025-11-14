using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Auth.Policies;
using OpenCampus.API.Common.Extensions;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.DTOs.Comments;
using OpenCampus.API.Filters;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicyNames.RequireAuthenticatedUser)]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> CreateAsync([FromBody] CommentCreateDto request, CancellationToken cancellationToken)
    {
        var authorId = User.GetUserId();
        if (!authorId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var comment = await _commentService.CreateAsync(authorId.Value, request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.CreatedAtAction(this, nameof(GetByReviewAsync), new { reviewId = request.ReviewId }, comment);
    }

    [HttpPost("official")]
    [Authorize(Policy = AuthorizationPolicyNames.RequireOfficialCommenter)]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> CreateOfficialAsync([FromBody] OfficialCommentCreateDto request, CancellationToken cancellationToken)
    {
        var authorId = User.GetUserId();
        if (!authorId.HasValue)
        {
            return ApiResponseFactory.Unauthorized();
        }

        var comment = await _commentService.CreateOfficialAsync(authorId.Value, request, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.CreatedAtAction(this, nameof(GetByReviewAsync), new { reviewId = request.ReviewId }, comment);
    }

    [HttpGet("review/{reviewId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByReviewAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var requesterId = User.GetUserId();
        var requesterRole = User.GetUserRole();
        var comments = await _commentService.GetByReviewAsync(reviewId, requesterId, requesterRole, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(comments);
    }
}
