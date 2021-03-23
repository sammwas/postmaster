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

        public OrdersController(ICookiesService cookiesService, IOrderInterface orderInterface)
        {
            _cookieService = cookiesService;
            _orderInterface = orderInterface;
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
				TempData.SetData(AlertLevel.Warning, "Receipts", result.Message);
			return View(result.Data);
		}
		public async Task<IActionResult> Edit(Guid id) 
		{
			var result = await _orderInterface.OrderByIdAsync(id);
			if (!result.Success)
				TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Customer Order", result.Message);
			return View(result.Data);
		}
		public IActionResult PlaceOrder() 
		{
			return View();
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
			var result = await _orderInterface.PlaceOrderAsync(model);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
			if (!result.Success)
				return View(model);
			return RedirectToAction(nameof(PlaceOrder));
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(OrderViewModel model)
		{
			var userData = _cookieService.Read();
			model.ClientId = userData.ClientId;
			model.InstanceId = userData.InstanceId;
			model.Personnel = User.Identity.Name;
			var option = model.IsEditMode ? "Update" : "Add";
			var title = $"{option} Expense";
			if (!ModelState.IsValid)
			{
				var message = "Missing fields";
				TempData.SetData(AlertLevel.Warning, title, message);
				return View(model);
			}

			var result = await _orderInterface.EditAsync(model);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
			if (!result.Success)
				return View(model);
			return RedirectToAction(nameof(Edit), new { result.Data.Id });
		}
	} 
}
