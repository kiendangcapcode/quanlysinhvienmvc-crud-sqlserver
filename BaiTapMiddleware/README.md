# Bài tập Middleware - Quản lý sinh viên

## Video minh chứng

[Xem video chạy và kiểm tra middleware](./bandicam%202026-10-05%2009-28-54-322.mp4)

Bài này tiếp tục mini project quản lý sinh viên và thêm `RequestLoggingMiddleware` để:

- Ghi thời gian tới millisecond, HTTP method và URL path trước khi request vào Controller.
- Ghi status code sau khi xử lý xong request.
- Chặn ID không hợp lệ với HTTP 400. URL của ứng dụng là `/SinhVien/Details/0` và `/SinhVien/Details/-1`; middleware cũng nhận hai URL ví dụ của đề `/Book/Detail/0` và `/Book/Detail/-1`.

## Chạy

1. Mở `QuanLySinhVienMiddleware.sln` trong Visual Studio 2026.
2. Đảm bảo SQL Server service `MSSQLSERVER` đang chạy.
3. Nhấn F5. Xem log trong cửa sổ Output/console của ứng dụng.

Thử các URL:

- `/SinhVien` - danh sách sinh viên; log method, path và status code.
- `/SinhVien/Details/0` - middleware trả HTTP 400 và `Mã sinh viên không hợp lệ`, không gọi Controller.
- `/Book/Detail/0` - đường dẫn ví dụ trong đề; middleware trả HTTP 400 và `Book id không hợp lệ`.
- `/Book/Detail/1` - middleware cho request đi tiếp; do project này quản lý sinh viên nên route Book không có Controller và sẽ trả 404.

Middleware được đăng ký trong `Program.cs` trước `UseRouting` và `MapControllerRoute`, vì vậy có thể ghi log và chặn request trước khi Controller xử lý.
