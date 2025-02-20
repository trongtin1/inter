using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using test.Models.Entity;
using test.Models.DTOs.Request;
using test.Models.DTOs.Response;
using test.Models.DTOs;
using test.Services;
using AutoMapper;
using test.Models.DTOs.Response.Statistic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Reflection;
using test.Extensions;
using Microsoft.AspNetCore.Authorization;
using test.Attributes;
using test.Models.DTOs.Response.Statistic.Email;
namespace test.Controllers.Api

{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailStatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MailStatisticsController(ApplicationDbContext context)
        {
            _context = context;
          
        }
        // GET: api/Statistics/email-frequencies
        [HttpGet("most-email-frequencies")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<EmailFrequencyRes>>>> GetEmailFrequencies()
        {
            try
            {
                var ls = new List<EmailFrequencyRes>();
                DataTable dt = new();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetEmailFrequencies";
                    command.CommandType = CommandType.StoredProcedure;
                    // command.Parameters.Add(new SqlParameter("@Year", year));

                    await _context.Database.OpenConnectionAsync();
                    using var result = await command.ExecuteReaderAsync();
                    dt.Load(result);
                    ls = dt.ConvertDataTable<EmailFrequencyRes>();
                }
                // var emailFrequencies = await _context.Mail
                //     .GroupBy(e => e.Email)
                //     .Select(g => new EmailFrequencyRes
                //     {
                //         Email = g.Key,
                //         Count = g.Count()
                //     })
                //     .OrderByDescending(x => x.Count)
                //     .ToListAsync();

                // if (!emailFrequencies.Any())
                // {
                //     return Ok(new ApiResponse<List<EmailFrequencyRes>>
                //     {
                //         Success = false,
                //         Message = "No emails found in the system.",
                //         Data = null
                //     });
                // }

                return Ok(new ApiResponse<List<EmailFrequencyRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved email frequencies.",
                    Data = ls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<EmailFrequencyRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        
        // GET: api/Statistics/email-frequencies-by-year
        [HttpGet("email-frequencies-by-year")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<MonthlyEmailCountRes>>> GetEmailFrequenciesByYear(int year)

        {
            try
            {
                var ls = new MonthlyEmailCountRes();
                DataTable dt = new();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetEmailFrequenciesByYear";
                    command.CommandType = CommandType.StoredProcedure;
                    
                    command.Parameters.Add(new SqlParameter("@Year", year));

                    await _context.Database.OpenConnectionAsync();
                    using var result = await command.ExecuteReaderAsync();
                    dt.Load(result);
                    ls.MonthlyStats = dt.ConvertDataTable<MonthlyEmailStatistic>();
                }
                //var monthlyStats = await _context.Mail
                //    .Where(e => e.CreateTime.Year == year)
                //    .GroupBy(e => e.CreateTime.Month)
                //    .Select(g => new MonthlyEmailCountRes
                //    {
                //        Year = year,
                //        Month = g.Key,
                //        Count = g.Count()
                //    })
                //    .OrderBy(x => x.Month)
                //    .ToListAsync();

                //// Tạo danh sách đầy đủ 12 tháng, nếu tháng nào không có data thì count = 0
                //var result = Enumerable.Range(1, 12)
                //    .Select(month => monthlyStats.FirstOrDefault(x => x.Month == month) ?? 
                //        new MonthlyEmailCountRes 
                //        { 
                //            Year = year, 
                //            Month = month, 
                //            Count = 0 
                //        })
                //    .ToList();

                return Ok(new ApiResponse<MonthlyEmailCountRes>
                {
                    Success = true,
                    Message = $"Successfully retrieved monthly email counts for year {year}.",
                    Data = ls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MonthlyEmailCountRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [HttpGet("email-monthly-stats")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<UserEmailRes>>> GetEmailMonthlyStats(string email, [FromQuery] int? year)

        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ApiResponse<UserEmailRes>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    });
                }

                var ls = new UserEmailRes { Email = email };
                DataTable dt = new();
                
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetEmailMonthlyStats";
                    command.CommandType = CommandType.StoredProcedure;
                    
                    command.Parameters.Add(new SqlParameter("@Email", email));
                    command.Parameters.Add(new SqlParameter("@Year", year));

                    await _context.Database.OpenConnectionAsync();
                    using var result = await command.ExecuteReaderAsync();
                    dt.Load(result);
                    
                    ls.MonthlyStats = dt.ConvertDataTable<MonthlyEmailStatistic>();
                    // Convert DataTable to List<MonthlyStatistic>
                    // ls.MonthlyStats = dt.AsEnumerable()
                    //     .Select(row => new MonthlyStatistic 
                    //     {
                    //         Month = row.Field<int>("Month"),
                    //         Count = row.Field<int>("Count")
                    //     })
                    //     .ToList();
                    
                    ls.YearlyTotal = ls.MonthlyStats.Sum(x => x.Count);
                }

                return Ok(new ApiResponse<UserEmailRes>
                {
                    Success = true,
                    Message = "Successfully retrieved email statistics.",
                    Data = ls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserEmailRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        
    }
}