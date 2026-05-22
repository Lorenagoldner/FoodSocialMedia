using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAppFood.Models;

namespace BlazorAppFood.Data
{
    public interface INotificationService
    {
        public interface INotificationService
        {
            Task CreateNotification(Notification notification);

            Task<List<Notification>> GetUserNotifications(int userId);

            Task MarkAsRead(int notificationId);
        }
    }
}
