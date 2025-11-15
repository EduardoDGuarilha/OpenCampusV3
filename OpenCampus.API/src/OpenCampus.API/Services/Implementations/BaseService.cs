using System;
using AutoMapper;
using OpenCampus.API.Data.UnitOfWork;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Services.Implementations;

public abstract class BaseService<TEntity>
    where TEntity : class
{
    protected BaseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected IUnitOfWork UnitOfWork { get; }

    protected IMapper Mapper { get; }

    protected IAsyncRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();
}
