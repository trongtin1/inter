using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.UserModule;
using test.Project.Application.DTOs.Response.UserModule;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserModulesController : ControllerBase
    {
        private readonly IUserModuleService _userModuleService;

        public UserModulesController(IUserModuleService userModuleService)
        {
            _userModuleService = userModuleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserModuleRes>>>> GetUserModules()
        {
            return await _userModuleService.GetAllUserModulesAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<UserModuleRes>>> GetUserModulesByUserId(int userId)
        {
            return await _userModuleService.GetUserModulesByUserIdAsync(userId);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserModuleReq>>> AssignModuleToUser([FromBody] UserModuleReq userModuleReq)
        {
            return await _userModuleService.AssignModuleToUserAsync(userModuleReq);
        }
    }
} 