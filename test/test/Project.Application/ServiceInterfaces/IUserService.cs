using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.User;
using test.Project.Application.DTOs.Request.User;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IUserService
    {
        
        Task<ApiResponse<PagedResult<UserRes>>> GetPageUser(
            string? username,
            string? email,
            int? pageIndex,
            int? pageSize);
        Task<ApiResponse<UserRes>> GetOneUser(string? username, string? email);
        Task<ApiResponse<List<UserRes>>> GetListUser(string? username, string? email);
        Task<ApiResponse<UserRes>> GetUserByIdAsync(int id);
        Task<ApiResponse<UserRes>> CreateUserAsync(UserReq userReq);
        Task<ApiResponse<UserRes>> UpdateUserAsync(int id, UserReq userReq);
        Task<ApiResponse<UserRes>> DeleteUserAsync(int id);
        
    }
} 