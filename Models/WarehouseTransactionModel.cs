using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoHang.Models
{
    public class WarehouseTransactionModel
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int? SupplierId { get; set; }  // ← THÊM DẤU ? ĐỂ CHO PHÉP NULL

        public SupplierModel? Supplier { get; set; }  // ← THÊM ? CHO SUPPLIER
        public ProductModel Product { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0")]
        public int Quantity { get; set; }
        public string? Username { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(50)]
        public string TransactionType { get; set; } // "Import" hoặc "Export"

        [Required(ErrorMessage = "Vui lòng nhập ghi chú")]
        [StringLength(200)]
        public string Notes { get; set; }
    }
}
