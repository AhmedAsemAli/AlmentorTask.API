using Almentor.Services.Abstraction;
using Almentor.Shared;
using Almentor.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Presentation.Controllers
{
    public class ProjectController:ApiBaseController
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // POST: api/projects
        [HttpPost]
     
        public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
        {
            var project = await _projectService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = project!.Id },
                project);
        }

        // GET: api/projects
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

        // PUT: api/projects/5
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

        // DELETE: api/projects/5
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


    }
}

