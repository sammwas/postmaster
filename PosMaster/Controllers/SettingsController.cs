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
    public class SettingsController : Controller
    {
        private readonly IClientInterface _clientInterface;
        private readonly UserCookieData _userData;
        private readonly ISystemSettingInterface _settingInterface;
        private readonly IEmailService _emailService;
        private readonly FileUploadService _fileUploadService;
        public SettingsController(IClientInterface clientInterface, ISystemSettingInterface settingInterface,
            ICookiesService cookiesService, IEmailService emailService, FileUploadService fileUploadService)
        {
            _clientInterface = clientInterface;
            _userData = cookiesService.Read();
            _settingInterface = settingInterface;
            _emailService = emailService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> System()
        {
            var result = await _settingInterface.ReadAsync();
            if (!result.Success)
                TempData.SetData(AlertLevel.Warning, "Settings", result.Message);
            return View(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> System(SystemSettingMiniViewModel model)
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

        public IActionResult SendTestEmail()
        {
            return View(new TestEmailViewModel
            {
                Recipient = _userData.EmailAddress,
                Subject = $"{_userData.ClientName} Test Email - {DateTime.Now}"
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

    }
}
