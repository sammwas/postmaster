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
    public class HumanResourceController : Controller
    {
        private readonly IHumanResourceInterface _humanResourceInterface;
        private readonly UserCookieData _userData;
        public HumanResourceController(IHumanResourceInterface humanResourceInterface, ICookiesService cookiesService)
        {
            _humanResourceInterface = humanResourceInterface;
            _userData = cookiesService.Read();
        }

        public async Task<IActionResult> EditBank(Guid? id)
        {
            if (id == null)
                return View(new BankViewModel { Status = EntityStatus.Active });

            var result = await _humanResourceInterface.BankByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Banks", result.Message);
                return RedirectToAction(nameof(Banks));
            }

            var model = new BankViewModel(result.Data);
            return View(model);
        }

        public async Task<IActionResult> EditLeaveCategory(Guid? id)
        {
            if (id == null)
                return View(new EmployeeLeaveCategoryViewModel { Status = EntityStatus.Active });

            var result = await _humanResourceInterface.LeaveCategoryByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Leave Categories", result.Message);
                return RedirectToAction(nameof(LeaveCategories));
            }

            var model = new EmployeeLeaveCategoryViewModel(result.Data);
            return View(model);
        }

        public async Task<IActionResult> EditEmployeeKin(string userId)
        {

            var result = await _humanResourceInterface.EmployeeKinByUserIdAsync(userId);
            var model = result.Success ? new EmployeeKinViewModel(result.Data)
                : new EmployeeKinViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployeeKin(EmployeeKinViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Kin";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _humanResourceInterface.EditEmployeeKinAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            return View(model);
        }

        public async Task<IActionResult> EditEmployeeSalary(string userId)
        {

            var result = await _humanResourceInterface.EmployeeSalaryByUserIdAsync(userId);
            var model = result.Success ? new EmployeeSalaryViewModel(result.Data)
                : new EmployeeSalaryViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployeeSalary(EmployeeSalaryViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Salary";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _humanResourceInterface.EditEmployeeSalaryAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBank(BankViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Bank";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _humanResourceInterface.EditBankAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Banks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLeaveCategory(EmployeeLeaveCategoryViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Leave Category";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _humanResourceInterface.EditLeaveCategoryAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(LeaveCategories));
        }

        public async Task<IActionResult> Banks()
        {
            var result = await _humanResourceInterface.BanksAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Banks", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> LeaveCategories()
        {
            var result = await _humanResourceInterface.LeaveCategoriesAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Leave Categories", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> EditLeaveApplication(Guid? id)
        {
            if (id == null)
                return View(new LeaveApplicationViewModel
                {
                    Status = EntityStatus.Active
                });

            var result = await _humanResourceInterface.LeaveApplicationByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Leave", result.Message);
                return RedirectToAction(nameof(MyApplications), new { dtFrom = Helpers.firstDayOfYear.ToString("dd-MMM-yyyy") });
            }

            var model = new LeaveApplicationViewModel(result.Data);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLeaveApplication(LeaveApplicationViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            model.UserId = _userData.UserId;
            model.Gender = _userData.Gender;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Leave";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _humanResourceInterface.EditLeaveApplicationAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(MyApplications), new { dtFrom = Helpers.firstDayOfYear.ToString("dd-MMM-yyyy") });
        }

        public async Task<IActionResult> MyApplications(string dtFrom = "", string dtTo = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            var result = await _humanResourceInterface
                .LeaveApplicationsAsync(_userData.ClientId, _userData.InstanceId, _userData.UserId, dtFrom, dtTo);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "My Applications", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> LeaveApplications(Guid? instanceId, string userId = "", string dtFrom = "",
            string dtTo = "", string search = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            ViewData["instanceId"] = instanceId;
            var result = await _humanResourceInterface.LeaveApplicationsAsync(_userData.ClientId, instanceId, userId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Leave Applications", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> EmployeeSalaries(Guid? id)
        {
            ViewData["InstanceId"] = id;
            var result = await _humanResourceInterface.EmployeeSalariesAsync(_userData.ClientId, id);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Salaries", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> ViewLeave(Guid id)
        {
            var result = await _humanResourceInterface.LeaveApplicationByIdAsync(id);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Leave", result.Message);
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRejectLeave(ApproveLeaveViewModel model, ApplicationStatus status)
        {
            model.Status = status;
            var result = await _humanResourceInterface.ApproveLeaveApplicationAsync(model);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Leave", result.Message);
            return RedirectToAction(nameof(LeaveApplications), new
            {
                dtFrom = Helpers.firstDayOfYear.ToString("dd-MMM-yyyy")
            });
        }

        public IActionResult ApproveMonthlyPayment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMonthlyPayment(ApproveMonthlyPaymentViewModel model)
        {
            var result = await _humanResourceInterface.ApproveMonthPaymentAsync(model);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Approve Payments", result.Message);
            return RedirectToAction(nameof(MonthlyPayments), new
            {
                model.Year,
                model.Month,
                model.InstanceId
            });
        }

        public async Task<IActionResult> MonthlyPayments(Guid? instanceId, int year, int month)
        {
            ViewData["InstanceId"] = instanceId;
            ViewData["ClientId"] = _userData.ClientId;
            ViewData["Year"] = year;
            ViewData["Month"] = month;
            var result = await _humanResourceInterface.MonthlyPaymentsAsync(month, year, _userData.ClientId, instanceId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Monthly Payments", result.Message);
            return View(result.Data);
        }
    }
}
