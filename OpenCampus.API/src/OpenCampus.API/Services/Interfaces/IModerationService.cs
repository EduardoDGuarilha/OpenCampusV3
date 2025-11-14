using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.DTOs.Reviews;

namespace OpenCampus.API.Services.Interfaces;

public interface IModerationService
{
    Task<IReadOnlyCollection<ReviewModerationDto>> GetPendingReviewsAsync(Guid moderatorId, CancellationToken cancellationToken = default);

    Task<ReviewModerationDto> ApproveReviewAsync(Guid reviewId, Guid moderatorId, CancellationToken cancellationToken = default);

    Task RejectReviewAsync(Guid reviewId, Guid moderatorId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ChangeRequestModerationDto>> GetPendingChangeRequestsAsync(Guid moderatorId, CancellationToken cancellationToken = default);

    Task<ChangeRequestModerationDto> UpdateChangeRequestStatusAsync(Guid changeRequestId, Guid moderatorId, UpdateChangeRequestStatusDto request, CancellationToken cancellationToken = default);
}