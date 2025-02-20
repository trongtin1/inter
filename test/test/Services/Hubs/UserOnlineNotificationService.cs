using Microsoft.AspNetCore.SignalR;
using test.Hubs;

namespace test.Services.Hubs
{
    public interface IUserOnlineNotificationService
    {
        Task NotifyUserOnlineCreated(string userId);
        Task NotifyUserOnlineDeleted(string userId);
    }

    public class UserOnlineNotificationService : IUserOnlineNotificationService
    {
        private readonly IHubContext<UserOnlineHub> _hubContext;

        public UserOnlineNotificationService(IHubContext<UserOnlineHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUserOnlineCreated(string userId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveUserOnlineNotification", 
                $"New user online: {userId}", "create");
            // Gửi trạng thái online cho tất cả clients
            await _hubContext.Clients.All.SendAsync("ReceiveOnlineStatus", userId, true);
        }

        public async Task NotifyUserOnlineDeleted(string userId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveUserOnlineNotification", 
                $"User online deleted: {userId}", "delete");
            // Gửi trạng thái offline cho tất cả clients
            await _hubContext.Clients.All.SendAsync("ReceiveOnlineStatus", userId, false);
        }

    }
} 