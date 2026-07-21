using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Almentor.Domain.Contracts
{
    public interface ISpecifications<TEntity> where TEntity :class
    {
        ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        Expression<Func<TEntity, bool>> Criteria { get; }
        Expression<Func<TEntity, object>> OrderBy { get; }
        Expression<Func<TEntity, object>> OrderByDescending { get; }

        int Skip { get; }

        int Take { get; }

        bool IsPaginated { get; }
    }
}
