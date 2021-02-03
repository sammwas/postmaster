using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using PosMaster.ViewModels;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		private readonly ICookiesService _cookiesService;
		private readonly IDashboardInterface _dashboardInterface;
		private readonly string _tag = "Dashboard";
		public DashboardController(ICookiesService cookiesService, IDashboardInterface dashboardInterface)
		{
			_cookiesService = cookiesService;
			_dashboardInterface = dashboardInterface;
		}
		public IActionResult Index()
		{
			if (User.IsInRole(Role.SuperAdmin))
				return RedirectToAction(nameof(SuperAdmin));
			if (User.IsInRole(Role.Manager) || User.IsInRole(Role.Admin))
				return RedirectToAction(nameof(Manager));
			if (User.IsInRole(Role.Clerk))
				return RedirectToAction(nameof(Clerk));
			return RedirectToAction(nameof(Clerk));
		}
		public async Task<IActionResult> SuperAdmin()
		{
			var result = await _dashboardInterface.SuperAdminDashboardAsync();
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, _tag, result.Message);
			var model = result.Success ? result.Data : new SuperAdminDashboardViewModel();
			return View(model);
		}

		public async Task<IActionResult> Manager()
		{
			var userData = _cookiesService.Read();
			var result = await _dashboardInterface.ManagerDashboardAsync(userData.ClientId);
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, _tag, result.Message);
			var model = result.Success ? result.Data : new ManagerDashboardViewModel();
			return View(model);
		}
		public async Task<IActionResult> Clerk()
		{
			var userData = _cookiesService.Read();
			var result = await _dashboardInterface.ClerkDashboardAsync(userData.ClientId, User.Identity.Name);
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, _tag, result.Message);
			var model = result.Success ? result.Data : new ClerkDashboardViewModel();
			return View(model);
		}
	}
}
