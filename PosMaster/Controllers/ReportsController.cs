﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IProductInterface _productInterface;
        public ReportsController(IReportingInterface reportingInterface, ICookiesService cookiesService,
            IProductInterface productInterface)
        {
            _reportingInterface = reportingInterface;
            _userData = cookiesService.Read();
            _productInterface = productInterface;
        }
        public async Task<IActionResult> SalesReport(Guid instanceId, string dtFrom = "", string dtTo = "", string search = "", string option = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            if (instanceId.Equals(Guid.Empty))
                instanceId = _userData.InstanceId;
            ViewData["instanceId"] = instanceId;
            ViewData["option"] = option;
            var result = await _reportingInterface.DailySalesReportAsync(instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"{option} Report", result.Message);
            return View(result.Data);
        }

        public async Task<JsonResult> MonthlySalesReport(Guid instanceId, string dtFrom = "", string dtTo = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            if (instanceId.Equals(Guid.Empty))
                instanceId = _userData.InstanceId;
            ViewData["instanceId"] = instanceId;
            var result = await _reportingInterface.MonthlySalesReportAsync(instanceId, dtFrom, dtTo);
            return Json(result);
        }

        public async Task<IActionResult> SalesByClerk(Guid instanceId, string dtFrom = "", string dtTo = "")
        {
            ViewData["dtTo"] = dtTo;
            if (string.IsNullOrEmpty(dtFrom))
                dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
            ViewData["dtFrom"] = dtFrom;
            if (instanceId.Equals(Guid.Empty))
                instanceId = _userData.InstanceId;
            ViewData["instanceId"] = instanceId;
            var result = await _reportingInterface.SalesByClerkAsync(instanceId, dtFrom, dtTo);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Clerk Sales", result.Message);
            return View(result.Data);
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

        public async Task<IActionResult> CloseOfDay(Guid? inId, string dt = "", string dtTo = "")
        {
            var hasDate = DateTime.TryParse(dt, out var date);
            var hasDateTo = DateTime.TryParse(dtTo, out var dateTo);
            if (!hasDate)
                date = DateTime.Now;
            if (!hasDateTo)
                dateTo = DateTime.Now;
            ViewData["dtDay"] = date.ToString("dd-MMM-yyyy");
            ViewData["dtDayTo"] = dateTo.ToString("dd-MMM-yyyy");
            inId = User.IsInRole(Role.Clerk) ? _userData.InstanceId : inId;
            var personnel = User.IsInRole(Role.Clerk) ? User.Identity.Name : "";
            ViewData["instanceId"] = inId;
            var result = await _reportingInterface.CloseOfDayAsync(_userData.ClientId, inId, date, dateTo, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Day Report", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> CustomerStatement(Guid id)
        {
            var result = await _reportingInterface.CustomerStatementAsync(id);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Customer Statement", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> CustomerSalesReport(Guid? insId, Guid? cId, string type = "",
            string dtFrom = "", string dtTo = "", string search = "", bool summarized = false)
        {
            var hasDateFrom = DateTime.TryParse(dtFrom, out var dateFrom);
            var hasDateTo = DateTime.TryParse(dtTo, out var dateTo);
            if (!hasDateFrom)
                dateFrom = DateTime.Now;
            if (!hasDateTo)
                dateTo = DateTime.Now;
            ViewData["Search"] = search;
            ViewData["DateFrom"] = dtFrom = dateFrom.ToString("dd-MMM-yyyy");
            ViewData["DateTo"] = dtTo = dateTo.ToString("dd-MMM-yyyy");
            insId = User.IsInRole(Role.Clerk) ? _userData.InstanceId : insId;
            var personnel = User.IsInRole(Role.Clerk) ? User.Identity.Name : "";
            bool? isCredit = null;
            if (!string.IsNullOrEmpty(type))
                isCredit = type.Equals("CREDIT");

            ViewData["InstanceId"] = insId;
            ViewData["CustomerId"] = cId;
            ViewData["SaleType"] = type;
            ViewData["Summarized"] = summarized;
            var result = await _reportingInterface
                .CustomerProductSaleReportAsync(_userData.ClientId, insId, cId, isCredit, null, dtFrom, dtTo, personnel, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Customer Sales Report", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> LowStockProducts(Guid? inId)
        {
            inId = User.IsInRole(Role.Clerk) ? _userData.InstanceId : inId;
            ViewData["instanceId"] = inId;
            var result = await _productInterface.LowStockProductsAsync(_userData.ClientId, inId, 0);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Low stock products", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> TopSellingVolume(Guid? inId, string dt = "", string dtTo = "")
        {
            var hasDate = DateTime.TryParse(dt, out var date);
            var hasDateTo = DateTime.TryParse(dtTo, out var dateTo);
            if (!hasDate)
            {
                date = DateTime.Now;
                dt = date.ToString();
            }
            if (!hasDateTo)
            {
                dateTo = DateTime.Now;
                dtTo = dateTo.ToString();
            }
            ViewData["dtDay"] = date.ToString("dd-MMM-yyyy");
            ViewData["dtDayTo"] = dateTo.ToString("dd-MMM-yyyy");
            inId = User.IsInRole(Role.Clerk) ? _userData.InstanceId : inId;
            var personnel = User.IsInRole(Role.Clerk) ? User.Identity.Name : "";
            ViewData["instanceId"] = inId;
            var result = await _productInterface.TopSellingProductsByVolumeAsync(_userData.ClientId, inId, dt, dtTo, personnel, 0);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, $"Top selling by volume", result.Message);
            return View(result.Data);
        }
    }
}
