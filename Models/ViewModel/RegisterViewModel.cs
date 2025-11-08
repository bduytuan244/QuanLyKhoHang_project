using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter email to register.")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter password to register")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
