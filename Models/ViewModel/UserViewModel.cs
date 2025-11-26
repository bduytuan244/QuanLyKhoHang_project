using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models.ViewModel
{
    public class UserViewModel
    {
        [Required]
        [EmailAddress]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
    }
}
