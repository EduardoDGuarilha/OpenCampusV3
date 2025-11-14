using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenCampus.API.Services.Repositories;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }

    IReadOnlyCollection<Expression<Func<T, bool>>> WhereExpressions { get; }

    IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }

    IReadOnlyCollection<string> IncludeStrings { get; }

    Expression<Func<T, object>>? OrderBy { get; }

    Expression<Func<T, object>>? OrderByDescending { get; }

    int? Take { get; }

    int? Skip { get; }

    bool AsNoTracking { get; }
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Selector { get; }
}
