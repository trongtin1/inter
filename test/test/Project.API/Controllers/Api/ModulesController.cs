using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.Module;
using test.Project.Application.DTOs.Response.Module;
using test.Project.Application.ServiceInterfaces;


namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet("getPage")]
        public async Task<ApiResponse<PagedResult<ModuleRes>>> GetModules(
            string? name,
            int? pageIndex,
            int? pageSize)
        {
            return await _moduleService.GetPageModule(name, pageIndex, pageSize);
        }

        [HttpGet("getOne")]
        public async Task<ApiResponse<ModuleRes>> GetOne(string? name)
        {
            return await _moduleService.GetOneModule(name);
        }

        [HttpGet("getList")]
        public async Task<ApiResponse<List<ModuleRes>>> GetList(string? name)
        {
            return await _moduleService.GetListModule(name);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<ModuleRes>> GetModule(int id)
        {
            return await _moduleService.GetModuleByIdAsync(id);
        }

        [HttpPost]
        public async Task<ApiResponse<ModuleReq>> CreateModule([FromBody] ModuleReq moduleReq)
        {
            return await _moduleService.CreateModuleAsync(moduleReq);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<ModuleReq>> UpdateModule(int id, ModuleReq moduleReq)
        {
            return await _moduleService.UpdateModuleAsync(id, moduleReq);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<object>> DeleteModule(int id)
        {
            return await _moduleService.DeleteModuleAsync(id);
        }
        
    }
} 