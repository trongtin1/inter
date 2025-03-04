using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOnlineController : ControllerBase
    {
        private readonly IUserOnlineService _userOnlineService;

        public UserOnlineController(IUserOnlineService userOnlineService)
        {
            _userOnlineService = userOnlineService;
        }

        [HttpGet("users-count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUsersCount()
        {
            return await _userOnlineService.GetUsersCountAsync();
        }

        [HttpGet("users-access-times")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetUserAccessTimes()
        {
            return await _userOnlineService.GetUserAccessTimesAsync();
        }
    }
}