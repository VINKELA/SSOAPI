using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSOService.Services.Interfaces
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {

        Task<T> AddAsync(T entity);
        Task<IList<T>> AddRangeAsync(IList<T> entity);

        Task<IList<T>> UpdateRangeAsync(IList<T> entity);
        void Delete(T t);

    }
}
