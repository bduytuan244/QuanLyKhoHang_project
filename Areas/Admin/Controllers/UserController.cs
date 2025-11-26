using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoHang.Models.ViewModel;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        
        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
        [HttpGet]
        public IActionResult Create() => View();

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if(id == currentUserId)
            {
                TempData["ErrorMessage"] = "Your current account could not be edited";
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);
            if ( user == null)
            {
                return NotFound();
            }
            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
            }
            TempData["SuccessMessage"] = "Cập nhật người dùng thành công";
            return RedirectToAction("Index");
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.ViewModel.UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser {UserName = model.Email,Email = model.Email};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Created user successfully.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if(id == currentUserId)
            {
                TempData["ErrorMessage"] = "Không thể sửa chính tài khoản của bạn.";
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
                TempData["SuccessMessage"] = "Deleted user";
                return RedirectToAction("Index");
            }
         }
    }