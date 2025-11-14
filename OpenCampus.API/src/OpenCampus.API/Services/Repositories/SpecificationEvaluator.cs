using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OpenCampus.API.Services.Repositories;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> specification)
        where T : class
    {
        if (specification == null)
        {
            return inputQuery;
        }

        var query = specification.AsNoTracking ? inputQuery.AsNoTracking() : inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.WhereExpressions.Count > 0)
        {
            foreach (var predicate in specification.WhereExpressions)
            {
                query = query.Where(predicate);
            }
        }

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (specification.IncludeStrings.Count > 0)
        {
            foreach (var include in specification.IncludeStrings)
            {
                if (!string.IsNullOrWhiteSpace(include))
                {
                    query = query.Include(include);
                }
            }
        }

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.Skip.HasValue || specification.Take.HasValue)
        {
            query = EnsureOrdering(query, specification);
        }

        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }

    public static IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        where T : class
    {
        if (specification == null)
        {
            return inputQuery.Cast<TResult>();
        }

        var query = GetQuery(inputQuery, (ISpecification<T>)specification);

        if (specification.Selector == null)
        {
            throw new InvalidOperationException("Specification selector must be provided for projections.");
        }

        return query.Select(specification.Selector);
    }

    private static IQueryable<T> EnsureOrdering<T>(IQueryable<T> query, ISpecification<T> specification)
        where T : class
    {
        if (specification.OrderBy != null || specification.OrderByDescending != null)
        {
            return query;
        }

        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            throw new InvalidOperationException("Paging operations require an explicit order or an entity identifier to maintain deterministic results.");
        }

        return query.OrderBy(e => EF.Property<object>(e, idProperty.Name));
    }
}
