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
    public class PointOfSaleController : Controller
    {
        private readonly IProductInterface _productInterface;
        private readonly ICookiesService _cookieService;
        private readonly IExpenseInterface _expenseInterface;
        public PointOfSaleController(IProductInterface productInterface, ICookiesService cookieService, IExpenseInterface expenseInterface)
        {
            _productInterface = productInterface;
            _cookieService = cookieService;
            _expenseInterface = expenseInterface;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProductSaleViewModel model)
        {
            var title = "Product Sale";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            model.IsWalkIn = true;
            model.Personnel = User.Identity.Name;
            var result = await _productInterface.ProductsSaleAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Receipts(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "") 
        {
            var result = await _productInterface.ReceiptsAsync(clientId, instanceId, dateFrom, dateTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Product Categories", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Expenses(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "") 
        {
            var result = await _expenseInterface.AllAsync(clientId, instanceId, dateFrom, dateTo, search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Product Categories", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> EditExpense(Guid? id) 
        {
            if (id == null)
                return View(new ExpenseViewModel { Status = EntityStatus.Active });
            var result = await _expenseInterface.ByIdAsync(id.Value);

            if (!result.Success)
            {
                var userData = _cookieService.Read();
                var clientId = userData.ClientId;
                TempData.SetData(AlertLevel.Warning, "Unit of Measure", result.Message);
                return RedirectToAction(nameof(Expenses), new { clientId, userData.InstanceId });
            }
            var model = new ExpenseViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpense(ExpenseViewModel model)
        {
            var userData = _cookieService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Expense Type";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _expenseInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Expenses), new { userData.ClientId, userData.InstanceId });
        }
    }
}
