using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private UserManager<User> _userManager;
        public DashboardController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
                return RedirectToAction("Index", "Home");
            if (user.Role.Equals(Role.SuperAdmin))
                return RedirectToAction(nameof(SuperAdmin));  
            
            if (user.Role.Equals(Role.Manager))
                return RedirectToAction(nameof(Manager), new { user.ClientId});
            
            if (user.Role.Equals(Role.Admin))
                return RedirectToAction(nameof(Admin), new { user.ClientId });
            
            if (user.Role.Equals(Role.Clerk))
                return RedirectToAction(nameof(Clerk), new { user.InstanceId });

            return RedirectToAction(nameof(Clerk), new { user.InstanceId });
        }
        public IActionResult SuperAdmin() 
        {
            return View();
        }        
        public IActionResult Admin(Guid clientId) 
        {
            return View();
        }        
        public IActionResult Manager(Guid clientId) 
        {
            return View();
        }        
        public IActionResult Clerk(Guid instanceId) 
        {
            return View();
        }        
    }
}
