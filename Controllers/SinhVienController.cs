using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Controllers;

public class SinhVienController(QuanLySinhVienContext context) : Controller
{
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

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SinhVien sinhVien)
    {
        if (await context.SinhViens.AnyAsync(s => s.MaSinhVien == sinhVien.MaSinhVien))
            ModelState.AddModelError(nameof(sinhVien.MaSinhVien), "Mã sinh viên đã tồn tại.");

        if (!ModelState.IsValid) return View(sinhVien);
        context.Add(sinhVien);
        await context.SaveChangesAsync();
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
    public async Task<IActionResult> Edit(int id, SinhVien sinhVien)
    {
        if (id != sinhVien.Id) return NotFound();
        if (await context.SinhViens.AnyAsync(s => s.MaSinhVien == sinhVien.MaSinhVien && s.Id != id))
            ModelState.AddModelError(nameof(sinhVien.MaSinhVien), "Mã sinh viên đã tồn tại.");

        if (!ModelState.IsValid) return View(sinhVien);
        try
        {
            context.Update(sinhVien);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await context.SinhViens.AnyAsync(s => s.Id == id)) return NotFound();
            throw;
        }
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
            context.SinhViens.Remove(sinhVien);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
