using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAppFood.Models;

namespace BlazorAppFood.Data
{
    public interface INotificationRepository
    {
            Task CreateNotification(Notification notification);

            Task<List<Notification>> GetUserNotifications(int userId);

            Task MarkAsRead(int notificationId);
            Task MarkAllAsRead(int userId);
    }
}
