using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên, Sales")]
    [Area("Admin")]
    public class Dashboard : Controller
    {
        private readonly DataContext _dbContext;

        public Dashboard(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            // Đếm tổng số
            ViewBag.TotalProducts = await _dbContext.Products.CountAsync();
            ViewBag.TotalImports = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Import").CountAsync();
            ViewBag.TotalExports = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Export").CountAsync();
            ViewBag.TotalSuppliers = await _dbContext.Supplier.CountAsync();

            // Lấy 5 giao dịch gần nhất
            var recentTransactions = await _dbContext.WarehouseTransactions
                .Include(x => x.Product)
                .Include(x => x.Supplier)
                .OrderByDescending(x => x.TransactionDate)
                .Take(5)
                .ToListAsync();

            return View(recentTransactions);
        }
    }
}
