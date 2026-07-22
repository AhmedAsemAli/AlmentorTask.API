using Almentor.Services.Abstraction;
using Almentor.Shared;
using Almentor.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Presentation.Controllers
{
    public class ProjectsController:ApiBaseController
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;

        public ProjectsController(IProjectService projectService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        [HttpPost]
     
        public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
        {
            var project = await _projectService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = project!.Id },
                project);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ProjectDto>>> GetAll(
            [FromQuery] ProjectQueryParams queryParams)
        {
            var result = await _projectService.GetAllAsync(queryParams);

            return Ok(result);
        }

        // GET: api/projects/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);

            if (project is null)
                return NotFound(new
                {
                    Message = $"Project with id {id} was not found."
                });

            return Ok(project);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectDto>> Update(
            int id,
            UpdateProjectDto dto)
        {
            var project = await _projectService.UpdateAsync(id, dto);

            if (project is null)
                return NotFound(new
                {
                    Message = $"Project with id {id} was not found."
                });

            return Ok(project);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _projectService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new
                {
                    Message = $"Project with id {id} was not found."
                });

            return NoContent();
        }


        [HttpGet("/tasks{id:int}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);

            if (task is null)
            {
                return NotFound(new
                {
                    Message = $"Task with id {id} was not found."
                });
            }

            return Ok(task);
        }


        [HttpPost("{projectId:int}/tasks")]
        public async Task<ActionResult<TaskDto>> CreateTask(
            int projectId,
            [FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateTaskForProjectAsync(projectId, dto);

            if (task is null)
            {
                return NotFound(new
                {
                    Message = $"Project with id {projectId} was not found."
                });
            }

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = task.Id },
                task);
        }

        [HttpGet("{projectId:int}/tasks")]
        public async Task<ActionResult<PaginatedResult<TaskDto>>> GetProjectTasks(
            int projectId,
            [FromQuery] TaskQueryParams queryParams)
        {
            var result = await _taskService.GetTasksAsync(projectId, queryParams);

            return Ok(result);
        }

    }
}

