using Microsoft.EntityFrameworkCore;
using test.Project.Domain.Entity;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.UserModule;
using test.Project.Application.DTOs.Request.UserModule;
using test.Project.Application.DTOs.Response.ModulePermission;
using test.Project.Domain.RepoInterfaces;

namespace test.Project.Infrastructure.Repositories
{
    public class UserModuleRepository : IUserModuleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModuleRes>> GetAllUserModulesAsync()
        {
            var users = await _context.User
                .Include(u => u.UserModules)
                    .ThenInclude(um => um.Module)
                .ToListAsync();

            return users.Select(user => new UserModuleRes
            {
                UserId = user.Id,
                UserName = user.Username,
                Modules = user.UserModules.Select(um => new ModulePermissionDTO
                {
                    ModuleId = um.ModuleId,
                    ModuleName = um.Module.Name,
                    CanCreate = um.CanCreate,
                    CanRead = um.CanRead,
                    CanUpdate = um.CanUpdate,
                    CanDelete = um.CanDelete
                }).ToList()
            });
        }

        public async Task<UserModuleRes> GetUserModulesByUserIdAsync(int userId)
        {
            return await _context.UserModule
                .Include(um => um.User)
                .Include(um => um.Module)
                .Where(um => um.UserId == userId)
                .GroupBy(um => new { um.UserId, UserName = um.User.Username })
                .Select(g => new UserModuleRes
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    Modules = g.Select(um => new ModulePermissionDTO
                    {
                        ModuleId = um.ModuleId,
                        ModuleName = um.Module.Name,
                        CanCreate = um.CanCreate,
                        CanRead = um.CanRead,
                        CanUpdate = um.CanUpdate,
                        CanDelete = um.CanDelete
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserModule> GetUserModuleAsync(int userId, int moduleId)
        {
            return await _context.UserModule
                .FirstOrDefaultAsync(um => um.UserId == userId && um.ModuleId == moduleId);
        }

        public async Task<UserModule> CreateUserModuleAsync(UserModuleReq userModuleReq)
        {
            var userModule = new UserModule
            {
                UserId = userModuleReq.UserId,
                ModuleId = userModuleReq.ModuleId,
                CanCreate = userModuleReq.CanCreate,
                CanRead = userModuleReq.CanRead,
                CanUpdate = userModuleReq.CanUpdate,
                CanDelete = userModuleReq.CanDelete
            };

            await _context.UserModule.AddAsync(userModule);
            return userModule;
        }

        public async Task<UserModule> UpdateUserModuleAsync(UserModuleReq userModuleReq)
        {
            var existingUserModule = await GetUserModuleAsync(userModuleReq.UserId, userModuleReq.ModuleId);
            if (existingUserModule != null)
            {
                existingUserModule.CanCreate = userModuleReq.CanCreate;
                existingUserModule.CanRead = userModuleReq.CanRead;
                existingUserModule.CanUpdate = userModuleReq.CanUpdate;
                existingUserModule.CanDelete = userModuleReq.CanDelete;
                _context.UserModule.Update(existingUserModule);
            }
            return existingUserModule;
        }

        public async Task<IEnumerable<User>> GetUsersWithModulesAsync()
        {
            return await _context.User
                .Include(u => u.UserModules)
                    .ThenInclude(um => um.Module)
                .ToListAsync();
        }
    }
}