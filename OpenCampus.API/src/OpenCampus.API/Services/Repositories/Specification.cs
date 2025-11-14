using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenCampus.API.Services.Repositories;

public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, bool>>> _whereExpressions = new();
    private readonly List<Expression<Func<T, object>>> _includes = new();
    private readonly List<string> _includeStrings = new();

    protected Specification(Expression<Func<T, bool>>? criteria = null)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    public IReadOnlyCollection<Expression<Func<T, bool>>> WhereExpressions => _whereExpressions;

    public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes;

    public IReadOnlyCollection<string> IncludeStrings => _includeStrings;

    public Expression<Func<T, object>>? OrderBy { get; protected set; }

    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

    public int? Take { get; protected set; }

    public int? Skip { get; protected set; }

    public bool AsNoTracking { get; protected set; }

    protected void AddInclude(Expression<Func<T, object>> includeExpression) => _includes.Add(includeExpression);

    protected void AddInclude(string includeString) => _includeStrings.Add(includeString);

    protected void AddWhere(Expression<Func<T, bool>> predicate) => _whereExpressions.Add(predicate);

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
        OrderByDescending = null;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
        OrderBy = null;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    protected void ApplySkip(int skip) => Skip = skip;

    protected void ApplyTake(int take) => Take = take;

    protected void DisableTracking() => AsNoTracking = true;
}

public abstract class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
{
    protected Specification(Expression<Func<T, bool>>? criteria = null)
        : base(criteria)
    {
    }

    public Expression<Func<T, TResult>>? Selector { get; protected set; }

    protected void ApplySelector(Expression<Func<T, TResult>> selector)
    {
        Selector = selector;
    }
}
