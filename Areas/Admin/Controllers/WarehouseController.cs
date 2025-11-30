using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;
using DocumentFormat.OpenXml.InkML;
using System.IO;
using System.IO.Packaging;
using System.Threading.Tasks;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên, Sales")]
    [Area("Admin")]
    public class WarehouseController : Controller
    {
        private readonly DataContext _dbContext;
        public WarehouseController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ExportToExcel_I(string type = "import")
        {
            var data = await _dbContext.WarehouseTransactions
             .Include(t => t.Product)
             .Include(t => t.Supplier) // include Supplier
             .OrderByDescending(t => t.TransactionDate)
             .Where(t => t.TransactionType == type)
             .ToListAsync();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách ");

            // Header
            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Sản phẩm";
            worksheet.Cell(1, 3).Value = "Số lượng";
            worksheet.Cell(1, 4).Value = "Ngày giao dịch";
            worksheet.Cell(1, 5).Value = "Ghi chú";
            worksheet.Cell(1, 6).Value = "Tình trạng";
            worksheet.Cell(1, 7).Value = "Nhà cung cấp";

            // Data
            int row = 2;
            int stt = 1;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = stt++;
                worksheet.Cell(row, 2).Value = item.Product?.ProductName;
                worksheet.Cell(row, 3).Value = item.Quantity;
                worksheet.Cell(row, 4).Value = item.TransactionDate.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 5).Value = item.Notes;
                worksheet.Cell(row, 6).Value = item.TransactionType;
                worksheet.Cell(row, 7).Value = item.Supplier?.Name;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"DanhSach_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        public async Task<IActionResult> ExportToExcel_E(string type = "Export")
        {
            var data = await _dbContext.WarehouseTransactions
             .Include(t => t.Product)
             .OrderByDescending(t => t.TransactionDate)
             .Where(t => t.TransactionType == type)
             .ToListAsync();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách ");

            // Header
            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Sản phẩm";
            worksheet.Cell(1, 3).Value = "Số lượng";
            worksheet.Cell(1, 4).Value = "Ngày giao dịch";
            worksheet.Cell(1, 5).Value = "Ghi chú";
            worksheet.Cell(1, 6).Value = "Tình trạng";

            // Data
            int row = 2;
            int stt = 1;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = stt++;
                worksheet.Cell(row, 2).Value = item.Product?.ProductName;
                worksheet.Cell(row, 3).Value = item.Quantity;
                worksheet.Cell(row, 4).Value = item.TransactionDate.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 5).Value = item.Notes;
                worksheet.Cell(row, 6).Value = item.TransactionType;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"DanhSach_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ImportIndex()
        {
            var imports = await _dbContext.WarehouseTransactions
                .Include(t => t.Product)
                .Include(t => t.Supplier)
                .Where(t => t.TransactionType == "Import")
                .OrderByDescending(t => t.TransactionDate)
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
            ViewBag.Suppliers = new SelectList(_dbContext.Supplier, "Id", "Name");
            ViewBag.Products = productList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(WarehouseTransactionModel model)
        {
            ModelState.Remove("TransactionType");
            ModelState.Remove("Supplier");
            ModelState.Remove("Username");
            ModelState.Remove("Product");
            if (ModelState.IsValid)
            {
                model.TransactionType = "Import";
                model.TransactionDate = DateTime.Now;
                model.Username = User.Identity?.Name ?? "Không rõ";
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
            ViewBag.Suppliers = new SelectList(_dbContext.Supplier, "Id", "Name");
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
            ModelState.Remove("Supplier");      // ← THÊM DÒNG NÀY
            ModelState.Remove("SupplierId");    // ← THÊM DÒNG NÀY
            ModelState.Remove("Username");

            if (ModelState.IsValid)
            {
                model.TransactionType = "Export";
                model.TransactionDate = DateTime.Now;
                model.Username = User.Identity?.Name ?? "Không rõ";
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
