using Almentor.Domain.Entities;
using Almentor.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.Specifications
{
    public class ProjectWithCountSpecification : BaseSpecifications<Project>
    {
        public ProjectWithCountSpecification(ProjectQueryParams queryParams) : base(ProjectSpecificationsHelper.GetCriteria(queryParams))
        {

        }
    }
}
