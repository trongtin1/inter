using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;
using test.Models.DTOs;
using test.Services;
using AutoMapper;

namespace test.Controllers
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDTO>>>> GetRoles()
        {
            var roles = await _context.Role
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Users = r.UserRoles.Select(ur => new UserDTO
                    {
                        Id = ur.User.Id,
                        Username = ur.User.Username,
                        Email = ur.User.Email,
                        Roles = ur.User.UserRoles.Select(ur => ur.Role.Name).ToList()
                    }).ToList()
                })  
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<RoleDTO>>
            {
                Success = true,
                Message = "Roles retrieved successfully",
                Data = roles
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDTO>>> GetRole(int id)
        {
            var role = await _context.Role
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .Where(r => r.Id == id)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Users = r.UserRoles.Select(ur => new UserDTO
                    {
                        Id = ur.User.Id,
                        Username = ur.User.Username,
                        Email = ur.User.Email
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound(new ApiResponse<RoleDTO>
                {
                    Success = false,
                    Message = "Role not found"
                });
            }

            return Ok(new ApiResponse<RoleDTO>
            {
                Success = true,
                Message = "Role retrieved successfully",
                Data = role
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDTO>>> CreateRole([FromBody] RoleDTO roleDto)
        {
            if (await _context.Role.AnyAsync(r => r.Name == roleDto.Name))
            {
                return BadRequest(new ApiResponse<RoleDTO>
                {
                    Success = false,
                    Message = "Role name already exists"
                });
            }

            var role = new Role { Name = roleDto.Name };
            _context.Role.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRole), 
                new { id = role.Id },
                new ApiResponse<RoleDTO>
                {
                    Success = true,
                    Message = "Role created successfully",
                    Data = new RoleDTO
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Users = new List<UserDTO>()
                    }
                });
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDTO>>> UpdateRole(int id, [FromBody] UpdateRoleDTO updateRoleDto)
        {
            if (string.IsNullOrWhiteSpace(updateRoleDto.Name))
            {
                return BadRequest(new ApiResponse<RoleDTO> 
                { 
                    Success = false, 
                    Message = "Role name cannot be empty" 
                });
            }

            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound(new ApiResponse<RoleDTO> 
                { 
                    Success = false, 
                    Message = "Role not found" 
                });
            }

            _mapper.Map(updateRoleDto, role);
            await _context.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDTO>(role);

            return Ok(new ApiResponse<RoleDTO> 
            { 
                Success = true, 
                Message = "Role updated successfully", 
                Data = roleDto
            });
        }
    }
}