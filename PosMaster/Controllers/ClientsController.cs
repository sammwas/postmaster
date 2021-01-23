using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class ClientsController : Controller
    {
        private readonly IClientInterface _clientInterface;
        public ClientsController(IClientInterface clientInterface)
        {
            _clientInterface = clientInterface;
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(new ClientViewModel { Status = EntityStatus.Active });
            var result = await _clientInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {

            }
            var model = result.Success ? new ClientViewModel(result.Data) : new ClientViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel model)
        {
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Client";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _clientInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);

            return RedirectToAction(nameof(All));
        }
        public async Task<IActionResult> All()
        {
            var result = await _clientInterface.AllAsync();
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Clients", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Summary()
        {
            return View();
        }
    }
}
