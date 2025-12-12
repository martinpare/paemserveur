using Microsoft.AspNetCore.SignalR;
using serveur.Hubs;
using System;
using System.Threading.Tasks;

namespace serveur.Services
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, string message, string type = "info");
        Task SendToGroupAsync(string groupName, string message, string niveau = "info");
        Task SendToAllAsync(string message, string niveau = "info");
        Task NotifyStatusChangeAsync(string userId, string status);
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, string message, string type = "info")
        {
            var notification = new
            {
                Id = Guid.NewGuid(),
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        public async Task SendToGroupAsync(string groupName, string message, string niveau = "info")
        {
            var alerte = new
            {
                Id = Guid.NewGuid(),
                Message = message,
                Niveau = niveau,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveAlert", alerte);
        }

        public async Task SendToAllAsync(string message, string niveau = "info")
        {
            var alerte = new
            {
                Id = Guid.NewGuid(),
                Message = message,
                Niveau = niveau,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.All.SendAsync("ReceiveAlert", alerte);
        }

        public async Task NotifyStatusChangeAsync(string userId, string status)
        {
            await _hubContext.Clients.All.SendAsync("StatusUpdate", userId, status);
        }
    }
}
