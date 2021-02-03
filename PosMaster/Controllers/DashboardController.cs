using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IDashboardInterface _dashboardInterface;
		private readonly string _tag = "Dashboard";
		public DashboardController(UserManager<User> userManager, IDashboardInterface dashboardInterface)
		{
			_userManager = userManager;
			_dashboardInterface = dashboardInterface;
		}
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return RedirectToAction("Index", "Home");
			if (user.Role.Equals(Role.SuperAdmin))
				return RedirectToAction(nameof(SuperAdmin));

			if (user.Role.Equals(Role.Manager))
				return RedirectToAction(nameof(Manager), new { user.ClientId });

			if (user.Role.Equals(Role.Admin))
				return RedirectToAction(nameof(Admin), new { user.ClientId });

			if (user.Role.Equals(Role.Clerk))
				return RedirectToAction(nameof(Clerk), new { user.InstanceId });

			return RedirectToAction(nameof(Clerk), new { user.InstanceId });
		}
		public async Task<IActionResult> SuperAdmin()
		{
			var result = await _dashboardInterface.SuperAdminDashboardAsync();
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, _tag, result.Message);
			var model = result.Success ? result.Data : new SuperAdminDashboardViewModel();
			return View(model);
		}
		public IActionResult Admin(Guid clientId)
		{
			return View();
		}
		public async Task<IActionResult> Manager(Guid clientId)
		{
			var result = await _dashboardInterface.ManagerDashboardAsync(clientId);
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, _tag, result.Message);
			var model = result.Success ? result.Data : new ManagerDashboardViewModel();
			return View(model);
		}
		public IActionResult Clerk(Guid instanceId)
		{
			return View();
		}
	}
}
