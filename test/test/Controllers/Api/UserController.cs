using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Services;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;
using test.Models.DTOs;
using AutoMapper;
namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDTO>>>> GetUsers()
        {
            var users = await _context.User
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<UserDTO>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = users
            });
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetUser(int id)
        {
            var user = await _context.User
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new ApiResponse<UserDTO>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = user
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDTO>>> CreateUser([FromBody] UserRegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<UserDTO>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = null
                });

            if (await _context.User.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest(new ApiResponse<UserDTO>
                {
                    Success = false,
                    Message = "Username already exists",
                    Data = null
                });
            }

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = new List<string>()
            };

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "User registered successfully",
                Data = userDto
            });
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> UpdateUser(int id, UpdateUserDTO updateUserDto)
        {
            var existingUser = await _context.User.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new ApiResponse<UserDTO>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            _mapper.Map(updateUserDto, existingUser);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDTO>(existingUser);

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "User updated successfully",
                Data = userDto
            });
        }

        // DELETE: api/Role/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(int id)
        // {
        //     var user = await _context.User
        //         .Include(r => r.UserRoles)
        //         .FirstOrDefaultAsync(r => r.Id == id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.User.Remove(user);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}