using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Statistic.Notification;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
    public class NotiStatisticsService : INotiStatisticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotiStatisticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<NotiFrequencyRes>>> GetNotiFrequenciesAsync()
        {
            try
            {
                var ls = await _unitOfWork.NotiStatistics.GetNotiFrequenciesAsync();
                return new ApiResponse<List<NotiFrequencyRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved notification frequencies.",
                    Data = ls
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NotiFrequencyRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<MonthlyNotiCountRes>> GetNotiFrequenciesByYearAsync(int year)
        {
            try
            {
                var ls = new MonthlyNotiCountRes();
                ls.MonthlyStats = await _unitOfWork.NotiStatistics.GetNotiFrequenciesByYearAsync(year);
                return new ApiResponse<MonthlyNotiCountRes>
                {
                    Success = true,
                    Message = $"Successfully retrieved monthly notification counts for year {year}.",
                    Data = ls
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MonthlyNotiCountRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<UserNotiRes>> GetNotiMonthlyStatsAsync(string email, int? year)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ApiResponse<UserNotiRes>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    };
                }

                var ls = new UserNotiRes { Email = email };
                ls.MonthlyStats = await _unitOfWork.NotiStatistics.GetNotiMonthlyStatsAsync(email, year);
                ls.YearlyTotal = ls.MonthlyStats.Sum(x => x.Count);

                return new ApiResponse<UserNotiRes>
                {
                    Success = true,
                    Message = "Successfully retrieved notification statistics.",
                    Data = ls
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserNotiRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<List<NotiTypeStatsRes>>> GetTypeDistributionAsync()
        {
            try
            {
                var typeStats = await _unitOfWork.NotiStatistics.GetTypeDistributionAsync();
                return new ApiResponse<List<NotiTypeStatsRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved notification type distribution.",
                    Data = typeStats
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NotiTypeStatsRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<List<UserNotiStatsRes>>> GetUserStatsByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ApiResponse<List<UserNotiStatsRes>>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    };
                }

                var userStats = await _unitOfWork.NotiStatistics.GetUserStatsByEmailAsync(email);
                return new ApiResponse<List<UserNotiStatsRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved user notification statistics.",
                    Data = userStats
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserNotiStatsRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<List<UserNotiTypeStatsRes>>> GetUserStatsByEmailAndTypeAsync(string email, string? type)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ApiResponse<List<UserNotiTypeStatsRes>>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    };
                }

                var userTypeStats = await _unitOfWork.NotiStatistics.GetUserStatsByEmailAndTypeAsync(email, type);
                return new ApiResponse<List<UserNotiTypeStatsRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved user notification type statistics.",
                    Data = userTypeStats
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserNotiTypeStatsRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
} 