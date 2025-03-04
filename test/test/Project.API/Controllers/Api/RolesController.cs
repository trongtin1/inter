using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request.Role;
using test.Project.Application.DTOs.Response.Role;
using test.Project.Application.ServiceInterfaces;


namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("getOne")]
        public async Task<ApiResponse<RoleRes>> GetOne(string? name)
        {
            return await _roleService.GetOneRole(name);
        }

        [HttpGet("getList")]
        public async Task<ApiResponse<List<RoleRes>>> GetList(string? name)
        {
            return await _roleService.GetListRole(name);
        }

        [HttpGet("getPage")]
        public async Task<ApiResponse<PagedResult<RoleRes>>> GetRoles(
            string? name,
            int? pageIndex,
            int? pageSize)
        {
            return await _roleService.GetPageRole(name, pageIndex, pageSize);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleRes>>> GetRole(int id)
        {
            return await _roleService.GetRoleByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleRes>>> CreateRole([FromBody] RoleReq roleReq)
        {
            return await _roleService.CreateRoleAsync(roleReq);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoleRes>>> UpdateRole(int id, [FromBody] RoleReq roleReq)
        {
            return await _roleService.UpdateRoleAsync(id, roleReq);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RoleRes>>> DeleteRole(int id)
        {
            return await _roleService.DeleteRoleAsync(id);
        }

        
    }
}