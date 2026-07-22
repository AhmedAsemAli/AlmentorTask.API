using Almentor.Services.Abstraction;
using Almentor.Shared;
using Almentor.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Presentation.Controllers
{
    public class TasksController:ApiBaseController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet()]
        public async Task<ActionResult<PaginatedResult<TaskDto>>> GetAllTasks(
            [FromQuery] TaskQueryParams queryParams)
        {
            var result = await _taskService.GetTasksAsync(null, queryParams);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
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

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(
            int id,
            UpdateTaskDto dto)
        {
            var task = await _taskService.UpdateTaskAsync(id, dto);

            if (task is null)
            {
                return NotFound(new
                {
                    Message = $"Task with id {id} was not found."
                });
            }

            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    Message = $"Task with id {id} was not found."
                });
            }

            return NoContent();
        }
    }
}
