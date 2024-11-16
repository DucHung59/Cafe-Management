using System;

namespace WNC.G06.Models.Repository
{
    public class CafeRepository
    {
        private readonly DataContext _context;

        public CafeRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<CafeModel> GetCafesByUserId(int userId)
        {
            return _context.Cafes.Where(c => c.UserID == userId && c.Status).ToList();
        }

        public bool IsCafenameExist(string cafename)
        {
            return _context.Cafes.Any(u => u.CafeName == cafename);
        }

        public async Task AddCafeAsync(CafeModel cafe)
        {
            await _context.Cafes.AddAsync(cafe);
            await _context.SaveChangesAsync();
        }
    }
}
