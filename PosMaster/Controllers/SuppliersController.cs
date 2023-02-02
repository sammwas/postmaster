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
    public class SuppliersController : Controller
    {
        private readonly UserCookieData _userData;
        private readonly ISupplierInterface _supplierInterface;
        public SuppliersController(ICookiesService cookieService, ISupplierInterface supplierInterface)
        {
            _userData = cookieService.Read();
            _supplierInterface = supplierInterface;
        }
        public async Task<IActionResult> ByClientId()
        {
            var result = await _supplierInterface.ByClientIdAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Suppliers", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(new SupplierViewModel { Status = EntityStatus.Active });

            var result = await _supplierInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Suppliers", result.Message);
                return RedirectToAction(nameof(ByClientId));
            }

            var model = new SupplierViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Supplier";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _supplierInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(ByClientId));
        }
    }
}
