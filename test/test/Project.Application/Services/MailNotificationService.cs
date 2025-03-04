using Microsoft.AspNetCore.SignalR;
using test.Project.API.Hubs;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Application.Services
{
    public class MailNotificationService : IMailNotificationService
    {
        private readonly IHubContext<MailHub> _hubContext;

        public MailNotificationService(IHubContext<MailHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyMailCreated(string mailId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMailNotification", 
                $"New mail: {mailId}", "create");
        }

        public async Task NotifyMailUpdated(string mailId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMailNotification", 
                $"Mail updated: {mailId}", "update");
        }

        public async Task NotifyMailDeleted(string mailId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMailNotification", 
                $"Mail deleted: {mailId}", "delete");
        }
    }
} 