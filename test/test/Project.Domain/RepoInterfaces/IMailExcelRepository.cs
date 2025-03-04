using test.Project.Domain.Entity;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IMailExcelRepository
    {
        Task<List<Mail>> GetMailsForExportAsync(
            string userEmail,
            List<string> rolesList,
            string? id = null,
            string? email = null,
            bool? isSend = null,
            string? timeType = null,
            string? sendStatus = null,
            string? emailCc = null,
            string? emailBcc = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<Mail> CreateMailAsync(Mail mail);
        Task<List<Mail>> CreateMailsAsync(List<Mail> mails);
       
    }
} 