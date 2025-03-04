using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using test.Project.Domain.Entity;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Common;
using test.Project.Infrastructure.Data;

namespace test.Project.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<bool> IsNameExistsAsync(string name, int? excludeId = null)
        {
            return await _dbSet.AnyAsync(r => r.Name == name && (!excludeId.HasValue || r.Id != excludeId));
        }
        public async Task<Role> GetOne(string? name)
        {
            return await GetOneAsync(u =>
                (string.IsNullOrEmpty(name) || u.Name == name));
        }

        public async Task<List<Role>> GetList(string? name)
        {
            return await GetListAsync(u =>
                (string.IsNullOrEmpty(name) || u.Name == name));
        }

        public async Task<(IEnumerable<Role> Items, int TotalCount)> GetPage(
            string? name,
            int pageIndex,
            int pageSize)
        {
            Expression<Func<Role, bool>> predicate = x =>
                (string.IsNullOrEmpty(name) || x.Name.Contains(name));

            return await FindPagedListAsync(predicate, pageIndex, pageSize);
        }
    }
} 