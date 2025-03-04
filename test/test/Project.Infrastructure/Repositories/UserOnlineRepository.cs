using Microsoft.Extensions.Caching.Memory;
using test.Project.Domain.RepoInterfaces;
using test.Project.Application.DTOs.Response.UserOnline;

namespace test.Project.Infrastructure.Repositories
{
    public class UserOnlineRepository : IUserOnlineRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string UsersCountKey = "UsersCount";
        private const string UserAccessTimesKey = "UserAccessTimes";

        public UserOnlineRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<int> GetUsersCountAsync()
        {
            return _memoryCache.GetOrCreate(UsersCountKey, entry => 0);
        }

        public async Task<Dictionary<string, UserAccessInfo>> GetUserAccessTimesAsync()
        {
            return _memoryCache.GetOrCreate(UserAccessTimesKey, entry =>
            {
                return new Dictionary<string, UserAccessInfo>();
            });
        }

        public async Task UpdateUserCountAsync(int count)
        {
            _memoryCache.Set(UsersCountKey, count);
        }

        public async Task UpdateUserAccessTimesAsync(Dictionary<string, UserAccessInfo> userAccessTimes)
        {
            _memoryCache.Set(UserAccessTimesKey, userAccessTimes);
        }
    }
} 