using Almentor.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Almentor.Services.Specifications
{
    public abstract class BaseSpecifications<TEntity> : ISpecifications<TEntity> where TEntity : class
    {
        protected BaseSpecifications(Expression<Func<TEntity, bool>> criteriaExp)
        {
            Criteria = criteriaExp;

        }
        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

        public Expression<Func<TEntity, bool>> Criteria { get; }

        public Expression<Func<TEntity, object>> OrderBy { private set; get; }

        public Expression<Func<TEntity, object>> OrderByDescending { private set; get; }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExp)
        {
            IncludeExpressions.Add(includeExp);
        }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExp)
        {
            OrderBy = orderByExp;
        }

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDesExp)
        {
            OrderByDescending = orderByDesExp;
        }





        public int Skip { get; private set; }

        public int Take { get; private set; }

        public bool IsPaginated { get; private set; }

        protected void ApplyPagination(int pageIndex, int pageSize)
        {
            IsPaginated = true;

            Skip = (pageIndex - 1) * pageSize;

            Take = pageSize;
        }
    }
}
