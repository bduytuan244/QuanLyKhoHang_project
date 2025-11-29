using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly DataContext _context;
        public ProductsApiController(DataContext context)
        {
            _context = context;
        }

        // Lấy tất cả sản phẩm
        [HttpGet]
        public ActionResult<IEnumerable<ProductModel>> Get()
            => _context.Products.ToList();

        // Thêm mới sản phẩm
        [HttpPost]
        public IActionResult Post([FromBody] ProductModel model)
        {
            _context.Products.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }

        //// Sửa sản phẩm
        //[HttpPut("{id}")]
        //public IActionResult Put(int id, [FromBody] ProductModel model)
        //{
        //    var prod = _context.Products.FirstOrDefault(p => p.Id == id);
        //    if (prod == null)
        //        return NotFound();

        //    // Cập nhật thông tin
        //    prod.Barcode = model.Barcode;
        //    prod.Name = model.Name;
        //    prod.Quantity = model.Quantity;
        //    // Thêm các trường cần update...

        //    _context.SaveChanges();
        //    return Ok(prod);
        //}

        // Xóa sản phẩm
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var prod = _context.Products.FirstOrDefault(p => p.Id == id);
            if (prod == null)
                return NotFound();

            _context.Products.Remove(prod);
            _context.SaveChanges();
            return NoContent(); // Trả về 204 nếu xóa thành công
        }
    }
}
