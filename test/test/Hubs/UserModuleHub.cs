using Microsoft.AspNetCore.SignalR;

namespace test.Hubs
{
    public class UserModuleHub : Hub
    {
        public async Task SendUserModuleNotification(string message, string type)
        {
            await Clients.All.SendAsync("ReceiveUserModuleNotification", message, type);
        }

        public async Task JoinGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task LeaveGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
    }
} 