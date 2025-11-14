using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Data.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IAsyncRepository<User> Users { get; }

    IAsyncRepository<Institution> Institutions { get; }

    IAsyncRepository<Course> Courses { get; }

    IAsyncRepository<Professor> Professors { get; }

    IAsyncRepository<Subject> Subjects { get; }

    IAsyncRepository<Review> Reviews { get; }

    IAsyncRepository<Comment> Comments { get; }

    IAsyncRepository<ChangeRequest> ChangeRequests { get; }

    IAsyncRepository<TEntity> Repository<TEntity>()
        where TEntity : class;

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
