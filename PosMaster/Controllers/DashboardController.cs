﻿using Microsoft.AspNetCore.Authorization;
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
    public class DashboardController : Controller
    {
        private readonly ICookiesService _cookiesService;
        private readonly IClientInterface _clientInterface;
        private readonly IDashboardInterface _dashboardInterface;
        private readonly string _tag = "Dashboard";
        public DashboardController(ICookiesService cookiesService, IDashboardInterface dashboardInterface,
            IClientInterface clientInterface)
        {
            _cookiesService = cookiesService;
            _dashboardInterface = dashboardInterface;
            _clientInterface = clientInterface;
        }
        public IActionResult Index()
        {
            if (User.IsInRole(Role.SuperAdmin))
                return RedirectToAction(nameof(SuperAdmin));
            if (User.IsInRole(Role.Manager))
                return RedirectToAction(nameof(Manager));
            if (User.IsInRole(Role.Clerk))
            {
                var allowDash = _cookiesService.Read().ShowClerkDashboard;
                return allowDash ?
                 RedirectToAction(nameof(Clerk)) : RedirectToAction("Index", "PointOfSale");
            }
            return RedirectToAction(nameof(Clerk));
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SuperAdmin(string inId = "", string dtFrom = "", string dtTo = "")
        {
            var userData = _cookiesService.Read();
            if (string.IsNullOrEmpty(dtFrom))
                dtFrom = Helpers.firstDayOfMonth.ToString("dd-MMM-yyyy");
            if (string.IsNullOrEmpty(dtTo))
                dtTo = DateTime.Now.ToString("dd-MMM-yyyy");
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            var hasInsId = Guid.TryParse(inId, out var insId);
            var instanceId = hasInsId ? insId : userData.InstanceId;
            ViewData["InstanceId"] = instanceId;
            var result = await _dashboardInterface.SuperAdminDashboardAsync(instanceId, DateTime.Parse(dtFrom), DateTime.Parse(dtTo));
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, _tag, result.Message);
            var model = result.Success ? result.Data : new SuperAdminDashboardViewModel();
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Manager(string inId = "", string dtFrom = "", string dtTo = "")
        {
            var userData = _cookiesService.Read();
            if (string.IsNullOrEmpty(dtFrom))
                dtFrom = Helpers.firstDayOfMonth.ToString("dd-MMM-yyyy");
            if (string.IsNullOrEmpty(dtTo))
                dtTo = DateTime.Now.ToString("dd-MMM-yyyy");
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            var hasInsId = Guid.TryParse(inId, out var insId);
            var instanceId = hasInsId ? insId : userData.InstanceId;
            ViewData["InstanceId"] = instanceId;
            var result = await _dashboardInterface.ManagerDashboardAsync(userData.ClientId, instanceId, DateTime.Parse(dtFrom), DateTime.Parse(dtTo));
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, _tag, result.Message);
            var model = result.Success ? result.Data : new ManagerDashboardViewModel();
            return View(model);
        }
        public async Task<IActionResult> Clerk()
        {
            var userData = _cookiesService.Read();
            var result = await _dashboardInterface.ClerkDashboardAsync(userData.InstanceId, User.Identity.Name);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, _tag, result.Message);
            var model = result.Success ? result.Data : new ClerkDashboardViewModel();
            return View(model);
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
