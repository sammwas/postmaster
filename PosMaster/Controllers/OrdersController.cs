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
	public class OrdersController : Controller
    {
        private readonly ICookiesService _cookieService;
        private readonly IOrderInterface _orderInterface;
		private readonly string firstDayOfWeek;
		public OrdersController(ICookiesService cookiesService, IOrderInterface orderInterface)
        {
            _cookieService = cookiesService;
            _orderInterface = orderInterface;
			firstDayOfWeek = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy");
		}
		public async Task<IActionResult> Index(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
		{
			ViewData["DtFrom"] = dtFrom;
			ViewData["DtTo"] = dtTo;
			ViewData["Search"] = search;
			var userData = _cookieService.Read();
			Guid? clientId = null;
			Guid? instanceId = null;
			if (!User.IsInRole(Role.SuperAdmin))
				clientId = userData.ClientId;
			if (Guid.TryParse(insId, out var iId))
				instanceId = iId;
			if (User.IsInRole(Role.Clerk))
				instanceId = userData.InstanceId;
			ViewData["InstanceId"] = instanceId;
			var result = await _orderInterface.OrdersAsync(clientId, instanceId, dtFrom, dtTo, search);
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
			return RedirectToAction(nameof(Index), new {dtFrom = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy"), dtTo = DateTime.Now.ToString("dd-MMM-yyyy") });
		}

		[HttpPost]
		public async Task<JsonResult> FulFilOrder(Guid id)
		{
			var title = "Fulfil order";
			var orderExists = Guid.TryParse(id.ToString(), out var orderId);
			if (!orderExists)
			{
				var message = "Order does not exist";
				TempData.SetData(AlertLevel.Warning, title, message);
				return Json(new Receipt());
			}
			if (!ModelState.IsValid)
			{
				var message = "Missing fields";
				TempData.SetData(AlertLevel.Warning, title, message);
				return Json(new Receipt());
			}

			var result = await _orderInterface.FulfillOrder(orderId);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Customer Order", result.Message);
			return Json(result);
		}
		public async Task<IActionResult> FulfilOrder(Guid? id) 
		{
			if (id == null)
				return View(new OrderViewModel { Status = EntityStatus.Active });
			var result = await _orderInterface.OrderByIdAsync(id.Value);
			if (!result.Success)
				TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Customer Order", result.Message);
			var model = new OrderViewModel(result.Data);
			return View(model);
		}
	} 
}
