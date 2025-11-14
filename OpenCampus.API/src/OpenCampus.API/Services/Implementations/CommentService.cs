using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.Comments;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;
using OpenCampus.API.Services.Validators;

namespace OpenCampus.API.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CommentEligibilityValidator _commentValidator;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, CommentEligibilityValidator commentValidator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _commentValidator = commentValidator ?? throw new ArgumentNullException(nameof(commentValidator));
    }

    public async Task<CommentListDto> CreateAsync(Guid authorId, CommentCreateDto request, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Users.GetByIdAsync(authorId, cancellationToken).ConfigureAwait(false);
        if (author == null)
        {
            throw new NotFoundException("User was not found.", ErrorCodes.UserNotFound);
        }

        var review = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken).ConfigureAwait(false);
        if (review == null)
        {
            throw new NotFoundException("Review was not found.", ErrorCodes.ReviewNotFound);
        }

        await _commentValidator.ValidateAsync(author, review, isOfficial: false, cancellationToken).ConfigureAwait(false);

        var comment = _mapper.Map<Comment>(request);
        comment.Id = Guid.NewGuid();
        comment.AuthorId = author.Id;
        comment.CreatedAt = DateTime.UtcNow;
        comment.UpdatedAt = null;
        comment.IsOfficial = false;

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return _mapper.Map<CommentListDto>(comment);
    }

    public async Task<CommentListDto> CreateOfficialAsync(Guid authorId, OfficialCommentCreateDto request, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Users.GetByIdAsync(authorId, cancellationToken).ConfigureAwait(false);
        if (author == null)
        {
            throw new NotFoundException("User was not found.", ErrorCodes.UserNotFound);
        }

        var review = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken).ConfigureAwait(false);
        if (review == null)
        {
            throw new NotFoundException("Review was not found.", ErrorCodes.ReviewNotFound);
        }

        await _commentValidator.ValidateAsync(author, review, isOfficial: true, cancellationToken).ConfigureAwait(false);

        var comment = _mapper.Map<Comment>(request);
        comment.Id = Guid.NewGuid();
        comment.AuthorId = author.Id;
        comment.CreatedAt = DateTime.UtcNow;
        comment.UpdatedAt = null;
        comment.IsOfficial = true;

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return _mapper.Map<CommentListDto>(comment);
    }

    public async Task<IReadOnlyCollection<CommentListDto>> GetByReviewAsync(Guid reviewId, Guid? requesterId, UserRole? requesterRole, CancellationToken cancellationToken = default)
    {
        var specification = new ReviewWithCommentsSpecification(reviewId);
        var review = await _unitOfWork.Reviews.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (review == null)
        {
            throw new NotFoundException("Review was not found.", ErrorCodes.ReviewNotFound);
        }

        if (!review.Approved && requesterRole != UserRole.Moderator && (!requesterId.HasValue || requesterId.Value != review.AuthorId))
        {
            throw new NotFoundException("Review is not available.", ErrorCodes.ReviewUnavailable);
        }

        var orderedComments = review.Comments
            .OrderBy(comment => comment.CreatedAt)
            .ToList();

        return _mapper.Map<IReadOnlyCollection<CommentListDto>>(orderedComments);
    }

    private sealed class ReviewWithCommentsSpecification : Specification<Review>
    {
        public ReviewWithCommentsSpecification(Guid reviewId)
            : base(review => review.Id == reviewId)
        {
            AddInclude(review => review.Comments);
            DisableTracking();
        }
    }
}