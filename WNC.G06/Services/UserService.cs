using Microsoft.EntityFrameworkCore;
using WNC.G06.Models;
using WNC.G06.Models.Repository;

namespace WNC.G06.Services
{
    public class UserService
    {
        private readonly DataContext _context;

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
