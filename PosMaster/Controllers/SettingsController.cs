using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	[Authorize]
	public class SettingsController : Controller
	{
		private readonly IClientInterface _clientInterface;
		private readonly ICookiesService _cookieService;
		private readonly ISystemSettingInterface _settingInterface;
		private readonly IEmailService _emailService;
		public SettingsController(IClientInterface clientInterface, ISystemSettingInterface settingInterface,
			ICookiesService cookiesService, IEmailService emailService)
		{
			_clientInterface = clientInterface;
			_cookieService = cookiesService;
			_settingInterface = settingInterface;
			_emailService = emailService;
		}

		[HttpGet]
		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> System()
		{
			var result = await _settingInterface.ReadAsync();
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, "Settings", result.Message);
			return View(result.Data);
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> System(SystemSettingMiniViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await _settingInterface.UpdateAsync(model, User.Identity.Name);
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, "Settings", result.Message);
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> EmailSettings(Guid clientId)
		{
			var result = await _clientInterface.ClientEmailSettingAsync(clientId);
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, "Email Settings", result.Message);
			return View(new EmailSettingViewModel(result.Data));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EmailSettings(EmailSettingViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var userData = _cookieService.Read();
			model.ClientId = userData.ClientId;
			model.InstanceId = userData.InstanceId;
			model.Personnel = User.Identity.Name;
			var result = await _clientInterface.UpdateEmailSettingAsync(model);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Email Settings", result.Message);
			return View(new EmailSettingViewModel(result.Data));
		}

		public IActionResult SendTestEmail()
		{
			var userData = _cookieService.Read();
			return View(new TestEmailViewModel
			{
				Recipient = userData.EmailAddress,
				Subject = $"{userData.ClientName} Test Email - {DateTime.Now}"
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendTestEmail(TestEmailViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var userData = _cookieService.Read();
			var address = model.Recipient.Equals(userData.EmailAddress) ?
				 new EmailAddress(userData) :
				new EmailAddress(userData)
				{
					Address = model.Recipient,
					Name = model.Recipient.Split('@')[0]
				};
			var result = await _emailService.SendAsync(address, model.Subject, model.Content);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Test Email", result.Message);
			return View(model);
		}
	}
}
