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
    public class ProductsController : Controller
    {
        private IProductInterface _producutInterface;
        private ICookiesService _cookieService;

        public ProductsController(IProductInterface productInterface, ICookiesService cookiesService)
        {
            _producutInterface = productInterface;
            _cookieService = cookiesService;
        }
        public async Task<IActionResult> All()
        {
            var user = _cookieService.Read();

            if (user.Role == Role.SuperAdmin) 
            {
                var result = await _producutInterface.AllAsync();
                if (!result.Success)
                {
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                    return View();
                }
                return View(result.Data);
            }
            if (user.Role == Role.Manager)
            {
                var result = await _producutInterface.ByClientIdAsync(user.ClientId);
                if (!result.Success)
                {
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                    return View();
                }
                return View(result.Data);
            }
            if (user.Role == Role.Admin)
            {
                var result = await _producutInterface.ByClientIdAsync(user.ClientId);
                if (!result.Success)
                {
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                    return View();
                }
                return View(result.Data);
            }
            if (user.Role == Role.Clerk) 
            {
                var result = await _producutInterface.ByInstanceIdAsync(user.InstanceId);
                if (!result.Success)
                {
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                    return View();
                }
                return View(result.Data);
            }
            return View();
        }
        public async Task<IActionResult> Edit(Guid? id) 
        {
            if (id == null)
                return View(new ProductViewModel { Status = EntityStatus.Active });
   
            var result = await _producutInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                return RedirectToAction(nameof(All));
            }

            var model = new ProductViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var userData = _cookieService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Product";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var result = await _producutInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(All));
        }

    }
}
