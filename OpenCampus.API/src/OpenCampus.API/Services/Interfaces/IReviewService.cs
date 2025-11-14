using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Reviews;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewListDto> CreateAsync(Guid authorId, ReviewCreateDto request, CancellationToken cancellationToken = default);

    Task<ReviewListDto> GetByIdAsync(Guid reviewId, Guid? requesterId, UserRole? requesterRole, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReviewListDto>> GetByTargetAsync(
        ReviewTargetType targetType,
        Guid targetId,
        Guid? requesterId,
        UserRole? requesterRole,
        bool includePending,
        CancellationToken cancellationToken = default);
}