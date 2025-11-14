using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenCampus.API.Data;

namespace OpenCampus.API.Services.Repositories;

public class EfRepository<T> : IAsyncRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext DbContext;

    protected DbSet<T> DbSet => DbContext.Set<T>();

    public EfRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity;
    }

    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        return await ApplySpecification(specification).AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        DbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        DbSet.UpdateRange(entities);
    }

    public virtual void Delete(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        DbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        DbSet.RemoveRange(entities);
    }

    protected IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
    }

    protected IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
    }
}
