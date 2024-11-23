using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text.Json;
using WNC.G06.Models;
using WNC.G06.Models.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //Trang chủ sau khi đăng nhập bằng admin
        public async Task<IActionResult> Index()
        {
            if (!CheckAccess(_adminPermission))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var users = await _dataContext.Users
                .Where(u => u.PermissionID != 1)
                .ToListAsync();
            return View(users);
        }

        //Kiểm tra quyền người dùng
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

        //Chi tiết thông tài khoản
        [Route("AccountDetail/{id}")]
        public async Task<IActionResult> AccountDetail(int id)
        {
            if (!CheckAccess(_adminPermission))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserID == id);
            return View(user);
        }

        //Xóa bên phía controller
        [HttpPost]
        [Route("Account/Delete")]
        public async Task<IActionResult> DeleteUser([FromBody] JsonElement data)
        {
            int id = data.GetProperty("UserID").GetInt32();
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserID == id);
            if (user != null)
            {
                user.Status = false;
                _dataContext.SaveChanges();

                int activeUserCount = await _dataContext.Users
                    .Where(u => u.PermissionID != 1)
                    .CountAsync(x => x.Status == true);

                return Json(new { success = true, activeUserCount });
            }
            return Json(new { success = false });
        }

        //Khôi phục bên phía controller
        [HttpPost]
        [Route("Account/Active")]
        public async Task<IActionResult> ActiveUser([FromBody] JsonElement data)
        {
            int id = data.GetProperty("UserID").GetInt32();
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.UserID == id);
            if (user != null)
            {
                user.Status = true;
                _dataContext.SaveChanges();

                int activeUserCount = await _dataContext.Users
                    .Where(u => u.PermissionID != 1)
                    .CountAsync(x => x.Status == true);

                return Json(new { success = true, activeUserCount });
            }
            return Json(new { success = false });
        }

        public async Task<IActionResult> PaymentList()
        {
            if (!CheckAccess(_adminPermission))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var payments = await
                (from user in _dataContext.Users.Where(x => x.PermissionID != 1)
                 join payment in _dataContext.Payments
                 on user.UserID equals payment.UserID into userPayments
                 from payment in userPayments.DefaultIfEmpty()
                 where payment == null ||
                       (payment.Date.Month == currentMonth && payment.Date.Year == currentYear)
                 select new
                 {
                     user.UserID,
                     Date = DateTime.Today,
                     user.UserName,
                     Amount = payment != null ? payment.Amount : 0
                 }).ToListAsync();

            return View(payments);
        }

        [HttpGet]
        [Route("PaymentDetail/{id}")]
        public async Task<IActionResult> PaymentDetail(int id)
        {
            if (!CheckAccess(_adminPermission))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var listPayments = await _dataContext.Payments
                .Where(x => x.UserID == id)
                .ToListAsync();

            return View(listPayments);
        }
    }
}
