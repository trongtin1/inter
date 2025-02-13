using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Models.Entity;
using test.Services;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;
using test.Models.DTOs;
using test.Models.DTOs.Response.User;
using test.Models.DTOs.Response.UserRole;
using AutoMapper;
using test.Models.DTOs.Request.User;

namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserRes>>>> GetUsers()
        {
            var users = await _context.User
                .Select(u => new UserRes
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .ToListAsync();


            return Ok(new ApiResponse<IEnumerable<UserRes>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = users
            });
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserRes>>> GetUser(int id)
        {
            var user = await _context.User
                .Where(u => u.Id == id)
                .Select(u => new UserRes
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserRes>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = user
            });
        }

        private async Task<bool> IsUsernameExists(string username, int? excludeId = null)
        {
            return await _context.User.AnyAsync(u => u.Username == username && (!excludeId.HasValue || u.Id != excludeId));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserRes>>> CreateUser([FromBody] UserReq model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = null
                });

            if (await IsUsernameExists(model.Username))
            {
                return BadRequest(new ApiResponse<UserRes>
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

            // Create response with user ID
            var userResponse = new UserRes
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return Ok(new ApiResponse<UserRes>
            {
                Success = true,
                Message = "User registered successfully",
                Data = userResponse
            });
        }


        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserRes>>> UpdateUser(int id, UserReq updateUserDto)
        {
            if (await IsUsernameExists(updateUserDto.Username, id))
            {
                return BadRequest(new ApiResponse<UserReq>
                {
                    Success = false,
                    Message = "Username already exists"
                });
            }

            var existingUser = await _context.User.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new ApiResponse<UserReq>
                {
                    Success = false,
                    Message = "User not found"
                });
            }
            _mapper.Map(updateUserDto, existingUser);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<UserReq>
            {
                Success = true,
                Message = "User updated successfully",
                Data = updateUserDto
            });
        }


        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<UserRes>>> DeleteUser(int id)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(r => r.Id == id);
            if (user == null)
            {
                return NotFound(new ApiResponse<UserRes>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserRes>
            {
                Success = true,
                Message = "User deleted successfully"
            });
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}