using Microsoft.EntityFrameworkCore;
using test.Project.Domain.RepoInterfaces;
using test.Project.Domain.Entity;
using test.Project.Infrastructure.Data;
using test.Project.Infrastructure.Common;
using System.Linq.Expressions;
namespace test.Project.Infrastructure.Repositories
{
    public class ModuleRepository : GenericRepository<Module>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<(IEnumerable<Module> Items, int TotalCount)> GetPage(
            string? name,
            int? pageIndex,
            int? pageSize)
        {
            return await FindPagedListAsync(x =>
                (string.IsNullOrEmpty(name) || x.Name.Contains(name)), pageIndex, pageSize);
        }

        public async Task<Module> GetOne(string? name)
        {
            return await GetOneAsync(u =>
                (string.IsNullOrEmpty(name) || u.Name == name));
        }

        public async Task<List<Module>> GetList(string? name)
        {
            return await GetListAsync(u =>
                (string.IsNullOrEmpty(name) || u.Name == name));
        }

        // public async Task<List<Module>> GetAllAsync()
        // {
        //     return await _context.Module.ToListAsync();
        // }

        // public async Task<Module> GetByIdAsync(int id)
        // {
        //     return await _context.Module.FindAsync(id);
        // }

        // public async Task<Module> CreateAsync(Module module)
        // {
        //     _context.Module.Add(module);
        //     await _context.SaveChangesAsync();
        //     return module;
        // }

        // public async Task<Module> UpdateAsync(Module module)
        // {
        //     _context.Module.Update(module);
        //     await _context.SaveChangesAsync();
        //     return module;
        // }

        // public async Task DeleteAsync(Module module)
        // {
        //     _context.Module.Remove(module);
        //     await _context.SaveChangesAsync();
        // }
    }
} 