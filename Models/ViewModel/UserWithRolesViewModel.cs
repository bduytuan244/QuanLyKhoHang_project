using Microsoft.AspNetCore.Identity;

namespace QuanLyKhoHang.Models.ViewModel
{
    public class UserWithRolesViewModel
    {
        public IdentityUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
