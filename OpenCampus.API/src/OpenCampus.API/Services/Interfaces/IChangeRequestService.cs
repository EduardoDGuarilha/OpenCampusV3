using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Interfaces;

public interface IChangeRequestService
{
    Task<ChangeRequestDetailDto> CreateAsync(Guid requesterId, CreateChangeRequestDto request, CancellationToken cancellationToken = default);

    Task<ChangeRequestDetailDto> GetByIdAsync(Guid changeRequestId, Guid requesterId, UserRole requesterRole, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ChangeRequestListDto>> GetForUserAsync(Guid requesterId, CancellationToken cancellationToken = default);
}