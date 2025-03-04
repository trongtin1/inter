using Microsoft.AspNetCore.Mvc;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.User;
using test.Project.Application.DTOs.Response.User;

namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        //[HttpGet("all")]
        //public async Task<ApiResponse<IEnumerable<UserRes>>> GetUsers()
        //{
        //    return await _userService.GetUserNoQuery();
        //}

        [HttpGet("{id}")]
        public async Task<ApiResponse<UserRes>> GetUser(int id)
        {
            return await _userService.GetUserByIdAsync(id);
        }

        [HttpPost]
        public async Task<ApiResponse<UserRes>> CreateUser([FromBody] UserReq userReq)
        {
            return await _userService.CreateUserAsync(userReq);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<UserRes>> UpdateUser(int id, [FromBody] UserReq userReq)
        {
            return await _userService.UpdateUserAsync(id, userReq);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<UserRes>> DeleteUser(int id)
        {
            return await _userService.DeleteUserAsync(id);
        }
        [HttpGet("getPage")]
        public async Task<ApiResponse<PagedResult<UserRes>>> GetUsers(
            string? username,
            string? email,
            int? pageIndex,
            int? pageSize)
        {
            return await _userService.GetPageUser(username, email, pageIndex, pageSize);
        }

        [HttpGet("getOne")]
        public async Task<ApiResponse<UserRes>> GetOne(string? username, string? email)
        {
            return await _userService.GetOneUser(username, email);
        }

        [HttpGet("getList")]
        public async Task<ApiResponse<List<UserRes>>> GetList(string? username, string? email)
        {
            return await _userService.GetListUser(username, email);
        }
    }
}