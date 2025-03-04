
namespace test.Project.Application.ServiceInterfaces
{
    public interface IMailNotificationService
    {
        Task NotifyMailCreated(string mailId);
        Task NotifyMailUpdated(string mailId);
        Task NotifyMailDeleted(string mailId);
    }
}