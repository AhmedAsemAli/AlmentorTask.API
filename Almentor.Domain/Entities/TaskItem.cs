using Almentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using TaskStatus = Almentor.Domain.Enums.TaskStatus;

namespace Almentor.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } 
       
        // Navigation Property
        public Project Project { get; set; } = null!;
    }
}
