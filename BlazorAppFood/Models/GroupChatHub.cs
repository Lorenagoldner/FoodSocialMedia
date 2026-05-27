namespace BlazorAppFood.Models
{
    using BlazorAppFood.Configuration;
    using Dapper;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Threading.Tasks;

    public class GroupChatHub : Hub
    {
        private readonly SqlConnectionConfiguration _configuration;

        public GroupChatHub(SqlConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessage(int groupId, string username, string message)
        {
            var timestamp = DateTime.Now;

            using var conn = new SqlConnection(_configuration._value);
            await conn.ExecuteAsync(
                "INSERT INTO GroupChatMessages (GroupId, Username, Message, Timestamp) VALUES (@GroupId, @Username, @Message, @Timestamp)",
                new { GroupId = groupId, Username = username, Message = message, Timestamp = timestamp });

            await Clients.Group(groupId.ToString())
                         .SendAsync("ReceiveMessage", groupId, username, message, timestamp);
        }

        public override async Task OnConnectedAsync()
        {
            var groupId = Context.GetHttpContext()?.Request.Query["groupId"].ToString();
            if (!string.IsNullOrEmpty(groupId))
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await base.OnConnectedAsync();
        }
    }
}