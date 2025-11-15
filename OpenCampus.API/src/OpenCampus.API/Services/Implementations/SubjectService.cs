using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.DTOs.Subjects;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Services.Implementations;

public class SubjectService : BaseService<Subject>, ISubjectService
{
    public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<IReadOnlyCollection<SubjectListDto>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course identifier must be provided.", nameof(courseId));
        }

        var course = await UnitOfWork.Courses.GetByIdAsync(courseId, cancellationToken).ConfigureAwait(false);
        if (course == null)
        {
            throw new NotFoundException("Course was not found.", ErrorCodes.TargetEntityNotFound);
        }

        var specification = new SubjectsByCourseSpecification(courseId);
        var subjects = await Repository.ListAsync(specification, cancellationToken).ConfigureAwait(false);
        return Mapper.Map<IReadOnlyCollection<SubjectListDto>>(subjects);
    }

    public async Task<SubjectDetailDto> GetByIdAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        if (subjectId == Guid.Empty)
        {
            throw new ArgumentException("Subject identifier must be provided.", nameof(subjectId));
        }

        var specification = new SubjectDetailSpecification(subjectId);
        var subject = await Repository.GetBySpecAsync(specification, cancellationToken).ConfigureAwait(false);
        if (subject == null)
        {
            throw new NotFoundException("Subject was not found.", ErrorCodes.TargetEntityNotFound);
        }

        return Mapper.Map<SubjectDetailDto>(subject);
    }

    private sealed class SubjectsByCourseSpecification : Specification<Subject>
    {
        public SubjectsByCourseSpecification(Guid courseId)
            : base(subject => subject.CourseId == courseId)
        {
            AddInclude(subject => subject.Course);
            AddInclude(subject => subject.Course.Institution);
            AddInclude(subject => subject.Reviews);
            ApplyOrderBy(subject => subject.Name);
            DisableTracking();
        }
    }

    private sealed class SubjectDetailSpecification : Specification<Subject>
    {
        public SubjectDetailSpecification(Guid subjectId)
            : base(subject => subject.Id == subjectId)
        {
            AddInclude(subject => subject.Course);
            AddInclude(subject => subject.Course.Institution);
            AddInclude(subject => subject.Reviews);
            DisableTracking();
        }
    }
}
