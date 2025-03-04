using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.RoleModule;
using test.Project.Application.DTOs.Request.RoleModule;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IRoleModuleRepository
    {
        Task<IEnumerable<RoleModuleRes>> GetAllRoleModulesAsync();
        Task<RoleModuleRes> GetRoleModulesByRoleIdAsync(int roleId);
        Task<RoleModule> UpdateRoleModuleAsync(RoleModuleReq roleModuleReq);
        Task<RoleModule> CreateRoleModuleAsync(RoleModuleReq roleModuleReq);
        Task<RoleModule> GetRoleModuleAsync(int roleId, int moduleId);
        Task UpdateRoleModulesPermissionsAsync(List<RoleModuleReq> roleModuleReqs);
        Task<List<int>> GetUsersWithRoleAsync(int roleId);
        Task UpdateUserModulePermissionsAsync(int userId, RoleModuleReq roleModuleReq);
    }
} 