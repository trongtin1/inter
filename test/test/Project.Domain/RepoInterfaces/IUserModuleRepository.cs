using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.UserModule;
using test.Project.Application.DTOs.Request.UserModule;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IUserModuleRepository
    {
        Task<IEnumerable<UserModuleRes>> GetAllUserModulesAsync();
        Task<UserModuleRes> GetUserModulesByUserIdAsync(int userId);
        Task<UserModule> GetUserModuleAsync(int userId, int moduleId);
        Task<UserModule> CreateUserModuleAsync(UserModuleReq userModuleReq);
        Task<UserModule> UpdateUserModuleAsync(UserModuleReq userModuleReq);
        Task<IEnumerable<User>> GetUsersWithModulesAsync();
    }
} 