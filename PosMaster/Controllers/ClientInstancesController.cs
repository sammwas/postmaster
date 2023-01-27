using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.ViewModels;
using System;
using System.Threading.Tasks;
using PosMaster.Services;

namespace PosMaster.Controllers
{
    [Authorize]
    public class ClientInstancesController : Controller
    {
        private readonly IClientInstanceInterface _clientInstanceInterface;
        private readonly ICookiesService _cookiesService;
        public ClientInstancesController(IClientInstanceInterface clientInstanceInterface, ICookiesService cookiesService)
        {
            _clientInstanceInterface = clientInstanceInterface;
            _cookiesService = cookiesService;
        }
        public async Task<IActionResult> All()
        {
            var userData = _cookiesService.Read();
            var result = User.IsInRole(Role.SuperAdmin) ?
                await _clientInstanceInterface.AllAsync() :
                await _clientInstanceInterface.ByClientIdAsync(userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Client Instances", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(new ClientInstanceViewModel
                {
                    ClientId = _cookiesService.Read().ClientId
                });

            var result = await _clientInstanceInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Client Instances", result.Message);
                return RedirectToAction(nameof(All));
            }
            var model = result.Success ? new ClientInstanceViewModel(result.Data) : new ClientInstanceViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientInstanceViewModel model)
        {
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Client";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _clientInstanceInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);

            return RedirectToAction(nameof(All));
        }
    }
}
