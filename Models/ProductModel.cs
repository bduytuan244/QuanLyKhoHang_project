using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui long nhap ma san pham")]
        [StringLength(100)]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Vui long nhap ten san pham")]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui long nhap mo ta san pham")]
        [StringLength(100)]
        public string ProductDescription { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "So luong phai lon hon hoac bang 0")]
        public int ProductQuantity { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductUnit { get; set; }

        [Required]
        public string Location { get; set; }

        // SỬA: Thêm dấu ? để cho phép Null (Tránh lỗi parse ngày tháng từ Android)
        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}