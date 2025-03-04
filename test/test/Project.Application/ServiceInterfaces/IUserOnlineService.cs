using test.Project.Application.DTOs;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IUserOnlineService
    {
        Task<ApiResponse<int>> GetUsersCountAsync();
        Task<ApiResponse<IEnumerable<object>>> GetUserAccessTimesAsync();
    }
} 