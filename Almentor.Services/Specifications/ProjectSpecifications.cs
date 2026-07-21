using Almentor.Domain.Entities;
using Almentor.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Almentor.Services.Specifications
{
    public class ProjectSpecifications : BaseSpecifications<Project>
    {
        public ProjectSpecifications(ProjectQueryParams queryParams)
            : base(ProjectSpecificationsHelper.GetCriteria(queryParams))
        {
            AddInclude(p => p.Tasks);
            switch (queryParams.sort)
            {
                case ProjectSortingOptions.NameAsc:
                    AddOrderBy(p => p.Name);
                    break;
                case ProjectSortingOptions.NameDes:
                    AddOrderByDescending(p => p.Name);
                    break;
                default:
                    AddOrderBy(p => p.Id);
                    break;
            }
            ApplyPagination(queryParams.PageIndex, queryParams.PageSize);
        }


        public ProjectSpecifications(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Tasks);
        }
    }
}
