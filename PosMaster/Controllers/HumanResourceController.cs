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
                return RedirectToAction(nameof(ClientBanks), new { userData.ClientId });
            }

            var model = new BankViewModel(result.Data);
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
            return RedirectToAction(nameof(ClientBanks), new { userData.ClientId });
        }

        public async Task<IActionResult> ClientBanks(Guid clientId)
        {
            var result = await _humanResourceInterface.BanksAsync(clientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Banks", result.Message);
            return View(result.Data);
        }
    }
}
