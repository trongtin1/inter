using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.UserOnline;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IUserOnlineRepository
    {
        Task<int> GetUsersCountAsync();
        Task<Dictionary<string, UserAccessInfo>> GetUserAccessTimesAsync();
        Task UpdateUserCountAsync(int count);
        Task UpdateUserAccessTimesAsync(Dictionary<string, UserAccessInfo> userAccessTimes);
    }
} 