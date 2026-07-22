using Almentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using TaskStatus = Almentor.Domain.Enums.TaskStatus;

namespace Almentor.Shared.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty; // Requirement: Each task must include project name
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
