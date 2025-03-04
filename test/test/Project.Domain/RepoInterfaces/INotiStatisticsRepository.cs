using test.Project.Application.DTOs.Response.Statistic.Notification;

namespace test.Project.Domain.RepoInterfaces
{
    public interface INotiStatisticsRepository
    {
        Task<List<NotiFrequencyRes>> GetNotiFrequenciesAsync();
        Task<List<MonthlyNotiStatistic>> GetNotiFrequenciesByYearAsync(int year);
        Task<List<MonthlyNotiStatistic>> GetNotiMonthlyStatsAsync(string email, int? year);
        Task<List<NotiTypeStatsRes>> GetTypeDistributionAsync();
        Task<List<UserNotiStatsRes>> GetUserStatsByEmailAsync(string email);
        Task<List<UserNotiTypeStatsRes>> GetUserStatsByEmailAndTypeAsync(string email, string? type);
    }
} 