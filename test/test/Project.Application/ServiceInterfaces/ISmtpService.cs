namespace test.Project.Application.ServiceInterfaces
{
    public interface ISmtpService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
} 