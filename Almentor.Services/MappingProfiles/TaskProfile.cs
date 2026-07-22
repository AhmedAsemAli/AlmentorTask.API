using Almentor.Domain.Entities;
using Almentor.Shared.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.MappingProfiles
{
    public class TaskProfile:Profile
    {
        public TaskProfile()
        {
            // Create Task
            CreateMap<CreateTaskDto, TaskItem>();

            // Update Task
            CreateMap<UpdateTaskDto, TaskItem>();

            // Entity -> DTO
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.ProjectName,
                    opt => opt.MapFrom(src => src.Project.Name));
        }
    }
}
