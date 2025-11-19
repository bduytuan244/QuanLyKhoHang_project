using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models.ViewModel
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]

        public string NewPassword { get; set; }
    }
}
