

namespace test.Project.Application.ServiceInterfaces
{
    public interface IUserModuleNotificationService
    {
        Task NotifyUserModuleCreated(string userId);
        Task NotifyUserModuleUpdated(string userId);
        Task NotifyUserModuleDeleted(string userId);
    }
} 