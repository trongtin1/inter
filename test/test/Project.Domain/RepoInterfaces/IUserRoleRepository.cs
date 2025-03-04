using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.UserRoles;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRoleRes>> GetAllUserRolesAsync();
        Task<UserRoleRes> GetUserRolesByUserIdAsync(int userId);
        Task<IEnumerable<UserRole>> CreateUserRolesAsync(int userId, List<int> roleIds);
        Task<List<UserRole>> GetUserRolesToDeleteAsync(int userId, List<int> roleIds);
        Task DeleteUserRolesAsync(IEnumerable<UserRole> userRoles);
        Task<List<int>> GetExistingRoleIdsAsync(int userId, List<int> roleIds);
    }
} 