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
	public class ClientsController : Controller
	{
		private readonly IClientInterface _clientInterface;
		private readonly FileUploadService _fileUploadService;
		public ClientsController(IClientInterface clientInterface, FileUploadService fileUploadService)
		{
			_clientInterface = clientInterface;
			_fileUploadService = fileUploadService;
		}
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
				return View(new ClientViewModel { Status = EntityStatus.Active });
			var result = await _clientInterface.ByIdAsync(id.Value);
			if (!result.Success)
			{
				TempData.SetData(AlertLevel.Warning, "Clients", result.Message);
				return RedirectToAction(nameof(All));
			}
			var model = result.Success ? new ClientViewModel(result.Data) : new ClientViewModel();
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ClientViewModel model)
		{
			var option = model.IsEditMode ? "Update" : "Add";
			var title = $"{option} Client";
			if (!ModelState.IsValid)
			{
				var message = "Missing fields";
				TempData.SetData(AlertLevel.Warning, title, message);
				return View(model);
			}
			var clientId = model.IsEditMode ? model.Id : Guid.NewGuid();
			if (model.File != null)
			{
				var upResult = await _fileUploadService.UploadAsync(clientId, model.File);
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
			model.Id = clientId;
			var result = await _clientInterface.EditAsync(model);
			TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
			if (!result.Success)
				return View(model);

			return RedirectToAction(nameof(All));
		}

		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> All()
		{
			var result = await _clientInterface.AllAsync();
			if (!result.Success)
				TempData.SetData(AlertLevel.Warning, "Clients", result.Message);
			return View(result.Data);
		}
		//public async Task<IActionResult> Summary()
		//{
		//    return View();
		//}
	}
}
