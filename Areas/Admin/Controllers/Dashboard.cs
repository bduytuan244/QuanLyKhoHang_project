using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên, Sales")]
    [Area("Admin")]
    public class Dashboard : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
