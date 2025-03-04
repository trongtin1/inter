using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Statistic.Email;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IMailStatisticsService
    {
        Task<ApiResponse<List<EmailFrequencyRes>>> GetEmailFrequenciesAsync();
        Task<ApiResponse<MonthlyEmailCountRes>> GetEmailFrequenciesByYearAsync(int year);
        Task<ApiResponse<UserEmailRes>> GetEmailMonthlyStatsAsync(string email, int? year);
    }
} 