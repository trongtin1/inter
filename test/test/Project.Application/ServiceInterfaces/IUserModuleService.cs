using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.UserModule;
using test.Project.Application.DTOs.Response.UserModule;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IUserModuleService
    {
        Task<ApiResponse<IEnumerable<UserModuleRes>>> GetAllUserModulesAsync();
        Task<ApiResponse<UserModuleRes>> GetUserModulesByUserIdAsync(int userId);
        Task<ApiResponse<UserModuleReq>> AssignModuleToUserAsync(UserModuleReq userModuleReq);
    }
} 