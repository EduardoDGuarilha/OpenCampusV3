using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Institutions;
using OpenCampus.API.DTOs.Shared;

namespace OpenCampus.API.Services.Interfaces;

public interface IInstitutionService
{
    Task<PagedResultDto<InstitutionListDto>> SearchAsync(SearchFilterDto filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InstitutionSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken = default);

    Task<InstitutionDetailDto> GetByIdAsync(Guid institutionId, CancellationToken cancellationToken = default);
}
