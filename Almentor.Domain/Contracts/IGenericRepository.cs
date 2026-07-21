using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Domain.Contracts
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity> specifications);
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity?> GetByIdAsync(ISpecifications<TEntity> specifications);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> CountAsync(ISpecifications<TEntity> specifications);
    }
}
