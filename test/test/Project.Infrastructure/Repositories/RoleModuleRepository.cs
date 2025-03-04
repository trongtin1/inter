using Microsoft.EntityFrameworkCore;
using test.Project.Domain.Entity;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.RoleModule;
using test.Project.Application.DTOs.Request.RoleModule;
using test.Project.Application.DTOs.Response.ModulePermission;

namespace test.Project.Infrastructure.Repositories
{
    public class RoleModuleRepository : IRoleModuleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //get all role modules
        public async Task<IEnumerable<RoleModuleRes>> GetAllRoleModulesAsync()
        {
            var roles = await _context.Role
                .Include(r => r.RoleModules)
                    .ThenInclude(rm => rm.Module)
                .Select(role => new RoleModuleRes
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Modules = role.RoleModules.Select(rm => new ModulePermissionDTO
                    {
                        ModuleId = rm.ModuleId,
                        ModuleName = rm.Module.Name,
                        CanCreate = rm.CanCreate,
                        CanRead = rm.CanRead,
                        CanUpdate = rm.CanUpdate,
                        CanDelete = rm.CanDelete
                    }).ToList()
                })
                .ToListAsync();

            return roles;
        }
        //get role modules permissions
        public async Task<RoleModuleRes> GetRoleModulesByRoleIdAsync(int roleId)
        {
            return await _context.RoleModule
                .Include(rm => rm.Role)
                .Include(rm => rm.Module)
                .Where(rm => rm.RoleId == roleId)
                .GroupBy(rm => new { rm.RoleId, RoleName = rm.Role.Name })
                .Select(g => new RoleModuleRes
                {
                    RoleId = g.Key.RoleId,
                    RoleName = g.Key.RoleName,
                    Modules = g.Select(rm => new ModulePermissionDTO
                    {
                        ModuleId = rm.ModuleId,
                        ModuleName = rm.Module.Name,
                        CanCreate = rm.CanCreate,
                        CanRead = rm.CanRead,
                        CanUpdate = rm.CanUpdate,
                        CanDelete = rm.CanDelete
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        //get role module permissions
        public async Task<RoleModule> GetRoleModuleAsync(int roleId, int moduleId)
        {
            return await _context.RoleModule
                .FirstOrDefaultAsync(rm => rm.RoleId == roleId && rm.ModuleId == moduleId);
        }
        //create role module permissions
        public async Task<RoleModule> CreateRoleModuleAsync(RoleModuleReq roleModuleReq)
        {
            var roleModule = new RoleModule
            {
                RoleId = roleModuleReq.RoleId,
                ModuleId = roleModuleReq.ModuleId,
                CanCreate = roleModuleReq.CanCreate,
                CanRead = roleModuleReq.CanRead,
                CanUpdate = roleModuleReq.CanUpdate,
                CanDelete = roleModuleReq.CanDelete
            };

            await _context.RoleModule.AddAsync(roleModule);
            return roleModule;
        }
        //update role module permissions
        public async Task<RoleModule> UpdateRoleModuleAsync(RoleModuleReq roleModuleReq)
        {
            var existingRoleModule = await GetRoleModuleAsync(roleModuleReq.RoleId, roleModuleReq.ModuleId);
            if (existingRoleModule != null)
            {
                existingRoleModule.CanCreate = roleModuleReq.CanCreate;
                existingRoleModule.CanRead = roleModuleReq.CanRead;
                existingRoleModule.CanUpdate = roleModuleReq.CanUpdate;
                existingRoleModule.CanDelete = roleModuleReq.CanDelete;
                _context.RoleModule.Update(existingRoleModule);
            }
            return existingRoleModule;
        }

        public async Task<List<int>> GetUsersWithRoleAsync(int roleId)
        {
            return await _context.UserRole
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync();
        }
        //update user module permissions
        public async Task UpdateUserModulePermissionsAsync(int userId, RoleModuleReq roleModuleReq)
        {
            var existingUserModule = await _context.UserModule
                .FirstOrDefaultAsync(um => um.UserId == userId && um.ModuleId == roleModuleReq.ModuleId);

            if (existingUserModule != null)
            {
                existingUserModule.CanCreate = roleModuleReq.CanCreate;
                existingUserModule.CanRead = roleModuleReq.CanRead;
                existingUserModule.CanUpdate = roleModuleReq.CanUpdate;
                existingUserModule.CanDelete = roleModuleReq.CanDelete;
                _context.UserModule.Update(existingUserModule);
            }
            else
            {
                await _context.UserModule.AddAsync(new UserModule
                {
                    UserId = userId,
                    ModuleId = roleModuleReq.ModuleId,
                    CanCreate = roleModuleReq.CanCreate,
                    CanRead = roleModuleReq.CanRead,
                    CanUpdate = roleModuleReq.CanUpdate,
                    CanDelete = roleModuleReq.CanDelete
                });
            }
        }
        //update role modules permissions
        public async Task UpdateRoleModulesPermissionsAsync(List<RoleModuleReq> roleModuleReqs)
        {   
            //Verify the role-module combination is valid
            foreach (var roleModuleReq in roleModuleReqs)
            {   

                var existingRoleModule = await GetRoleModuleAsync(roleModuleReq.RoleId, roleModuleReq.ModuleId);
                if (existingRoleModule == null)
                {
                    throw new Exception($"Role-Module combination not found for RoleId: {roleModuleReq.RoleId} and ModuleId: {roleModuleReq.ModuleId}");
                }
                //Update the role module permissions
                await UpdateRoleModuleAsync(roleModuleReq);
                //Get the users with the role
                var usersWithRole = await GetUsersWithRoleAsync(roleModuleReq.RoleId);
                //Update the user module permissions
                foreach (var userId in usersWithRole)
                {
                    await UpdateUserModulePermissionsAsync(userId, roleModuleReq);
                }
            }
        }

    }
} 