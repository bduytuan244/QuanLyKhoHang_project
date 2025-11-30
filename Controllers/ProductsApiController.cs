using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Bảo mật bằng Token
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsApiController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsApiController(DataContext context)
        {
            _context = context;
        }

        // 1. Lấy tất cả sản phẩm
        // GET: api/ProductsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get()
        {
            return await _context.Products.OrderByDescending(p => p.Id).ToListAsync();
        }

        // 2. Lấy sản phẩm theo Mã Vạch (Cho chức năng Scan của App)
        // GET: api/ProductsApi/barcode/123456
        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ProductModel>> GetByBarcode(string barcode)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductCode == barcode);

            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm với mã vạch này");
            }

            return Ok(product);
        }

        // 3. Tìm kiếm sản phẩm theo Tên (Cho ô nhập liệu của App)
        // GET: api/ProductsApi/search?name=abc
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Ok(new List<ProductModel>());
            }

            var products = await _context.Products
                .Where(p => p.ProductName.Contains(name))
                .Take(10) // Chỉ lấy 10 kết quả gợi ý thôi cho nhẹ
                .ToListAsync();

            return Ok(products);
        }

        // 4. Thêm mới sản phẩm
        // POST: api/ProductsApi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductModel model)
        {
            // Kiểm tra trùng mã vạch
            var exists = await _context.Products.AnyAsync(p => p.ProductCode == model.ProductCode);
            if (exists)
            {
                return BadRequest("Mã sản phẩm này đã tồn tại!");
            }

            model.CreateDate = DateTime.Now;
            model.UpdateDate = DateTime.Now;

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            // Trả về sản phẩm vừa tạo (bao gồm ID mới sinh ra)
            return Ok(model);
        }

        // 5. Cập nhật sản phẩm (Sửa lỗi của bạn ở đây)
        // PUT: api/ProductsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductModel model)
        {
            var prod = await _context.Products.FindAsync(id);
            if (prod == null)
                return NotFound();

            // Cập nhật thông tin đầy đủ
            prod.ProductCode = model.ProductCode; // Sửa dòng lỗi cũ của bạn
            prod.ProductName = model.ProductName;
            prod.ProductQuantity = model.ProductQuantity; // Cập nhật số lượng tồn kho

            // Cập nhật thêm các trường phụ
            prod.Location = model.Location;
            prod.ProductUnit = model.ProductUnit;
            prod.ProductDescription = model.ProductDescription;

            // Cập nhật ngày sửa
            prod.UpdateDate = DateTime.Now;

            // Nếu bạn có dùng SupplierId trong bảng Product thì uncomment dòng dưới
            // prod.SupplierId = model.SupplierId;

            _context.Products.Update(prod);
            await _context.SaveChangesAsync();

            return Ok(prod);
        }

        // 6. Xóa sản phẩm
        // DELETE: api/ProductsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prod = await _context.Products.FindAsync(id);
            if (prod == null)
                return NotFound();

            _context.Products.Remove(prod);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}