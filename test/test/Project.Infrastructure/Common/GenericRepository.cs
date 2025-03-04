using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using test.Project.Domain.Common;
using test.Project.Infrastructure.Data;

namespace test.Project.Infrastructure.Common
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public virtual async Task<T?> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            //await _context.SaveChangesAsync();
            return entity;
        }
        //public virtual async Task<IEnumerable<T?>> GetAllNoQuery()
        //{
        //    return await _dbSet.ToListAsync();
        //}
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<T?> GetOneAsync(Expression<Func<T?, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            //await _context.SaveChangesAsync();
        }
        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            //await _context.SaveChangesAsync();
        }
        public virtual async Task<T?> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            //await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> FindPagedListAsync(
            Expression<Func<T, bool>> predicate,
            int? pageIndex = 1,
            int? pageSize = 10)
        {
            var query = _dbSet.Where(predicate);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();
                
            return (items, totalCount);
        }

   
    }
}
