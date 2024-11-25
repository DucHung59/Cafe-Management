using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WNC.G06.Models;

namespace WNC.G06.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            switch (User.FindFirst(ClaimTypes.Role)?.Value)
            {
                case "admin":
                    return RedirectToAction("Index", "Admin");
                case "manager":
                    return RedirectToAction("Index", "Manager");
                case "staff":
                    return RedirectToAction("Index", "Staff");
                default:
                    return View();
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
