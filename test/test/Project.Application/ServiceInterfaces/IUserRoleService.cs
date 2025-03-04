using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.UserRole;
using test.Project.Application.DTOs.Response.UserRoles;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IUserRoleService
    {
        Task<ApiResponse<IEnumerable<UserRoleRes>>> GetAllUserRolesAsync();
        Task<ApiResponse<UserRoleRes>> GetUserRolesByUserIdAsync(int userId);
        Task<ApiResponse<UserRoleReq>> AssignRolesAsync(UserRoleReq request);
        Task<ApiResponse<string>> DeleteUserRolesAsync(DeleteUserRolesReq request);
    }
} 