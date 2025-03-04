using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.RoleModule;
using test.Project.Application.DTOs.Response.RoleModule;
using test.Project.Application.ServiceInterfaces;
namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleModulesController : ControllerBase
    {
        private readonly IRoleModuleService _roleModuleService;

        public RoleModulesController(IRoleModuleService roleModuleService)
        {
            _roleModuleService = roleModuleService;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<RoleModuleRes>>> GetRoleModules()
        {
            return await _roleModuleService.GetAllRoleModulesAsync();
        }

        [HttpGet("{roleId}")]
        public async Task<ApiResponse<RoleModuleRes>> GetRoleModulesByRoleId(int roleId)
        {
            return await _roleModuleService.GetRoleModulesByRoleIdAsync(roleId);
        }

        [HttpPost]
        public async Task<ApiResponse<RoleModuleReq>> AssignModuleToRole([FromBody] RoleModuleReq roleModuleReq)
        {
            return await _roleModuleService.AssignModuleToRoleAsync(roleModuleReq);
        }

        [HttpPut]
        public async Task<ApiResponse<List<RoleModuleReq>>> UpdateModulePermissions([FromBody] List<RoleModuleReq> roleModuleReqs)
        {
            return await _roleModuleService.UpdateModulePermissionsAsync(roleModuleReqs);
        }
    }
} 