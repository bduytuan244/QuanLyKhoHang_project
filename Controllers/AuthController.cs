using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyKhoHang.Controllers
{
    // 1. Đổi thành [ApiController] để báo đây là API
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase // Dùng ControllerBase cho nhẹ (thay vì Controller)
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        // 2. Inject (Tiêm) các dịch vụ cần thiết vào
        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // 3. API Đăng nhập: Nhận JSON { username, password }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Tìm user trong database
            var user = await _userManager.FindByNameAsync(model.Username);

            // Kiểm tra mật khẩu
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Lấy các quyền (Role) của user (ví dụ: Admin, Staff)
                var userRoles = await _userManager.GetRolesAsync(user);

                // Tạo danh sách thông tin (Claims) để in vào "vé" Token
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Thêm Role vào Token
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Lấy "Chữ ký bí mật" từ file appsettings.json
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                // Tạo Token
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3), // Token sống được 3 tiếng
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // Trả về kết quả JSON cho Android
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            // Nếu sai mật khẩu
            return Unauthorized();
        }
    }

    // Class phụ để hứng dữ liệu từ Android gửi lên
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}