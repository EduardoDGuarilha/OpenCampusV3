using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.Reviews;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;
using OpenCampus.API.Services.Validators;

namespace OpenCampus.API.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ReviewBusinessValidator _reviewValidator;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ReviewBusinessValidator reviewValidator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _reviewValidator = reviewValidator ?? throw new ArgumentNullException(nameof(reviewValidator));
    }

    public async Task<ReviewListDto> CreateAsync(Guid authorId, ReviewCreateDto request, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Users.GetByIdAsync(authorId, cancellationToken).ConfigureAwait(false);
        if (author == null)
        {
            throw new NotFoundException("User was not found.", ErrorCodes.UserNotFound);
        }

        await _reviewValidator.ValidateCreationAsync(author, request, cancellationToken).ConfigureAwait(false);

        var review = _mapper.Map<Review>(request);
        review.Id = Guid.NewGuid();
        review.AuthorId = author.Id;
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = null;

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return _mapper.Map<ReviewListDto>(review);
    }

    public async Task<ReviewListDto> GetByIdAsync(Guid reviewId, Guid? requesterId, UserRole? requesterRole, CancellationToken cancellationToken = default)
    {
        var specification = new ReviewWithCommentsSpecification(reviewId);
        var review = await _unitOfWork.Reviews.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (review == null)
        {
            throw new NotFoundException("Review was not found.", ErrorCodes.ReviewNotFound);
        }

        if (!review.Approved && !CanViewPendingReview(review, requesterId, requesterRole))
        {
            throw new NotFoundException("Review is not available.", ErrorCodes.ReviewUnavailable);
        }

        return _mapper.Map<ReviewListDto>(review);
    }

    public async Task<IReadOnlyCollection<ReviewListDto>> GetByTargetAsync(
        ReviewTargetType targetType,
        Guid targetId,
        Guid? requesterId,
        UserRole? requesterRole,
        bool includePending,
        CancellationToken cancellationToken = default)
    {
        if (includePending && requesterRole != UserRole.Moderator)
        {
            throw new ForbiddenException("Only moderators may access pending reviews.", ErrorCodes.UnauthorizedAction);
        }

        var specification = new ReviewsByTargetSpecification(targetType, targetId, includePending);
        var reviews = await _unitOfWork.Reviews.ListAsync(specification, cancellationToken).ConfigureAwait(false);

        if (!includePending)
        {
            return _mapper.Map<IReadOnlyCollection<ReviewListDto>>(reviews);
        }

        var filtered = new List<Review>(reviews.Count);
        foreach (var review in reviews)
        {
            if (review.Approved || CanViewPendingReview(review, requesterId, requesterRole))
            {
                filtered.Add(review);
            }
        }

        return _mapper.Map<IReadOnlyCollection<ReviewListDto>>(filtered);
    }

    private static bool CanViewPendingReview(Review review, Guid? requesterId, UserRole? requesterRole)
    {
        if (requesterRole == UserRole.Moderator)
        {
            return true;
        }

        return requesterId.HasValue && requesterId.Value == review.AuthorId;
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

    private sealed class ReviewsByTargetSpecification : Specification<Review>
    {
        public ReviewsByTargetSpecification(ReviewTargetType targetType, Guid targetId, bool includePending)
            : base(review => review.TargetType == targetType)
        {
            switch (targetType)
            {
                case ReviewTargetType.Institution:
                    AddWhere(review => review.InstitutionId == targetId);
                    break;
                case ReviewTargetType.Course:
                    AddWhere(review => review.CourseId == targetId);
                    break;
                case ReviewTargetType.Professor:
                    AddWhere(review => review.ProfessorId == targetId);
                    break;
                case ReviewTargetType.Subject:
                    AddWhere(review => review.SubjectId == targetId);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported review target type.");
            }

            if (!includePending)
            {
                AddWhere(review => review.Approved);
            }

            AddInclude(review => review.Comments);
            ApplyOrderByDescending(review => review.CreatedAt);
            DisableTracking();
        }
    }
}
