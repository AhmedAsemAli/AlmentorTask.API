using Almentor.Domain.Contracts;
using Almentor.Domain.Entities;
using Almentor.Services.Abstraction;
using Almentor.Services.Exceptions;
using Almentor.Services.Specifications;
using Almentor.Shared;
using Almentor.Shared.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProjectDto?> CreateAsync(CreateProjectDto dto)
        {

            var repo = _unitOfWork.GetRepository<Project>();

            var projects = await repo.GetAllAsync();

            if (projects.Any(p => p.Name.ToLower() == dto.Name.ToLower()))
            {
                throw new DuplicateProjectNameException(dto.Name);
            }

            var project = _mapper.Map<Project>(dto);

            await repo.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task DeleteAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Project>();

            var project = await repo.GetByIdAsync(id);

            if (project is null)
                throw new ProjectNotFoundException(id);
            repo.Delete(project);

            await _unitOfWork.SaveChangesAsync();

        }
        public async Task<PaginatedResult<ProjectDto>> GetAllAsync(ProjectQueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepository<Project>();
            var spec = new ProjectSpecifications(queryParams);
            var projects = await repo.GetAllAsync(spec);
            var projectsWithCointSpc = new ProjectWithCountSpecification(queryParams);
            var TotalCount = await repo.CountAsync(projectsWithCointSpc);
            var DataToReturn = _mapper.Map<IEnumerable<ProjectDto>>(projects);
            var countOfReturnedData = DataToReturn.Count();
            return new PaginatedResult<ProjectDto>(queryParams.PageIndex, countOfReturnedData, TotalCount, DataToReturn);

        }

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _unitOfWork
          .GetRepository<Project>()
          .GetByIdAsync(id);

            if (project is null)
                throw new ProjectNotFoundException(id);

            return _mapper.Map<ProjectDto?>(project);

            
        }

        public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
        {
            var repo = _unitOfWork.GetRepository<Project>();

            var project = await repo.GetByIdAsync(id);

            if (project is null)
                throw new ProjectNotFoundException(id);
            var projects = await repo.GetAllAsync();

            if (projects.Any(p =>
                p.Id != id &&
                p.Name.ToLower() == dto.Name.ToLower()))
            {
                throw new DuplicateProjectNameException(dto.Name);
            }
            project.Name = dto.Name;
            project.Description = dto.Description;
            project.UpdatedAt = DateTime.UtcNow;

            repo.Update(project);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProjectDto>(project);
          
        }
    }
}
