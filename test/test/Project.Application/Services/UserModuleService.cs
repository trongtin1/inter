using AutoMapper;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.UserModule;
using test.Project.Application.DTOs.Response.UserModule;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
    public class UserModuleService : IUserModuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserModuleNotificationService _notificationService;

        public UserModuleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserModuleNotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<IEnumerable<UserModuleRes>>> GetAllUserModulesAsync()
        {
            try
            {
                var userModules = await _unitOfWork.UserModules.GetAllUserModulesAsync();
                return new ApiResponse<IEnumerable<UserModuleRes>>
                {
                    Success = true,
                    Message = "User modules retrieved successfully",
                    Data = userModules
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<UserModuleRes>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user modules: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserModuleRes>> GetUserModulesByUserIdAsync(int userId)
        {
            try
            {
                var userModules = await _unitOfWork.UserModules.GetUserModulesByUserIdAsync(userId);
                if (userModules == null)
                {
                    return new ApiResponse<UserModuleRes>
                    {
                        Success = false,
                        Message = $"No modules found for user with ID {userId}",
                        Data = null
                    };
                }

                return new ApiResponse<UserModuleRes>
                {
                    Success = true,
                    Message = "User modules retrieved successfully",
                    Data = userModules
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserModuleRes>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user modules: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserModuleReq>> AssignModuleToUserAsync(UserModuleReq userModuleReq)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingUserModule = await _unitOfWork.UserModules.GetUserModuleAsync(
                    userModuleReq.UserId, userModuleReq.ModuleId);

                if (existingUserModule != null)
                {
                    await _unitOfWork.UserModules.UpdateUserModuleAsync(userModuleReq);
                }
                else
                {
                    await _unitOfWork.UserModules.CreateUserModuleAsync(userModuleReq);
                }

                //await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                await _notificationService.NotifyUserModuleCreated(userModuleReq.UserId.ToString());

                return new ApiResponse<UserModuleReq>
                {
                    Success = true,
                    Message = existingUserModule != null ? 
                        "Module permissions updated successfully" : 
                        "Module assigned successfully",
                    Data = userModuleReq
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<UserModuleReq>
                {
                    Success = false,
                    Message = "Error occurred while processing request: " + ex.Message,
                    Data = null
                };
            }
        }
    }
} 