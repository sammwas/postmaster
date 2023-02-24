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
    public class OrdersController : Controller
    {
        private readonly UserCookieData _userData;
        private readonly IOrderInterface _orderInterface;
        private readonly string firstDayOfWeek;
        public OrdersController(ICookiesService cookiesService, IOrderInterface orderInterface)
        {
            _userData = cookiesService.Read();
            _orderInterface = orderInterface;
            firstDayOfWeek = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy");
        }
        public async Task<IActionResult> Index(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? instanceId = null;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
                instanceId = _userData.InstanceId;
            ViewData["InstanceId"] = instanceId;
            var result = await _orderInterface.OrdersAsync(_userData.ClientId, instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Customer Orders", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> PlaceOrder(Guid? id)
        {
            if (id == null)
                return View(new OrderViewModel { Status = EntityStatus.Active });
            var result = await _orderInterface.OrderByIdAsync(id.Value);
            if (!result.Success)
                TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Customer Order", result.Message);
            var model = new OrderViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(OrderViewModel model)
        {
            var title = "Customer Order";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            model.Personnel = User.Identity.Name;
            var result = model.IsEditMode ? await _orderInterface.EditAsync(model) : await _orderInterface.PlaceOrderAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index), new { dtFrom = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy"), dtTo = DateTime.Now.ToString("dd-MMM-yyyy") });
        }

        [HttpPost]
        public async Task<IActionResult> FulFillOrder(FulfillOrderViewModel model)
        {
            var title = "Fulfil order";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            model.PersonnelName = _userData.FullName;
            var result = await _orderInterface.FulfillOrder(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Customer Order", result.Message);
            return RedirectToAction(nameof(Index), new { dtFrom = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy"), dtTo = DateTime.Now.ToString("dd-MMM-yyyy") });
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _orderInterface.OrderByIdAsync(id);
            if (!result.Success)
                TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Order Details", result.Message);
            return View(result.Data);
        }
    }
}
