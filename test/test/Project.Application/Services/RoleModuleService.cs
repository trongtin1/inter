using AutoMapper;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.RoleModule;
using test.Project.Application.DTOs.Request.RoleModule;

namespace test.Project.Application.Services
{
    public class RoleModuleService : IRoleModuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleModuleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<RoleModuleRes>>> GetAllRoleModulesAsync()
        {
            try
            {
                var roleModules = await _unitOfWork.RoleModules.GetAllRoleModulesAsync();
                return new ApiResponse<IEnumerable<RoleModuleRes>>
                {
                    Success = true,
                    Message = "Role modules retrieved successfully",
                    Data = roleModules
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<RoleModuleRes>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving role modules: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RoleModuleRes>> GetRoleModulesByRoleIdAsync(int roleId)
        {
            try
            {
                var roleModules = await _unitOfWork.RoleModules.GetRoleModulesByRoleIdAsync(roleId);
                if (roleModules == null)
                {
                    return new ApiResponse<RoleModuleRes>
                    {
                        Success = false,
                        Message = $"No roles found for role with ID {roleId}"
                    };
                }

                return new ApiResponse<RoleModuleRes>
                {
                    Success = true,
                    Message = "Role modules retrieved successfully",
                    Data = roleModules
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleModuleRes>
                {
                    Success = false,
                    Message = "Error occurred while retrieving role modules: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RoleModuleReq>> AssignModuleToRoleAsync(RoleModuleReq roleModuleReq)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingRoleModule = await _unitOfWork.RoleModules.GetRoleModuleAsync(roleModuleReq.RoleId, roleModuleReq.ModuleId);
                
                if (existingRoleModule != null)
                {
                    await _unitOfWork.RoleModules.UpdateRoleModuleAsync(roleModuleReq);
                }
                else
                {
                    await _unitOfWork.RoleModules.CreateRoleModuleAsync(roleModuleReq);
                }

                await _unitOfWork.CommitAsync();

                return new ApiResponse<RoleModuleReq>
                {
                    Success = true,
                    Message = existingRoleModule != null ? "Module permissions updated successfully" : "Module assigned successfully",
                    Data = roleModuleReq
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<RoleModuleReq>
                {
                    Success = false,
                    Message = "Error occurred while processing request: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<RoleModuleReq>>> UpdateModulePermissionsAsync(List<RoleModuleReq> roleModuleReqs)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.RoleModules.UpdateRoleModulesPermissionsAsync(roleModuleReqs);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<List<RoleModuleReq>>
                {
                    Success = true,
                    Message = "Module permissions updated successfully for roles and associated users",
                    Data = roleModuleReqs
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<List<RoleModuleReq>>
                {
                    Success = false,
                    Message = "Error occurred while processing request: " + ex.Message
                };
            }
        }

    }
} 