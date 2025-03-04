using test.Project.Domain.Common;
using test.Project.Domain.Entity;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<bool> IsNameExistsAsync(string name, int? excludeId = null);
        Task<Role> GetOne(string? name);
        Task<List<Role>> GetList(string? name);

        Task<(IEnumerable<Role> Items, int TotalCount)> GetPage(
            string? name,
            int pageIndex,
            int pageSize);
    }
} 