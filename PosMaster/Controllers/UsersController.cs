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
		public UsersController(UserManager<User> userManager, IEmailService emailService, ILogger<UsersController> logger,
			  IClientInstanceInterface clientInstanceInterface, IUserInterface userInterface, ICookiesService cookiesService)
		{
			_userManager = userManager;
			_emailService = emailService;
			_logger = logger;
			_clientInstanceInterface = clientInstanceInterface;
			_userInterface = userInterface;
			_cookiesService = cookiesService;
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
				_logger.LogInformation($"edit user : provided user {id} not found");
			}
			return View(new UserViewModel(user));
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
				{
					ModelState.AddModelError(string.Empty, $"Update failed");
					TempData.SetData(AlertLevel.Warning, $"{tag}", updateRes.Message);
				}
			}
			var user = new User
			{
				Status = EntityStatus.Active,
				ClientId = model.ClientId,
				InstanceId = model.Id,
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
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
			await _emailService.SendEmailConfirmationAsync(new EmailAddress(user), callbackUrl);
			return View(model);

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

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}
	}
}
