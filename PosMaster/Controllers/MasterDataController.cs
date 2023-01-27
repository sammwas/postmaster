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
    public class MasterDataController : Controller
    {
        private readonly IMasterDataInterface _masterDataInterface;
        private readonly UserCookieData _userData;
        public MasterDataController(IMasterDataInterface masterDataInterface, ICookiesService cookiesService)
        {
            _masterDataInterface = masterDataInterface;
            _userData = cookiesService.Read();
        }
        public async Task<IActionResult> ProductCategories()
        {
            var result = await _masterDataInterface.ProductCategoriesAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Product Categories", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> EditProductCategory(Guid? id)
        {
            if (id == null)
                return View(new ProductCategoryViewModel { Status = EntityStatus.Active });

            var result = await _masterDataInterface.ByIdProductCategoryAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Product Categories", result.Message);
                return RedirectToAction(nameof(ProductCategories));
            }

            var model = new ProductCategoryViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProductCategory(ProductCategoryViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Product Category";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _masterDataInterface.EditProductCategoryAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(ProductCategories));
        }
        public async Task<IActionResult> UnitOfMeasures()
        {
            var result = await _masterDataInterface.UnitOfMeasuresAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Unit of Measures", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> EditUnitOfMeasure(Guid? id)
        {
            if (id == null)
                return View(new UnitOfMeasureViewModel { Status = EntityStatus.Active });
            var result = await _masterDataInterface.ByIdUnitOfMeasureAsync(id.Value);

            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Unit of Measure", result.Message);
                return RedirectToAction(nameof(UnitOfMeasures));
            }
            var model = new UnitOfMeasureViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnitOfMeasure(UnitOfMeasureViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Unit of Measure";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _masterDataInterface.EditUnitOfMeasureAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(UnitOfMeasures));
        }
        public async Task<IActionResult> ExpenseTypes()
        {
            var result = await _masterDataInterface.ExpenseTypesAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Expense Types", result.Message);

            return View(result.Data);
        }
        public async Task<IActionResult> EditExpenseType(Guid? id)
        {
            if (id == null)
                return View(new ExpenseTypeViewModel { Status = EntityStatus.Active });
            var result = await _masterDataInterface.ByIdExpenseTypeAsync(id.Value);

            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Expense Type", result.Message);
                return RedirectToAction(nameof(ExpenseTypes));
            }
            var model = new ExpenseTypeViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpenseType(ExpenseTypeViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Expense Type";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _masterDataInterface.EditExpenseTypeAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(ExpenseTypes));
        }
        public async Task<IActionResult> PaymentModes()
        {
            var result = await _masterDataInterface.PaymentModesAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Expense Types", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> EditPaymentMode(Guid? id)
        {
            if (id == null)
                return View(new PaymentModeViewModel { Status = EntityStatus.Active });

            var result = await _masterDataInterface.ByIdPaymentModeAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Payment Mode", result.Message);
                return RedirectToAction(nameof(PaymentModes));
            }
            var model = new PaymentModeViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPaymentMode(PaymentModeViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Payment Mode";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _masterDataInterface.EditPaymentModeAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(PaymentModes));
        }
        public async Task<IActionResult> TaxTypes()
        {
            var result = await _masterDataInterface.TaxTypesAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Tax Types", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> EditTaxType(Guid? id)
        {
            if (id == null)
                return View(new TaxTypeViewModel { Status = EntityStatus.Active });
            var result = await _masterDataInterface.ByIdTaxTypeAsync(id.Value);

            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Tax Type", result.Message);
                return RedirectToAction(nameof(TaxTypes));
            }
            var model = new TaxTypeViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTaxType(TaxTypeViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Tax Type";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _masterDataInterface.EditTaxTypesAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(TaxTypes));
        }
    }
}
