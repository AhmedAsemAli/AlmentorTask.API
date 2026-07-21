using Almentor.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Presistence
{
    internal static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity>(IQueryable<TEntity> entryPoint, ISpecifications<TEntity> specifications) where TEntity : class
        {
            var Query = entryPoint;
            if (specifications is not null)
            {
                if (specifications.Criteria is not null)
                {
                    Query = Query.Where(specifications.Criteria);
                }
                if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Any())
                {
                    Query = specifications.IncludeExpressions.Aggregate(Query, (CurrentQuery, includeEx) => CurrentQuery.Include(includeEx));

                }

                if (specifications.OrderBy is not null)
                {
                    Query = Query.OrderBy(specifications.OrderBy);
                }

                if (specifications.OrderByDescending is not null)
                {
                    Query = Query.OrderByDescending(specifications.OrderByDescending);
                }
                if (specifications.IsPaginated == true)
                {
                    Query = Query.Skip(specifications.Skip).Take(specifications.Take);
                }

            }
            return Query;
        }
    }
}
