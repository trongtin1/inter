using Microsoft.EntityFrameworkCore;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.Statistic.Email;
using System.Data;
using test.Project.Infrastructure.Extensions;

namespace test.Project.Infrastructure.Repositories
{
    public class MailStatisticsRepository : IMailStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public MailStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmailFrequencyRes>> GetEmailFrequenciesAsync()
        {
            var ls = new List<EmailFrequencyRes>();
            DataTable dt = new();
            
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetEmailFrequencies";
                command.CommandType = CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();
                using var result = await command.ExecuteReaderAsync();
                dt.Load(result);
                ls = dt.ConvertDataTable<EmailFrequencyRes>();
            }

            return ls;
        }

        public async Task<List<MonthlyEmailStatistic>> GetEmailFrequenciesByYearAsync(int year)
        {
            var ls = new List<MonthlyEmailStatistic>();
            DataTable dt = new();
            
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetEmailFrequenciesByYear";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Year", year));

                await _context.Database.OpenConnectionAsync();
                using var result = await command.ExecuteReaderAsync();
                dt.Load(result);
                ls = dt.ConvertDataTable<MonthlyEmailStatistic>();
            }

            return ls;
        }

        public async Task<List<MonthlyEmailStatistic>> GetEmailMonthlyStatsAsync(string email, int? year)
        {
            var ls = new List<MonthlyEmailStatistic>();
            DataTable dt = new();
            
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetEmailMonthlyStats";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Email", email));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Year", year));

                await _context.Database.OpenConnectionAsync();
                using var result = await command.ExecuteReaderAsync();
                dt.Load(result);
                ls = dt.ConvertDataTable<MonthlyEmailStatistic>();
            }

            return ls;
        }
    }
} 