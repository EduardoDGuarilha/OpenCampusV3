using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.Institutions;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Services.Implementations;

public class InstitutionService : BaseService<Institution>, IInstitutionService
{
    public InstitutionService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<IReadOnlyCollection<InstitutionSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken = default)
    {
        var specification = new InstitutionSummariesSpecification();
        var institutions = await Repository.ListAsync(specification, cancellationToken).ConfigureAwait(false);
        return Mapper.Map<IReadOnlyCollection<InstitutionSummaryDto>>(institutions);
    }

    public async Task<IReadOnlyCollection<InstitutionListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var specification = new InstitutionListSpecification();
        var institutions = await Repository.ListAsync(specification, cancellationToken).ConfigureAwait(false);
        return Mapper.Map<IReadOnlyCollection<InstitutionListDto>>(institutions);
    }

    public async Task<InstitutionDetailDto> GetByIdAsync(Guid institutionId, CancellationToken cancellationToken = default)
    {
        if (institutionId == Guid.Empty)
        {
            throw new ArgumentException("Institution identifier must be provided.", nameof(institutionId));
        }

        var specification = new InstitutionDetailSpecification(institutionId);
        var institution = await Repository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (institution == null)
        {
            throw new NotFoundException("Institution was not found.", ErrorCodes.TargetEntityNotFound);
        }

        EnsureNavigationBackReferences(institution);
        return Mapper.Map<InstitutionDetailDto>(institution);
    }

    private static void EnsureNavigationBackReferences(Institution institution)
    {
        if (institution.Courses != null)
        {
            foreach (var course in institution.Courses)
            {
                course.Institution ??= institution;
            }
        }

        if (institution.Professors != null)
        {
            foreach (var professor in institution.Professors)
            {
                professor.Institution ??= institution;
            }
        }
    }

    private sealed class InstitutionSummariesSpecification : Specification<Institution>
    {
        public InstitutionSummariesSpecification()
        {
            ApplyOrderBy(institution => institution.Name);
            DisableTracking();
        }
    }

    private sealed class InstitutionListSpecification : Specification<Institution>
    {
        public InstitutionListSpecification()
        {
            AddInclude(institution => institution.Reviews);
            ApplyOrderBy(institution => institution.Name);
            DisableTracking();
        }
    }

    private sealed class InstitutionDetailSpecification : Specification<Institution>
    {
        public InstitutionDetailSpecification(Guid institutionId)
            : base(institution => institution.Id == institutionId)
        {
            AddInclude(institution => institution.Courses);
            AddInclude(institution => institution.Professors);
            AddInclude(institution => institution.Reviews);
            ApplyOrderBy(institution => institution.Name);
            DisableTracking();
        }
    }
}
