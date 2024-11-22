using Microsoft.EntityFrameworkCore;

namespace WNC.G06.Models.Repository
{
    public class PaymentRepository
    {
        private readonly DataContext _context;
        public PaymentRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddPaymentAsync(PaymentModel payment)
        {
            // Thêm đối tượng PaymentModel vào DbSet Payments
            await _context.Payments.AddAsync(payment);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }

        // kiểm tra xem đã thanh toán hóa đơn trong tháng chưa

        public async Task<bool> HasPaymentForMonthAsync(int userId, int month, int year)
        {
            return await _context.Payments
                .AnyAsync(p => p.UserID == userId &&
                               p.Date.Month == month &&
                               p.Date.Year == year);
        }

    }
}
