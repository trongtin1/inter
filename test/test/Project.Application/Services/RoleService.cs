using AutoMapper;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.Role;
using test.Project.Application.DTOs.Response.Role;
using test.Project.Application.ServiceInterfaces;
using test.Project.Domain.Entity;

namespace test.Project.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<RoleRes>>> GetPageRole(
            string? name,
            int? pageIndex,
            int? pageSize)
        {
            int finalPageIndex = pageIndex ?? 1;
            int finalPageSize = pageSize ?? 10;
            try
            {
                var (items, totalCount) = await _unitOfWork.Roles.GetPage(
                    name,
                    finalPageIndex,
                    finalPageSize);

                var rolesRes = _mapper.Map<IEnumerable<RoleRes>>(items);
                var pagedResult = new PagedResult<RoleRes>
                {
                    Items = rolesRes,
                    TotalItems = totalCount,
                    PageIndex = finalPageIndex,
                    PageSize = finalPageSize
                };

                return new ApiResponse<PagedResult<RoleRes>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = pagedResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<RoleRes>>
                {
                    Success = false,
                    Message = "Error retrieving users: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RoleRes>> GetOneRole(string? name)
        {
            try
            {
                var user = await _unitOfWork.Roles.GetOne(name);
                if (user == null)
                {
                    return new ApiResponse<RoleRes>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userRes = _mapper.Map<RoleRes>(user);
                return new ApiResponse<RoleRes>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = userRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Error retrieving user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<RoleRes>>> GetListRole(string? name)
        {
            try
            {
                var users = await _unitOfWork.Roles.GetList(name);
                var usersRes = _mapper.Map<List<RoleRes>>(users);
                return new ApiResponse<List<RoleRes>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = usersRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoleRes>>
                {
                    Success = false,
                    Message = "Error retrieving users: " + ex.Message
                };
            }
        }
        public async Task<ApiResponse<RoleRes>> GetRoleByIdAsync(int id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return new ApiResponse<RoleRes>
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                var roleRes = _mapper.Map<RoleRes>(role);
                return new ApiResponse<RoleRes>
                {
                    Success = true,
                    Message = "Role retrieved successfully",
                    Data = roleRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Error retrieving role: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RoleRes>> CreateRoleAsync(RoleReq roleReq)
        {
            try
            {
                if (await _unitOfWork.Roles.IsNameExistsAsync(roleReq.Name))
                {
                    return new ApiResponse<RoleRes>
                    {
                        Success = false,
                        Message = "Role name already exists"
                    };
                }

                await _unitOfWork.BeginTransactionAsync();
                
                var role = _mapper.Map<Role>(roleReq);
                await _unitOfWork.Roles.CreateAsync(role);
                //await _unitOfWork.SaveChangesAsync();
                
                await _unitOfWork.CommitAsync();

                var roleRes = _mapper.Map<RoleRes>(role);
                return new ApiResponse<RoleRes>
                {
                    Success = true,
                    Message = "Role created successfully",
                    Data = roleRes
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Error creating role: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RoleRes>> UpdateRoleAsync(int id, RoleReq roleReq)
        {
            try
            {
                if (await _unitOfWork.Roles.IsNameExistsAsync(roleReq.Name, id))
                {
                    return new ApiResponse<RoleRes>
                    {
                        Success = false,
                        Message = "Role name already exists"
                    };
                }

                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return new ApiResponse<RoleRes>
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                await _unitOfWork.BeginTransactionAsync();

                _mapper.Map(roleReq, role);
                await _unitOfWork.Roles.UpdateAsync(role);
                //await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                var roleRes = _mapper.Map<RoleRes>(role);
                return new ApiResponse<RoleRes>
                {
                    Success = true,
                    Message = "Role updated successfully",
                    Data = roleRes
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Error updating role: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RoleRes>> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return new ApiResponse<RoleRes>
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                await _unitOfWork.BeginTransactionAsync();

                await _unitOfWork.Roles.DeleteAsync(role);
                //await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                return new ApiResponse<RoleRes>
                {
                    Success = true,
                    Message = "Role deleted successfully"
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Error deleting role: " + ex.Message
                };
            }
        }
        

    }
} 