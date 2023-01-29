using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
    public class PointOfSaleController : Controller
    {
        private readonly IProductInterface _productInterface;
        private readonly UserCookieData _userData;
        private readonly IExpenseInterface _expenseInterface;
        private readonly IInvoiceInterface _invoiceInterface;
        public PointOfSaleController(IProductInterface productInterface, ICookiesService cookieService,
            IExpenseInterface expenseInterface, IInvoiceInterface invoiceInterface)
        {
            _productInterface = productInterface;
            _userData = cookieService.Read();
            _expenseInterface = expenseInterface;
            _invoiceInterface = invoiceInterface;
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
            model.Personnel = User.Identity.Name;
            var result = await _productInterface.ProductsSaleAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Receipts(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            if (!string.IsNullOrEmpty(dtFrom))
            {
                dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                ViewData["DtFrom"] = dtFrom;
            }
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? clientId = null;
            Guid? instanceId = null;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = _userData.ClientId;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
                instanceId = _userData.InstanceId;
            ViewData["InstanceId"] = instanceId;
            var result = await _productInterface.ReceiptsAsync(clientId, instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Receipts", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Invoices(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? clientId = null;
            Guid? instanceId = null;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = _userData.ClientId;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
                instanceId = _userData.InstanceId;

            ViewData["InstanceId"] = instanceId;
            var result = await _invoiceInterface.GetAsync(clientId, instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Invoices", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Expenses(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? clientId = null;
            Guid? instanceId = null;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = _userData.ClientId;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            var personnel = "";
            if (User.IsInRole(Role.Clerk))
            {
                instanceId = _userData.InstanceId;
                personnel = User.Identity.Name;
            }
            ViewData["InstanceId"] = instanceId;
            var result = await _expenseInterface.AllAsync(clientId, instanceId, dtFrom, dtTo, search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Expenses", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> EditExpense(Guid? id)
        {
            if (id == null)
                return View(new ExpenseViewModel { Status = EntityStatus.Active });
            var result = await _expenseInterface.ByIdAsync(id.Value);

            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Expense", result.Message);
                return RedirectToAction(nameof(Expenses));
            }
            var model = new ExpenseViewModel(result.Data);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpense(ExpenseViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Expense";
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
            return RedirectToAction(nameof(Expenses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayInvoice(PayInvoiceViewModel model)
        {
            var returnUrl = Request.Headers[HeaderNames.Referer];
            var result = await _invoiceInterface.PayAsync(model.Id, User.Identity.Name);
            if (!result.Success)
                TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Pay Invoice", result.Message);
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Invoice(Guid id)
        {
            var result = await _invoiceInterface.ByIdAsync(id);
            if (!result.Success)
                TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Invoice", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Receipt(Guid id)
        {
            var result = await _productInterface.ReceiptByIdAsync(id);
            if (!result.Success)
                TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Receipt", result.Message);
            return View(result.Data);
        }

        public async Task<JsonResult> PrintReceipt(Guid id)
        {
            var result = await _productInterface.PrintReceiptByIdAsync(id, User.Identity.Name);
            return Json(result);
        }
    }
}
