using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WNC.G06.Models;
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

            var users = await _dataContext.Users
                .Where(u => u.PermissionID != 1)
                .ToListAsync();
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

        [Route("AccountDetail/{id}")]
        public async Task<IActionResult> AccountDetail(int id)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserID == id);
            return View(user);
        }

        [HttpPost]
        [Route("Account/Delete")]
        public async Task<IActionResult> DeleteUser([FromBody] JsonElement data)
        {
            int id = data.GetProperty("UserID").GetInt32();
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserID == id);
            if (user != null)
            {
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> DisableUser(int id)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Status = false;
            _dataContext.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> ActiveUser(int id)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Status = true;
            _dataContext.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }
    }
}
