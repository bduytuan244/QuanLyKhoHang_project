using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;
using X.PagedList.Extensions;
namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên, Sales")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dbContext;
        public ProductController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(int? page, string searchString) // Thêm tham số searchString
        {
            // Lưu lại từ khóa để hiện lại trên ô input
            ViewData["CurrentFilter"] = searchString;

            // Bước 1: Tạo truy vấn cơ bản
            var productsQuery = from p in _dbContext.Products
                                select p;

            // Bước 2: Lọc nếu có từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(searchString)
                                                      || p.ProductCode.Contains(searchString)
                                                      || p.Location.Contains(searchString));
            }

            // Bước 3: Sắp xếp (Mới nhất lên đầu)
            productsQuery = productsQuery.OrderByDescending(p => p.Id);

            // Bước 4: Phân trang
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Chuyển đổi kết quả cuối cùng sang PagedList
            var pagedProducts = productsQuery.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }
        [HttpGet]

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;

                await _dbContext.Products.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Add product successfull";
                return RedirectToAction("Index", "Product");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductModel model, int id)
        {
            if(id != model.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
                if(product == null)
                {
                    return NotFound();
                }
                product.ProductCode = model.ProductCode;
                product.ProductName = model.ProductName;   
                product.ProductUnit = model.ProductUnit;
                product.ProductQuantity = model.ProductQuantity;
                product.Location = model.Location;
                product.ProductDescription = model.ProductDescription;
                product.UpdateDate = DateTime.Now;

                _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Update product successfull";
                return RedirectToAction("Index", "Product");
            }
            return View(model);
        }
    }
}
