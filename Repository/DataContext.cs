using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<ProductModel> Products { get; set; }

        public DbSet<WarehouseTransactionModel> WarehouseTransactions { get; set; }

        public DbSet<SupplierModel> Supplier { get; set; }
        public DbSet<ExportOrder> ExportOrders { get; set; }
        public DbSet<ExportOrderDetail> ExportOrderDetails { get; set; }

    }
}
