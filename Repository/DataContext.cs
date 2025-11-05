using Microsoft.EntityFrameworkCore;

namespace QuanLyKhoHang.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        { 
        }
    }
}
