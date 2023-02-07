﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class ProductsController : Controller
    {
        private readonly IProductInterface _productInterface;
        private readonly UserCookieData _userData;
        private readonly FileUploadService _fileUploadService;
        private readonly string xlsxContentType = Constants.XlsxContentType;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMasterDataInterface _masterDataInterface;

        public ProductsController(IProductInterface productInterface, ICookiesService cookiesService,
            FileUploadService fileUploadService, IWebHostEnvironment hostingEnvironment, IMasterDataInterface masterDataInterface)
        {
            _productInterface = productInterface;
            _fileUploadService = fileUploadService;
            _userData = cookiesService.Read();
            _hostingEnvironment = hostingEnvironment;
            _masterDataInterface = masterDataInterface;
        }
        public async Task<IActionResult> Index(string insId = "", string search = "")
        {
            if (string.IsNullOrEmpty(insId))
                insId = _userData.InstanceId.ToString();

            ViewData["InstanceId"] = insId;
            ViewData["Search"] = search;
            var result = await _productInterface.ByInstanceIdAsync(_userData.ClientId, Guid.Parse(insId));
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Products", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(new ProductViewModel { Status = EntityStatus.Active });

            var result = await _productInterface.ByIdAsync(id.Value);
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
            model.ClientId = _userData.ClientId;
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

            var result = await _productInterface.EditAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ProductStockAdjustment()
        {
            var result = _userData.Role.Equals(Role.Clerk) ?
             await _productInterface.ByInstanceIdAsync(_userData.ClientId, _userData.InstanceId) :
             await _productInterface.ByInstanceIdAsync(_userData.ClientId);
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
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var title = "Adjust stock";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            var result = await _productInterface.AdjustProductStockAsync(model);
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
            var result = await _productInterface.PurchaseOrdersAsync(clientId, instanceId, dtFrom, dtTo, search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Purchase Orders", result.Message);
            return View(result.Data);
        }

        public async Task<IActionResult> EditPurchaseOrder(Guid? id)
        {
            if (id == null)
                return View(new PurchaseOrderViewModel { Status = EntityStatus.Active });
            var result = await _productInterface.PurchaseOrderByIdAsync(id.Value);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Purchase Order", result.Message);
                return RedirectToAction(nameof(Index));
            }

            var model = new PurchaseOrderViewModel(result.Data);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPurchaseOrder(PurchaseOrderViewModel model)
        {
            var title = "Add Purchase Order";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            var result = await _productInterface.EditPurchaseOrderAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return RedirectToAction(nameof(PurchaseOrders));
            return View(model);
        }
        public async Task<IActionResult> LowStockProducts()
        {
            Guid? clientId = null;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = _userData.ClientId;
            Guid? instanceId = null;
            if (User.IsInRole(Role.Clerk))
                instanceId = _userData.InstanceId;
            var data = await _productInterface.LowStockProductsAsync(clientId, instanceId, 5);
            return Json(data);
        }

        public async Task<IActionResult> TopSellingByVolume()
        {
            Guid? clientId = null;
            if (!User.IsInRole(Role.SuperAdmin))
                clientId = _userData.ClientId;
            Guid? instanceId = null;
            if (User.IsInRole(Role.Clerk))
                instanceId = _userData.InstanceId;
            var data = await _productInterface.TopSellingProductsByVolumeAsync(clientId, instanceId, 5);
            return Json(data);
        }
        public async Task<IActionResult> ProductPrice(string instId = "")
        {
            var data = new ProductPriceViewModel();
            ViewData["InstanceId"] = instId;
            if (string.IsNullOrEmpty(instId))
                return View(data);
            var result = await _productInterface.ByInstanceIdAsync(_userData.ClientId, Guid.Parse(instId));
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "Products", result.Message);
                return View(data);
            }

            foreach (var item in result.Data)
            {
                data.ProductPriceMiniViewModels.Add(new ProductPriceMiniViewModel(item));
            }
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductPrice(ProductPriceViewModel model)
        {
            var title = "Edit Price";
            ViewData["InstanceId"] = model.InstanceId;
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            var result = await _productInterface.EditPriceAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return RedirectToAction(nameof(ProductPrice), new { instId = model.InstanceId });
            return View(model);
        }

        public async Task<JsonResult> Search(string term = "", bool isPos = false)
        {
            var products = User.IsInRole(Role.Clerk) ?
                    await _productInterface.ByInstanceIdAsync(_userData.ClientId, _userData.InstanceId, isPos, term) :
                    await _productInterface.ByInstanceIdAsync(_userData.ClientId, (Guid?)null, isPos, term);
            return Json(products);
        }
    }
}
