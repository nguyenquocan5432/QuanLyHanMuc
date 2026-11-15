# Hệ thống kiểm soát hạn mức mua sắm (FIN-07)

Ứng dụng web giúp sinh viên và người cấp ngân sách đặt hạn mức chi tiêu theo danh mục và theo kỳ, ghi giao dịch, chặn hoặc chuyển duyệt khi vượt hạn mức.

| Phần | Công nghệ | Thư mục |
|---|---|---|
| Giao diện | React 18, TypeScript, Vite | `client/` (kết quả build nằm ở `wwwroot/`) |
| Máy chủ | C# ASP.NET Core 8 Web API | `Controllers/`, `Services/`, `Models/`, `Data/`, `Infrastructure/` |
| Cơ sở dữ liệu | SQL Server 2022 | `CSDL/QuanLyHanMuc.sql` |
| Đăng nhập | OpenID Connect (Keycloak) hoặc tài khoản trong CSDL | `Infrastructure/CauHinhOidc.cs`, `keycloak/` |
| Kiểm thử | xUnit | `Tests/` |
| Triển khai | Docker Compose | `Dockerfile`, `docker-compose.yml` |

Sinh viên thực hiện: Nguyễn Quốc An, MSSV 1250080002, lớp 12_CNPM1. Giảng viên hướng dẫn: Th.S Phạm Trọng Huynh.

Báo cáo, slide, sơ đồ, ảnh giao diện và bảng biểu nằm trong thư mục [`docs/`](docs/README.md).

Tài khoản mẫu (mật khẩu đều là `123456`): `admin`, `phuhuynh1`, `sinhvien1`, `sinhvien2`, `tuquanly1`.

## Cách 1: chạy trên máy bằng dotnet run

Cần .NET SDK 8 và SQL Server.

1. Mở `CSDL/QuanLyHanMuc.sql` trong SSMS, bấm F5 để tạo database `QuanLyHanMuc`.
2. Chép file `appsettings.Example.json` thành `appsettings.json`, rồi sửa chuỗi kết nối cho đúng máy. File mẫu dùng đăng nhập Windows (`Trusted_Connection=True`); nếu dùng tài khoản `sa` thì thay bằng `User ID=sa;Password=...`. File `appsettings.json` đã nằm trong `.gitignore` nên mật khẩu không bị đưa lên GitHub.
3. Chạy:

```
dotnet run
```

4. Mở http://localhost:5080.

Ở cách này ứng dụng kiểm tra tên đăng nhập và mật khẩu trong bảng `NguoiDung` (chế độ `Local`), không cần Keycloak. Giao diện React đã build sẵn trong `wwwroot/` nên không cần cài Node.

## Cách 2: chạy cả hệ thống bằng Docker

Cần Docker Desktop.

1. Chép file `.env.example` thành `.env` (cùng thư mục với `docker-compose.yml`) rồi điền ba giá trị: mật khẩu `sa` của SQL Server trong container (ít nhất 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt), mật khẩu quản trị Keycloak và client secret. File `.env` nằm trong `.gitignore` nên không bị đưa lên GitHub. Thiếu file này thì Docker Compose dừng ngay và báo cần tạo `.env`.

   ```
   copy .env.example .env
   ```

2. Dựng và chạy:

   ```
   docker compose up --build
   ```

Lệnh này dựng 3 dịch vụ: SQL Server (tự chạy file script nếu chưa có database), Keycloak (tự nạp realm `hanmuc` và 5 tài khoản mẫu) và ứng dụng web.

- Ứng dụng: http://localhost:8080
- Keycloak: http://localhost:8081 (quản trị: tài khoản `admin`, mật khẩu là `KEYCLOAK_ADMIN_PASSWORD` trong `.env`)

Ở cách này ứng dụng chạy chế độ `Oidc`: bấm "Đăng nhập" thì trình duyệt chuyển sang trang của Keycloak, đăng nhập xong quay về ứng dụng. Lần đầu Keycloak khởi động mất khoảng một phút; nếu bấm đăng nhập mà báo lỗi thì chờ thêm rồi thử lại.

Dừng: `docker compose down`. Xóa luôn dữ liệu để dựng lại từ đầu: `docker compose down -v`.

## Chạy kiểm thử

```
dotnet test Tests
```

Bộ test tự tạo database `QuanLyHanMuc_Test` từ file script trên cùng máy chủ SQL trong `appsettings.json` (database `QuanLyHanMuc` không bị đụng tới). Muốn dùng máy chủ khác thì đặt biến môi trường `QLHM_TEST_SQL` bằng một chuỗi kết nối.

| File | Nội dung |
|---|---|
| `DanhGiaGiaoDichTests.cs` | Kiểm thử đơn vị lớp `HanMuc`: các mức 70%, 80%, 100%, vượt cứng, vượt mềm, đã khóa |
| `NghiepVuTests.cs` | Kiểm thử tích hợp qua Service và stored procedure: ghi giao dịch, duyệt ngoại lệ, hủy, khóa, trùng kỳ, phân quyền |
| `DongThoiTests.cs` | Xử lý đồng thời: 20 giao dịch ghi cùng lúc vào một hạn mức cứng, ghi và hủy xen kẽ, hai người cùng sửa một hạn mức |
| `ApiTests.cs` | Kiểm thử API: chưa đăng nhập trả 401, sai vai trò trả 403, thiếu mã chống giả mạo trả 400 |

## Sửa giao diện React

Cần Node.js 20.

```
cd client
npm install
npm run dev
```

Mở http://localhost:5173 (máy chủ .NET vẫn phải đang chạy ở cổng 5080). Sửa xong chạy `npm run build` để ghi bản mới vào `wwwroot/`.

## Xử lý đồng thời

- Ghi giao dịch: `sp_GhiNhanGiaoDich` đọc dòng hạn mức với `UPDLOCK, HOLDLOCK` trong transaction. Hai request cùng ghi vào một hạn mức phải xếp hàng, request sau đọc được số đã chi mới nhất.
- Sửa hạn mức: bảng `HanMuc` có cột `PhienBan` kiểu `rowversion`. Form sửa gửi kèm `PhienBan` đã đọc; nếu trong lúc đó người khác đã lưu thì `sp_DieuChinhHanMuc` báo lỗi 50035 và giao diện yêu cầu tải lại.
- Trigger `TR_GiaoDich_BaoVe` kiểm tra lại tổng giao dịch hợp lệ không vượt hạn mức cứng, kể cả khi có lệnh ghi thẳng vào bảng.
- View `vw_DoiSoatHanMuc` so số đã chi với tổng giao dịch hợp lệ; cột `ChenhLech` phải luôn bằng 0.

## Cấu trúc thư mục

```
QuanLyHanMucWeb/
├── Controllers/            API controller
├── Services/               Nghiệp vụ (hạn mức, giao dịch, ngoại lệ, báo cáo)
├── Models/                 Lớp thực thể, lớp HanMuc giữ quy tắc kiểm tra
├── Data/                   Gọi stored procedure, dịch lỗi SQL Server
├── Infrastructure/         Đăng nhập Local / OIDC, quản trị Keycloak
├── client/                 Mã nguồn React + TypeScript
├── wwwroot/                Bản build của React
├── CSDL/                   Script tạo cơ sở dữ liệu và dữ liệu mẫu
├── keycloak/               Realm hanmuc nạp sẵn cho Keycloak
├── Tests/                  32 ca kiểm thử xUnit
├── docs/                   Báo cáo, slide, sơ đồ, ảnh, bảng biểu
├── Dockerfile, docker-compose.yml
├── appsettings.Example.json  Cấu hình mẫu, chép thành appsettings.json khi chạy dotnet run
└── .env.example            Mẫu mật khẩu cho Docker, chép thành .env
```

Repo không chứa mật khẩu nào của máy chủ: mật khẩu SQL Server trong container, mật khẩu quản trị Keycloak và client secret đều đọc từ file `.env` không đưa lên GitHub (xem `.env.example`). Mật khẩu `123456` của 5 tài khoản mẫu là dữ liệu mẫu để chạy thử; trong CSDL nó được lưu dạng băm.
