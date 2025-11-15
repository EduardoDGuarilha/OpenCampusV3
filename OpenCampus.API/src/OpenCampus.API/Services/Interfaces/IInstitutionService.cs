using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Institutions;

namespace OpenCampus.API.Services.Interfaces;

public interface IInstitutionService
{
    Task<IReadOnlyCollection<InstitutionSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InstitutionListDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<InstitutionDetailDto> GetByIdAsync(Guid institutionId, CancellationToken cancellationToken = default);
}
