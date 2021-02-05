using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
	[Authorize]
	public class UsersController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailService _emailService;
		private readonly ILogger<UsersController> _logger;
		private readonly IClientInstanceInterface _clientInstanceInterface;
		private readonly IUserInterface _userInterface;
		private readonly ICookiesService _cookiesService;
		private readonly FileUploadService _fileUploadService;
		public UsersController(UserManager<User> userManager, IEmailService emailService, ILogger<UsersController> logger,
			  IClientInstanceInterface clientInstanceInterface, IUserInterface userInterface, ICookiesService cookiesService,
			  FileUploadService fileUploadService)
		{
			_userManager = userManager;
			_emailService = emailService;
			_logger = logger;
			_clientInstanceInterface = clientInstanceInterface;
			_userInterface = userInterface;
			_cookiesService = cookiesService;
			_fileUploadService = fileUploadService;
		}

		public async Task<IActionResult> All()
		{
			var tag = "Users";
			var user = _cookiesService.Read();
			if (User.IsInRole(Role.SuperAdmin))
			{
				var result = await _userInterface.AllAsync();
				if (!result.Success)
					TempData.SetData(AlertLevel.Warning, tag, result.Message);
				return View(result.Data);
			}
			if (User.IsInRole(Role.Manager) || User.IsInRole(Role.Admin))
			{
				var result = await _userInterface.ByClientIdAsync(user.ClientId);
				if (!result.Success)
					TempData.SetData(AlertLevel.Warning, tag, result.Message);
				return View(result.Data);
			}

			if (User.IsInRole(Role.Clerk))
			{
				var result = await _userInterface.ByInstanceIdAsync(user.InstanceId);
				if (!result.Success)
					TempData.SetData(AlertLevel.Warning, tag, result.Message);
				return View(result.Data);
			}
			return View(new List<UserViewModel>());
		}

		public async Task<IActionResult> Edit(string id = null)
		{
			if (id == null)
			{
				return View(new UserViewModel());
			}
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				TempData.SetData(AlertLevel.Warning, "Users", "Provided user not Found");
				_logger.LogInformation($"edit user : provided user {id} not found");
				return RedirectToAction(nameof(All));
			}
			var model = new UserViewModel(user)
			{
				HasPassword = await _userManager.HasPasswordAsync(user)
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(UserViewModel model)
		{
			var tag = model.IsEditMode ? "Update User" : "Add User";
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var instanceRes = await _clientInstanceInterface.ByIdAsync(Guid.Parse(model.UserInstanceId));
			if (!instanceRes.Success)
			{
				ModelState.AddModelError(string.Empty, $"Provided instance not Found");
				TempData.SetData(AlertLevel.Warning, $"{tag}", instanceRes.Message);
			}
			var hasDob = DateTime.TryParse(model.DoB, out var dob);
			var instance = instanceRes.Data;
			model.InstanceId = instance.Id;
			model.ClientId = instance.ClientId;
			model.DoB = hasDob ? dob.ToString() : DateTime.Now.AddYears(-18).ToString();
			if (model.IsEditMode)
			{
				var updateRes = await _userInterface.UpdateAsync(model);
				if (!updateRes.Success)
					ModelState.AddModelError(string.Empty, $"Update failed");
				TempData.SetData(updateRes.Success ? AlertLevel.Success : AlertLevel.Warning, $"{tag}", updateRes.Message);
				return RedirectToAction(nameof(Edit), new { id = model.UserId });
			}
			var user = new User
			{
				Status = EntityStatus.Active,
				ClientId = model.ClientId,
				InstanceId = model.InstanceId,
				Role = model.Role,
				Personnel = User.Identity.Name,
				IdNumber = model.IdNumber,
				Gender = model.Gender,
				Title = model.Title,
				FirstName = model.FirstName,
				MiddleName = model.MiddleName,
				LastName = model.LastName,
				Email = model.EmailAddress,
				UserName = model.EmailAddress,
				MaritalStatus = model.MaritalStatus,
				DateOfBirth = DateTime.Parse(model.DoB),
				PhoneNumber = model.PhoneNumber
			};
			user.NormalizedEmail = user.NormalizedUserName = model.EmailAddress.ToUpper();
			var result = await _userManager.CreateAsync(user);
			if (result.Succeeded)
				await _userManager.AddToRoleAsync(user, model.Role);
			else
				AddErrors(result);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
			await _emailService.SendEmailConfirmationAsync(new EmailAddress(user), callbackUrl);
			TempData.SetData(AlertLevel.Success, $"{tag}", $"User {model.FirstName} Added");
			return RedirectToAction(nameof(All));
		}

		public async Task<IActionResult> ResendConfirmLink(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
			var result = await _emailService.SendEmailConfirmationAsync(new EmailAddress(user), callbackUrl);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, "Confirm Link", result.Message);
			return RedirectToAction(nameof(Edit), new { id });
		}

		public async Task<IActionResult> ResetPassword(string id)
		{
			var userResult = await _userManager.FindByIdAsync(id);
			var code = await _userManager.GeneratePasswordResetTokenAsync(userResult);
			var model = new ResetPasswordViewModel { UserName = userResult.UserName, Id = id, Code = code };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			var tag = "Reset Password";
			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}
				var user = await _userManager.FindByIdAsync(model.Id);
				if (user == null)
				{
					ModelState.AddModelError(string.Empty, $"Provided user not Found");
					TempData.SetData(AlertLevel.Warning, $"{tag}", "Unable to get provided account");
					return View(model);
				}

				var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
				if (!result.Succeeded)
				{
					TempData.SetData(AlertLevel.Warning, $"{tag}", "Unable to reset password");
					AddErrors(result);
					return View(model);
				}
				TempData.SetData(AlertLevel.Success, $"{tag}", "Password reset successfully");
				return RedirectToAction("ResetPassword", new { user.Id });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				ModelState.AddModelError(string.Empty, $"Error occured. Try again later");
				TempData.SetData(AlertLevel.Error, $"{tag}", "Error occured. Try later");
				return View(model);
			}
		}

		public async Task<IActionResult> Confirm(string id)
		{
			try
			{
				var confirmRes = await _userInterface.ConfirmEmailAsync(id, User.Identity.Name);
				if (!confirmRes.Success)
				{
					TempData.SetData(AlertLevel.Warning, $"Confirm", $"Not Cofirmed :- {confirmRes.Message}");
				}
				return RedirectToAction("Edit", new { id });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_logger.LogError($"Error getting user {id} to confirm email :-{e.Message}");
				return RedirectToAction("Edit", new { id });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upload(UploadImageViewModel model)
		{
			var tag = "Upload";
			if (!ModelState.IsValid)
			{
				TempData.SetData(AlertLevel.Warning, tag, "Image is Required");
				return RedirectToAction(nameof(Edit), new { id = model.UserId });
			}
			var user = _cookiesService.Read();
			var uploadResult = await _fileUploadService.UploadAsync(user.ClientId, model.File);
			if (uploadResult.Success)
			{
				model.CurrentImage = uploadResult.PathUrl;
				var iRes = await _userInterface.UpdateImageAsync(model);
				if (iRes.Success)
				{
					user.ImagePath = model.CurrentImage;
					_cookiesService.Store(user);
					_fileUploadService.Delete(iRes.Data);
				}
			}
			TempData.SetData(uploadResult.Success ? AlertLevel.Success : AlertLevel.Warning, tag, uploadResult.Message);
			return RedirectToAction(nameof(Edit), new { id = model.UserId });
		}

		public IActionResult SetPassword(string id)
		{
			return View(new ResetPasswordViewModel { Id = id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SetPassword(ResetPasswordViewModel model)
		{
			var tag = "Set Password";
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				TempData.SetData(AlertLevel.Warning, tag, "User not Found");
				_logger.LogInformation($"Unable to load user with ID '{model.Id}'.");
				return View(model);
			}

			var result = await _userManager.AddPasswordAsync(user, model.Password);
			if (result.Succeeded)
				TempData.SetData(AlertLevel.Success, tag, "Password set");
			else
				AddErrors(result);
			return RedirectToAction(nameof(Edit), new { model.Id });
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}
	}
}
