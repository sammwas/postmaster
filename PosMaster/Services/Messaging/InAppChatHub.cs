using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PosMaster.Services.Messaging
{
    public class InAppChatHub : Hub
    {

        public async Task SendSyncStatus(bool finished, bool success, string message, decimal progress = 0)
        {
            await Clients.All.SendAsync(SignalRMethods.ProgressStatus, finished, success, message, progress);
        }


        public async Task SendInAppChat(System.Guid instanceId, string sender, string senderName, string receiver, string message, string timeStamp)
        {
            await Clients.All.SendAsync($"{SignalRMethods.SendChatMessage}_{instanceId}", sender, senderName, receiver, message, timeStamp);
        }
    }
}

