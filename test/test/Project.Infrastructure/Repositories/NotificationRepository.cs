using Microsoft.EntityFrameworkCore;
using test.Project.Domain.Entity;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Infrastructure.Common;
using System.Linq.Expressions;
using System.Threading;

namespace test.Project.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Notification> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<Notification>> GetByIdsAsync(List<long> ids)
        {
            return await _dbSet
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();
        }

        //public async Task<(List<Notification> items, int totalItems)> GetNotificationsAsync(
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
        //    DateTime? toDate)
        //{
        //    var query = _context.Notification.AsQueryable();

        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        query = query.Where(m => m.Id.ToString().Equals(id));
        //    }
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        query = query.Where(m => m.Type == type);
        //    }
        //    if (!string.IsNullOrEmpty(from))
        //    {
        //        query = query.Where(m => m.From.Contains(from));
        //    }
        //    if (!string.IsNullOrEmpty(email))
        //    {
        //        query = query.Where(m => m.Email == email);
        //    }
        //    if (isRead.HasValue)
        //    {
        //        query = query.Where(m => m.IsRead == isRead.Value);
        //    }
        //    if (isSeen.HasValue)
        //    {
        //        query = query.Where(m => m.IsSeen == isSeen.Value);
        //    }
        //    if (!string.IsNullOrEmpty(timeType) && fromDate.HasValue && toDate.HasValue)
        //    {
        //        if (fromDate.Value <= toDate.Value)
        //        {
        //            if (timeType == "createdTime")
        //            {
        //                query = query.Where(m => m.CreatedTime >= fromDate.Value &&
        //                                       m.CreatedTime <= toDate.Value);
        //            }
        //        }
        //    }

        //    var totalItems = await query.CountAsync();
        //    var items = await query
        //        .OrderByDescending(m => m.Id)
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return (items, totalItems);
        //}
        public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetPageNotification(
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
            int pageSize)
            {
                Expression<Func<Notification, bool>> predicate = x =>
                    (string.IsNullOrEmpty(id) || x.Id.ToString().Equals(id)) &&
                    (string.IsNullOrEmpty(type) || x.Type == type) &&
                    (string.IsNullOrEmpty(email) || x.Email == email) &&
                    (string.IsNullOrEmpty(from) || x.From.Contains(from)) &&
                    (!isRead.HasValue || x.IsRead == isRead.Value) &&
                    (!isSeen.HasValue || x.IsSeen == isSeen.Value) &&
                    (string.IsNullOrEmpty(timeType) ||
                        (fromDate.HasValue && toDate.HasValue && fromDate.Value <= toDate.Value &&
                            ((timeType == "createdTime" && x.CreatedTime >= fromDate.Value && x.CreatedTime <= toDate.Value))));

                return await FindPagedListAsync(predicate, pageIndex, pageSize);
            }
        public async Task<Notification> GetOneNotification(
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
            return await GetOneAsync(x =>
                    (string.IsNullOrEmpty(id) || x.Id.ToString().Equals(id)) &&
                    (string.IsNullOrEmpty(type) || x.Type == type) &&
                    (string.IsNullOrEmpty(email) || x.Email == email) &&
                    (string.IsNullOrEmpty(from) || x.From.Contains(from)) &&
                    (!isRead.HasValue || x.IsRead == isRead.Value) &&
                    (!isSeen.HasValue || x.IsSeen == isSeen.Value) &&
                    (string.IsNullOrEmpty(timeType) ||
                        (fromDate.HasValue && toDate.HasValue && fromDate.Value <= toDate.Value &&
                            ((timeType == "createdTime" && x.CreatedTime >= fromDate.Value && x.CreatedTime <= toDate.Value)))));
        }

        public async Task<List<Notification>> GetListNotification(
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
            return await GetListAsync(x =>
                    (string.IsNullOrEmpty(id) || x.Id.ToString().Equals(id)) &&
                    (string.IsNullOrEmpty(type) || x.Type == type) &&
                    (string.IsNullOrEmpty(email) || x.Email == email) &&
                    (string.IsNullOrEmpty(from) || x.From.Contains(from)) &&
                    (!isRead.HasValue || x.IsRead == isRead.Value) &&
                    (!isSeen.HasValue || x.IsSeen == isSeen.Value) &&
                    (string.IsNullOrEmpty(timeType) ||
                        (fromDate.HasValue && toDate.HasValue && fromDate.Value <= toDate.Value &&
                            ((timeType == "createdTime" && x.CreatedTime >= fromDate.Value && x.CreatedTime <= toDate.Value)))));
        }
        public async Task<List<Notification>> GetFilterDataAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
} 