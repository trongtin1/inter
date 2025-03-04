using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using test.Project.API.Attributes;
using test.Project.Application.DTOs.Response.Statistic.Email;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailStatisticsController : ControllerBase
    {
        private readonly IMailStatisticsService _mailStatisticsService;

        public MailStatisticsController(IMailStatisticsService mailStatisticsService)
        {
            _mailStatisticsService = mailStatisticsService;
        }

        [HttpGet("most-email-frequencies")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ApiResponse<List<EmailFrequencyRes>>> GetEmailFrequencies()
        {
            return await _mailStatisticsService.GetEmailFrequenciesAsync();
            
        }

        [HttpGet("email-frequencies-by-year")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ApiResponse<MonthlyEmailCountRes>> GetEmailFrequenciesByYear(int year)
        {
            return await _mailStatisticsService.GetEmailFrequenciesByYearAsync(year);
           
        }

        [HttpGet("email-monthly-stats")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ApiResponse<UserEmailRes>> GetEmailMonthlyStats(
            string email, [FromQuery] int? year)
        {
            return await _mailStatisticsService.GetEmailMonthlyStatsAsync(email, year);
            
        }
    }
}