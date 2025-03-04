using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.UserRole;
using test.Project.Application.DTOs.Response.UserRoles;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<UserRoleRes>>> GetAllUserRolesAsync()
        {
            try
            {
                var userRoles = await _unitOfWork.UserRoles.GetAllUserRolesAsync();
                return new ApiResponse<IEnumerable<UserRoleRes>>
                {
                    Success = true,
                    Message = "User roles retrieved successfully",
                    Data = userRoles
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<UserRoleRes>>
                {
                    Success = false,
                    Message = "Error retrieving user roles: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRoleRes>> GetUserRolesByUserIdAsync(int userId)
        {
            try
            {
                var userRoles = await _unitOfWork.UserRoles.GetUserRolesByUserIdAsync(userId);
                if (userRoles == null)
                {
                    return new ApiResponse<UserRoleRes>
                    {
                        Success = false,
                        Message = $"No roles found for user with ID {userId}"
                    };
                }

                return new ApiResponse<UserRoleRes>
                {
                    Success = true,
                    Message = "User roles retrieved successfully",
                    Data = userRoles
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserRoleRes>
                {
                    Success = false,
                    Message = "Error retrieving user roles: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRoleReq>> AssignRolesAsync(UserRoleReq request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingRoles = await _unitOfWork.UserRoles.GetExistingRoleIdsAsync(request.UserId, request.RoleIds);
                var newRoleIds = request.RoleIds.Except(existingRoles).ToList();

                if (!newRoleIds.Any())
                {
                    return new ApiResponse<UserRoleReq>
                    {
                        Success = false,
                        Message = "User already has all the specified roles"
                    };
                }

                await _unitOfWork.UserRoles.CreateUserRolesAsync(request.UserId, newRoleIds);
                //await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new ApiResponse<UserRoleReq>
                {
                    Success = true,
                    Message = $"Successfully assigned {newRoleIds.Count} roles to user",
                    Data = request
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<UserRoleReq>
                {
                    Success = false,
                    Message = "Error assigning roles: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteUserRolesAsync(DeleteUserRolesReq request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var userRolesToDelete = await _unitOfWork.UserRoles.GetUserRolesToDeleteAsync(request.UserId, request.RoleIds);

                if (!userRolesToDelete.Any())
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No matching user roles found to delete"
                    };
                }

                await _unitOfWork.UserRoles.DeleteUserRolesAsync(userRolesToDelete);
                //await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = $"Successfully deleted {userRolesToDelete.Count} roles for user",
                    Data = $"Deleted roles: {string.Join(", ", userRolesToDelete.Select(ur => ur.RoleId))}"
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error deleting user roles: " + ex.Message
                };
            }
        }
    }
} 