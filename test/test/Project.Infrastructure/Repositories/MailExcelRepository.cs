using Microsoft.EntityFrameworkCore;
using test.Project.Domain.RepoInterfaces;
using test.Project.Domain.Entity;
using test.Project.Infrastructure.Data;

namespace test.Project.Infrastructure.Repositories
{
    public class MailExcelRepository : IMailExcelRepository
    {
        private readonly ApplicationDbContext _context;

        public MailExcelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mail>> GetMailsForExportAsync(
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
            DateTime? toDate = null)
        {
            var query = _context.Mail.AsQueryable();

            // Apply role-based filtering
            if (!rolesList.Contains("admin"))
            {
                query = query.Where(m => m.Email == userEmail);
            }

            // Apply other filters
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(m => m.Id.ToString().Equals(id));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(m => m.Email != null && m.Email.Equals(email));
            }

            if (isSend.HasValue)
            {
                query = query.Where(m => m.IsSend == isSend.Value);
            }

            if (!string.IsNullOrEmpty(sendStatus))
            {
                query = query.Where(m => m.SentStatus != null && m.SentStatus.Contains(sendStatus));
            }

            if (!string.IsNullOrEmpty(emailCc))
            {
                var emailCcList = emailCc.Split(';', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(m => m.EmailCc != null && emailCcList.Any(cc => m.EmailCc.Contains(";" + cc + ";")));
            }

            if (!string.IsNullOrEmpty(emailBcc))
            {
                var emailBccList = emailBcc.Split(';', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(m => m.EmailBcc != null && emailBccList.Any(bcc => m.EmailBcc.Contains(";" + bcc + ";")));
            }

            if (!string.IsNullOrEmpty(timeType) && fromDate.HasValue && toDate.HasValue)
            {
                if (fromDate.Value <= toDate.Value)
                {
                    if (timeType == "createTime")
                    {
                        query = query.Where(m => m.CreateTime >= fromDate.Value &&
                                               m.CreateTime <= toDate.Value);
                    }
                    else if (timeType == "sendTime")
                    {
                        query = query.Where(m => m.SendTime >= fromDate.Value &&
                                               m.SendTime <= toDate.Value);
                    }
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Mail> CreateMailAsync(Mail mail)
        {
            await _context.Mail.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        public async Task<List<Mail>> CreateMailsAsync(List<Mail> mails)
        {
            await _context.Mail.AddRangeAsync(mails);
            await _context.SaveChangesAsync();
            return mails;
        }
    }
} 