// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using PosMaster.Dal;
// using PosMaster.Dal.Interfaces;
// using PosMaster.Extensions;
// using PosMaster.Services;
// using PosMaster.ViewModels;

// namespace PosMaster.Controllers
// {
//     [Authorize]
//     public class ItemsController : Controller
//     {
//         private readonly IItemRegisterInterface _itemRegisterInterface;
//         private readonly ICookiesService _cookieService;
//         private readonly FileUploadService _fileUploadService;

//         public ItemsController(IItemRegisterInterface itemRegisterInterface, ICookiesService cookiesService, FileUploadService fileUploadService)
//         {
//             _itemRegisterInterface = itemRegisterInterface;
//             _cookieService = cookiesService;
//             _fileUploadService = fileUploadService;
//         }

//         public async Task<IActionResult> Index()
//         {
//             var user = _cookieService.Read();
//             if (User.IsInRole(Role.SuperAdmin))
//             {
//                 var result = await _itemRegisterInterface.AllAsync();
//                 if (!result.Success)
//                     TempData.SetData(AlertLevel.Warning, "Items", result.Message);
//                 return View(result.Data);
//             }
//             if (User.IsInRole(Role.Manager) || User.IsInRole(Role.Admin))
//             {
//                 var result = await _itemRegisterInterface.ByClientIdAsync(user.ClientId);
//                 if (!result.Success)
//                     TempData.SetData(AlertLevel.Warning, "Items", result.Message);
//                 return View(result.Data);
//             }

//             if (User.IsInRole(Role.Clerk))
//             {
//                 var result = await _itemRegisterInterface.ByInstanceIdAsync(user.InstanceId);
//                 if (!result.Success)
//                     TempData.SetData(AlertLevel.Warning, "Items", result.Message);
//                 return View(result.Data);
//             }
//             return View(new List<Item>());
//         }
//         public async Task<IActionResult> Edit(Guid? id)
//         {
//             if (id == null)
//                 return View(new ItemViewModel { Status = EntityStatus.Active });

//             var result = await _itemRegisterInterface.ByIdAsync(id.Value);
//             if (!result.Success)
//             {
//                 TempData.SetData(AlertLevel.Warning, "Items", result.Message);
//                 return RedirectToAction(nameof(Index));
//             }

//             var model = new ItemViewModel(result.Data);
//             return View(model);
//         }
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(ItemViewModel model)
//         {
//             model.Personnel = User.Identity.Name;
//             var option = model.IsEditMode ? "Update" : "Add";
//             var title = $"{option} Item";
//             if (!ModelState.IsValid)
//             {
//                 var message = "Missing fields";
//                 TempData.SetData(AlertLevel.Warning, title, message);
//                 return View(model);
//             }
//             var userData = _cookieService.Read();
//             model.ClientId = userData.ClientId;
//             if (model.InstancesIdStr.Any())
//             {
//                 var instancesId = new List<Guid>();
//                 foreach (var instanceId in model.InstancesIdStr)
//                 {
//                     instancesId.Add(Guid.Parse(instanceId));
//                 }
//                 model.InstancesId = instancesId;
//             }
//             else
//             {
//                 model.InstancesId = new List<Guid>();
//             }
//             if (model.ItemNature.Equals(ItemNature.Product))
//             {
//                 if (model.File != null)
//                 {
//                     var uploadResult = await _fileUploadService.UploadAsync(model.ClientId, model.File);
//                     if (!uploadResult.Success)
//                     {
//                         ModelState.AddModelError("File", $"Upload Failed :-{uploadResult.Message}");
//                         return View(model);
//                     }
//                     model.IsNewImage = true;
//                     model.ImagePath = uploadResult.PathUrl;
//                 }

//             }
//             var result = await _itemRegisterInterface.EditAsync(model);
//             TempData.SetData(result.Success ? AlertLevel.Success : AlertLevel.Warning, title, result.Message);
//             if (!result.Success)
//                 return View(model);
//             return RedirectToAction(nameof(Index));
//         }
//     }
// }