namespace BlazorAppFood.Models
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using System;

    public class GroupChatHub : Hub
    {
        public async Task SendMessage(int groupId, string username, string message)
        {
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", groupId, username, message, DateTime.Now);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var groupId = httpContext?.Request.Query["groupId"];

            if (!string.IsNullOrEmpty(groupId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            }

            await base.OnConnectedAsync();
        }
    }
}
