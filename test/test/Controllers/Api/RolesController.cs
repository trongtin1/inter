using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Models.Entity;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;
using test.Models.DTOs;
using test.Models.DTOs.Response.Role;
using test.Models.DTOs.Response.User;
using test.Services;
using AutoMapper;
using test.Models.DTOs.Request.Role;

namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RolesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task<bool> IsRoleNameExists(string name, int? excludeId = null)
        {
            return await _context.Role.AnyAsync(r => r.Name == name && (!excludeId.HasValue || r.Id != excludeId));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleRes>>>> GetRoles()
        {
            var roles = await _context.Role
                .Select(r => new RoleRes
                {
                    Id = r.Id,
                    Name = r.Name
                })  
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<RoleRes>>
            {
                Success = true,
                Message = "Roles retrieved successfully",
                Data = roles
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleRes>>> GetRole(int id)
        {
            var role = await _context.Role
                .Where(r => r.Id == id)
                .Select(r => new RoleRes
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound(new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Role not found"
                });
            }

            return Ok(new ApiResponse<RoleRes>
            {
                Success = true,
                Message = "Role retrieved successfully",
                Data = role
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleRes>>> CreateRole([FromBody] RoleReq RoleReq)
        {   
            if (await IsRoleNameExists(RoleReq.Name))
            {
                return BadRequest(new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Role name already exists"
                });
            }
            var role = _mapper.Map<Role>(RoleReq);
            _context.Role.Add(role);
            await _context.SaveChangesAsync();

            // Map back to response DTO including the ID
            var roleRes = new RoleRes
            {
                Id = role.Id,
                Name = role.Name
            };

            return Ok(new ApiResponse<RoleRes> 
            { 
                Success = true, 
                Message = "Role created successfully", 
                Data = roleRes
            });
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoleRes>>> UpdateRole(int id, [FromBody] RoleReq updateRoleDto)
        {
            // if (string.IsNullOrWhiteSpace(updateRoleDto.Name))
            // {

            //     return BadRequest(new ApiResponse<RoleDTO> 
            //     { 
            //         Success = false, 
            //         Message = "Role name cannot be empty" 
            //     });
            // }
            if (await IsRoleNameExists(updateRoleDto.Name, id))
            {
                return BadRequest(new ApiResponse<RoleRes>
                {
                    Success = false,
                    Message = "Role name already exists"
                });
            }
            var role = await _context.Role.FindAsync(id);
            
            if (role == null)
            {
                return NotFound(new ApiResponse<RoleRes> 
                { 
                    Success = false, 
                    Message = "Role not found" 
                });
            }

            _mapper.Map(updateRoleDto, role);
            await _context.SaveChangesAsync();           
            var roleRes = new RoleRes
            {
                Id = role.Id,
                Name = role.Name
            };
            return Ok(new ApiResponse<RoleRes> 
            { 
                Success = true, 
                Message = "Role updated successfully", 
                Data = roleRes
            });

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RoleRes>>> DeleteRole(int id)
        {
            var role = await _context.Role
                .FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<RoleRes>
            {
                Success = true,
                Message = "Role deleted successfully"
            });
        }
    }

}