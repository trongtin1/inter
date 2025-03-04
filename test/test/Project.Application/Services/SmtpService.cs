using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services;

public class SmtpService : ISmtpService
{
    private readonly IConfiguration _configuration;

    public SmtpService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpClient = new SmtpClient
            {
                Host = smtpSettings["Host"],
                Port = int.Parse(smtpSettings["Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
} 