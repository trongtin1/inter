using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using test.Models;
using test.Models.DTOs;
namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOnlineController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private const string UsersCountKey = "UsersCount";
        private const string UserAccessTimesKey = "UserAccessTimes";

        public UserOnlineController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("users-count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUsersCount()
        {
            var count = _memoryCache.GetOrCreate(UsersCountKey, entry => 0);
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Success",
                Data = count
            });
        }

        [HttpGet("users-access-times")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetUserAccessTimes()
        {
            var userAccessTimes = _memoryCache.GetOrCreate(UserAccessTimesKey, entry =>
            {
                // entry.SlidingExpiration = TimeSpan.FromMinutes(60);
                return new Dictionary<string, UserAccessInfo>();
            });

            var result = userAccessTimes.Values.Select(u => new
            {
                name = u.Name,
                email = u.Email,
                connectedTime = u.ConnectedTime.ToString(@"hh\:mm\:ss"),
                totalConnectedTime = u.IsOnline
                    ? (u.TotalConnectedTime + (DateTime.Now - u.ConnectedTime)).ToString(@"hh\:mm\:ss")
                    : u.TotalConnectedTime.ToString(@"hh\:mm\:ss"),
                isOnline = u.IsOnline
            });

            return Ok(new ApiResponse<IEnumerable<object>>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
    }
}