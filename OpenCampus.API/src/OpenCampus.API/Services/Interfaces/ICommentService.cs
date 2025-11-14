using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Comments;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Interfaces;

public interface ICommentService
{
    Task<CommentListDto> CreateAsync(Guid authorId, CommentCreateDto request, CancellationToken cancellationToken = default);

    Task<CommentListDto> CreateOfficialAsync(Guid authorId, OfficialCommentCreateDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CommentListDto>> GetByReviewAsync(Guid reviewId, Guid? requesterId, UserRole? requesterRole, CancellationToken cancellationToken = default);
}