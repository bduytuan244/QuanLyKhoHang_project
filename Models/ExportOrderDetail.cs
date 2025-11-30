using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoHang.Models
{
    public class ExportOrderDetail
    {
        public int Id { get; set; }

        public int ExportOrderId { get; set; } // Thuộc đơn nào
        [ForeignKey("ExportOrderId")]
        public virtual ExportOrder ExportOrder { get; set; }

        public int ProductId { get; set; } // Lấy sản phẩm nào
        [ForeignKey("ProductId")]
        public virtual ProductModel Product { get; set; }

        public int Quantity { get; set; } // Cần lấy bao nhiêu cái
    }
}