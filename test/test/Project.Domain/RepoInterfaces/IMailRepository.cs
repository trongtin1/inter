using test.Project.Application.DTOs.Response.Mails;
using test.Project.Domain.Entity;
using test.Project.Domain.Common;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IMailRepository : IGenericRepository<Mail>
    {
        Task<MailRes> GetPage(
            int pageIndex,
            string? id,
            string? email,
            bool? isSend,
            string? timeType,
            string? sendStatus,
            string? emailCc,
            string? emailBcc,
            DateTime? fromDate,
            DateTime? toDate);
            
        Task<Mail?> GetOne(
            string? id,
            string? email,
            bool? isSend,
            string? timeType,
            string? sendStatus,
            string? emailCc,
            string? emailBcc,
            DateTime? fromDate,
            DateTime? toDate);

        Task<List<Mail>> GetList(
            string? id,
            string? email,
            bool? isSend,
            string? timeType,
            string? sendStatus,
            string? emailCc,
            string? emailBcc,
            DateTime? fromDate,
            DateTime? toDate);

        Task<List<Mail>> GetByIdsAsync(List<long> ids);
        Task<IEnumerable<Mail>> GetFilterDataAsync(string userEmail, List<string> rolesList);
        Task<Mail?> GetByIdAsync(long id);
    }
}