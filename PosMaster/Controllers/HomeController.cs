using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Models;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IEmailService _emailService;
		private readonly IUserInterface _userInterface;
		private readonly IClientInstanceInterface _instanceInterface;
		private readonly ILogger<HomeController> _logger;
		private readonly ICookiesService _cookiesService;

		public HomeController(UserManager<User> userManager, SignInManager<User> signInManager,
		IEmailService emailService, IUserInterface userInterface, ILogger<HomeController> logger,
		IClientInstanceInterface instanceInterface, ICookiesService cookiesService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailService = emailService;
			_userInterface = userInterface;
			_logger = logger;
			_instanceInterface = instanceInterface;
			_cookiesService = cookiesService;
		}

		public IActionResult Index(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		public async Task<IActionResult> Login(string returnUrl = null)
		{
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
			_cookiesService.Remove();
			ViewData["ReturnUrl"] = returnUrl;
			_logger.LogInformation($"{nameof(Index)} application started");
			return View(new LoginViewModel
			{
				AuthenticationSchemes = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
			});
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			var log = new UserLoginLog
			{
				ReturnUrl = returnUrl,
				UserName = model.EmailAddress
			};
			try
			{
				log.Source = DataSource.Web;
				log.Agent = Request.Headers[HeaderNames.UserAgent];
				log.IsHttps = Request.IsHttps;
				log.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
				log.RefererUrl = Request.Headers[HeaderNames.Referer];
				if (!ModelState.IsValid)
				{
					var msg = "Invalid Request";
					TempData.SetData(AlertLevel.Warning, "Login Failed", msg);
					log.Notes = msg;
					await _userInterface.AddLoginLogAsync(log);
					return View(model);
				}

				var result = await _signInManager
					.PasswordSignInAsync(model.EmailAddress, model.Password, model.RememberMe,
					lockoutOnFailure: true);
				if (result.Succeeded)
				{
					var user = await _userManager.FindByEmailAsync(model.EmailAddress);
					log.UserRole = user.Role;
					log.Personnel = user.UserName;
					log.ClientId = user.ClientId;
					log.InstanceId = user.InstanceId;
					if (!user.EmailConfirmed)
					{
						var msg = "Your have not confirmed your email.";
						TempData.SetData(AlertLevel.Warning, "Login Failed", msg);
						log.Notes = msg;
						await _userInterface.AddLoginLogAsync(log);
						ModelState.AddModelError(string.Empty, $"{msg} Kindly check your inbox for the confirmation link.");
						await _signInManager.SignOutAsync();
						return View(model);
					}

					if (!user.Status.Equals(EntityStatus.Active))
					{
						var msg = $"Account is not Active - {user.Status} .";
						TempData.SetData(AlertLevel.Warning, "Login Failed", msg);
						log.Notes = msg;
						await _userInterface.AddLoginLogAsync(log);
						ModelState.AddModelError(string.Empty, $"{msg} Kindly contact admin for activation.");
						await _signInManager.SignOutAsync();
						return View(model);
					}

					var msg_ = "User logged in success";
					_logger.LogInformation(msg_);
					TempData.SetData(AlertLevel.Success, "Login Success", msg_);
					log.Success = true;
					log.Notes = msg_;
					await _userInterface.AddLoginLogAsync(log);
					await StoreCookiesDataAsync(user);
					return RedirectToLocal(returnUrl);
				}

				if (result.RequiresTwoFactor)
				{
					return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
				}

				if (result.IsLockedOut)
				{
					var msg_ = "User account locked out.";
					TempData.SetData(AlertLevel.Warning, "Login Failed", msg_);
					_logger.LogWarning(msg_);
					log.Notes = msg_;
					await _userInterface.AddLoginLogAsync(log);
					return RedirectToAction(nameof(Lockout));
				}
				else
				{
					var msg = "Invalid login attempt.";
					ModelState.AddModelError(string.Empty, msg);
					TempData.SetData(AlertLevel.Error, "Login Failed", msg);
					log.Notes = msg;
					await _userInterface.AddLoginLogAsync(log);
					return View(model);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_logger.LogError($"Error during log in :- {e.Message}");
				var eMsg = "Unexpected error occured. Try later.";
				ModelState.AddModelError(string.Empty, eMsg);
				TempData.SetData(AlertLevel.Error, "Login", eMsg);
				log.Notes = eMsg;
				await _userInterface.AddLoginLogAsync(log);
				return View(model);
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			var model = new LoginWith2faViewModel { RememberMe = rememberMe };
			ViewData["ReturnUrl"] = returnUrl;

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe,
		string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

			var result =
				await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe,
					model.RememberMachine);

			if (result.Succeeded)
			{
				_logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
				return RedirectToLocal(returnUrl);
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
				return RedirectToAction(nameof(Lockout));
			}
			else
			{
				_logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
				ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
				return View();
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Lockout()
		{
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			var personnel = User.Identity.Name;
			await _userInterface.LogOutAsync(personnel);
			await _signInManager.SignOutAsync();
			_logger.LogInformation($"User {personnel} logged out.");
			//TempData.SetData(AlertLevel.Success, "Logout", "You have logged out");
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction("Index");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				_logger.LogError($"Unable to load user with ID '{userId}'.");
				return View(nameof(Error));
			}

			var result = await _userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
			{
				return View(nameof(Error));
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);
			if (hasPassword)
				return View(nameof(ConfirmEmailConfirmation));
			return RedirectToAction(nameof(SetPassword), new { userId });
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ConfirmEmailConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult SetPassword(string userId)
		{
			return View(new ResetPasswordViewModel { Id = userId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> SetPassword(ResetPasswordViewModel model)
		{

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				_logger.LogInformation($"Unable to load user with ID '{model.Id}'.");
				return View(model);
			}

			var result = await _userManager.AddPasswordAsync(user, model.Password);
			if (result.Succeeded)
				return View(nameof(SetPasswordConfirmation));
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult SetPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.EmailAddress);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					return RedirectToAction(nameof(ForgotPasswordConfirmation));
				}

				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
				await _emailService.SendAsync(new EmailAddress(user), "Reset Password",
					$"Dear {user.FullName},<br/><br/>Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

				return View(model);
			}
			return View(model);
		}


		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string userId, string code)
		{
			if (code == null)
			{
				throw new ApplicationException("A code must be supplied for password reset.");
			}

			var model = new ResetPasswordViewModel { Code = code, Id = userId };
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}

			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}

			AddErrors(result);
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Terms()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


		[HttpPost]
		[AllowAnonymous]
		public IActionResult ExternalLogin(string provider, string returnUrl)
		{
			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Home", new { returnUrl });
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return new ChallengeResult(provider, properties);
		}

		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
		{
			var tag = "External Login";
			ViewData["ReturnUrl"] = returnUrl;
			var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			var model = new LoginViewModel { AuthenticationSchemes = externalLogins };
			if (!string.IsNullOrEmpty(remoteError))
			{
				TempData.SetData(AlertLevel.Error, tag, "Error occured. Try later");
				ModelState.AddModelError(string.Empty, $"Error from External provider : {remoteError}");
				return View("Login", model);
			}
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				TempData.SetData(AlertLevel.Warning, tag, "Failed to get External Info");
				ModelState.AddModelError(string.Empty, $"Error loading login information");
				return View("Login", model);
			}

			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
				isPersistent: false, bypassTwoFactor: true);
			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(email);
			if (result.Succeeded)
			{
				await StoreCookiesDataAsync(user);
				TempData.SetData(AlertLevel.Success, tag, "Success");
				_logger.LogInformation($"{email} logged in with google - exists");
				return RedirectToLocal(returnUrl);
			}
			if (user == null)
			{
				TempData.SetData(AlertLevel.Warning, tag, "Failed to get User");
				ModelState.AddModelError(string.Empty, $"Provided Email ({email}) is not registered");
				return View("Login", model);
			}
			await _userManager.AddLoginAsync(user, info);
			await _signInManager.SignInAsync(user, isPersistent: false);
			await StoreCookiesDataAsync(user);
			_logger.LogInformation($"{email} logged in with google - added to login");
			TempData.SetData(AlertLevel.Success, tag, "Success-Added");
			return RedirectToLocal(returnUrl);
		}

		#region Helpers 
		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private async Task StoreCookiesDataAsync(User user)
		{
			var userData = new UserCookieData(user);
			var instanceResult = await _instanceInterface.ByIdAsync(user.InstanceId);
			if (instanceResult.Success)
			{
				var instance = instanceResult.Data;
				userData.InstanceCode = instance.Code;
				userData.InstanceName = instance.Name;
				var client = instance.Client;
				userData.ClientCode = client.Code;
				userData.ClientName = client.Name;
				userData.ClientLogoPath = client.LogoPath;
			}
			_cookiesService.Store(userData);
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			else
				return RedirectToAction("Index", "Dashboard");
		}
		#endregion
	}
}
