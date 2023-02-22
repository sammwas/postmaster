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
    public class CustomersController : Controller
    {
        private readonly ICustomerInterface _customerInterface;
        private readonly ISupplierInterface _supplierInterface;
        private readonly UserCookieData _userData;

        public CustomersController(ICookiesService cookiesService, ICustomerInterface customerInterface,
        ISupplierInterface supplierInterface)
        {
            _supplierInterface = supplierInterface;
            _userData = cookiesService.Read();
            _customerInterface = customerInterface;
        }
        public async Task<IActionResult> ByClientId()
        {
            var result = await _customerInterface.ByClientIdAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Customers", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(new CustomerViewModel { Status = EntityStatus.Active });

            var result = await _customerInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Customers", result.Message);
                return RedirectToAction(nameof(ByClientId));
            }

            var model = new CustomerViewModel(result.Data);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
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
            return RedirectToAction(nameof(ByClientId));
        }

        public async Task<JsonResult> Search(string term, bool isSupplier = false)
        {
            var data = isSupplier ? await _supplierInterface.SearchClientSupplierAsync(_userData.ClientId, term, 10) : await _customerInterface.SearchClientCustomerAsync(_userData.ClientId, term, 10);
            return Json(data);
        }

        public async Task<JsonResult> Balance(Guid id, GlUserType type)
        {
            var data = await _customerInterface.GlUserBalanceAsync(type, id);
            return Json(data);
        }
    }
}
