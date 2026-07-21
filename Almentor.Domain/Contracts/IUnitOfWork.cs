using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;
    }
}
