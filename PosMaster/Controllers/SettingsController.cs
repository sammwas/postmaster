using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	[Authorize]
	public class SettingsController : Controller
	{
		private readonly IClientInterface _clientInterface;
		private readonly ICookiesService _cookieService;
		private readonly ISystemSettingInterface _settingInterface;

		public SettingsController(IClientInterface clientInterface, ISystemSettingInterface settingInterface, ICookiesService cookiesService)
		{
			_clientInterface = clientInterface;
			_cookieService = cookiesService;
			_settingInterface = settingInterface;
		}

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
	}
}
