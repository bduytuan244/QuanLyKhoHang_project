using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;
using System.Threading.Tasks;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class WarehouseController : Controller
    {
        private readonly DataContext _dbContext;
        public WarehouseController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> ImportIndex()
        {
            var imports = await _dbContext.WarehouseTransactions
                .Include(t => t.Product)
                .Where(t => t.TransactionType == "Import")
                .OrderByDescending(t=> t.TransactionDate)
                .ToListAsync();

            return View(imports);
        }
        public async Task<IActionResult> ExportIndex()
        {
            var exports = await _dbContext.WarehouseTransactions
                .Include(t => t.Product)
                .Where(t => t.TransactionType == "Export")
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(exports);
        }

        [HttpGet]
        public IActionResult Import()
        {
            var productList = _dbContext.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (Tồn: {p.ProductQuantity})"
                })
                .ToList();

            ViewBag.Products = productList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(WarehouseTransactionModel model)
        {
            ModelState.Remove("TransactionType");
            ModelState.Remove("Product");
            if (ModelState.IsValid)
            {
                model.TransactionType = "Import";
                model.TransactionDate = DateTime.Now;
                _dbContext.WarehouseTransactions.Add(model);
                var product = await _dbContext.Products.FindAsync(model.ProductId);
                if (product != null)
                {
                    product.ProductQuantity += model.Quantity;
                    _dbContext.Products.Update(product);
                }
                TempData["SuccessMessage"] = "Nhập kho thành công!";
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Import", "Warehouse");
            }
            var productList = _dbContext.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (Tồn: {p.ProductQuantity})"
                })
                .ToList();

            ViewBag.Products = productList;
            return View(model);
        }
        [HttpGet]
        public IActionResult Export()
        {
            var productList = _dbContext.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (Tồn: {p.ProductQuantity})"
                })
                .ToList();

            ViewBag.Products = productList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(WarehouseTransactionModel model)
        {
            ModelState.Remove("TransactionType");
            ModelState.Remove("Product");
            if (ModelState.IsValid)
            {
                model.TransactionType = "Export";
                model.TransactionDate = DateTime.Now;

                _dbContext.WarehouseTransactions.Add(model);

                var product = await _dbContext.Products.FindAsync(model.ProductId);
                if (product != null)
                {
                    product.ProductQuantity -= model.Quantity;
                    _dbContext.Products.Update(product);
                }
                TempData["SuccessMessage"] = "Xuất kho thành công!";
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Export", "Warehouse");
            }
            var productList = _dbContext.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (Tồn: {p.ProductQuantity})"
                })
                .ToList();

            ViewBag.Products = productList;
            return View(model);
        }
    }
}
