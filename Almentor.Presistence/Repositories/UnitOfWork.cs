using Almentor.Domain.Contracts;
using Almentor.Presistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Presistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out var repository))
            {
                return (IGenericRepository<TEntity>)repository;
            }

            var newRepo = new GenericRepository<TEntity>(_dbContext);
            _repositories[entityType] = newRepo;
            return newRepo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
