using test.Project.Domain.Entity;
using test.Project.Domain.Common;

namespace test.Project.Domain.RepoInterfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        //Task<(List<Notification> items, int totalItems)> GetNotificationsAsync(
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
        Task<(IEnumerable<Notification> Items, int TotalCount)> GetPageNotification(
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize);
        Task<Notification> GetOneNotification(
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate);
        Task<List<Notification>> GetListNotification(
            string? id,
            string? type,
            bool? isRead,
            string? email,
            string? from,
            bool? isSeen,
            string? timeType,
            DateTime? fromDate,
            DateTime? toDate);
        Task<Notification> GetByIdAsync(long id);
        Task<List<Notification>> GetByIdsAsync(List<long> ids);
        Task<List<Notification>> GetFilterDataAsync();
    }
} 