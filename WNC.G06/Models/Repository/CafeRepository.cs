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
        // Cập nhật thông tin cửa hàng
        public async Task UpdateCafeAsync(CafeModel cafe)
        {
            _context.Cafes.Update(cafe); // Marks the cafe entity as modified
            await _context.SaveChangesAsync();
        }

        // Xóa cửa hàng dựa trên CafeID
        public async Task DeleteCafeAsync(int cafeId)
        {
            var cafe = await _context.Cafes.FindAsync(cafeId);
            if (cafe != null)
            {
                _context.Cafes.Remove(cafe);
                await _context.SaveChangesAsync();
            }
        }

        // Lấy thông tin cửa hàng theo CafeID
        public async Task<CafeModel> GetCafeByIdAsync(int cafeId)
        {
            return await _context.Cafes
                                 .FirstOrDefaultAsync(c => c.CafeID == cafeId);
        }

        // Lấy tất cả cửa hàng
        public async Task<IEnumerable<CafeModel>> GetAllCafesAsync()
        {
            return await _context.Cafes
                                 .Where(c => c.Status) // Only active cafes
                                 .ToListAsync();
        }

        // Kiểm tra nếu tên cửa hàng đã tồn tại, ngoại trừ với CafeID đã cho (dùng cho chức năng cập nhật)
        public bool IsCafenameExistExcept(string cafeName, int cafeId)
        {
            return _context.Cafes
                           .Any(u => u.CafeName.Equals(cafeName, StringComparison.OrdinalIgnoreCase) && u.CafeID != cafeId);
        }
    }
}
