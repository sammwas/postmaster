﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IReportingInterface _reportingInterface;
        public PointOfSaleController(IProductInterface productInterface, ICookiesService cookieService,
            IExpenseInterface expenseInterface, IInvoiceInterface invoiceInterface, IReportingInterface reportingInterface)
        {
            _productInterface = productInterface;
            _userData = cookieService.Read();
            _expenseInterface = expenseInterface;
            _invoiceInterface = invoiceInterface;
            _reportingInterface = reportingInterface;

        }
        public IActionResult Index()
        {
            return View(new ProductSaleViewModel()
            { IsCredit = false });
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
            model.PersonnelName = _userData.FullName;
            var result = await _productInterface.ProductsSaleAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Receipt), new { result.Data.Id });
        }

        public async Task<IActionResult> Receipts(string insId = "", string dtFrom = "", string dtTo = "", string search = "",
            string personnel = "", string modeId = "", string type = "")
        {
            ViewData["DtFrom"] = dtFrom;
            if (string.IsNullOrEmpty(dtFrom))
            {
                dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                ViewData["DtFrom"] = dtFrom;
            }
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            ViewData["PaymentModeId"] = modeId;
            ViewData["SaleType"] = type;

            Guid? instanceId = null;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
            {
                instanceId = _userData.InstanceId;
                personnel = User.Identity.Name;
            }
            ViewData["InstanceId"] = instanceId;
            ViewData["Personnel"] = personnel;

            var result = await _productInterface.ReceiptsAsync(_userData.ClientId, instanceId, dtFrom, dtTo, search,
                personnel, modeId, type);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Receipts", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Invoices(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            if (string.IsNullOrEmpty(dtFrom))
            {
                dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                ViewData["DtFrom"] = dtFrom;
            }
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? instanceId = null;
            var personnel = "";
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
            {
                personnel = User.Identity.Name;
                instanceId = _userData.InstanceId;
            }
            ViewData["InstanceId"] = instanceId;
            var result = await _invoiceInterface
                .GetAsync(_userData.ClientId, instanceId, dtFrom, dtTo, search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Invoices", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Repayments(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            if (string.IsNullOrEmpty(dtFrom))
            {
                dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                ViewData["DtFrom"] = dtFrom;
            }
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? instanceId = null;
            var personnel = "";
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
            {
                personnel = User.Identity.Name;
                instanceId = _userData.InstanceId;
            }
            ViewData["InstanceId"] = instanceId;
            var result = await _reportingInterface
                .RepaymentsAsync(_userData.ClientId, instanceId, DateTime.Parse(dtFrom), DateTime.Parse(dtTo), search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Repayments", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Expenses(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? instanceId = null;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            var personnel = "";
            if (User.IsInRole(Role.Clerk))
            {
                instanceId = _userData.InstanceId;
                personnel = User.Identity.Name;
            }
            if (string.IsNullOrEmpty(dtFrom))
            {
                var dateFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                ViewData["DtFrom"] = dateFrom;
                dtFrom = dateFrom;
            }
            ViewData["InstanceId"] = instanceId;
            var result = await _expenseInterface.AllAsync(_userData.ClientId, instanceId, dtFrom, dtTo, search, personnel);
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
        public async Task<IActionResult> PayInvoice(FulfillOrderViewModel model)
        {
            var returnUrl = Request.Headers[HeaderNames.Referer];
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            model.PersonnelName = _userData.FullName;
            var result = await _invoiceInterface.PayAsync(model);
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
            var result = await _productInterface.ReceiptByIdAsync(id.ToString());
            if (!result.Success)
                TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Receipt", result.Message);
            return View(result.Data);
        }

        public async Task<JsonResult> PrintReceipt(Guid id)
        {
            var result = await _productInterface.PrintReceiptByIdAsync(id, User.Identity.Name);
            return Json(result);
        }

        public async Task<IActionResult> GeneralLedgers(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["InstanceId"] = insId;
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            Guid? instanceId = null;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            if (User.IsInRole(Role.Clerk))
                instanceId = _userData.InstanceId;
            var result = await _productInterface.GeneralLedgersAsync(_userData.ClientId, instanceId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "General Ledgers", result.Message);
            return View(result.Data);
        }

        public IActionResult ReceivePayment()
        {
            return View(new ReceiptUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceivePayment(ReceiptUserViewModel model)
        {
            model.Personnel = User.Identity.Name;
            model.InstanceId = _userData.InstanceId;
            model.ClientId = _userData.ClientId;
            model.PersonnelName = _userData.FullName;
            var result = await _productInterface.ReceiptUserAsync(model);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Receipt", result.Message);
                return View(model);
            }
            TempData.SetData(AlertLevel.Success, $"{model.UserType} Receipt", result.Message);
            return RedirectToAction(nameof(ReceivePayment));
        }

        public IActionResult CancelReceipt(string code = "")
        {
            return View(new CancelReceiptViewModel()
            {
                Code = code,
                Notes = "CANCELLED"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReceipt(CancelReceiptViewModel model)
        {
            model.Personnel = User.Identity.Name;
            model.ClientId = _userData.ClientId;
            model.Personnel = User.Identity.Name;
            var result = await _productInterface.CancelReceiptAsync(model);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Cancel Receipt", result.Message);
                return View(model);
            }
            TempData.SetData(AlertLevel.Success, $"Receipt {model.Code} ", result.Message);
            return RedirectToAction(nameof(CancelReceipt));
        }

        public async Task<JsonResult> ReceiptByCode(string code = "")
        {
            var result = await _productInterface.ReceiptByIdAsync(code);
            return Json(result);
        }
    }
}
