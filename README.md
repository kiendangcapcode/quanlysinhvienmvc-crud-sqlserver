# Web app quản lý sinh viên

## Video trình bày

[Xem video demo trên GitHub](./bandicam%202026-10-04%2021-22-06-426.mp4)

Ứng dụng ASP.NET Core MVC dùng **Entity Framework Core Code First** và SQL Server. Chức năng gồm xem/tìm kiếm danh sách, thêm, xem chi tiết, sửa và xóa sinh viên. Có thể tải ảnh sinh viên dạng `.jpg`; ứng dụng kiểm tra cả phần mở rộng và chữ ký file, lưu ảnh vào `wwwroot/uploads/sinhvien` và lưu đường dẫn trong database. Database `QuanLySinhVienMvcDb` cùng bảng sinh viên được tạo tự động khi chạy ứng dụng lần đầu.

## Bài tập Middleware

Project tiếp nối quản lý sinh viên, thêm request logging và chặn ID không hợp lệ: [mở thư mục bài Middleware](./BaiTapMiddleware/README.md). [Xem video Middleware mới nhất](./BaiTapMiddleware/bandicam%202026-10-07%2009-09-27-696.mp4).

## Chạy bằng Visual Studio 2026

1. Mở `QuanLySinhVien.sln`.
2. Đảm bảo dịch vụ **SQL Server (MSSQLSERVER)** đang chạy.
3. Nhấn F5 hoặc Ctrl+F5. Gói NuGet sẽ được restore tự động.
4. Ứng dụng mặc định kết nối tới SQL Server cài trên máy (`localhost`, instance mặc định) bằng Windows Authentication.

Nếu dùng SQL Server/SQL Server Express khác, sửa `ConnectionStrings:DefaultConnection` trong `appsettings.json`, ví dụ:

```json
"DefaultConnection": "Server=localhost;Database=QuanLySinhVienMvcDb;Trusted_Connection=True;TrustServerCertificate=True"
```

Hoặc với SQL Login:

```json
"DefaultConnection": "Server=localhost;Database=QuanLySinhVienMvcDb;User Id=sa;Password=your_password;TrustServerCertificate=True"
```

## Chạy bằng dòng lệnh

```powershell
dotnet restore
dotnet run
```

Mở URL được in trong terminal. Bản mẫu dùng `EnsureCreated` cho gọn; nếu đổi cấu trúc model sau khi database đã được tạo, hãy xóa database `QuanLySinhVienMvcDb` trên SQL Server rồi chạy lại để tạo bảng mới. Việc này sẽ xóa dữ liệu cũ.
