using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.RoleModule;
using test.Project.Application.DTOs.Request.RoleModule;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IRoleModuleService
    {
        Task<ApiResponse<IEnumerable<RoleModuleRes>>> GetAllRoleModulesAsync();
        Task<ApiResponse<RoleModuleRes>> GetRoleModulesByRoleIdAsync(int roleId);
        Task<ApiResponse<RoleModuleReq>> AssignModuleToRoleAsync(RoleModuleReq roleModuleReq);
        Task<ApiResponse<List<RoleModuleReq>>> UpdateModulePermissionsAsync(List<RoleModuleReq> roleModuleReqs);
    }
} 