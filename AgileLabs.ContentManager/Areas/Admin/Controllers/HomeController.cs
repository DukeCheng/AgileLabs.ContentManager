using System;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        [HttpGet]
        //[ResponseCache(Duration = 3600)]
        public IActionResult Index()
        {
            //var actionName = nameof(PagesController.Index);
            //var controllerName = nameof(PagesController).EndsWith("Controller");
            //return RedirectToAction(actionName, controllerName);
            return View();
        }
    }
}
