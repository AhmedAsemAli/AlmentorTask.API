using Almentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Shared.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
