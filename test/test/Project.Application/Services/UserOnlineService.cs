using test.Project.Application.DTOs;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
    public class UserOnlineService : IUserOnlineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserOnlineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> GetUsersCountAsync()
        {
            try
            {
                var count = await _unitOfWork.UserOnline.GetUsersCountAsync();
                return new ApiResponse<int>
                {
                    Success = true,
                    Message = "Success",
                    Data = count
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = "Error retrieving users count: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<object>>> GetUserAccessTimesAsync()
        {
            try
            {
                var userAccessTimes = await _unitOfWork.UserOnline.GetUserAccessTimesAsync();
                
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

                return new ApiResponse<IEnumerable<object>>
                {
                    Success = true,
                    Message = "Success",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<object>>
                {
                    Success = false,
                    Message = "Error retrieving user access times: " + ex.Message
                };
            }
        }
    }
} 