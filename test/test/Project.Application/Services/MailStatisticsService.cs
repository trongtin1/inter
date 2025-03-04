using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Statistic.Email;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
    public class MailStatisticsService : IMailStatisticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MailStatisticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<EmailFrequencyRes>>> GetEmailFrequenciesAsync()
        {
            try
            {
                var emailFrequencies = await _unitOfWork.MailStatistics.GetEmailFrequenciesAsync();

                if (!emailFrequencies.Any())
                {
                    return new ApiResponse<List<EmailFrequencyRes>>
                    {
                        Success = false,
                        Message = "No emails found in the system.",
                        Data = null
                    };
                }

                return new ApiResponse<List<EmailFrequencyRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved email frequencies.",
                    Data = emailFrequencies
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<EmailFrequencyRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<MonthlyEmailCountRes>> GetEmailFrequenciesByYearAsync(int year)
        {
            try
            {
                var monthlyStats = await _unitOfWork.MailStatistics.GetEmailFrequenciesByYearAsync(year);

                var response = new MonthlyEmailCountRes
                {
                    MonthlyStats = monthlyStats
                };

                return new ApiResponse<MonthlyEmailCountRes>
                {
                    Success = true,
                    Message = $"Successfully retrieved monthly email counts for year {year}.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MonthlyEmailCountRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<UserEmailRes>> GetEmailMonthlyStatsAsync(string email, int? year)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ApiResponse<UserEmailRes>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    };
                }

                var monthlyStats = await _unitOfWork.MailStatistics.GetEmailMonthlyStatsAsync(email, year);
                
                var response = new UserEmailRes
                {
                    Email = email,
                    MonthlyStats = monthlyStats,
                    YearlyTotal = monthlyStats.Sum(x => x.Count)
                };

                return new ApiResponse<UserEmailRes>
                {
                    Success = true,
                    Message = "Successfully retrieved email statistics.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserEmailRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
} 