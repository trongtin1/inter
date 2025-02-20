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
using test.Models.DTOs.Response.Statistic.Notification;
namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotiStatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotiStatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("most-noti-frequencies")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<NotiFrequencyRes>>>> GetNotiFrequencies()
        {
            try
            {
                var ls = new List<NotiFrequencyRes>();
                DataTable dt = new();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetNotiFrequencies";
                    command.CommandType = CommandType.StoredProcedure;
                    // command.Parameters.Add(new SqlParameter("@Year", year));

                    await _context.Database.OpenConnectionAsync();
                    using var result = await command.ExecuteReaderAsync();
                    dt.Load(result);
                    ls = dt.ConvertDataTable<NotiFrequencyRes>();
                }

                return Ok(new ApiResponse<List<NotiFrequencyRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved email frequencies.",
                    Data = ls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<NotiFrequencyRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        
        [HttpGet("noti-frequencies-by-year")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<MonthlyNotiCountRes>>> GetNotiFrequenciesByYear(int year)

        {
            try
            {
                var ls = new MonthlyNotiCountRes();
                DataTable dt = new();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetNotiFrequenciesByYear";
                    command.CommandType = CommandType.StoredProcedure;
                    
                    command.Parameters.Add(new SqlParameter("@Year", year));

                    await _context.Database.OpenConnectionAsync();
                    using var result = await command.ExecuteReaderAsync();
                    dt.Load(result);
                    ls.MonthlyStats = dt.ConvertDataTable<MonthlyNotiStatistic>();
                }

                return Ok(new ApiResponse<MonthlyNotiCountRes>
                {
                    Success = true,
                    Message = $"Successfully retrieved monthly notification counts for year {year}.",
                    Data = ls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MonthlyNotiCountRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [HttpGet("noti-monthly-stats")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<UserNotiRes>>> GetNotiMonthlyStats(string email, [FromQuery] int? year)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ApiResponse<UserNotiRes>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    });
                }

                var ls = new UserNotiRes { Email = email };
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
                    
                    ls.MonthlyStats = dt.ConvertDataTable<MonthlyNotiStatistic>();
                    
                    ls.YearlyTotal = ls.MonthlyStats.Sum(x => x.Count);
                }

                return Ok(new ApiResponse<UserNotiRes>
                {
                    Success = true,
                    Message = "Successfully retrieved notification statistics.",
                    Data = ls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserNotiRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("type-distribution")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<NotiTypeStatsRes>>>> GetTypeDistribution()
        {
            try
            {
                var typeStats = await _context.Notification
                    .GroupBy(n => n.Type)
                    .Select(g => new NotiTypeStatsRes
                    {
                        Type = g.Key ?? "Unknown",
                        Count = g.Count(),
                        UnreadCount = g.Count(n => !n.IsRead),
                        UnseenCount = g.Count(n => n.IsSeen != true)
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<NotiTypeStatsRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved notification type distribution.",
                    Data = typeStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<NotiTypeStatsRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("user-stats-by-email")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<UserNotiStatsRes>>>> GetUserStatsByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ApiResponse<List<UserNotiStatsRes>>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    });
                }

                var userStats = await _context.Notification
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

                return Ok(new ApiResponse<List<UserNotiStatsRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved user notification statistics.",
                    Data = userStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<UserNotiStatsRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("user-stats-by-email-and-type")]
        [ModulePermission("Statistics", requireRead: true)]
        public async Task<ActionResult<ApiResponse<List<UserNotiTypeStatsRes>>>> GetUserStatsByEmailAndType(string email, string? type = null)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ApiResponse<List<UserNotiTypeStatsRes>>
                    {
                        Success = false,
                        Message = "Email is required.",
                        Data = null
                    });
                }

                var query = _context.Notification.Where(n => n.Email == email);
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(n => n.Type == type);
                }

                var userTypeStats = await query
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

                return Ok(new ApiResponse<List<UserNotiTypeStatsRes>>
                {
                    Success = true,
                    Message = "Successfully retrieved user notification type statistics.",
                    Data = userTypeStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<UserNotiTypeStatsRes>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}