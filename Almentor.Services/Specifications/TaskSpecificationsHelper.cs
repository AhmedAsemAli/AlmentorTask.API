using Almentor.Domain.Entities;
using Almentor.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Almentor.Services.Specifications
{
    public static class TaskSpecificationsHelper
    {
        public static Expression<Func<TaskItem, bool>> GetCriteria(TaskQueryParams queryParams)
        {

            return task =>

                // Status
                (!queryParams.Status.HasValue ||
                 task.Status == queryParams.Status.Value)

                &&

                // Priority
                (!queryParams.Priority.HasValue ||
                 task.Priority == queryParams.Priority.Value)

                &&

                // Due Date From
                (!queryParams.DueDateFrom.HasValue ||
                 (task.DueDate.HasValue &&
                  task.DueDate.Value.Date >= queryParams.DueDateFrom.Value.Date))

                &&

                // Due Date To
                (!queryParams.DueDateTo.HasValue ||
                 (task.DueDate.HasValue &&
                  task.DueDate.Value.Date <= queryParams.DueDateTo.Value.Date))

                &&

                // Search
                (string.IsNullOrWhiteSpace(queryParams.q) ||

                 task.Title.ToLower().Contains(queryParams.q.ToLower()) ||

                 (task.Description != null &&
                task.Description.ToLower().Contains(queryParams.q.ToLower())));
        }
    }
}
