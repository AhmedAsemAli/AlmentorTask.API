using Almentor.Domain.Contracts;
using Almentor.Domain.Entities;
using Almentor.Services.Abstraction;
using Almentor.Services.Specifications;
using Almentor.Shared;
using Almentor.Shared.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Almentor.Domain.Enums;
using TaskStatus = Almentor.Domain.Enums.TaskStatus;
namespace Almentor.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskService> _logger;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<TaskDto?> CreateTaskForProjectAsync(int projectId, CreateTaskDto dto)
        {
            if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                throw new Exception("Due date cannot be in the past.");
            }

            var projectRepo = _unitOfWork.GetRepository<Project>();

            var project = await projectRepo.GetByIdAsync(projectId);

            

            if (project is null)
                return null;

            var task = _mapper.Map<TaskItem>(dto);
            task.ProjectId = projectId;

            await _unitOfWork.GetRepository<TaskItem>().AddAsync(task);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<TaskItem>();

            var task = await repo.GetByIdAsync(id);

            if (task is null)
                return false;

            repo.Delete(task);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<TaskItem>();

            var task = await repo.GetByIdAsync(id);

            if (task is null)
                return null;

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<PaginatedResult<TaskDto>> GetTasksAsync(
            int? projectId,
            TaskQueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepository<TaskItem>();

            var spec = new TaskSpecifications(projectId, queryParams);

            var tasks = await repo.GetAllAsync(spec);

            var countSpec = new TaskWithCountSpecification(projectId, queryParams);

            var totalCount = await repo.CountAsync(countSpec);

            var data = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            return new PaginatedResult<TaskDto>(
                queryParams.PageIndex,
                data.Count(),
                totalCount,
                data);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto dto)
        {
            if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                throw new Exception("Due date cannot be in the past.");
            }
            var repo = _unitOfWork.GetRepository<TaskItem>();

            var task = await repo.GetByIdAsync(id);

            if (task is null)
                return null;

            task.Title = dto.Title;
            task.Description = dto.Description;
            if (task.Status == Domain.Enums.TaskStatus.Done && dto.Status == TaskStatus.Todo)
            {
                _logger.LogWarning(
                    "Unusual task status transition detected. TaskId: {TaskId}, Transition: Done -> Todo",
                    task.Id
                );
            }
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            repo.Update(task);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }
    }
}
