using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models;

public class SinhVien
{
    public int Id { get; set; }

    [Display(Name = "Mã sinh viên")]
    [Required(ErrorMessage = "Vui lòng nhập mã sinh viên.")]
    [StringLength(20)]
    public string MaSinhVien { get; set; } = string.Empty;

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100)]
    public string HoTen { get; set; } = string.Empty;

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    [Display(Name = "Giới tính")]
    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Display(Name = "Lớp")]
    [StringLength(50)]
    public string? Lop { get; set; }

    [Display(Name = "Ảnh sinh viên")]
    [StringLength(255)]
    public string? ImagePath { get; set; }
}
