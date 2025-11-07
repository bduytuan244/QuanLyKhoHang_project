using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductCode { get; set; }
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductDescription { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int ProductQuantity { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductUnit { get; set; }
        [Required]
        public string Location { get; set; }

    }
}
