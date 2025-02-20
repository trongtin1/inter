using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using test.Services;
using test.Models.Entity;
using test.Models.DTOs.Response.UserModule;
using test.Models.DTOs.Request.UserModule;
using test.Models.DTOs.Response.ModulePermission;
using test.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using test.Attributes;
using test.Services.Hubs;
namespace test.Controllers.Api

{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserModulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserModuleNotificationService _notificationService;
        public UserModulesController(ApplicationDbContext context, IMapper mapper, IUserModuleNotificationService notificationService   )
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        [HttpGet]
        // [ModulePermission("UserModules", requireRead: true)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserModuleRes>>>> GetUserModules()
        {
            var users = await _context.User
                .Include(u => u.UserModules)
                    .ThenInclude(um => um.Module)
                .ToListAsync();

            var userWithModules = users.Select(user => new UserModuleRes
            {
                UserId = user.Id,
                UserName = user.Username,
                Modules = user.UserModules.Select(um => new ModulePermissionDTO
                {
                    ModuleId = um.ModuleId,
                    ModuleName = um.Module.Name,
                    CanCreate = um.CanCreate,
                    CanRead = um.CanRead,
                    CanUpdate = um.CanUpdate,
                    CanDelete = um.CanDelete

                }).ToList()
            });

            return Ok(new ApiResponse<IEnumerable<UserModuleRes>>
            {
                Success = true,
                Message = "User modules retrieved successfully",
                Data = userWithModules
            });
        }

        [HttpPost]
        // [ModulePermission("UserModules", requireCreate: true)]
        public async Task<ActionResult<ApiResponse<UserModuleReq>>> AssignModuleToUser([FromBody] UserModuleReq userModuleReq)
        {
            try 
            {
                var existingUserModule = await _context.UserModule
                    .FirstOrDefaultAsync(um => um.UserId == userModuleReq.UserId && um.ModuleId == userModuleReq.ModuleId);

                if (existingUserModule != null)
                {
                    _mapper.Map(userModuleReq, existingUserModule);
                }
                else
                {
                    var userModule = _mapper.Map<UserModule>(userModuleReq);
                    await _context.UserModule.AddAsync(userModule);
                }

                await _context.SaveChangesAsync();
                await _notificationService.NotifyUserModuleCreated(userModuleReq.UserId.ToString());
                return Ok(new ApiResponse<UserModuleReq>
                {
                    Success = true,
                    Message = existingUserModule != null ? "Module permissions updated successfully" : "Module assigned successfully",
                    Data = userModuleReq
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserModuleReq>
                {
                    Success = false,
                    Message = "Error occurred while processing request: " + ex.Message,
                    Data = null
                });
            }
        }
        [HttpGet("{userId}")]
        // [ModulePermission("UserModules", requireRead: true)]
        public async Task<ActionResult<ApiResponse<UserModuleRes>>> GetUserModulesByUserId(int userId)
        {
            var userModules = await _context.UserModule
                .Include(um => um.User)
                .Include(um => um.Module)
                .Where(um => um.UserId == userId)
                .GroupBy(um => new { um.UserId, UserName = um.User.Username })
                .Select(g => new UserModuleRes
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    Modules = g.Select(um => new ModulePermissionDTO
                    {
                        ModuleId = um.ModuleId,
                        ModuleName = um.Module.Name,
                        CanCreate = um.CanCreate,
                        CanRead = um.CanRead,
                        CanUpdate = um.CanUpdate,
                        CanDelete = um.CanDelete
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (userModules == null)
            {
                return NotFound(new ApiResponse<UserModuleRes>
                {
                    Success = false,
                    Message = $"No roles found for role with ID {userId}",
                    Data = null
                });
            }
            return Ok(new ApiResponse<UserModuleRes>
            {
                Success = true,
                Message = "Role modules retrieved successfully",
                Data = userModules
            });
        }
        
    }
} 