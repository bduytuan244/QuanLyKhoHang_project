using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class Dashboard : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
