using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using test.Project.API.Attributes;
using test.Project.Application.DTOs;
using test.Project.Domain.Entity;
using test.Project.Application.ServiceInterfaces;
namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [ModulePermission("Notifications", requireRead: true)]
        public async Task<ApiResponse<PagedResult<Notification>>> GetNotifications(
            int? page, 
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate,
            int? pageIndex,
            int? pageSize)
        {
            return await _notificationService.GetPageNotification(
                 id, type, isRead, email, from, isSeen,
                timeType, fromDate, toDate, pageIndex,pageSize);
        }

        [HttpGet("{id}")]
        [ModulePermission("Notifications", requireRead: true)]
        public async Task<ApiResponse<Notification>> GetNotification(long id)
        {
            //var (claimsId, userEmail, rolesList) = HttpContext.GetUserClaims();
            return await _notificationService.GetNotificationByIdAsync(id);
        }

        [HttpPost]
        [ModulePermission("Notifications", requireCreate: true)]
        public async Task<ApiResponse<Notification>> CreateNotification(Notification notification)
        {
            return await _notificationService.CreateNotificationAsync(notification);
        }

        [HttpPut("{id}")]
        [ModulePermission("Notifications", requireUpdate: true)]
        public async Task<ApiResponse<Notification>> UpdateNotification(long id, Notification notification)
        {
           
            return await _notificationService.UpdateNotificationAsync(id, notification);
        }

        [HttpDelete("{id}")]
        [ModulePermission("Notifications", requireDelete: true)]
        public async Task<ApiResponse<object>> DeleteNotification(long id)
        {
            return await _notificationService.DeleteNotificationAsync(id);
        }

        [HttpPost("deleteMultiple")]
        [ModulePermission("Notifications", requireDelete: true)]
        public async Task<ApiResponse<object>> DeleteMultiple([FromBody] List<long> ids)
        {
            return await _notificationService.DeleteMultipleNotificationsAsync(ids);
        }

        [HttpGet("filter-options")]
        public async Task<ApiResponse<object>> GetFilterOptions()
        {
            return await _notificationService.GetFilterOptionsAsync();
        }


        [HttpGet("getOne")]
        [ModulePermission("Notifications", requireRead: true)]
        public async Task<ApiResponse<Notification>> GetOne(
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
            
            return await _notificationService.GetOneNotification(
                id, type, isRead, email, from, isSeen,
                timeType, fromDate, toDate);
        }

        [HttpGet("getList")]
        [ModulePermission("Notifications", requireRead: true)]
        public async Task<ApiResponse<List<Notification>>> GetList(
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
            
            return await _notificationService.GetListNotification(
                id, type, isRead, email, from, isSeen,
                timeType, fromDate, toDate);
        }
    }
}