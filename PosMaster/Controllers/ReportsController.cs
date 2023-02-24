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
        private readonly UserCookieData _userData;
        public ReportsController(IReportingInterface reportingInterface, ICookiesService cookiesService)
        {
            _reportingInterface = reportingInterface;
            _userData = cookiesService.Read();
        }
        public async Task<IActionResult> SalesReport(string instanceId = "", string dtFrom = "", string dtTo = "", string search = "", string option = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            ViewData["instanceId"] = instanceId;
            ViewData["option"] = option;
            var result = await _reportingInterface.DailySalesReportAsync(instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"{option} Report", result.Message);
            return View(result.Data);
        }

        public async Task<JsonResult> MonthlySalesReport(string instanceId = "", string dtFrom = "", string dtTo = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            ViewData["instanceId"] = instanceId;
            var result = await _reportingInterface.MonthlySalesReportAsync(instanceId, dtFrom, dtTo);
            return Json(result);
        }

        public async Task<IActionResult> CustomerBalances(Guid instanceId, string dtFrom = "", string dtTo = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            if (instanceId.Equals(Guid.Empty))
                instanceId = _userData.InstanceId;
            ViewData["instanceId"] = instanceId;

            var result = await _reportingInterface.CustomerBalancesAsync(instanceId, dtFrom, dtTo);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Customer Balances", result.Message);
            return View(result.Data);
        }


        public async Task<IActionResult> CustomerStatement(Guid id)
        {
            return View();
        }
    }
}
