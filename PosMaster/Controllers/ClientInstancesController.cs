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
    public class ClientInstancesController : Controller
    {
        private IClientInstanceInterface _clientInstanceInterface;
        public ClientInstancesController(IClientInstanceInterface clientInstanceInterface)
        {
            _clientInstanceInterface = clientInstanceInterface;
        }
        public async Task<IActionResult> All()
        {
            var result = await _clientInstanceInterface.AllAsync();
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Client Instances", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Edit(Guid clientId, Guid? id) 
        {
            if(id == null)
                return View(new ClientInstanceViewModel {ClientId = clientId });

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
