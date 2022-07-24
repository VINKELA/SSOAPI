using Microsoft.EntityFrameworkCore;
using SSOService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SSOService.Services.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private bool _disposed;
        //inject Audit service
        public GenericRepository(DbContext context)
        {
            _context = context;
            _disposed = false;
        }

        public bool Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            var result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<bool> AddRangeAsync(IList<T> entity)
        {
            _context.Set<T>().AddRange(entity);
            var result = await _context.SaveChangesAsync() > 0;
            return result;
        }
        public async Task<bool> UpdateRangeAsync(IList<T> entity)
        {
            _context.Set<T>().UpdateRange(entity);
            var result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public void Delete(T t)
        {
            _context.Set<T>().Remove(t);
            _context.SaveChanges();
        }

        public async Task<int> DeleteAsync(T t)
        {
            _context.Set<T>().Remove(t);
            return await _context.SaveChangesAsync();
        }

        public bool Exist(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> exist = _context.Set<T>().Where(predicate);
            return exist.Any();
        }

        public IEnumerable<T> Filter(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>,
            IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (includeProperties != null)
            {
                foreach (string includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim()))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().SingleOrDefault(match);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(match);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().Where(match).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public ICollection<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public T GetById(long id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public T GetById_IsGuid(Guid id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetById_IsGuid_Async(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public T GetByUniqueId(string id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByUniqueIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }

        public bool Update(T updated)
        {
            if (updated is null)
            {
                return false;
            }

            _context.Set<T>().Attach(updated);
            _context.Entry(updated).State = EntityState.Modified;
            return _context.SaveChanges() > 0;

        }

        public async Task<bool> UpdateAsync(T updated)
        {
            var status = false;
            if (updated is null)
            {
                return status;
            }

            _context.Set<T>().Attach(updated);
            _context.Entry(updated).State = EntityState.Modified;
            status = await _context.SaveChangesAsync() > 0;
            return status;

        }

        /*        async Task<List<T>> IGenericRepository<T>.GetAllByList()
                {
                    return await _context.Set<T>().ToListAsync();
                }
        */
        public async Task<int> CountAsyncFilter(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }
        /*        public async Task<PagedList<T>> GetPaginatedList(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int page = 1, int pageSize = 20)
                {
                    PagedList<T> res = new PagedList<T>
                    {
                        PageSize = pageSize
                    };
                    IQueryable<T> query = _context.Set<T>();
                    if (filter != null)
                    {
                        query = query.Where(filter);
                        res.TotalCount = await query.CountAsync();

                    }

                    if (orderBy != null)
                    {
                        query = orderBy(query);
                    }
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);


                    res.Data.AddRange(query);

                    return res;
                }
        */
        public IQueryable<T> FilterAsNoTracking(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty).AsNoTracking();
                }
            }

            return query;
        }

        public T FirstOrDefaultInclude(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault(filter);
        }

        /*        public virtual IEnumerable<TEntity> SqlQuery<TEntity>(String sql, params object[] parameters) where TEntity : new()
                {
                    return _context.Database.GetModelFromQuery<TEntity>(sql, parameters);
                }
        */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }


    }
}
