using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dbContext;
        public ProductController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var products = _dbContext.Products.OrderBy(p => p.Id);
            return View(products);
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
