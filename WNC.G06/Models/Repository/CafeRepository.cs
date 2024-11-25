using Microsoft.EntityFrameworkCore;
using System;

namespace WNC.G06.Models.Repository
{
    public class CafeRepository
    {
        private readonly DataContext _context;
        public bool Success { get; set; }
        public Dictionary<string, string> Errors { get; set; }
        public CafeRepository(DataContext context)
        {
            _context = context;
            Success = false;
            Errors = new Dictionary<string, string>();
        }
        // Lấy thông tin danh sách cửa hàng dựa trên UserID người dùng đăng nhập
       public IEnumerable<CafeModel> GetCafesByUserId(int userId)
        {
            return _context.Cafes.Where(c => c.UserID == userId && c.Status).ToList();
        }


       // Kiểm tra tên cửa hàng đã tồn tại hay chưa
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
