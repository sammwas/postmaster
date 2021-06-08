using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductInterface _producutInterface;
        private readonly ICookiesService _cookieService;
        private readonly FileUploadService _fileUploadService;

        public ProductsController(IProductInterface productInterface, ICookiesService cookiesService, FileUploadService fileUploadService)
        {
            _producutInterface = productInterface;
            _cookieService = cookiesService;
            _fileUploadService = fileUploadService;
        }
        public async Task<IActionResult> Index()
        {
            var user = _cookieService.Read();
            if (User.IsInRole(Role.SuperAdmin))
            {
                var result = await _producutInterface.AllAsync();
                if (!result.Success)
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                return View(result.Data);
            }
            if (User.IsInRole(Role.Manager) || User.IsInRole(Role.Admin))
            {
                var result = await _producutInterface.ByClientIdAsync(user.ClientId);
                if (!result.Success)
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                return View(result.Data);
            }

            if (User.IsInRole(Role.Clerk))
            {
                var result = await _producutInterface.ByInstanceIdAsync(user.InstanceId);
                if (!result.Success)
                    TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                return View(result.Data);
            }
            return View(new List<Product>());
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(new ProductViewModel { Status = EntityStatus.Active });

            var result = await _producutInterface.ByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductViewModel(result.Data);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            model.Personnel = User.Identity.Name;
            var option = model.IsEditMode ? "Update" : "Add";
            var title = $"{option} Product";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }

            var userData = _cookieService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = Guid.Parse(model.InstanceIdStr);
            if (model.File != null)
            {
                var uploadResult = await _fileUploadService.UploadAsync(model.ClientId, model.File);
                if (!uploadResult.Success)
                {
                    ModelState.AddModelError("File", $"Upload Failed :-{uploadResult.Message}");
                    return View(model);
                }
                model.IsNewImage = true;
                model.ImagePath = uploadResult.PathUrl;
            }

            var result = await _producutInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ProductStockAdjustment(Guid? clientId)
        {
            var result = await _producutInterface.ByClientIdAsync(clientId.Value);
            var model = new ProductStockAdjustmentViewModel
            {
                Products = result.Data
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductStockAdjustment(ProductStockAdjustmentViewModel model)
        {
            var userData = _cookieService.Read();
            model.ClientId = userData.ClientId;
            model.InstanceId = userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var title = "Adjust stock";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            var result = await _producutInterface.AdjustProductStockAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PurchaseOrders(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
        {
            ViewData["InstanceId"] = insId;
            ViewData["DtFrom"] = dtFrom;
            ViewData["DtTo"] = dtTo;
            ViewData["Search"] = search;
            var userData = _cookieService.Read();
            Guid? clientId = null;
            Guid? instanceId = null;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = userData.ClientId;
            if (Guid.TryParse(insId, out var iId))
                instanceId = iId;
            var personnel = "";
            if (User.IsInRole(Role.Clerk))
            {
                instanceId = userData.InstanceId;
                personnel = User.Identity.Name;
            }
            var result = await _producutInterface.PurchaseOrdersAsync(clientId, instanceId, dtFrom, dtTo, search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Purchase Orders", result.Message);
            return View(result.Data);
        }

        public IActionResult EditPurchaseOrder()
        {
            return View();
        }

        public async Task<IActionResult> LowStockProducts()
        {
            var userData = _cookieService.Read();
            var clientId = userData.ClientId;
            Guid? instanceId = null;
            if (User.IsInRole(Role.Clerk))
                instanceId = userData.InstanceId;
            var data = await _producutInterface.LowStockProductsAsync(clientId, instanceId);
            return Json(data);
        }

        public async Task<IActionResult> TopSellingByVolume()
        {
            var userData = _cookieService.Read();
            var clientId = userData.ClientId;
            Guid? instanceId = null;
            if (User.IsInRole(Role.Clerk))
                instanceId = userData.InstanceId;
            var data = await _producutInterface.TopSellingProductsByVolumeAsync(clientId, instanceId, 5);
            return Json(data);
        }
    }
}
