using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Controllers;

public class SinhVienController(QuanLySinhVienContext context, IWebHostEnvironment environment) : Controller
{
    private const long MaxImageSize = 5 * 1024 * 1024;

    public async Task<IActionResult> Index(string? tuKhoa)
    {
        var sinhViens = context.SinhViens.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            sinhViens = sinhViens.Where(s => s.MaSinhVien.Contains(tuKhoa) || s.HoTen.Contains(tuKhoa));
        }

        ViewData["TuKhoa"] = tuKhoa;
        return View(await sinhViens.OrderBy(s => s.MaSinhVien).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var sinhVien = await context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return sinhVien == null ? NotFound() : View(sinhVien);
    }

    public IActionResult Create() => View(new SinhVien());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SinhVien sinhVien, IFormFile? anhDaiDien)
    {
        if (await context.SinhViens.AnyAsync(s => s.MaSinhVien == sinhVien.MaSinhVien))
            ModelState.AddModelError(nameof(sinhVien.MaSinhVien), "Mã sinh viên đã tồn tại.");

        if (anhDaiDien == null)
            ModelState.AddModelError("anhDaiDien", "Vui lòng chọn ảnh JPG cho sinh viên.");
        else
            await ValidateImageAsync(anhDaiDien);

        if (!ModelState.IsValid) return View(sinhVien);

        sinhVien.ImagePath = await SaveImageAsync(anhDaiDien!);
        try
        {
            context.Add(sinhVien);
            await context.SaveChangesAsync();
        }
        catch
        {
            DeleteImageFile(sinhVien.ImagePath);
            throw;
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var sinhVien = await context.SinhViens.FindAsync(id);
        return sinhVien == null ? NotFound() : View(sinhVien);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SinhVien sinhVien, IFormFile? anhDaiDien)
    {
        if (id != sinhVien.Id) return NotFound();
        var existingStudent = await context.SinhViens.FindAsync(id);
        if (existingStudent == null) return NotFound();

        if (await context.SinhViens.AnyAsync(s => s.MaSinhVien == sinhVien.MaSinhVien && s.Id != id))
            ModelState.AddModelError(nameof(sinhVien.MaSinhVien), "Mã sinh viên đã tồn tại.");

        if (anhDaiDien != null) await ValidateImageAsync(anhDaiDien);
        sinhVien.ImagePath = existingStudent.ImagePath;
        if (!ModelState.IsValid) return View(sinhVien);

        var oldImagePath = existingStudent.ImagePath;
        var newImagePath = anhDaiDien == null ? oldImagePath : await SaveImageAsync(anhDaiDien);
        try
        {
            existingStudent.MaSinhVien = sinhVien.MaSinhVien;
            existingStudent.HoTen = sinhVien.HoTen;
            existingStudent.NgaySinh = sinhVien.NgaySinh;
            existingStudent.GioiTinh = sinhVien.GioiTinh;
            existingStudent.Email = sinhVien.Email;
            existingStudent.Lop = sinhVien.Lop;
            existingStudent.ImagePath = newImagePath;
            await context.SaveChangesAsync();
        }
        catch
        {
            if (anhDaiDien != null) DeleteImageFile(newImagePath);
            throw;
        }
        if (anhDaiDien != null) DeleteImageFile(oldImagePath);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var sinhVien = await context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return sinhVien == null ? NotFound() : View(sinhVien);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sinhVien = await context.SinhViens.FindAsync(id);
        if (sinhVien != null)
        {
            var imagePath = sinhVien.ImagePath;
            context.SinhViens.Remove(sinhVien);
            await context.SaveChangesAsync();
            DeleteImageFile(imagePath);
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateImageAsync(IFormFile image)
    {
        if (Path.GetExtension(image.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) == false)
            ModelState.AddModelError("anhDaiDien", "Chỉ chấp nhận ảnh có đuôi .jpg.");

        if (image.Length == 0 || image.Length > MaxImageSize)
            ModelState.AddModelError("anhDaiDien", "Ảnh phải có dung lượng lớn hơn 0 và không quá 5 MB.");

        if (image.Length == 0) return;
        await using var stream = image.OpenReadStream();
        var header = new byte[3];
        var bytesRead = await stream.ReadAsync(header.AsMemory());
        if (bytesRead != 3 || header[0] != 0xFF || header[1] != 0xD8 || header[2] != 0xFF)
            ModelState.AddModelError("anhDaiDien", "Nội dung file không phải ảnh JPG hợp lệ.");
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var uploadFolder = Path.Combine(webRoot, "uploads", "sinhvien");
        Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid():N}.jpg";
        var fullPath = Path.Combine(uploadFolder, fileName);
        await using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        await image.CopyToAsync(fileStream);
        return $"/uploads/sinhvien/{fileName}";
    }

    private void DeleteImageFile(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return;

        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var uploadRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads", "sinhvien")) + Path.DirectorySeparatorChar;
        var relativePath = imagePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(webRoot, relativePath));
        if (fullPath.StartsWith(uploadRoot, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }
}
