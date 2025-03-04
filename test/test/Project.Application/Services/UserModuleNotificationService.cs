using Microsoft.AspNetCore.SignalR;
using test.Project.API.Hubs;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
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