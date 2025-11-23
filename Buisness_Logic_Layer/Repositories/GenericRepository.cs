using Buisness_Logic_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context ;
            _dbSet = _context.Set<T>();
        }


        public async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            { 
                throw new ArgumentNullException(nameof(entities)); 
            }
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null)
        {
            if(criteria==null)
            {
                return await _dbSet.CountAsync();
            }
            else
            {
                return await _dbSet.CountAsync(criteria);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
            { 
                throw new ArgumentNullException(nameof(entity)); 
            }
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _dbSet.RemoveRange(entities);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<IEnumerable<T>> FindAllAsync(int? skip = null, int? take = null, Expression<Func<T, object>>? orderBy = null, bool isDesc = false, Expression<Func<T, bool>>? criteria = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (criteria != null)
                query = query.Where(criteria);

            if (orderBy != null)
                query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? criteria = null, string[]? includes = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    if (!string.IsNullOrWhiteSpace(include))
                        query = query.Include(include);
                }
            }

            if (criteria != null)
                query = query.Where(criteria);

            return await query.ToListAsync();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>>? criteria = null, string[]? includes = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    if (!string.IsNullOrWhiteSpace(include))
                        query = query.Include(include);
                }
            }

            if (criteria != null)
                query = query.Where(criteria);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            // FindAsync handles primary key lookup; returns null if not found
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<string>> GetDistinctAsync(Expression<Func<T, string>> column)
        {
            if (column == null) throw new ArgumentNullException(nameof(column));
            return await _dbSet.Select(column).Distinct().ToListAsync();
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));
            return await _dbSet.AnyAsync(criteria);
        }

        public async Task<T?> LastAsync(Expression<Func<T, object>> column, Expression<Func<T, bool>>? criteria = null)
        {
            if (column == null) throw new ArgumentNullException(nameof(column));

            IQueryable<T> query = _dbSet;
            if (criteria != null)
                query = query.Where(criteria);

            return await query.OrderByDescending(column).FirstOrDefaultAsync();
        }

        public async Task<long> MaxAsync(Expression<Func<T, object>> column)
        {
            if (column == null) throw new ArgumentNullException(nameof(column));

            var maxObj = await _dbSet.OrderByDescending(column).Select(column).FirstOrDefaultAsync();
            if (maxObj == null) return 0L;
            return Convert.ToInt64(maxObj);
        }

        public async Task<long> MaxAsync(Expression<Func<T, object>> column, Expression<Func<T, bool>>? criteria = null)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            IQueryable<T> query = _dbSet;
            if (criteria != null)
                query = query.Where(criteria);

            var maxObj = await query.OrderByDescending(column).Select(column).FirstOrDefaultAsync();
            if (maxObj == null) return 0L;
            return Convert.ToInt64(maxObj);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _dbSet.UpdateRange(entities);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}
