using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Data;

public class QuanLySinhVienContext(DbContextOptions<QuanLySinhVienContext> options) : DbContext(options)
{
    public DbSet<SinhVien> SinhViens => Set<SinhVien>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SinhVien>().HasIndex(s => s.MaSinhVien).IsUnique();
    }
}
