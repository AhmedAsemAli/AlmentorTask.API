using Almentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Shared.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; } = Domain.Enums.TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
    }
}
