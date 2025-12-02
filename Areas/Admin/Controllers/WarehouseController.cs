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

        // 1. Thêm tham số searchString
        public async Task<IActionResult> ImportIndex(string searchString)
        {
            // Lưu lại từ khóa để hiển thị lại trên ô tìm kiếm
            ViewData["CurrentFilter"] = searchString;

            // Bước 1: Lấy Query cơ bản (Chưa thực thi)
            var imports = _dbContext.WarehouseTransactions
                .Include(t => t.Product)
                .Include(t => t.Supplier)
                .Where(t => t.TransactionType == "Import");

            // Bước 2: Áp dụng bộ lọc nếu có từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                // Chuyển về chữ thường để tìm kiếm không phân biệt hoa thường (tùy chọn)
                // Ở đây mình dùng Contains trực tiếp vì EF Core tự xử lý
                imports = imports.Where(t =>
                    t.Product.ProductName.Contains(searchString) || // Tìm theo tên SP
                    t.Supplier.Name.Contains(searchString) ||       // Tìm theo NCC
                    t.Username.Contains(searchString) ||            // Tìm theo người làm
                    t.TransactionDate.ToString().Contains(searchString) // Tìm theo ngày (cơ bản)
                );
            }

            // Bước 3: Sắp xếp và Thực thi
            var result = await imports
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(result);
        }
        // Thêm tham số searchString
        public async Task<IActionResult> ExportIndex(string searchString)
        {
            // Lưu lại từ khóa để hiện lại trên ô tìm kiếm
            ViewData["CurrentFilter"] = searchString;

            // Bước 1: Tạo Query cơ bản
            var exports = _dbContext.WarehouseTransactions
                .Include(t => t.Product)
                // .Include(t => t.Supplier) // Xuất kho không cần Supplier
                .Where(t => t.TransactionType == "Export");

            // Bước 2: Lọc dữ liệu nếu có từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                exports = exports.Where(t =>
                    t.Product.ProductName.Contains(searchString) || // Tìm theo Tên SP
                    t.Username.Contains(searchString) ||            // Tìm theo Người thực hiện
                    t.Notes.Contains(searchString) ||               // Tìm theo Ghi chú
                    t.TransactionDate.ToString().Contains(searchString) // Tìm theo Ngày
                );
            }

            // Bước 3: Sắp xếp và Lấy dữ liệu
            var result = await exports
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(result);
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
            ModelState.Remove("Supplier");
            ModelState.Remove("SupplierId");
            ModelState.Remove("Username");

            if (ModelState.IsValid)
            {
                // KIỂM TRA SỐ LƯỢNG TỒN KHO TRƯỚC KHI XUẤT
                var product = await _dbContext.Products.FindAsync(model.ProductId);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
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

                // KIỂM TRA SỐ LƯỢNG XUẤT > SỐ LƯỢNG TỒN
                if (model.Quantity > product.ProductQuantity)
                {
                    TempData["ErrorMessage"] = $"Không thể xuất {model.Quantity} {product.ProductUnit}! Tồn kho hiện tại chỉ có {product.ProductQuantity} {product.ProductUnit}.";
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

                // NẾU HỢP LỆ THÌ XUẤT KHO
                model.TransactionType = "Export";
                model.TransactionDate = DateTime.Now;
                model.Username = User.Identity?.Name ?? "Không rõ";
                _dbContext.WarehouseTransactions.Add(model);

                product.ProductQuantity -= model.Quantity;
                _dbContext.Products.Update(product);

                TempData["SuccessMessage"] = $"Xuất kho thành công {model.Quantity} {product.ProductUnit} {product.ProductName}!";
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Export", "Warehouse");
            }

            var products = _dbContext.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (Tồn: {p.ProductQuantity})"
                })
                .ToList();
            ViewBag.Products = products;
            return View(model);
        }

    }
}
