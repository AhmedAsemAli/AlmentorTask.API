using Almentor.Domain.Entities;
using Almentor.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.Specifications
{
    public class TaskSpecifications : BaseSpecifications<TaskItem>
    {
        //public TaskSpecifications(TaskQueryParams queryParams)
        //   : base(TaskSpecificationsHelper.GetCriteria(queryParams))
        //{
        //    AddInclude(p => p.Project);
        //    switch (queryParams.sort)
        //    {
        //        case TaskSortingOptions.due_date:
        //            AddOrderBy(x => x.DueDate);
        //            break;

        //        case TaskSortingOptions.priority:
        //            AddOrderByDescending(x => x.Priority);
        //            break;

        //        case TaskSortingOptions.created_at_Asc:
        //            AddOrderBy(x => x.CreatedAt);
        //            break;

        //        case TaskSortingOptions.created_at_Desc:
        //            AddOrderByDescending(x => x.CreatedAt);
        //            break;

        //        default:
        //            AddOrderByDescending(x => x.CreatedAt);
        //            break;
        //    }
        //    ApplyPagination(queryParams.PageIndex, queryParams.PageSize);
        //}


        //public TaskSpecifications(int id) : base(p => p.Id == id)
        //{
        //    AddInclude(p => p.Project);
        //}



        public TaskSpecifications(int? projectId, TaskQueryParams queryParams)
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

                (string.IsNullOrWhiteSpace(queryParams.search) ||

                 x.Title.Contains(queryParams.search) ||

                 (x.Description != null &&
                  x.Description.Contains(queryParams.search)))
            )
        {
            AddInclude(x => x.Project);

            switch (queryParams.sort)
            {
                case TaskSortingOptions.due_date:
                    AddOrderBy(x => x.DueDate);
                    break;

                case TaskSortingOptions.priority:
                    AddOrderByDescending(x => x.Priority);
                    break;

                case TaskSortingOptions.created_at_Asc:
                    AddOrderBy(x => x.CreatedAt);
                    break;

                case TaskSortingOptions.created_at_Desc:
                    AddOrderByDescending(x => x.CreatedAt);
                    break;

                default:
                    AddOrderByDescending(x => x.CreatedAt);
                    break;
            }

            ApplyPagination(queryParams.PageIndex, queryParams.PageSize);
        }

        public TaskSpecifications(int id)
            : base(x => x.Id == id)
        {
            AddInclude(x => x.Project);
        }
    }
}
