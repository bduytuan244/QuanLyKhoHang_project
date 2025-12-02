using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Bắt buộc đăng nhập
    public class ExportOrderController : Controller
    {
        private readonly DataContext _context;

        public ExportOrderController(DataContext context)
        {
            _context = context;
        }

        // 1. Danh sách đơn hàng
        // 1. Danh sách đơn hàng (Có tìm kiếm)
        public async Task<IActionResult> Index(string searchString) // Thêm tham số searchString
        {
            // Lưu từ khóa để hiện lại
            ViewData["CurrentFilter"] = searchString;

            // Bước 1: Query cơ bản
            var ordersQuery = _context.ExportOrders
                .Include(o => o.Details)
                .AsQueryable(); // Để dễ nối chuỗi lệnh

            // Bước 2: Lọc dữ liệu
            if (!string.IsNullOrEmpty(searchString))
            {
                ordersQuery = ordersQuery.Where(o =>
                    o.OrderCode.Contains(searchString) ||           // Tìm theo Mã đơn
                    o.CreatedBy.Contains(searchString) ||           // Tìm theo Người tạo
                    o.CreatedDate.ToString().Contains(searchString) // Tìm theo Ngày (dạng chuỗi)
                );
            }

            // Bước 3: Sắp xếp và Lấy dữ liệu
            var orders = await ordersQuery
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(orders);
        }
        // 2. Xem chi tiết 1 đơn
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.ExportOrders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product) // Kèm thông tin sản phẩm
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // 3. Mở giao diện tạo mới
        [HttpGet]
        public IActionResult Create()
        {
            LoadProductList(); // Load danh sách sản phẩm vào ViewBag
            return View();
        }

        // 4. Lưu đơn hàng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExportOrder order)
        {
            // Tự động sinh mã đơn (Ví dụ: OUT-20251130-1200)
            order.OrderCode = $"OUT-{DateTime.Now:yyyyMMdd-HHmmss}";
            order.CreatedDate = DateTime.Now;
            // Tự động lấy tên người đăng nhập, nếu lỗi thì lấy "Admin"
            order.CreatedBy = User.Identity?.Name ?? "Admin";
            order.Status = "New"; // Trạng thái ban đầu

            // --- [QUAN TRỌNG] BỎ QUA CÁC LỖI VALIDATION KHÔNG CẦN THIẾT ---

            // 1. Bỏ qua các trường tự sinh
            ModelState.Remove("OrderCode");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Status");

            // 2. Bỏ qua lỗi của danh sách chi tiết (Do Product và ExportOrder bị null)
            // Lệnh này cực kỳ quan trọng để fix lỗi không lưu được
            ModelState.Remove("Details");

            // Duyệt qua tất cả các lỗi còn lại, nếu lỗi nào liên quan đến Product/ExportOrder thì xóa nốt
            foreach (var key in ModelState.Keys)
            {
                if (key.Contains("Product") || key.Contains("ExportOrder"))
                {
                    ModelState.Remove(key);
                }
            }
            // -------------------------------------------------------------

            if (ModelState.IsValid)
            {
                // Kiểm tra xem có sản phẩm nào được chọn không
                if (order.Details == null || order.Details.Count == 0)
                {
                    ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm!");
                }
                else
                {
                    _context.ExportOrders.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Lưu xong quay về danh sách
                }
            }

            // Nếu lỗi thì load lại danh sách sản phẩm để hiện lại form
            LoadProductList();
            return View(order);
        }

        // Hàm phụ để load danh sách sản phẩm (tránh viết lặp lại)
        private void LoadProductList()
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (Tồn: {p.ProductQuantity})"
                }).ToList();
        }
    }
}