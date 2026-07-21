using Almentor.Domain.Entities;
using Almentor.Presistence.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Presistence.Data.DbContexts
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectConfigurations).Assembly);
        }


        public DbSet<Project> Projects {  get; set; }
        public DbSet<TaskItem> Tasks {  get; set; }
    }
}
