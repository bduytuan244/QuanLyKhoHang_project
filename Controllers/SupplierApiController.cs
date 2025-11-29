using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Quan trọng để dùng JwtBearerDefaults
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Dòng này để bảo mật, bắt buộc phải có Token mới lấy được danh sách
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SupplierApiController : ControllerBase
    {
        private readonly DataContext _context;

        public SupplierApiController(DataContext context)
        {
            _context = context;
        }

        // GET: api/SupplierApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierModel>>> GetSuppliers()
        {
            // Lấy tất cả nhà cung cấp từ database
            return await _context.Supplier.ToListAsync();
        }
    }
}