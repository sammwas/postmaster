using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PosMaster.Services;
using PosMaster.Services.Messaging;
using PosMaster.ViewModels;
using System;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IHubContext<InAppChatHub> _hub;
        private UserCookieData _userCookieData;
        public MessagesController(IHubContext<InAppChatHub> hub, ICookiesService cookiesService)
        {
            _hub = hub;
            _userCookieData = cookiesService.Read();
        }

        [HttpPost]
        public async Task InAppPost(InAppChatViewModel model)
        {
            try
            {
                var senderName = _userCookieData.FirstName;
                var time = $"{string.Format("{0:hh:mm tt}", DateTime.Now)}";
                await _hub.Clients.All.SendAsync($"{SignalRMethods.SendChatMessage}_{_userCookieData.InstanceId}",
                  _userCookieData.InstanceId, User.Identity.Name, senderName, model.Receiver, model.Message, time);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
