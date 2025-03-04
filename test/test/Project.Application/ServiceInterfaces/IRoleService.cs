using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.Role;
using test.Project.Application.DTOs.Response.Role;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IRoleService
    {
        Task<ApiResponse<PagedResult<RoleRes>>> GetPageRole(
            string? name,
            int? pageIndex,
            int? pageSize);
        Task<ApiResponse<RoleRes>> GetOneRole(string? name);
        Task<ApiResponse<List<RoleRes>>> GetListRole(string? name);
        Task<ApiResponse<RoleRes>> GetRoleByIdAsync(int id);
        Task<ApiResponse<RoleRes>> CreateRoleAsync(RoleReq roleReq);
        Task<ApiResponse<RoleRes>> UpdateRoleAsync(int id, RoleReq roleReq);
        Task<ApiResponse<RoleRes>> DeleteRoleAsync(int id);
        
    }
} 