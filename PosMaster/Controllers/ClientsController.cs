using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
	public class ClientsController : Controller
	{
		private readonly IClientInterface _clientInterface;
		public ClientsController(IClientInterface clientInterface)
		{
			_clientInterface = clientInterface;
		}
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
				return View(new ClientViewModel { Status = EntityStatus.Active });
			var result = await _clientInterface.ByIdAsync(id.Value);
			if (!result.Success)
			{

			}
			return View(new ClientViewModel(result.Data));
		}
	}
}
