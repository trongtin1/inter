using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Models.DTOs;
using test.Services;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleDTO>>>> GetUserRoles()
        {
            var userRoles = await _context.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Select(ur => new UserRoleDTO
                {
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    Username = ur.User.Username,
                    RoleName = ur.Role.Name
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<UserRoleDTO>>
            {
                Success = true,
                Message = "User roles retrieved successfully",
                Data = userRoles
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserRoleDTO>>> AssignRole([FromBody] UserRoleDTO userRoleDto)
        {
            if (await _context.UserRole.AnyAsync(ur => 
                ur.UserId == userRoleDto.UserId && 
                ur.RoleId == userRoleDto.RoleId))
            {
                return BadRequest(new ApiResponse<UserRoleDTO>
                {
                    Success = false,
                    Message = "User already has this role"
                });
            }

            var userRole = new UserRole
            {
                UserId = userRoleDto.UserId,
                RoleId = userRoleDto.RoleId
            };

            _context.UserRole.Add(userRole);
            await _context.SaveChangesAsync();

            
            var user = await _context.User.FindAsync(userRole.UserId);
            var role = await _context.Role.FindAsync(userRole.RoleId);

            var createdUserRole = new UserRoleDTO
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                Username = user.Username,
                RoleName = role.Name
            };

            return Ok(new ApiResponse<UserRoleDTO>
            {
                Success = true,
                Message = "Role assigned successfully",
                Data = createdUserRole
            });
        }

        // [HttpDelete("{userId}/{roleId}")]
        // public async Task<ActionResult<ApiResponse<object>>> RemoveRole(int userId, int roleId)
        // {
        //     var userRole = await _context.UserRole
        //         .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        //     if (userRole == null)
        //     {
        //         return NotFound(new ApiResponse<object>
        //         {
        //             Success = false,
        //             Message = "User role not found"
        //         });
        //     }

        //     _context.UserRole.Remove(userRole);
        //     await _context.SaveChangesAsync();

        //     return Ok(new ApiResponse<object>
        //     {
        //         Success = true,
        //         Message = "Role removed successfully"
        //     });
        // }
    }
}