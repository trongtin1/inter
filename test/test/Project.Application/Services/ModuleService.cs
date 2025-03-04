using AutoMapper;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.Module;
using test.Project.Application.DTOs.Response.Module;
using test.Project.Application.DTOs.Response.User;
using test.Project.Application.ServiceInterfaces;
using test.Project.Domain.Entity;

namespace test.Project.Application.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ModuleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<ModuleRes>>> GetPageModule(
            string? name,
            int? pageIndex,
            int? pageSize)
        {
            try
            {
                int finalPageIndex = pageIndex ?? 1;
                int finalPageSize = pageSize ?? 10;
                var (items, totalCount) = await _unitOfWork.Modules.GetPage(
                    name,
                    finalPageIndex,
                    finalPageSize);

                var modulesRes = _mapper.Map<IEnumerable<ModuleRes>>(items);

                var pagedResult = new PagedResult<ModuleRes>
                {
                    Items = modulesRes,
                    TotalItems = totalCount,
                    PageIndex = finalPageIndex,
                    PageSize = finalPageSize
                };

                return new ApiResponse<PagedResult<ModuleRes>>
                {
                    Success = true,
                    Message = "Modules retrieved successfully",
                    Data = pagedResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ModuleRes>>
                {
                    Success = false,
                    Message = "Error retrieving modules: " + ex.Message
                };
            }
        }
        public async Task<ApiResponse<ModuleRes>> GetOneModule(string? name)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetOne(name);
                if (module == null)
                {
                    return new ApiResponse<ModuleRes>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userRes = _mapper.Map<ModuleRes>(module);
                return new ApiResponse<ModuleRes>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = userRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ModuleRes>
                {
                    Success = false,
                    Message = "Error retrieving user: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<ModuleRes>>> GetListModule(string? name)
        {
            try
            {
                var modules = await _unitOfWork.Modules.GetList(name);
                var moduleRes = _mapper.Map<List<ModuleRes>>(modules);
                return new ApiResponse<List<ModuleRes>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = moduleRes
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ModuleRes>>
                {
                    Success = false,
                    Message = "Error retrieving users: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<ModuleRes>> GetModuleByIdAsync(int id)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return new ApiResponse<ModuleRes>
                    {
                        Success = false,
                        Message = "Module not found"
                    };
                }

                return new ApiResponse<ModuleRes>
                {
                    Success = true,
                    Message = "Module retrieved successfully",
                    Data = _mapper.Map<ModuleRes>(module)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ModuleRes>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<ModuleReq>> CreateModuleAsync(ModuleReq moduleReq)
        {
            try
            {
                var module = _mapper.Map<Module>(moduleReq);
                await _unitOfWork.Modules.CreateAsync(module);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<ModuleReq>
                {
                    Success = true,
                    Message = "Module created successfully",
                    Data = moduleReq
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ModuleReq>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<ModuleReq>> UpdateModuleAsync(int id, ModuleReq moduleReq)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return new ApiResponse<ModuleReq>
                    {
                        Success = false,
                        Message = "Module not found"
                    };
                }

                _mapper.Map(moduleReq, module);
                await _unitOfWork.Modules.UpdateAsync(module);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<ModuleReq>
                {
                    Success = true,
                    Message = "Module updated successfully",
                    Data = moduleReq
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ModuleReq>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteModuleAsync(int id)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Module not found"
                    };
                }

                await _unitOfWork.Modules.DeleteAsync(module);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Module deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        
    }
} 