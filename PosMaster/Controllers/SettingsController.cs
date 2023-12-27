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
    public class SettingsController : Controller
    {
        private readonly IClientInterface _clientInterface;
        private readonly UserCookieData _userData;
        private readonly ISystemSettingInterface _settingInterface;
        private readonly IEmailService _emailService;
        private readonly FileUploadService _fileUploadService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IProductInterface _productInterface;
        private readonly IMasterDataInterface _masterDataInterface;
        private readonly ICustomerInterface _customerInterface;

        public SettingsController(IClientInterface clientInterface, ISystemSettingInterface settingInterface,
            ICookiesService cookiesService, IEmailService emailService, FileUploadService fileUploadService,
            IWebHostEnvironment hostingEnvironment, IProductInterface productInterface, IMasterDataInterface masterDataInterface,
            ICustomerInterface customerInterface)
        {
            _clientInterface = clientInterface;
            _userData = cookiesService.Read();
            _settingInterface = settingInterface;
            _emailService = emailService;
            _fileUploadService = fileUploadService;
            _hostingEnvironment = hostingEnvironment;
            _productInterface = productInterface;
            _masterDataInterface = masterDataInterface;
            _customerInterface = customerInterface;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GlobalSystem()
        {
            var result = await _settingInterface.ReadAsync();
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Settings", result.Message);
            return View(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GlobalSystem(SystemSettingMiniViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.File != null)
            {
                var upResult = await _fileUploadService.UploadAsync(_userData.ClientId, model.File);
                if (!upResult.Success)
                {
                    ModelState.AddModelError("File", $"File not Uploaded : {upResult.Message}");
                    TempData.SetData(AlertLevel.Warning, "Upload", upResult.Message);
                    return View(model);
                }
                _fileUploadService.Delete(model.LogoPath);
                model.LogoPath = upResult.PathUrl;
                model.IsNewImage = true;
            }
            var result = await _settingInterface.UpdateAsync(model, User.Identity.Name);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Settings", result.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EmailSettings()
        {
            var result = await _clientInterface.ClientEmailSettingAsync(_userData.ClientId);
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Email Settings", result.Message);
            return View(new EmailSettingViewModel(result.Data));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailSettings(EmailSettingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.ClientId = _userData.ClientId;
            model.InstanceId = _userData.InstanceId;
            model.Personnel = User.Identity.Name;
            var result = await _clientInterface.UpdateEmailSettingAsync(model);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Email Settings", result.Message);
            return View(new EmailSettingViewModel(result.Data));
        }

        public IActionResult SendTestEmail(bool test = false)
        {
            return View(new TestEmailViewModel
            {
                Recipient = test ? _userData.EmailAddress : "",
                Subject = test ? $"{_userData.ClientName} Test Email - {DateTime.Now}" : "",
                IsTest = test
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTestEmail(TestEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var address = model.Recipient.Equals(_userData.EmailAddress) ?
                 new EmailAddress(_userData) :
                new EmailAddress(_userData)
                {
                    Address = model.Recipient,
                    Name = model.Recipient.Split('@')[0]
                };
            var result = await _emailService.SendAsync(address, model.Subject, model.Content);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Test Email", result.Message);
            return View(model);
        }

        public async Task<IActionResult> TermsAndPrivacy()
        {
            var tp = await _settingInterface.TermsAndPrivacyAsync();
            return View(tp.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> TermsAndPrivacy(TermsAndPrivacyViewModel model)
        {
            var result = await _settingInterface.UpdateTermsAndPrivacyAsync(model, User.Identity.Name);
            TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Terms and Privacy", result.Message);
            return View(model);
        }

        public IActionResult UploadExcel(UploadExelOption id)
        {
            return View(new UploadExcelViewModel
            {
                Option = id,
                InstanceIdStr = _userData.InstanceId.ToString()
            });
        }

        public FileContentResult DownloadTemplate(UploadExelOption option)
        {
            try
            {
                var title = $"{option}_Upload_Template";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage();
                package.Workbook.Properties.Title = title;
                package.Workbook.Properties.Company = _userData.ClientName;
                package.Workbook.Properties.Author = "cassignpro@gmail.com";
                package.Workbook.Properties.Subject = $"{option} Upload Template";

                var date = DateTime.Now.ToString("dd-MMM-yyyy");
                var headers = new List<string>();
                switch (option)
                {
                    case UploadExelOption.Products:
                        headers = new List<string>()
                         {
                            "Code", "Product name", "Allow discount (yes/no)", "Tax rate (0.16)","Service (yes/no)",
                            "Product category",   "Unit of measure", "Reorder level","Buying price","Available quantity",
                            "Selling price", $"Price start date ({date})","Price end date (optional)"
                         };
                        break;
                    case UploadExelOption.Customers:
                        headers = new List<string>
                        {
                            "Code", "First name", "Last name","Gender", "Phone number", "Email address","PIN number",
                            "ID number","Town","Credit limit","Opening balance", "O/B as at (Date)","Notes"
                         };
                        break;
                    default:
                        break;
                }
                var worksheet = package.Workbook.Worksheets.Add(title);
                const int cols = 1;
                var index = 1;
                foreach (var e in headers)
                {
                    worksheet.Cells[cols, index].Style.Font.Bold = true;
                    worksheet.Cells[cols, index].Value = e;
                    index++;
                }

                var unique = Guid.NewGuid().ToString("")[..8].ToUpper();
                var excelName = $"{title}-{unique}-{date}";
                return File(package.GetAsByteArray(), Constants.XlsxContentType, excelName + ".xlsx");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(UploadExcelViewModel model)
        {
            var tag = $"{model.Option} Upload excel";
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
                        var returnData = new ReturnData<string>();
                        var code = workSheet.Cells[i, 1]?.Value?.ToString() ?? "";
                        switch (model.Option)
                        {
                            case UploadExelOption.Products:
                                var discount = workSheet.Cells[i, 3]?.Value?.ToString() ?? "";
                                var tax = workSheet.Cells[i, 4]?.Value?.ToString() ?? "0";
                                var rate = decimal.Parse(tax);
                                var taxRes = await _masterDataInterface
                                    .ByRateTaxTypeAsync(_userData.ClientId, _userData.InstanceId, rate);
                                var service = workSheet.Cells[i, 5]?.Value?.ToString() ?? "";
                                var category = workSheet.Cells[i, 6]?.Value?.ToString() ?? "";
                                var categoryRes = await _masterDataInterface
                                    .ByNameProductCategoryAsync(_userData.ClientId, _userData.InstanceId, category);
                                var uom = workSheet.Cells[i, 7]?.Value?.ToString() ?? "";
                                var uomRes = await _masterDataInterface
                                   .ByNameUnitOfMeasureAsync(_userData.ClientId, _userData.InstanceId, uom);
                                var level = workSheet.Cells[i, 8]?.Value?.ToString() ?? "0";
                                var buyingPrice = workSheet.Cells[i, 9]?.Value?.ToString() ?? "0";
                                var quantity = workSheet.Cells[i, 10]?.Value?.ToString() ?? "0";
                                var sellingPrice = workSheet.Cells[i, 11]?.Value?.ToString() ?? "0";
                                var productViewModel = new ProductViewModel
                                {
                                    IsExcelUpload = true,
                                    Code = code,
                                    Name = workSheet.Cells[i, 2]?.Value?.ToString() ?? "",
                                    AllowDiscount = discount.ToLower().Equals("yes"),
                                    IsService = service.ToLower().Equals("yes"),
                                    UnitOfMeasureId = uomRes.Data.Id.ToString(),
                                    ReorderLevel = decimal.Parse(level),
                                    ProductCategoryId = categoryRes.Data.Id.ToString(),
                                    InstanceId = Guid.Parse(model.InstanceIdStr),
                                    InstanceIdStr = model.InstanceIdStr,
                                    ClientId = _userData.ClientId,
                                    TaxTypeId = taxRes.Data.Id.ToString(),
                                    SellingPrice = decimal.Parse(sellingPrice),
                                    BuyingPrice = decimal.Parse(buyingPrice),
                                    AvailableQuantity = decimal.Parse(quantity),
                                    PriceStartDateStr = workSheet.Cells[i, 12]?.Value?.ToString() ?? "",
                                    PriceEndDateStr = workSheet.Cells[i, 13]?.Value?.ToString() ?? "",
                                };

                                var resultProduct = await _productInterface.EditAsync(productViewModel);
                                returnData.Success = resultProduct.Success;
                                returnData.Message = resultProduct.Message;
                                returnData.ErrorMessage = resultProduct.ErrorMessage;
                                break;
                            case UploadExelOption.Customers:
                                var limitStr = workSheet.Cells[i, 10]?.Value?.ToString() ?? "0";
                                var balanceStr = workSheet.Cells[i, 11]?.Value?.ToString() ?? "0";
                                var customerViewModel = new CustomerViewModel
                                {
                                    Code = code,
                                    FirstName = workSheet.Cells[i, 2]?.Value?.ToString() ?? "",
                                    LastName = workSheet.Cells[i, 3]?.Value?.ToString() ?? "",
                                    Gender = workSheet.Cells[i, 4]?.Value?.ToString() ?? "",
                                    PhoneNumber = workSheet.Cells[i, 5]?.Value?.ToString() ?? "",
                                    EmailAddress = workSheet.Cells[i, 6]?.Value?.ToString() ?? "",
                                    PinNo = workSheet.Cells[i, 7]?.Value?.ToString() ?? "",
                                    IdNumber = workSheet.Cells[i, 8]?.Value?.ToString() ?? "",
                                    Town = workSheet.Cells[i, 9]?.Value?.ToString() ?? "",
                                    CreditLimit = decimal.Parse(limitStr),
                                    OpeningBalance = decimal.Parse(balanceStr),
                                    OpeningBalanceAsAt = workSheet.Cells[i, 12]?.Value?.ToString() ?? "",
                                    Notes = workSheet.Cells[i, 13]?.Value?.ToString() ?? "",
                                    InstanceId = _userData.InstanceId,
                                    ClientId = _userData.ClientId,
                                    PersonnelName = _userData.FullName
                                };
                                var resultCustomer = await _customerInterface.EditAsync(customerViewModel);
                                returnData.ErrorMessage = resultCustomer.ErrorMessage;
                                returnData.Message = resultCustomer.Message;
                                returnData.Success = resultCustomer.Success;
                                break;
                            default:
                                break;
                        }

                        var _msg = $"Processing {prog} of {totalRows} ... {returnData.Message}";
                        var percentage = decimal.Divide(prog, totalRows) * 100;
                        model.Result.Add(new FormSelectViewModel
                        {
                            Id = returnData.Success.ToString(),
                            Text = $"{code} - {returnData.Message} :: {returnData.ErrorMessage}"
                        });
                        if (returnData.Success)
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

        public async Task<IActionResult> LicenceStatus()
        {
            var clientRes = await _clientInterface.ByIdAsync(_userData.ClientId);
            if (!clientRes.Success)
                TempData.SetData(AlertLevel.Warning, "Licence", clientRes.Message);
            var client = clientRes.Data;
            var licence = LicencingService.VerifyLicence(client.Licence, client.Id, client.Name);
            return View(licence.Data);
        }
    }
}
