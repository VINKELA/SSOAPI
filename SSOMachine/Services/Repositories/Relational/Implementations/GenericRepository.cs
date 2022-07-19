using Microsoft.EntityFrameworkCore;
using SSOService.Services.Interfaces;
using System;
using System.Collections.Generic;
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

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IList<T>> AddRangeAsync(IList<T> entity)
        {
            _context.Set<T>().AddRange(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<IList<T>> UpdateRangeAsync(IList<T> entity)
        {
            _context.Set<T>().UpdateRange(entity);
            await _context.SaveChangesAsync();
            return entity;
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
