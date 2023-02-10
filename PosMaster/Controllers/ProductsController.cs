using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductInterface _productInterface;
        private readonly UserCookieData _userData;
        private readonly FileUploadService _fileUploadService;
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
        public IActionResult ProductStockAdjustment()
        {
            return View(new ProductStockAdjustmentViewModel());
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
                return View(model);

            return RedirectToAction(nameof(PurchaseOrders), new { dtFrom = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy"), dtTo = DateTime.Now.ToString("dd-MMM-yyyy") });
        }
        public async Task<IActionResult> ReceivedGoods(string insId = "", string dtFrom = "", string dtTo = "", string search = "")
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
            var result = await _productInterface.GoodsReceivedAsync(clientId, instanceId, dtFrom, dtTo, search, personnel);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Received Goods", result.Message);
            return View(result.Data);
        }
        public async Task<IActionResult> EditReceivedGoods(Guid? id)
        {
            var model = new GoodsReceivedNoteViewModel { Status = EntityStatus.Active };
            if (string.IsNullOrEmpty(id.ToString()))
                return View(model);
            ViewData["OrderId"] = id.Value.ToString();
            var result = await _productInterface.GrnByIdAsync(id.Value);
            if (result.Success)
            {
                var grnViewModel = new GoodsReceivedNoteViewModel(result.Data);
                ViewData["Supplier"] = result.Data.Supplier.Name;
                return View(grnViewModel);
            }
            var order = await _productInterface.PurchaseOrderByIdAsync(id.Value);
            var po = order.Data;
            model.GrnItems = model.GetPoProducts(po);
            model.PoId = po.Id.ToString();
            model.SupplierId = po.SupplierId.ToString();
            ViewData["Supplier"] = po.Supplier.Name;
            return View(model);
        }
        public IActionResult GetPurchaseOrderDetails(string poId = "")
        {
            if (string.IsNullOrEmpty(poId))
            {
                return RedirectToAction(nameof(EditReceivedGoods), new { id = "" });

            }
            var purchaseOrderId = Guid.Parse(poId);
            return RedirectToAction(nameof(EditReceivedGoods), new { id = purchaseOrderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReceivedGoods(GoodsReceivedNoteViewModel model)
        {
            var title = "Add Goods Received Note";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            var result = await _productInterface.EditGrnAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);

            return RedirectToAction(nameof(ReceivedGoods), new { dtFrom = Helpers.FirstDayOfWeek().ToString("dd-MMM-yyyy"), dtTo = DateTime.Now.ToString("dd-MMM-yyyy") });
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

        public async Task<IActionResult> ViewProduct(Guid id, string code = "")
        {
            ViewData["ProductCode"] = code;
            var result = await _productInterface.ProductDetailsAsync(code, id);
            if (!result.Success)
            {
                TempData.SetData(AlertLevel.Warning, "View Product", $"{code} {result.Message}");
            }
            return View(result.Data);
        }

        public IActionResult UploadExcel()
        {
            return View(new UploadExcelViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(UploadExcelViewModel model)
        {
            var tag = "Upload excel";
            try
            {
                var fileExtension = Path.GetExtension(model.File.FileName);
                var exts = new List<string> { ".xls", ".xlsx" };
                if (!exts.Contains(fileExtension))
                {
                    var fErr = $"Allowed formats {string.Join(", ", exts)}";
                    TempData.SetData(AlertLevel.Warning, tag, fErr);
                    ModelState.AddModelError("File", fErr);
                    return View(model);
                }

                var personnel = User.Identity.Name;
                var rootFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", $"{_userData.ClientId}");
                if (!Directory.Exists(rootFolder))
                    Directory.CreateDirectory(rootFolder);

                var fileName = $"temp_{_userData.UserId}_upload{fileExtension}";
                var filePath = Path.Combine(rootFolder, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"Delete exisiting File at {filePath}");
                    System.IO.File.Delete(filePath);
                    Console.WriteLine("Deleted");
                }

                var fileLocation = new FileInfo(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }
                var added = 0;
                var totalRows = 0;
                var prog = 1;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(fileLocation))
                {
                    var workSheet = package.Workbook.Worksheets.First();
                    totalRows = workSheet.Dimension.Rows;
                    for (int i = 2; i <= totalRows; i++)
                    {

                        var discount = workSheet.Cells[i, 3]?.Value?.ToString() ?? "";
                        var tax = workSheet.Cells[i, 4]?.Value?.ToString() ?? "";
                        var hasRate = decimal.TryParse(tax, out var rate);
                        var taxRes = await _masterDataInterface
                            .ByRateTaxTypeAsync(_userData.ClientId, _userData.InstanceId, hasRate ? rate : 0);
                        var service = workSheet.Cells[i, 5]?.Value?.ToString() ?? "";
                        var category = workSheet.Cells[i, 6]?.Value?.ToString() ?? "";
                        var categoryRes = await _masterDataInterface
                            .ByNameProductCategoryAsync(_userData.ClientId, _userData.InstanceId, category);
                        var level = workSheet.Cells[i, 8]?.Value?.ToString() ?? "";
                        var uom = workSheet.Cells[i, 7]?.Value?.ToString() ?? "";
                        var productViewModel = new ProductViewModel
                        {
                            Code = workSheet.Cells[i, 1]?.Value?.ToString() ?? "",
                            Name = workSheet.Cells[i, 2]?.Value?.ToString() ?? "",
                            AllowDiscount = discount.ToLower().Equals("yes"),
                            IsService = service.ToLower().Equals("yes"),
                            UnitOfMeasure = uom.ToUpper(),
                            ReorderLevel = string.IsNullOrEmpty(level) ? 0 : decimal.Parse(level),
                            ProductCategoryId = categoryRes.Data.Id.ToString(),
                            InstanceId = Guid.Parse(model.InstanceIdStr),
                            InstanceIdStr = model.InstanceIdStr,
                            ClientId = _userData.ClientId,
                            TaxTypeId = taxRes.Data.Id.ToString()
                        };

                        var result = await _productInterface.EditAsync(productViewModel);
                        var _msg = $"Processing {prog} of {totalRows} ... {result.Message}";
                        var percentage = decimal.Divide(prog, totalRows) * 100;
                        model.Result.Add(new FormSelectViewModel
                        {
                            Id = result.Success.ToString(),
                            Text = $"{productViewModel.Code} - {result.Message}"
                        });
                        if (result.Success)
                            added++;
                        prog++;
                    }

                }
                totalRows--;
                System.IO.File.Delete(filePath);
                var msg = $"Uploaded {added} out of {totalRows}";
                TempData.SetData(added.Equals(totalRows) ? AlertLevel.Success : AlertLevel.Warning, tag, msg);
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData.SetData(AlertLevel.Error, tag, "Error occured. Try later");
                return View(model);
            }
        }
        public IActionResult ItemPrice()
        {
            return View(new ItemPriceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemPrice(ItemPriceViewModel model)
        {
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var title = "Product Price";
            if (!ModelState.IsValid)
            {
                var message = "Missing fields";
                TempData.SetData(AlertLevel.Warning, title, message);
                return View(model);
            }
            var result = await _productInterface.ProductPriceAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
            if (!result.Success)
                return View(model);
            return RedirectToAction(nameof(Index));
        }

    }
}
