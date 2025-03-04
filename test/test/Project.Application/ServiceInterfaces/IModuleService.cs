using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.Module;
using test.Project.Application.DTOs.Response.Module;


namespace test.Project.Application.ServiceInterfaces
{
    public interface IModuleService
    {
        
        Task<ApiResponse<PagedResult<ModuleRes>>> GetPageModule(
            string? name,
            int? pageIndex,
            int? pageSize);
        Task<ApiResponse<ModuleRes>> GetOneModule(string? name);
        Task<ApiResponse<List<ModuleRes>>> GetListModule(string? name);
        Task<ApiResponse<ModuleRes>> GetModuleByIdAsync(int id);
        Task<ApiResponse<ModuleReq>> CreateModuleAsync(ModuleReq moduleReq);
        Task<ApiResponse<ModuleReq>> UpdateModuleAsync(int id, ModuleReq moduleReq);
        Task<ApiResponse<object>> DeleteModuleAsync(int id);
        
    }
} 