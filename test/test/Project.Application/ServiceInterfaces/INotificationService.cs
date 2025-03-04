using test.Project.Application.DTOs;
using test.Project.Domain.Entity;
namespace test.Project.Application.ServiceInterfaces
{
    public interface INotificationService
    {
        //Task<ApiResponse<PagedResult<Notification>>> GetNotificationsAsync(
        //    int pageIndex, 
        //    int pageSize, 
        //    string? id,
        //    string? type,
        //    bool? isRead,
        //    string? email,
        //    string? from,
        //    bool? isSeen,
        //    string? timeType,
        //    DateTime? fromDate,
        //    DateTime? toDate);
        Task<ApiResponse<PagedResult<Notification>>> GetPageNotification(
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
            int? pageSize);
        Task<ApiResponse<Notification>> GetOneNotification(
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate);
        Task<ApiResponse<List<Notification>>> GetListNotification(
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate);
        Task<ApiResponse<Notification>> GetNotificationByIdAsync(long id);
        Task<ApiResponse<Notification>> CreateNotificationAsync(Notification notification);
        Task<ApiResponse<Notification>> UpdateNotificationAsync(long id, Notification notification);
        Task<ApiResponse<object>> DeleteNotificationAsync(long id);
        Task<ApiResponse<object>> DeleteMultipleNotificationsAsync(List<long> ids);
        Task<ApiResponse<object>> GetFilterOptionsAsync();
    }
} 