using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.DTOs.Shared;
using OpenCampus.API.DTOs.Subjects;

namespace OpenCampus.API.Services.Interfaces;

public interface ISubjectService
{
    Task<PagedResultDto<SubjectListDto>> SearchAsync(SearchFilterDto filter, CancellationToken cancellationToken = default);

    Task<SubjectDetailDto> GetByIdAsync(Guid subjectId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectSummaryDto>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}
