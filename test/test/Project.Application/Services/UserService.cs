using AutoMapper;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.User;
using test.Project.Application.DTOs.Request.User;
using test.Project.Domain.Entity;

namespace test.Project.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<UserRes>>> GetPageUser(
            string? username,
            string? email,
            int? pageIndex, 
            int? pageSize)
        {
            try
            {
                int finalPageIndex = pageIndex ?? 1;
                int finalPageSize = pageSize ?? 10;
                var (items, totalCount) = await _unitOfWork.Users.GetPage(
                    username,
                    email,
                    finalPageIndex,
                    finalPageSize);

                var usersRes = _mapper.Map<IEnumerable<UserRes>>(items);
                
                var pagedResult = new PagedResult<UserRes>
                {
                    Items = usersRes,
                    TotalItems = totalCount,
                    PageIndex = finalPageIndex,
                    PageSize = finalPageSize
                };

                return new ApiResponse<PagedResult<UserRes>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = pagedResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<UserRes>>
                {
                    Success = false,
                    Message = "Error retrieving users: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRes>> GetOneUser(string? username, string? email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetOne(username, email);
                if (user == null)
                {
                    return new ApiResponse<UserRes>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userRes = _mapper.Map<UserRes>(user);
                return new ApiResponse<UserRes>
                {
                    Success = true, 
                    Message = "User retrieved successfully",
                    Data = userRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "Error retrieving user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<UserRes>>> GetListUser(string? username, string? email)
        {
            try
            {
                var users = await _unitOfWork.Users.GetList(username, email);
                var usersRes = _mapper.Map<List<UserRes>>(users);
                return new ApiResponse<List<UserRes>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = usersRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserRes>>
                {
                    Success = false,
                    Message = "Error retrieving users: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRes>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return new ApiResponse<UserRes>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userRes = _mapper.Map<UserRes>(user);
                return new ApiResponse<UserRes>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = userRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "Error retrieving user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRes>> CreateUserAsync(UserReq userReq)
        {
            try
            {
                if (await _unitOfWork.Users.IsUsernameExistsAsync(userReq.Username))
                {
                    return new ApiResponse<UserRes>
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                await _unitOfWork.BeginTransactionAsync();
                
                var user = _mapper.Map<User>(userReq);
                await _unitOfWork.Users.CreateAsync(user);
                //await _unitOfWork.SaveChangesAsync();
                
                await _unitOfWork.CommitAsync();

                var userRes = _mapper.Map<UserRes>(user);
                return new ApiResponse<UserRes>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = userRes
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "Error creating user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRes>> UpdateUserAsync(int id, UserReq userReq)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return new ApiResponse<UserRes>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                if (await _unitOfWork.Users.IsUsernameExistsAsync(userReq.Username, id))
                {
                    return new ApiResponse<UserRes>
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                await _unitOfWork.BeginTransactionAsync();

                var updatedUser = _mapper.Map<User>(userReq);
                updatedUser.Id = id;
                await _unitOfWork.Users.UpdateAsync(updatedUser);
                //await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                var userRes = _mapper.Map<UserRes>(updatedUser);
                return new ApiResponse<UserRes>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = userRes
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "Error updating user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserRes>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return new ApiResponse<UserRes>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                await _unitOfWork.BeginTransactionAsync();

                await _unitOfWork.Users.DeleteAsync(user);
                //await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                return new ApiResponse<UserRes>
                {
                    Success = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                //await _unitOfWork.RollbackAsync();
                return new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "Error deleting user: " + ex.Message
                };
            }
        }
        
        
    }
} 