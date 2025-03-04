using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.UserRole;
using test.Project.Application.DTOs.Response.UserRoles;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleRes>>>> GetUserRoles()
        {
            return await _userRoleService.GetAllUserRolesAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<UserRoleRes>>> GetUserRolesByUserId(int userId)
        {
            return await _userRoleService.GetUserRolesByUserIdAsync(userId);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserRoleReq>>> AssignRole([FromBody] UserRoleReq request)
        {
            return await _userRoleService.AssignRolesAsync(request);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUserRoles([FromBody] DeleteUserRolesReq request)
        {
            return await _userRoleService.DeleteUserRolesAsync(request);
        }
    }
}