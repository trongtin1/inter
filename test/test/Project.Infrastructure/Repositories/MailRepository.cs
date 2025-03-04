using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using test.Project.Domain.Entity;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.Mails;
using test.Project.Infrastructure.Extensions;
using test.Project.Infrastructure.Common;

namespace test.Project.Infrastructure.Repositories
{
    public class MailRepository : GenericRepository<Mail>, IMailRepository
    {
        private const int DefaultPageSize = 6;

        public MailRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Mail?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<MailRes> GetPage(
            int pageIndex,
            string? id,
            string? email,
            bool? isSend,
            string? timeType,
            string? sendStatus,
            string? emailCc,
            string? emailBcc,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var result = new MailRes();
  
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetMails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Page", pageIndex));
            command.Parameters.Add(new SqlParameter("@PageSize", DefaultPageSize));
            command.Parameters.Add(new SqlParameter("@Id", id ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Email", email ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@IsSend", isSend ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@TimeType", timeType ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@SendStatus", sendStatus ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@EmailCc", emailCc ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@EmailBcc", emailBcc ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));

            var totalItemsParam = new SqlParameter("@TotalItems", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(totalItemsParam);

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);

            result.Items = dt.ConvertDataTable<Mail>();
            result.TotalItems = (int)(totalItemsParam.Value ?? 0);
            result.PageIndex = pageIndex;
            result.PageSize = DefaultPageSize;
            return result;
        }
        
        public async Task<List<Mail>> GetByIdsAsync(List<long> ids)
        {
            return await _dbSet
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();
        }
        public async Task<IEnumerable<Mail>> GetFilterDataAsync(string userEmail, List<string> rolesList)
        {
            var query = _dbSet.AsQueryable();

            if (!rolesList.Contains("admin"))
            {
                query = query.Where(m => m.Email == userEmail);
            }
            return await query.ToListAsync();
        }

        public async Task<Mail?> GetOne(
            string? id,
            string? email,
            bool? isSend,
            string? timeType,
            string? sendStatus,
            string? emailCc,
            string? emailBcc,
            DateTime? fromDate,
            DateTime? toDate)
        {
            return await GetOneAsync(x =>
                (string.IsNullOrEmpty(id) || x.Id.ToString().Equals(id)) &&
                (string.IsNullOrEmpty(email) || x.Email == email) &&
                (!isSend.HasValue || x.IsSend == isSend.Value) &&
                (string.IsNullOrEmpty(sendStatus) || x.SentStatus == sendStatus) &&
                (string.IsNullOrEmpty(emailCc) || x.EmailCc.Contains(emailCc)) &&
                (string.IsNullOrEmpty(emailBcc) || x.EmailBcc.Contains(emailBcc)) &&
                (string.IsNullOrEmpty(timeType) ||
                    (fromDate.HasValue && toDate.HasValue && fromDate.Value <= toDate.Value &&
                        ((timeType == "createdTime" && x.CreateTime >= fromDate.Value && x.CreateTime <= toDate.Value)))));
        }

        public async Task<List<Mail>> GetList(
            string? id,
            string? email,
            bool? isSend,
            string? timeType,
            string? sendStatus,
            string? emailCc,
            string? emailBcc,
            DateTime? fromDate,
            DateTime? toDate)
        {
            return await GetListAsync(x =>
                (string.IsNullOrEmpty(id) || x.Id.ToString().Equals(id)) &&
                (string.IsNullOrEmpty(email) || x.Email == email) &&
                (!isSend.HasValue || x.IsSend == isSend.Value) &&
                (string.IsNullOrEmpty(sendStatus) || x.SentStatus == sendStatus) &&
                (string.IsNullOrEmpty(emailCc) || x.EmailCc.Contains(emailCc)) &&
                (string.IsNullOrEmpty(emailBcc) || x.EmailBcc.Contains(emailBcc)) &&
                (string.IsNullOrEmpty(timeType) ||
                    (fromDate.HasValue && toDate.HasValue && fromDate.Value <= toDate.Value &&
                        ((timeType == "createdTime" && x.CreateTime >= fromDate.Value && x.CreateTime <= toDate.Value)))));
        }
    }
}