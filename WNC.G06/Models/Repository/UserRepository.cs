using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WNC.G06.Models.Repository
{
    public class UserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(UserModel user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); 
        }
        public IEnumerable<UserModel> GetAllUsers()
        {
            return _context.Users.Include(u => u.Permission).ToList();
        }
        public IEnumerable<PermissionsModel> GetUserPermission()
        {
            return _context.Permissions.Where(p => p.Role == "manager").ToList();
        }

        public bool IsUsernameExist(string username)
        {
            return _context.Users.Any(u => u.UserName == username);
        }
    }
}
