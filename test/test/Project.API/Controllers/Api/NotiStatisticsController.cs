using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using test.Project.API.Attributes;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Statistic.Notification;
using test.Project.Application.ServiceInterfaces;
namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotiStatisticsController : ControllerBase
    {
        private readonly INotiStatisticsService _notiStatisticsService;

        public NotiStatisticsController(INotiStatisticsService notiStatisticsService)
        {
            _notiStatisticsService = notiStatisticsService;
        }
        
        [HttpGet("most-noti-frequencies")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<NotiFrequencyRes>>>> GetNotiFrequencies()
        {
            return await _notiStatisticsService.GetNotiFrequenciesAsync();
        }
        
        [HttpGet("noti-frequencies-by-year")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<MonthlyNotiCountRes>>> GetNotiFrequenciesByYear(int year)
        {
            return await _notiStatisticsService.GetNotiFrequenciesByYearAsync(year);
        }

        [HttpGet("noti-monthly-stats")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<UserNotiRes>>> GetNotiMonthlyStats(string email, [FromQuery] int? year)
        {
            return await _notiStatisticsService.GetNotiMonthlyStatsAsync(email, year);
        }

        [HttpGet("type-distribution")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<NotiTypeStatsRes>>>> GetTypeDistribution()
        {
            return await _notiStatisticsService.GetTypeDistributionAsync();
        }

        [HttpGet("user-stats-by-email")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<UserNotiStatsRes>>>> GetUserStatsByEmail(string email)
        {
            return await _notiStatisticsService.GetUserStatsByEmailAsync(email);
        }

        [HttpGet("user-stats-by-email-and-type")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<UserNotiTypeStatsRes>>>> GetUserStatsByEmailAndType(string email, string? type = null)
        {
            return await _notiStatisticsService.GetUserStatsByEmailAndTypeAsync(email, type);
        }
    }
}