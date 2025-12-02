using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên")]
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly DataContext _dbContext;
        public SupplierController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            // Lưu lại từ khóa để hiển thị trên ô tìm kiếm
            ViewData["CurrentFilter"] = searchString;

            // Tạo truy vấn cơ bản (chưa chạy ngay)
            // Lưu ý: Kiểm tra lại tên bảng trong DataContext là 'Supplier' hay 'Suppliers' nhé
            var suppliers = from s in _dbContext.Supplier
                            select s;

            // Lọc dữ liệu nếu có từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s => s.Name.Contains(searchString)
                                              || s.Address.Contains(searchString)
                                              || s.Phone.Contains(searchString)
                                              || s.Email.Contains(searchString));
            }

            // Sắp xếp ID giảm dần (Mới nhất lên đầu) và trả về View
            return View(await suppliers.OrderByDescending(s => s.Id).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SupplierModel supplier)
        {
            if (ModelState.IsValid)
            {
                var supplierModel = new SupplierModel();

                _dbContext.Supplier.Add(supplier);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _dbContext.Supplier.Find(id);
            return View(supplier);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SupplierModel supplier)
        {
            if (ModelState.IsValid)
            {
                var supplierModel = new SupplierModel();

                _dbContext.Supplier.Update(supplier);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhập nhà cung cấp thành công";

                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var supplier = _dbContext.Supplier.Find(id);
            if (supplier != null)
            {
                _dbContext.Supplier.Remove(supplier);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Xóa nhà cung cấp thành công";

            }
            return RedirectToAction("Index");
        }
    }
}
