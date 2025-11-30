using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Để dùng JWT
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Controllers
{
    // Đường dẫn sẽ là: .../api/WarehouseTransactionApi
    [Route("api/[controller]")]
    [ApiController]
    // BẮT BUỘC: Phải có Token từ App mới được gọi
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WarehouseTransactionApiController : ControllerBase // Dùng ControllerBase cho API
    {
        private readonly DataContext _context;

        public WarehouseTransactionApiController(DataContext context)
        {
            _context = context;
        }

        // 1. API Ghi lịch sử (App gọi cái này khi Nhập/Xuất thành công)
        // POST: api/WarehouseTransactionApi
        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] WarehouseTransactionModel model)
        {
            if (model == null) return BadRequest("Dữ liệu trống");

            try
            {
                // 1. Tự động điền ngày giờ nếu thiếu
                if (model.TransactionDate == DateTime.MinValue)
                {
                    model.TransactionDate = DateTime.Now;
                }

                // 2. TỰ ĐỘNG ĐIỀN USERNAME TỪ TOKEN (Quan trọng!)
                // Lấy tên từ Token mà App Android gửi kèm
                model.Username = User.Identity?.Name ?? "App Mobile User";

                // 3. Xử lý logic SupplierId = null nếu bằng 0 (để tránh lỗi khóa ngoại)
                if (model.SupplierId == 0) model.SupplierId = null;

                // 4. Lưu vào Database
                _context.WarehouseTransactions.Add(model);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã lưu lịch sử thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi Server: " + ex.Message);
            }
        }

        // 2. API Lấy lịch sử (Nếu sau này bạn muốn hiển thị lịch sử lên App)
        // GET: api/WarehouseTransactionApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseTransactionModel>>> GetHistory()
        {
            return await _context.WarehouseTransactions
                                 .Include(t => t.Product) // Kèm thông tin sản phẩm
                                 .OrderByDescending(t => t.TransactionDate) // Mới nhất lên đầu
                                 .ToListAsync();
        }
    }
}