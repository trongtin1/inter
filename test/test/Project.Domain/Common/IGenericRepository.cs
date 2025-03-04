using System.Linq.Expressions;

namespace test.Project.Domain.Common
{
    public interface IGenericRepository<T> where T : class
    {
        //Task<IEnumerable<T?>> GetAllNoQuery();
        Task<List<T?>> GetListAsync(Expression<Func<T?, bool>> predicate);
        Task<T?> GetOneAsync(Expression<Func<T?, bool>> predicate);
        Task<T?> GetByIdAsync(int id);
        Task<T?> CreateAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T?> entities);
        Task<(IEnumerable<T?> Items, int TotalCount)> FindPagedListAsync(
            Expression<Func<T?, bool>> predicate,
            int? pageIndex,
            int? pageSize);
    }
}
