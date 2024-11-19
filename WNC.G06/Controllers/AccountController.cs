using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WNC.G06.Services;
using WNC.G06.Models;
using Microsoft.EntityFrameworkCore;
using WNC.G06.Models.Repository;

namespace WNC.G06.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserService _userService;


        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.AuthenticateAsync(username, password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Role, user.Permission.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity));

                // Điều hướng theo vai trò
                switch (user.Permission.Role.ToLower())
                {
                    case "admin":
                        return RedirectToAction("Index", "Admin");
                    case "manager":
                        return RedirectToAction("Index", "Manager");
                    case "staff":
                        return RedirectToAction("Index", "Staff");
                    default:
                        return RedirectToAction("AccessDenied", "Home");
                }
            }

            TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
            try
            {
                var user = await _userService.Register(model.UserName, model.Password, model.Email);
                TempData["InfoMessage"] = "Đăng ký thành công, vui lòng đăng nhập";
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
