using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter email to login.")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter password to login")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
