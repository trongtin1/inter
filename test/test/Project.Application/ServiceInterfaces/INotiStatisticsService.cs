using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Statistic.Notification;

namespace test.Project.Application.ServiceInterfaces
{
    public interface INotiStatisticsService
    {
        Task<ApiResponse<List<NotiFrequencyRes>>> GetNotiFrequenciesAsync();
        Task<ApiResponse<MonthlyNotiCountRes>> GetNotiFrequenciesByYearAsync(int year);
        Task<ApiResponse<UserNotiRes>> GetNotiMonthlyStatsAsync(string email, int? year);
        Task<ApiResponse<List<NotiTypeStatsRes>>> GetTypeDistributionAsync();
        Task<ApiResponse<List<UserNotiStatsRes>>> GetUserStatsByEmailAsync(string email);
        Task<ApiResponse<List<UserNotiTypeStatsRes>>> GetUserStatsByEmailAndTypeAsync(string email, string? type);
    }
} 