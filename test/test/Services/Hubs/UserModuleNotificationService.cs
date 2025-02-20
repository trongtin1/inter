using Microsoft.AspNetCore.SignalR;
using test.Hubs;

namespace test.Services.Hubs
{
    public interface IUserModuleNotificationService
    {
        Task NotifyUserModuleCreated(string userId);
        Task NotifyUserModuleUpdated(string userId);
        Task NotifyUserModuleDeleted(string userId);
    }


    public class UserModuleNotificationService : IUserModuleNotificationService
    {
        private readonly IHubContext<UserModuleHub> _hubContext;

        public UserModuleNotificationService(IHubContext<UserModuleHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUserModuleCreated(string userId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveUserModuleNotification", 
                $"New user module: {userId}", "create");
        }

        public async Task NotifyUserModuleUpdated(string userId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveUserModuleNotification", 
                $"User module updated: {userId}", "update");
        }

        public async Task NotifyUserModuleDeleted(string userId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveUserModuleNotification", 
                $"User module deleted: {userId}", "delete");
        }
    }
} 