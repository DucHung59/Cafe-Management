using Microsoft.EntityFrameworkCore;

namespace WNC.G06.Models.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<PermissionsModel> Permissions { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CafeModel> Cafes { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ giữa UserModel và PermissionsModel
            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.Permission)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PermissionID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ giữa CafeModel và UserModel
            modelBuilder.Entity<CafeModel>()
                .HasOne(c => c.User)
                .WithMany(u => u.Cafes)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ giữa ProductModel và CafeModel
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Cafe)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CafeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ giữa OrderModel và CafeModel
            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.Cafe)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CafeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ giữa OrderDetailModel và OrderModel
            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ giữa OrderDetailModel và ProductModel
            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ giữa PaymentModel và UserModel
            modelBuilder.Entity<PaymentModel>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
