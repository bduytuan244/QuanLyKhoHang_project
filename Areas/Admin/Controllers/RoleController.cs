using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dbContext;

        public RoleController(RoleManager<IdentityRole> roleManager, DataContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(Name));
                if(result.Succeeded)
                    return RedirectToAction("Index");
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    }
            return View();
        }
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string Name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role != null)
            {
                role.Name = Name;
                var result = await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                    return RedirectToAction("Index");
            }
            return View(role);
        }
    }
}
    
