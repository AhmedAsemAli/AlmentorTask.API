using Almentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Shared
{
    public class TaskQueryParams
    {
        public Domain.Enums.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public string? search { get; set; } // Search query

        //public string? SortBy { get; set; } = "created_at"; // due_date, priority, created_at

        
        public TaskSortingOptions sort { get; set; }

        private int _pageIndex = 1;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = (value <= 0) ? 1 : value; }
        }



        private const int MaxPageSize = 10;
        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                    _pageSize = 5;
                else if (value >= 10)
                    _pageSize = MaxPageSize;
                else
                    _pageSize = value;
            }
        }

    }
}
