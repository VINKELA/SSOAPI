using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SSOService.Services.Interfaces
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {

        Task<bool> AddAsync(T entity);
        Task<bool> AddRangeAsync(IList<T> entity);

        Task<bool> UpdateRangeAsync(IList<T> entity);
        void Delete(T t);
        IEnumerable<T> Filter(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>,
           IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int? page = null, int? pageSize = null);


    }
}
