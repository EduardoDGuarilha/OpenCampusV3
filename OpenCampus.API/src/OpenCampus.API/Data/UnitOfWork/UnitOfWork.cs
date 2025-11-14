using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenCampus.API.Data;
using OpenCampus.API.Entities;
using OpenCampus.API.Services.Repositories;

namespace OpenCampus.API.Data.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private static readonly IReadOnlyDictionary<Type, Type> CustomRepositories = new Dictionary<Type, Type>
    {
        { typeof(Review), typeof(ReviewRepository) }
    };

    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories = new();
    private readonly object _syncRoot = new();

    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IAsyncRepository<User> Users => Repository<User>();

    public IAsyncRepository<Institution> Institutions => Repository<Institution>();

    public IAsyncRepository<Course> Courses => Repository<Course>();

    public IAsyncRepository<Professor> Professors => Repository<Professor>();

    public IAsyncRepository<Subject> Subjects => Repository<Subject>();

    public IAsyncRepository<Review> Reviews => Repository<Review>();

    public IAsyncRepository<Comment> Comments => Repository<Comment>();

    public IAsyncRepository<ChangeRequest> ChangeRequests => Repository<ChangeRequest>();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("There is no active transaction to commit.");
        }

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
        finally
        {
            await DisposeCurrentTransactionAsync().ConfigureAwait(false);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await DisposeCurrentTransactionAsync().ConfigureAwait(false);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _dbContext.ChangeTracker.DetectChanges();
        return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public IAsyncRepository<TEntity> Repository<TEntity>()
        where TEntity : class
    {
        var entityType = typeof(TEntity);

        if (_repositories.TryGetValue(entityType, out var repository))
        {
            return (IAsyncRepository<TEntity>)repository;
        }

        lock (_syncRoot)
        {
            if (!_repositories.TryGetValue(entityType, out repository))
            {
                repository = CreateRepositoryInstance(entityType);
                _repositories[entityType] = repository;
            }

            return (IAsyncRepository<TEntity>)repository;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync().ConfigureAwait(false);
            _currentTransaction = null;
        }

        await _dbContext.DisposeAsync().ConfigureAwait(false);
        _disposed = true;
    }

    private async Task DisposeCurrentTransactionAsync()
    {
        if (_currentTransaction is null)
        {
            return;
        }

        await _currentTransaction.DisposeAsync().ConfigureAwait(false);
        _currentTransaction = null;
    }

    private object CreateRepositoryInstance(Type entityType)
    {
        if (CustomRepositories.TryGetValue(entityType, out var repositoryType))
        {
            return ActivatorUtilities.CreateInstance(_serviceProvider, repositoryType, _dbContext);
        }

        var genericRepositoryType = typeof(EfRepository<>).MakeGenericType(entityType);
        return ActivatorUtilities.CreateInstance(_serviceProvider, genericRepositoryType, _dbContext);
    }
}
