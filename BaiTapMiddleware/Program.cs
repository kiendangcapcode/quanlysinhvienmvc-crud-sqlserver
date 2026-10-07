using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuanLySinhVienContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Chạy sớm để quan sát request trước khi định tuyến tới Controller.
app.UseMiddleware<RequestLoggingMiddleware>();

// Code First: tạo database và bảng lần đầu chạy ứng dụng.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuanLySinhVienContext>();
    db.Database.EnsureCreated();
    // EnsureCreated không cập nhật bảng đã tồn tại, nên bổ sung cột ảnh cho database cũ.
    db.Database.ExecuteSqlRaw("IF COL_LENGTH(N'dbo.SinhViens', N'ImagePath') IS NULL ALTER TABLE dbo.SinhViens ADD ImagePath nvarchar(255) NULL;");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// Phục vụ file upload phát sinh lúc chạy (MapStaticAssets chỉ lập manifest lúc build).
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SinhVien}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
