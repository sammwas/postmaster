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
        private readonly ICookiesService _cookiesService;
        public HumanResourceController(IHumanResourceInterface humanResourceInterface, ICookiesService cookiesService)
        {
            _humanResourceInterface = humanResourceInterface;
            _cookiesService = cookiesService;
        }

        public async Task<IActionResult> EditBank(Guid? id)
        {
            var userData = _cookiesService.Read();
            if (id == null)
                return View(new BankViewModel { Status = EntityStatus.Active });

            var result = await _humanResourceInterface.BankByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Banks", result.Message);
                return RedirectToAction(nameof(Banks), new { userData.ClientId });
            }

            var model = new BankViewModel(result.Data);
            return View(model);
        }

        public async Task<IActionResult> EditLeaveCategory(Guid? id)
        {
            var userData = _cookiesService.Read();
            if (id == null)
                return View(new EmployeeLeaveCategoryViewModel { Status = EntityStatus.Active });

            var result = await _humanResourceInterface.LeaveCategoryByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Leave Categories", result.Message);
                return RedirectToAction(nameof(LeaveCategories), new { userData.ClientId });
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
            var userData = _cookiesService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
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
            var userData = _cookiesService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
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
            var userData = _cookiesService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
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
            return RedirectToAction(nameof(Banks), new { userData.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLeaveCategory(EmployeeLeaveCategoryViewModel model)
        {
            var userData = _cookiesService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
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
            return RedirectToAction(nameof(LeaveCategories), new { userData.ClientId });
        }

        public async Task<IActionResult> Banks(Guid clientId)
        {
            var result = await _humanResourceInterface.BanksAsync(clientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Banks", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> LeaveCategories(Guid clientId)
        {
            var result = await _humanResourceInterface.LeaveCategoriesAsync(clientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Leave Categories", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> EditLeaveApplication(Guid? id)
        {
            var userData = _cookiesService.Read();
            if (id == null)
                return View(new LeaveApplicationViewModel
                {
                    Status = EntityStatus.Active,
                    UserId = userData.UserId
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
            var userData = _cookiesService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
            model.Personnel = User.Identity.Name;
            model.UserId = userData.UserId;
            model.Gender = userData.Gender;
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
            var userData = _cookiesService.Read();
            var result = await _humanResourceInterface.LeaveApplicationsAsync(userData.ClientId, userData.InstanceId, userData.UserId, dtFrom, dtTo);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "My Applications", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> LeaveApplications(Guid? clientId, Guid? instanceId, string userId = "", string dtFrom = "",
            string dtTo = "", string search = "")
        {
            ViewData["dtTo"] = dtTo;
            ViewData["dtFrom"] = dtFrom;
            ViewData["instanceId"] = instanceId;
            var result = await _humanResourceInterface.LeaveApplicationsAsync(clientId, instanceId, userId, dtFrom, dtTo, search);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Leave Applications", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> EmployeeSalaries(Guid? instanceId)
        {
            ViewData["InstanceId"] = instanceId;
            var userData = _cookiesService.Read();
            var result = await _humanResourceInterface.EmployeeSalariesAsync(userData.ClientId, instanceId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Salaries", result.Message);
            return View(result.Data);
        }
    }
}
