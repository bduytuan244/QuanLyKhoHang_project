using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        private readonly DataContext _dbContext;

        public StatisticsController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            // Tổng quan
            ViewBag.TotalProducts = await _dbContext.Products.CountAsync();
            ViewBag.TotalImports = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Import").CountAsync();
            ViewBag.TotalExports = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Export").CountAsync();
            ViewBag.TotalSuppliers = await _dbContext.Supplier.CountAsync();

            // Tổng số lượng nhập/xuất
            ViewBag.TotalImportQuantity = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Import")
                .SumAsync(x => x.Quantity);

            ViewBag.TotalExportQuantity = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Export")
                .SumAsync(x => x.Quantity);

            // Tồn kho hiện tại
            ViewBag.TotalStock = await _dbContext.Products.SumAsync(x => x.ProductQuantity);

            // Top 5 sản phẩm nhập nhiều nhất
            var topImports = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Import")
                .Include(x => x.Product)
                .GroupBy(x => new { x.ProductId, x.Product.ProductName })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();
            ViewBag.TopImports = topImports;

            // Top 5 sản phẩm xuất nhiều nhất
            var topExports = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Export")
                .Include(x => x.Product)
                .GroupBy(x => new { x.ProductId, x.Product.ProductName })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();
            ViewBag.TopExports = topExports;

            // Top 5 nhà cung cấp cung cấp nhiều nhất
            var topSuppliers = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionType == "Import" && x.SupplierId != null)
                .Include(x => x.Supplier)
                .GroupBy(x => new { x.SupplierId, x.Supplier.Name })
                .Select(g => new
                {
                    SupplierName = g.Key.Name,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();
            ViewBag.TopSuppliers = topSuppliers;

            // Sản phẩm sắp hết hàng (< 10)
            var lowStock = await _dbContext.Products
                .Where(x => x.ProductQuantity < 10)
                .OrderBy(x => x.ProductQuantity)
                .Take(10)
                .ToListAsync();
            ViewBag.LowStock = lowStock;

            // Thống kê theo tháng (6 tháng gần nhất)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyStats = await _dbContext.WarehouseTransactions
                .Where(x => x.TransactionDate >= sixMonthsAgo)
                .GroupBy(x => new {
                    Year = x.TransactionDate.Year,
                    Month = x.TransactionDate.Month,
                    Type = x.TransactionType
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Type = g.Key.Type,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
            ViewBag.MonthlyStats = monthlyStats;

            return View();
        }
    }
}
