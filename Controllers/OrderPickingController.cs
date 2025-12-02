using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderPickingController : ControllerBase
    {
        private readonly DataContext _context;

        public OrderPickingController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("start/{orderId}")]
        public async Task<IActionResult> StartPicking(int orderId)
        {
            var order = await _context.ExportOrders.FindAsync(orderId);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            if (order.Status != "New")
            {
                return BadRequest("Đơn hàng này đã có người khác nhận hoặc đã hoàn thành!");
            }

            // 1. Cập nhật trạng thái
            order.Status = "Picking";

            // 2. LƯU TÊN NGƯỜI LẤY HÀNG (Lấy từ Token)
            order.Picker = User.Identity?.Name ?? "Unknown Staff";
            // -----------------------------------------------

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bắt đầu soạn hàng!" });
        }
        // 1. Lấy danh sách các đơn đang chờ soạn hàng (Status = "New")
        // GET: api/OrderPicking/pending
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ExportOrder>>> GetPendingOrders()
        {
            var orders = await _context.ExportOrders
                .Where(o => o.Status == "New")
                .Include(o => o.Details) // <--- THÊM DÒNG NÀY ĐỂ LẤY CHI TIẾT
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
            return Ok(orders);
        }

        // 2. Lấy chi tiết một đơn hàng (để nhân viên biết cần lấy những món gì)
        // GET: api/OrderPicking/detail/5
        [HttpGet("detail/{orderId}")]
        public async Task<ActionResult<ExportOrder>> GetOrderDetail(int orderId)
        {
            var order = await _context.ExportOrders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product) // <--- QUAN TRỌNG: Lấy thêm tên sản phẩm
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();

            return Ok(order);
        }

        // 3. Xác nhận đã soạn hàng xong
        // POST: api/OrderPicking/complete/5
        [HttpPost("complete/{orderId}")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var order = await _context.ExportOrders.FindAsync(orderId);
            if (order == null) return NotFound();

            // Cập nhật trạng thái thành Xong
            order.Status = "Done";

            // (Ở đây bạn có thể thêm logic trừ tồn kho luôn, hoặc để bước Xuất kho làm)
            // Tạm thời mình chỉ đổi trạng thái để báo cho Web biết là xong việc.

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã hoàn thành soạn hàng!" });
        }
        // 5. Hủy nhận đơn (Nhả đơn về trạng thái New)
        // POST: api/OrderPicking/cancel/5
        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelPicking(int orderId)
        {
            var order = await _context.ExportOrders.FindAsync(orderId);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            // Chỉ cho phép hủy nếu đơn đang ở trạng thái Picking
            if (order.Status == "Picking")
            {
                // 1. Trả về trạng thái New
                order.Status = "New";

                // 2. Xóa người lấy (để người khác có thể nhận)
                order.Picker = null;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Đã hủy nhận đơn, đơn hàng quay về hàng chờ!" });
            }

            return BadRequest("Không thể hủy đơn này (Đơn đã xong hoặc chưa nhận).");
        }
    }
}