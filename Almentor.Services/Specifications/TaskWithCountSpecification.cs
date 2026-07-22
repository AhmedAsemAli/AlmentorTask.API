using Almentor.Domain.Entities;
using Almentor.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.Specifications
{
    public class TaskWithCountSpecification : BaseSpecifications<TaskItem>
    {

        public TaskWithCountSpecification(int? projectId, TaskQueryParams queryParams)
            : base(x =>
                (!projectId.HasValue || x.ProjectId == projectId.Value) &&

                (!queryParams.Status.HasValue ||
                 x.Status == queryParams.Status.Value) &&

                (!queryParams.Priority.HasValue ||
                 x.Priority == queryParams.Priority.Value) &&

                (!queryParams.DueDateFrom.HasValue ||
                 (x.DueDate.HasValue &&
                  x.DueDate.Value.Date >= queryParams.DueDateFrom.Value.Date)) &&

                (!queryParams.DueDateTo.HasValue ||
                 (x.DueDate.HasValue &&
                  x.DueDate.Value.Date <= queryParams.DueDateTo.Value.Date)) &&

                (string.IsNullOrWhiteSpace(queryParams.q) ||

                 x.Title.ToLower().Contains(queryParams.q.ToLower()) ||

                 (x.Description != null &&
                  x.Description.ToLower().Contains(queryParams.q.ToLower())))
            )
        { }

    }
}

