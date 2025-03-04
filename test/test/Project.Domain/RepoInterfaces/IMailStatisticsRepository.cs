using test.Project.Application.DTOs.Response.Statistic.Email;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IMailStatisticsRepository
    {
        Task<List<EmailFrequencyRes>> GetEmailFrequenciesAsync();
        Task<List<MonthlyEmailStatistic>> GetEmailFrequenciesByYearAsync(int year);
        Task<List<MonthlyEmailStatistic>> GetEmailMonthlyStatsAsync(string email, int? year);
    }
} 