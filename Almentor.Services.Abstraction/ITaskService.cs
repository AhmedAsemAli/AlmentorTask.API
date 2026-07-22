using Almentor.Shared;
using Almentor.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.Abstraction
{
    public interface ITaskService
    {
        // 1. Create a task under a project
        Task<TaskDto?> CreateTaskForProjectAsync(int projectId, CreateTaskDto dto);

        // 2. List tasks for a specific project (paginated, filterable, sortable)
        // 3. List all tasks across all projects (paginated, filterable, sortable, searchable)
        
        Task<PaginatedResult<TaskDto>> GetTasksAsync(int? projectId, TaskQueryParams queryParams);

        // 4. Get a single task
        Task<TaskDto?> GetByIdAsync(int id);

        // 5. Update a task
        Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto dto);

        // 6. Delete a task
        Task DeleteTaskAsync(int id);
    }
}
