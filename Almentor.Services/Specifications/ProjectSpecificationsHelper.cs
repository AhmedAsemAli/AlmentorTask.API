using Almentor.Domain.Entities;
using Almentor.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Almentor.Services.Specifications
{
    public static class ProjectSpecificationsHelper
    {
        public static Expression<Func<Project, bool>> GetCriteria(ProjectQueryParams queryParams)
        {
            return p => ((string.IsNullOrEmpty(queryParams.search)) || p.Name.ToLower().Contains(queryParams.search.ToLower()));

        }
    }
}
