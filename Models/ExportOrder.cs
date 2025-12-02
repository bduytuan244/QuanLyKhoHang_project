using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoHang.Models
{
    public class ExportOrder
    {
        public int Id { get; set; }

        [Required]
        public string OrderCode { get; set; } // Mã đơn (ví dụ: OUT-001)

        public string Status { get; set; } // "New" (Mới), "Picking" (Đang lấy), "Done" (Đã lấy xong)
        public string? Picker { get; set; }
        public string CreatedBy { get; set; } // Người tạo đơn (Admin trên web)

        public DateTime CreatedDate { get; set; }

        // Danh sách chi tiết các món trong đơn này
        public virtual ICollection<ExportOrderDetail> Details { get; set; }
    }
}