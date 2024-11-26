using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text.Json;
using WNC.G06.Models;
using WNC.G06.Models.Repository;
using WNC.G06.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WNC.G06.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserService _userService;
        private readonly string _adminPermission = "admin";

        public AdminController(DataContext dataContext, UserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
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

            var payments = await _dataContext.Users
                .Where(user => user.PermissionID != 1)
                .Select(user => new
                {
                    user.UserID,
                    user.UserName,
                    Payments = _dataContext.Payments
                        .Where(p => p.UserID == user.UserID)
                        .ToList() 
                })
                .ToListAsync();

            var result = payments.Select(user => new
                {
                    user.UserID,
                    user.UserName,
                    Date = DateTime.Today,
                    Amount = user.Payments.Any(p => p.Date.Month == currentMonth && p.Date.Year == currentYear)
                        ? user.Payments.Where(p => p.Date.Month == currentMonth && p.Date.Year == currentYear).Sum(p => p.Amount)
                        : 0
                 }).ToList();

            return View(result);
        }

        [HttpGet]
        [Route("PaymentDetail/{id}")]
        public async Task<IActionResult> PaymentDetail(int id)
        {
            if (!CheckAccess(_adminPermission))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var listPayments = await
                (from p in _dataContext.Payments
                 join u in _dataContext.Users on p.UserID equals u.UserID
                 where p.UserID == id
                 select new
                 {
                     p.PaymentID,
                     u.UserName,
                     p.Date,
                     p.Amount
                 }).OrderByDescending(p => p.Date.Year)
                   .ThenByDescending(p => p.Date.Month)
                   .ToListAsync();

            return View(listPayments);
        }


        [HttpPost]
        public async Task<IActionResult> EditAccount(UserModel model)
        {
            if (!CheckAccess(_adminPermission)) { 
                return RedirectToAction("AccessDenied", "Home");
            }

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

            try
            {
                user.UserName = model.UserName;
                user.Password = model.Password;
                user.Email = model.Email;
                user.Status = model.Status;
                user.PermissionID = model.PermissionID;

                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
