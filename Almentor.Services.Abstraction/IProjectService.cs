using Almentor.Shared;
using Almentor.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.Abstraction
{
    public interface IProjectService
    {
        // 1. Create a project
        Task<ProjectDto?> CreateAsync(CreateProjectDto dto);

        // 2. List all projects (paginated)
        Task<PaginatedResult<ProjectDto>> GetAllAsync(ProjectQueryParams queryParams);

        // 3. Get a single project
        Task<ProjectDto?> GetByIdAsync(int id);

        // 4. Update a project
        Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto);

        // 5. Delete a project (cascade deletes all its tasks)
        Task DeleteAsync(int id);

        
    }
}

