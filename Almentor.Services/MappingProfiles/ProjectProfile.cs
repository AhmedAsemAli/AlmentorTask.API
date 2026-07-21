using Almentor.Domain.Entities;
using Almentor.Shared.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.MappingProfiles
{
    public class ProjectProfile:Profile
    {
        public ProjectProfile()
        {
            CreateMap<CreateProjectDto, Project>();
            CreateMap<Project, ProjectDto>().ReverseMap();
        }
    }
}
