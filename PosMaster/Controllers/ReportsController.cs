using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportingInterface _reportingInterface;
        private readonly ICookiesService _cookiesService;
        public ReportsController(IReportingInterface reportingInterface, ICookiesService cookiesService)
        {
            _reportingInterface = reportingInterface;
            _cookiesService = cookiesService;
        }
        public async Task<IActionResult> SalesReport(string instanceId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            ViewData["instanceId"] = instanceId;
            var userData = _cookiesService.Read();
            Guid? clientId;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = userData.ClientId;
            else clientId = null;
            var result = await _reportingInterface.DailySalesReportAsync(clientId, instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Product Categories", result.Message);
            return View(result.Data);
        }
    }
}
