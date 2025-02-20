using Microsoft.AspNetCore.Mvc;
using test.Models.Entity;
using test.Models;
using test.Services;
using test.Models.DTOs.Response.Mails;
using test.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using AutoMapper;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using test.Models.DTOs;
using OfficeOpenXml;
using test.Extensions;
using test.Attributes;
using test.Services.Hubs;
using test.Models.DTOs.Response.Notification;

namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public NotificationsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        
        }

        [HttpGet]
        [ModulePermission("Notifications", requireRead: true)]
        public async Task<ActionResult<ApiResponse<PaginatedList<Notification>>>> GetNotifications(
            int? page, 
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate)
        {
            try 
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                int pageSize = 6;
                int pageIndex = page ?? 1;
                
                // Start with base query
                var query = _context.Notification.AsQueryable();

                if (!string.IsNullOrEmpty(id))
                {
                    query = query.Where(m => m.Id.ToString().Equals(id));
                }
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(m => m.Type == type);
                }
                if (!string.IsNullOrEmpty(from))
                {
                    query = query.Where(m => m.From.Contains(from));
                }
                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(m => m.Email == email);
                }
                if (isRead.HasValue)
                {
                    query = query.Where(m => m.IsRead == isRead.Value);
                }
                if (isSeen.HasValue)
                {
                    query = query.Where(m => m.IsSeen == isSeen.Value);
                }
                if (!string.IsNullOrEmpty(timeType) && fromDate.HasValue && toDate.HasValue)
                {
                    if (fromDate.Value <= toDate.Value)
                    {
                        if (timeType == "createdTime")
                        {
                            query = query.Where(m => m.CreatedTime >= fromDate.Value &&
                                                   m.CreatedTime <= toDate.Value);
                        }
                    }
                }
                var totalItems = await query.CountAsync();
                var items = await query
                    .OrderByDescending(m => m.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    items = items,
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    totalItems = totalItems,
                    hasPreviousPage = pageIndex > 1,
                    hasNextPage = pageIndex < (int)Math.Ceiling(totalItems / (double)pageSize)
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        [HttpGet("{id}")]
        [ModulePermission("Notifications", requireRead: true)]
        public async Task<ActionResult<ApiResponse<Notification>>> GetNotification(long id)
        {
            try
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                
                var notification = await _context.Notification.FindAsync(id);

                // Check authorization
                if (notification == null || (!rolesList.Contains("admin")))
                {
                    return StatusCode(403, new ApiResponse<Notification> 
                    {   Success = false, 
                        Message = "Access denied" ,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<Notification>
                {
                    Success = true,
                    Message = "Notification fetched successfully",
                    Data = notification
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Notification>
                {
                    Success = false,
                    Message = "Error fetching notification"
                });
            }
        }

        [HttpPost]
        [ModulePermission("Notifications", requireCreate: true)]
        public async Task<ActionResult<ApiResponse<Notification>>> CreateNotification( Notification notification)
        {
            await _context.Notification.AddAsync(notification);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Notification> 
            { 
                Success = true, 
                Message = "Notification created successfully",
                Data = notification 
            });
        }

        [HttpPut("{id}")]
        [ModulePermission("Notifications", requireUpdate: true)]
        public async Task<ActionResult<ApiResponse<Notification>>> UpdateNotification(long id, Notification notification)
        {
            try
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                var existingNotification = await _context.Notification.FindAsync(id);

                if (existingNotification == null)
                {
                    return StatusCode(403, new ApiResponse<Notification>
                    {
                        Success = false,
                        Message = "Notification not found",
                        Data = null
                    });
                }
                notification.CreatedTime = existingNotification.CreatedTime;
                notification.LastModified = DateTime.Now;
                _mapper.Map(notification, existingNotification);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Notification>
                {
                    Success = true,
                    Data = existingNotification
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Notification>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [ModulePermission("Notifications", requireDelete: true)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(long id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Notification not found" });
            }

            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Notification deleted successfully", Data = null });
        }

        [HttpPost("deleteMultiple")]
        [ModulePermission("Notifications", requireDelete: true)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMultiple([FromBody] List<long> ids)
        {
            var notifications = await _context.Notification.Where(m => ids.Contains(m.Id)).ToListAsync();
            _context.Notification.RemoveRange(notifications);
            await _context.SaveChangesAsync();
            // await _notificationService.NotifyNotificationDeleted(ids.ToString());
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpGet("filter-options")]
        public async Task<ActionResult<ApiResponse<object>>> GetFilterOptions()
        {
            try
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                // Base query: Fetch necessary data only
                var query = _context.Notification.AsQueryable();

                var data = await query
                    .Select(m => new { m.Id, m.Email, m.From, m.Type })
                    .ToListAsync();

                var filterOptions = new NotificationFiltersRes
                {
                    Ids = data.Select(m => m.Id.ToString()).Distinct().ToList(),
                    Emails = data.Select(m => m.Email).ToList(),
                    Froms = data.Select(m => m.From).ToList(),
                    Types = data.Select(m => m.Type).Distinct().ToList()
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Notification filter options fetched successfully",
                    Data = filterOptions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}