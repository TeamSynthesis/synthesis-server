using Microsoft.AspNetCore.SignalR;

namespace synthesis.api.Services.Notifications;


public interface INotificationHub
{
    Task SendUserNotification( string userId, string message);

    Task SendBroadcastNotification(string message);

}


public class NotificationHub:Hub, INotificationHub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage",$"{Context.ConnectionId}: has connected");
    }

    public async Task SendUserNotification(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task SendBroadcastNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveBroadcast", message);
    }
}