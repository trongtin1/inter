using test.Project.Domain.Common;
using test.Project.Domain.Entity;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IModuleRepository : IGenericRepository<Module>
    {
        Task<Module> GetOne(string? name);
        Task<List<Module>> GetList(string? name);
        Task<(IEnumerable<Module> Items, int TotalCount)> GetPage(
            string? name,
            int? pageIndex,
            int? pageSize);
    }
} 