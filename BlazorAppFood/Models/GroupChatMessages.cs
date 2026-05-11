using System;

namespace BlazorAppFood.Models
{
    public class GroupChatMessages
    {
        public int Id_Group { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
