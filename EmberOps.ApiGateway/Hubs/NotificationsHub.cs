using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmberOps.ApiGateway.Hubs
{
    public class NotificationsHub : Hub
    {

        // Method Executed when user is conected the notification hub
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine($"user connected: {userId}");

            await base.OnConnectedAsync();
        }

        // send of messages
        public async Task SendMessage(string message)
        {
            var email = Context.User?.FindFirstValue(ClaimTypes.Email);

            await Clients.All.SendAsync("ReceiveMessage", email, message);
        }
    }
}
