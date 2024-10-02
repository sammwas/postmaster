using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        private readonly UserCookieData _userCookieData;
        private readonly ILogger<MessagesController> _logger;
        public MessagesController(IHubContext<InAppChatHub> hub, ICookiesService cookiesService,
            ILogger<MessagesController> logger)
        {
            _hub = hub;
            _userCookieData = cookiesService.Read();
            _logger = logger;
        }

        [HttpPost]
        public async Task InAppPost(InAppChatViewModel model)
        {
            var tag = nameof(InAppPost);
            try
            {
                var senderName = _userCookieData.FirstName;
                var time = $"{string.Format("{0:hh:mm tt}", DateTime.Now)}";
                _logger.LogInformation($"{tag} sending in-app message from {senderName} to {model.Receiver} at {time}");

                await _hub.Clients.All.SendAsync($"{SignalRMethods.SendChatMessage}_{_userCookieData.InstanceId}",
                  _userCookieData.InstanceId, User.Identity.Name, senderName, model.Receiver, model.Message, time);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogInformation($"{tag} error occurred while sending in-app message. {ex.Message}");
            }
        }
    }
}
