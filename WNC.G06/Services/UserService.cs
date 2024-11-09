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
    }
}
