using Microsoft.AspNetCore.SignalR;

namespace test.Hubs
{
    public class MailHub : Hub
    {
        public async Task SendMailNotification(string message, string type)
        {
            await Clients.All.SendAsync("ReceiveMailNotification", message, type);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
} 