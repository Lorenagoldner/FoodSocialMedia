using System;

namespace BlazorAppFood.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int RecipientUserId { get; set; }

        public int ActorUserId { get; set; }

        public NotificationType Type { get; set; }

        public string Message { get; set; }

        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum NotificationType //utilize this enum to differentiate between types of notifications, such as comments, likes, follows
    {
        Comment,
        Favorite,
        Follow,
        Rating,
        GroupInvite,
        GroupAdmin
    }
}