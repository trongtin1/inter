using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Models.Entity;
using test.Models.DTOs;
using test.Services;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;
using test.Models.DTOs.Response.UserRole;
using test.Models.DTOs.Response.Role;
using test.Models.DTOs.Request.UserRole;
using AutoMapper;
namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRolesController(ApplicationDbContext context, IMapper mapper)

        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleRes>>>> GetUserRoles()
        {
            var userRoles = await _context.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .GroupBy(ur => new { ur.UserId, ur.User.Username })
                .Select(g => new UserRoleRes
                {
                    UserId = g.Key.UserId,
                    Username = g.Key.Username,
                    Roles = g.Select(ur => new RoleRes
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<UserRoleRes>>
            {
                Success = true,
                Message = "User roles retrieved successfully",
                Data = userRoles
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserRoleReq>>> AssignRole([FromBody] UserRoleReq request)
        {
            // Kiểm tra những role nào user chưa có
            var existingRoles = await _context.UserRole
                .Where(ur => ur.UserId == request.UserId && request.RoleIds.Contains(ur.RoleId))
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var newRoleIds = request.RoleIds.Except(existingRoles).ToList();

            if (!newRoleIds.Any())
            {
                return BadRequest(new ApiResponse<UserRoleReq>
                {
                    Success = false,
                    Message = "User already has all the specified roles"
                });
            }

            var userRoles = newRoleIds.Select(roleId => new UserRole
            {
                UserId = request.UserId,
                RoleId = roleId
            });

            _context.UserRole.AddRange(userRoles);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserRoleReq>
            {
                Success = true,
                Message = $"Successfully assigned {newRoleIds.Count} roles to user",
                Data = request
            });
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<UserRoleRes>>> GetUserRolesByUserId(int userId)
        {
            var userRoles = await _context.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .GroupBy(ur => new { ur.UserId, ur.User.Username })
                .Select(g => new UserRoleRes
                {
                    UserId = g.Key.UserId,
                    Username = g.Key.Username,
                    Roles = g.Select(ur => new RoleRes
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            if (userRoles == null)
            {
                return NotFound(new ApiResponse<UserRoleRes>
                {
                    Success = false,
                    Message = $"No roles found for user with ID {userId}"
                });
            }

            return Ok(new ApiResponse<UserRoleRes>
            {
                Success = true,
                Message = "User roles retrieved successfully",
                Data = userRoles
            });
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUserRoles([FromBody] DeleteUserRolesReq request)
        {
            var userRolesToDelete = await _context.UserRole
                .Where(ur => ur.UserId == request.UserId && request.RoleIds.Contains(ur.RoleId))
                .ToListAsync();

            if (!userRolesToDelete.Any())
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "No matching user roles found to delete"
                });
            }

            _context.UserRole.RemoveRange(userRolesToDelete);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = $"Successfully deleted {userRolesToDelete.Count} roles for user",
                Data = $"Deleted roles: {string.Join(", ", userRolesToDelete.Select(ur => ur.RoleId))}"
            });
        }
    }
}