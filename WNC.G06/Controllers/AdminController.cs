using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WNC.G06.Models.Repository;

namespace WNC.G06.Controllers
{

    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly string _adminPermission = "admin";

        public AdminController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task<IActionResult> Index()
        {
            if(!CheckAccess(_adminPermission))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var users = await _dataContext.Users.ToListAsync();
            return View(users);
        }

        private bool CheckAccess(string Permission)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            if (User.FindFirst(ClaimTypes.Role)?.Value != Permission)
            {
                return false;
            }

            return true;
        }
    }
}
