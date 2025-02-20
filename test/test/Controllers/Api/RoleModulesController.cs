using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using test.Services;
using test.Models.Entity;
using test.Models.DTOs.Response.RoleModule;
using test.Models.DTOs.Request.RoleModule;
using test.Models.DTOs.Response.ModulePermission;
using test.Models.DTOs;

namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleModulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoleModulesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleModuleRes>>>> GetRoleModules()
        {
            var roles = await _context.Role
                .Include(r => r.RoleModules)
                    .ThenInclude(rm => rm.Module)
                .ToListAsync();

            var roleWithModules = roles.Select(role => new RoleModuleRes
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Modules = role.RoleModules.Select(rm => new ModulePermissionDTO
                {
                    ModuleId = rm.ModuleId,
                    ModuleName = rm.Module.Name,
                    CanCreate = rm.CanCreate,
                    CanRead = rm.CanRead,
                    CanUpdate = rm.CanUpdate,
                    CanDelete = rm.CanDelete

                }).ToList()
            });

            return Ok(new ApiResponse<IEnumerable<RoleModuleRes>>
            {
                Success = true,
                Message = "Role modules retrieved successfully",
                Data = roleWithModules
            });
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<ApiResponse<RoleModuleRes>>> GetRoleModulesByRoleId(int roleId)
        {
            var roleModules = await _context.RoleModule
                .Include(rm => rm.Role)
                .Include(rm => rm.Module)
                .Where(rm => rm.RoleId == roleId)
                .GroupBy(rm => new { rm.RoleId, RoleName = rm.Role.Name })
                .Select(g => new RoleModuleRes
                {
                    RoleId = g.Key.RoleId,
                    RoleName = g.Key.RoleName,
                    Modules = g.Select(rm => new ModulePermissionDTO
                    {
                        ModuleId = rm.ModuleId,
                        ModuleName = rm.Module.Name,
                        CanCreate = rm.CanCreate,
                        CanRead = rm.CanRead,
                        CanUpdate = rm.CanUpdate,
                        CanDelete = rm.CanDelete
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (roleModules == null)
            {
                return NotFound(new ApiResponse<RoleModuleRes>
                {
                    Success = false,
                    Message = $"No roles found for role with ID {roleId}"

                });
            }

            return Ok(new ApiResponse<RoleModuleRes>
            {
                Success = true,
                Message = "Role modules retrieved successfully",
                Data = roleModules


            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleModuleReq>>> AssignModuleToRole([FromBody] RoleModuleReq roleModuleReq)
        {
            try 
            {
                var existingRoleModule = await _context.RoleModule
                    .FirstOrDefaultAsync(rm => rm.RoleId == roleModuleReq.RoleId && rm.ModuleId == roleModuleReq.ModuleId);

                if (existingRoleModule != null)
                {
                    _mapper.Map(roleModuleReq, existingRoleModule);
                }
                else
                {
                    var roleModule = _mapper.Map<RoleModule>(roleModuleReq);
                    await _context.RoleModule.AddAsync(roleModule);
                }

                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<RoleModuleReq>
                {
                    Success = true,
                    Message = existingRoleModule != null ? "Module permissions updated successfully" : "Module assigned successfully",
                    Data = roleModuleReq
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<RoleModuleReq>
                {
                    Success = false,
                    Message = "Error occurred while processing request: " + ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<List<RoleModuleReq>>>> UpdateModulePermissions([FromBody] List<RoleModuleReq> roleModuleReqs)
        {
            try 
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    foreach (var roleModuleReq in roleModuleReqs)
                    {
                        // Cập nhật RoleModule
                        var existingRoleModule = await _context.RoleModule
                            .FirstOrDefaultAsync(rm => rm.RoleId == roleModuleReq.RoleId && rm.ModuleId == roleModuleReq.ModuleId);

                        if (existingRoleModule == null)
                        {
                            return BadRequest(new ApiResponse<List<RoleModuleReq>>
                            {
                                Success = false,
                                Message = $"Role-Module combination not found for RoleId: {roleModuleReq.RoleId} and ModuleId: {roleModuleReq.ModuleId}",
                                Data = null
                            });
                        }

                        // Cập nhật quyền cho RoleModule
                        existingRoleModule.CanCreate = roleModuleReq.CanCreate;
                        existingRoleModule.CanRead = roleModuleReq.CanRead;
                        existingRoleModule.CanUpdate = roleModuleReq.CanUpdate;
                        existingRoleModule.CanDelete = roleModuleReq.CanDelete;

                        // Lấy danh sách user có role này
                        var usersWithRole = await _context.UserRole
                            .Where(ur => ur.RoleId == roleModuleReq.RoleId)
                            .Select(ur => ur.UserId)
                            .ToListAsync();

                        // Cập nhật UserModule cho từng user
                        foreach (var userId in usersWithRole)
                        {
                            var existingUserModule = await _context.UserModule
                                .FirstOrDefaultAsync(um => um.UserId == userId && um.ModuleId == roleModuleReq.ModuleId);

                            if (existingUserModule != null)
                            {
                                existingUserModule.CanCreate = roleModuleReq.CanCreate;
                                existingUserModule.CanRead = roleModuleReq.CanRead;
                                existingUserModule.CanUpdate = roleModuleReq.CanUpdate;
                                existingUserModule.CanDelete = roleModuleReq.CanDelete;
                            }
                            else
                            {
                                await _context.UserModule.AddAsync(new UserModule
                                {
                                    UserId = userId,
                                    ModuleId = roleModuleReq.ModuleId,
                                    CanCreate = roleModuleReq.CanCreate,
                                    CanRead = roleModuleReq.CanRead,
                                    CanUpdate = roleModuleReq.CanUpdate,
                                    CanDelete = roleModuleReq.CanDelete
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new ApiResponse<List<RoleModuleReq>>
                    {
                        Success = true,
                        Message = "Module permissions updated successfully for roles and associated users",
                        Data = roleModuleReqs
                    });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<RoleModuleReq>>
                {
                    Success = false,
                    Message = "Error occurred while processing request: " + ex.Message,
                    Data = null
                });
            }
        }
    }
} 