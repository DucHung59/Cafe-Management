using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WNC.G06.Models;
using WNC.G06.Models.Repository;

namespace WNC.G06.Services
{
    public class UserService
    {
        private readonly DataContext _context;
        private const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$";

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<UserModel> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Permission)
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password && u.Status);

            return user;
        }

        public async Task<UserModel> Register(string username, string password, string email)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username || u.Email == email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username hoặc email đã được sử dụng.");
            }

            if (!Regex.IsMatch(email, emailPattern))
            {
                throw new InvalidOperationException("Email không đúng định dạng");
            }

            var user = new UserModel
            {
                UserName = username,
                Password = password,
                Email = email,
                Status = true,
                PermissionID = 2
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Status = user.Status,
                PermissionID = user.PermissionID
            };
        }
    }
}
