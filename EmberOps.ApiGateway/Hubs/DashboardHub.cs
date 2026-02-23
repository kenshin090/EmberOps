using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmberOps.ApiGateway.Hubs
{
    public class DashboardHub : Hub
    {

        // Method Executed when user is conected the darshboardHub
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine($"user connected: {userId}");

            await base.OnConnectedAsync();
        }

        // send messages form the hub
        public async Task SendMessage(string message)
        {
           await Clients.All.SendAsync("ReceiveMessage");
        }
    }
}
