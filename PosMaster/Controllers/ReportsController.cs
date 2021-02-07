using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using System;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	[Authorize]
	public class ReportsController : Controller
	{
		private readonly IReportingInterface _reportingInterface;
		private readonly ICookiesService _cookiesService;
		public ReportsController(IReportingInterface reportingInterface, ICookiesService cookiesService)
		{
			_reportingInterface = reportingInterface;
			_cookiesService = cookiesService;
		}
		public async Task<IActionResult> SalesReport(string instanceId = "", string dtFrom = "", string dtTo = "", string search = "", string option = "")
		{
			ViewData["dtTo"] = dtTo;
			ViewData["dtFrom"] = dtFrom;
			ViewData["instanceId"] = instanceId;
			ViewData["option"] = option;
			var userData = _cookiesService.Read();
			Guid? clientId;
			if (!User.IsInRole(Role.SuperAdmin))
				clientId = userData.ClientId;
			else clientId = null;
			var result = await _reportingInterface.DailySalesReportAsync(clientId, instanceId, dtFrom, dtTo, search);
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, $"{option} Report", result.Message);
			return View(result.Data);
		}
	}
}
