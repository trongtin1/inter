using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.User;
using test.Project.Application.DTOs.Request.User;
using test.Project.Domain.Common;
using System.Linq.Expressions;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Task<IEnumerable<UserRes>> GetPageNotification();
        // Task<UserRes> GetUserByIdAsync(int id);
        // Task<User> CreateUserAsync(UserReq userReq);
        // Task<User> UpdateUserAsync(int id, UserReq userReq);
        // Task<User> DeleteUserAsync(int id);
        Task<bool> IsUsernameExistsAsync(string username, int? excludeId = null);
        Task<User> GetOne(string? username, string? email);
        Task<List<User>> GetList(string? username, string? email);
        Task<(IEnumerable<User> Items, int TotalCount)> GetPage(
            string? username,
            string? email,
            int pageIndex,
            int pageSize);
    }
} 