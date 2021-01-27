using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using PosMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    public class PointOfSaleController : Controller
    {
        private readonly IProductInterface _productInterface;
        private readonly ICookiesService _cookieService;
        public PointOfSaleController(IProductInterface productInterface, ICookiesService cookieService)
        {
            _productInterface = productInterface;
            _cookieService = cookieService;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var user = _cookieService.Read();
        //    var result = await _productInterface.ByInstanceIdAsync(user.InstanceId);
        //    if (result.Success)
        //        TempData.SetData(AlertLevel.Warning, "Products", result.Message);
        //    return View(result.Data);
        //}
        public IActionResult Index() 
        {
            return View();
        }
    }
}
