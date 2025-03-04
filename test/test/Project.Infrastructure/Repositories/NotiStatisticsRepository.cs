using Microsoft.EntityFrameworkCore;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.Statistic.Notification;
using System.Data;
using Microsoft.Data.SqlClient;
using test.Project.Infrastructure.Extensions;

namespace test.Project.Infrastructure.Repositories
{
    public class NotiStatisticsRepository : INotiStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public NotiStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotiFrequencyRes>> GetNotiFrequenciesAsync()
        {
            var ls = new List<NotiFrequencyRes>();
            DataTable dt = new();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetNotiFrequencies";
                command.CommandType = CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();
                using var result = await command.ExecuteReaderAsync();
                dt.Load(result);
                ls = dt.ConvertDataTable<NotiFrequencyRes>();
            }
            return ls;
        }

        public async Task<List<MonthlyNotiStatistic>> GetNotiFrequenciesByYearAsync(int year)
        {
            var ls = new List<MonthlyNotiStatistic>();
            DataTable dt = new();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetNotiFrequenciesByYear";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Year", year));

                await _context.Database.OpenConnectionAsync();
                using var result = await command.ExecuteReaderAsync();
                dt.Load(result);
                ls = dt.ConvertDataTable<MonthlyNotiStatistic>();
            }
            return ls;
        }

        public async Task<List<MonthlyNotiStatistic>> GetNotiMonthlyStatsAsync(string email, int? year)
        {
            var ls = new List<MonthlyNotiStatistic>();
            DataTable dt = new();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetNotiMonthlyStats";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Email", email));
                command.Parameters.Add(new SqlParameter("@Year", year));

                await _context.Database.OpenConnectionAsync();
                using var result = await command.ExecuteReaderAsync();
                dt.Load(result);
                ls = dt.ConvertDataTable<MonthlyNotiStatistic>();
            }
            return ls;
        }

        public async Task<List<NotiTypeStatsRes>> GetTypeDistributionAsync()
        {
            return await _context.Notification
                .GroupBy(n => n.Type)
                .Select(g => new NotiTypeStatsRes
                {
                    Type = g.Key,
                    Count = g.Count(),
                    UnreadCount = g.Count(n => !n.IsRead),
                    UnseenCount = g.Count(n => n.IsSeen != true)
                })
                .ToListAsync();
        }

        public async Task<List<UserNotiStatsRes>> GetUserStatsByEmailAsync(string email)
        {
            return await _context.Notification
                .Where(n => n.Email == email)
                .GroupBy(n => n.Email)
                .Select(g => new UserNotiStatsRes
                {
                    Email = g.Key,
                    TotalNotifications = g.Count(),
                    ReadCount = g.Count(n => n.IsRead),
                    UnreadCount = g.Count(n => !n.IsRead),
                    SeenCount = g.Count(n => n.IsSeen == true),
                    UnseenCount = g.Count(n => n.IsSeen != true)
                })
                .ToListAsync();
        }

        public async Task<List<UserNotiTypeStatsRes>> GetUserStatsByEmailAndTypeAsync(string email, string? type)
        {
            var query = _context.Notification.Where(n => n.Email == email);
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }

            return await query
                .GroupBy(n => new { n.Email, n.Type })
                .Select(g => new UserNotiTypeStatsRes
                {
                    Email = g.Key.Email,
                    Type = g.Key.Type ?? "Unknown",
                    TotalNotifications = g.Count(),
                    ReadCount = g.Count(n => n.IsRead),
                    UnreadCount = g.Count(n => !n.IsRead),
                    SeenCount = g.Count(n => n.IsSeen == true),
                    UnseenCount = g.Count(n => n.IsSeen != true)
                })
                .OrderBy(x => x.Type)
                .ToListAsync();
        }
    }
} 