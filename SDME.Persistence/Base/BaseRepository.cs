using Microsoft.EntityFrameworkCore;
using SDME.Domain.Base;
using SDME.Domain.Interfaces;
using SDME.Persistence.Context;
using System.Collections.Generic;

namespace SDME.Persistence.Base
{
    /// Repositorio genérico base con operaciones CRUD
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        protected readonly SDMEDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(SDMEDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
