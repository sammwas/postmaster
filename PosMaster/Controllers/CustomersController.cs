using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ICookiesService _cookieService;
        private readonly ICustomerInterface _customerInterface;
        public CustomersController(ICookiesService cookiesService, ICustomerInterface customerInterface)
        {
            _cookieService = cookiesService;
            _customerInterface = customerInterface;
        }
        public async Task<IActionResult> ByClientId(Guid clientId)
        {
            var result = await _customerInterface.ByClientIdAsync(clientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Customers", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            var userData = _cookieService.Read();
            if (id == null)
                return View(new CustomerViewModel { Status = EntityStatus.Active });

            var result = await _customerInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Customers", result.Message);
                return RedirectToAction(nameof(ByClientId), new { userData.ClientId });
            }

            var model = new CustomerViewModel(result.Data);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            var userData = _cookieService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Customer";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _customerInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(ByClientId), new { userData.ClientId });
        }

        public async Task<JsonResult> Search(Guid cId, string term)
        {
            var data = await _customerInterface.SearchClientCustomerAsync(cId, term);
            return Json(data);
        }
    }
}
