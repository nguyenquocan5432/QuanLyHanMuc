# Bảng biểu chương 5: Kiến trúc và cài đặt

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 5.1. Công nghệ sử dụng trong hệ thống

| Thành phần | Công nghệ | Phiên bản | Vai trò |
|---|---|---|---|
| Giao diện | React, TypeScript, React Router | 18.3, 5.9, 6.30 | 13 trang, gọi API bằng fetch |
| Đóng gói giao diện | Vite | 5.4 | Build React vào wwwroot; chạy thử ở cổng 5173 khi sửa giao diện |
| Ngôn ngữ máy chủ | C# | 12 | Controller, service, lớp truy cập dữ liệu |
| Web framework | ASP.NET Core Web API | .NET 8 | Định tuyến /api, gắn JSON vào tham số, phân quyền, chống CSRF |
| Truy cập dữ liệu | Microsoft.Data.SqlClient | 5.2.2 | SqlCommand có tham số, gọi stored procedure |
| Hệ quản trị CSDL | Microsoft SQL Server | 2022 | 8 bảng, 2 trigger, 2 hàm, 2 view, 9 stored procedure |
| Đăng nhập | Keycloak; Microsoft.AspNetCore.Authentication.OpenIdConnect | 26.0; 8.0.11 | Máy chủ OpenID Connect, realm hanmuc; middleware OIDC phía .NET |
| Phiên đăng nhập | Cookie authentication | .NET 8 | Cookie HttpOnly đã mã hóa, hết hạn sau 8 giờ |
| Kiểm thử | xUnit, Microsoft.AspNetCore.Mvc.Testing | 2.9.2; 8.0.11 | 32 ca kiểm thử đơn vị, tích hợp, đồng thời, API |
| Triển khai | Docker, Docker Compose |  | Ba dịch vụ web, csdl, keycloak |

### Bảng 5.2. Ánh xạ kiến trúc vào mã nguồn

| Tầng | Hiện thực | Thư mục |
|---|---|---|
| Giao diện (React) | 13 trang .tsx, khung BoCuc, api.ts, Phien.tsx, site.css | client/src/ |
| API (Controller) | 9 API controller kế thừa CoSoController; 3 chính sách phân quyền; cấu hình OIDC | Controllers/, Infrastructure/, Program.cs |
| Nghiệp vụ (Service) | 8 service tĩnh; lớp HanMuc tính tỷ lệ và dự báo; KetQuaGhiNhan | Services/, Models/ |
| Truy cập dữ liệu | KetNoi mở kết nối và chạy lệnh; LoiCSDL dịch lỗi SQL Server | Data/ |
| CSDL | Bảng, ràng buộc, trigger, hàm, view, stored procedure | CSDL/QuanLyHanMuc.sql |

### Bảng 5.3. Các dịch vụ trong docker-compose.yml

| Dịch vụ | Image | Cổng | Việc làm |
|---|---|---|---|
| csdl | mcr.microsoft.com/mssql/server:2022-latest | 14333 → 1433 | SQL Server 2022 bản Developer. Dữ liệu nằm trong volume csdl-data nên tắt container không mất. Có healthcheck chạy SELECT 1 mỗi 10 giây |
| csdl-khoi-tao | mcr.microsoft.com/mssql/server:2022-latest | không mở | Chỉ chạy khi csdl đã khỏe. Hỏi DB_ID('QuanLyHanMuc'); nếu bằng 0 thì chạy QuanLyHanMuc.sql bằng sqlcmd (-f 65001 để đọc đúng tiếng Việt), xong thì dừng |
| keycloak | quay.io/keycloak/keycloak:26.0 | 8081 → 8080 | Máy chủ OpenID Connect, chạy start-dev --import-realm, nạp keycloak/hanmuc-realm.json |
| web | Build từ Dockerfile | 8080 → 8080 | React và Web API. Chỉ chạy sau khi csdl-khoi-tao kết thúc thành công; cấu hình nhận qua biến môi trường |

### Bảng 5.4. Biến môi trường của dịch vụ web

| Biến | Giá trị và ý nghĩa |
|---|---|
| ConnectionStrings__QuanLyHanMuc | Server=csdl;Database=QuanLyHanMuc;User ID=sa;... Thay chuỗi kết nối trong appsettings.json; tên máy chủ là tên dịch vụ csdl |
| XacThuc__CheDo | Oidc. Không đặt thì ứng dụng dùng chế độ Local |
| XacThuc__Oidc__Authority | http://localhost:8081/realms/hanmuc, địa chỉ realm trình duyệt thấy, dùng để kiểm tra trường iss của id_token |
| XacThuc__Oidc__MetadataAddress | http://keycloak:8080/realms/hanmuc/.well-known/openid-configuration, máy chủ web đọc khóa ký và các endpoint từ đây |
| XacThuc__Oidc__ClientId, ClientSecret | hanmuc-web và secret khai báo trong realm |
| XacThuc__Keycloak__AdminUrl, AdminUser, AdminPassword | Tài khoản quản trị Keycloak cho KeycloakQuanTri gọi Admin REST API (UC13) |

### Bảng 5.5. Cấu hình realm hanmuc trong Keycloak

| Mục | Giá trị |
|---|---|
| Realm | hanmuc; sslRequired none để chạy http://localhost; tắt tự đăng ký, tắt quên mật khẩu, không đổi tên đăng nhập |
| Client | hanmuc-web, loại confidential (có secret). Chỉ bật Standard Flow (Authorization Code); tắt Implicit Flow và Direct Access Grants |
| PKCE | pkce.code.challenge.method = S256 |
| Redirect URI | http://localhost:8080/signin-oidc và http://localhost:5080/signin-oidc |
| Sau đăng xuất | http://localhost:8080/* và http://localhost:5080/* |
| Người dùng | admin, phuhuynh1, sinhvien1, sinhvien2, tuquanly1; mật khẩu 123456; có họ, tên, email vì Keycloak 26 bắt buộc ba trường này trong hồ sơ người dùng |

### Bảng 5.6. Các API controller

| Controller | Đường dẫn | Chính sách | Use case |
|---|---|---|---|
| Phien | GET /api/phien; POST /api/dang-nhap; GET /xac-thuc/dang-nhap; POST /xac-thuc/dang-xuat | Không cần đăng nhập | UC12 |
| TrangChu | GET /api/trang-chu | Đã đăng nhập | UC08, UC09 |
| GiaoDich | GET nhap, GET danh-gia, POST /api/giao-dich | Vai trò SinhVien | UC05 |
|  | GET /api/giao-dich | GhiGiaoDich | UC06 |
|  | GET /api/giao-dich/{id}, POST {id}/huy | QuanLyHanMuc | UC07 |
| HanMuc | GET, GET lua-chon, GET {id}, POST, PUT {id}, POST {id}/khoa, POST {id}/mo-khoa | QuanLyHanMuc | UC01–UC03 |
| NgoaiLe | GET /api/ngoai-le, POST /api/ngoai-le/{maYeuCau} | QuanLyHanMuc | UC09 |
| ThongBao | GET /api/thong-bao, POST /api/thong-bao/da-doc | Đã đăng nhập | UC08 |
| BaoCao | GET /api/bao-cao/chi-tieu; GET /api/bao-cao/vi-pham | GhiGiaoDich; vi-pham thêm QuanLyHanMuc | UC10, UC11 |
| DanhMuc | GET, POST /api/danh-muc | QuanTri | UC04 |
| NguoiDung | GET, POST /api/nguoi-dung, POST {id}/dat-lai-mat-khau | QuanTri | UC13 |

### Bảng 5.7. Các service

| Service | Module | Use case | Trách nhiệm |
|---|---|---|---|
| TaiKhoanService | M1 | UC12 | Đọc tài khoản theo tên đăng nhập; so mật khẩu đã băm SHA-256 (chế độ Local) |
| HanMucService | M2 | UC01–UC03 | Danh sách hạn mức theo kỳ, sinh viên được quản lý, tạo cho một hoặc tất cả sinh viên, điều chỉnh, khóa, mở khóa |
| GiaoDichService | M3, M4, M6 | UC05–UC07 | Danh mục kèm cờ có hạn mức, hạn mức hiệu lực, ghi nhận, lịch sử, giao dịch gần đây, hủy |
| NgoaiLeService | M4 | UC09 | Danh sách chờ duyệt kèm số vượt, duyệt hoặc từ chối |
| ThongBaoService | M5 | UC08 | Đếm chưa đọc, danh sách, đánh dấu đã đọc |
| BaoCaoService | M6 | UC10, UC11 | Chi tiêu theo danh mục trong khoảng ngày, danh sách vi phạm |
| QuanTriService | M7 | UC04, UC13 | Danh mục, người dùng, đặt lại mật khẩu về 123456 |
| KeycloakQuanTri | M7 | UC13 | Gọi Admin REST API của Keycloak: tạo, cập nhật tài khoản, đặt lại mật khẩu (chế độ Oidc) |

### Bảng 5.8. Các bước xử lý một request

| Bước | Thành phần | Việc làm |
|---|---|---|
| 1 | UseExceptionHandler | Lỗi chưa bắt ở các bước sau được trả về 500 kèm {loi} tiếng Việt |
| 2 | UseDefaultFiles, UseStaticFiles | Trả trực tiếp index.html và file JavaScript, CSS của React trong wwwroot |
| 3 | UseRouting | Chọn controller và action theo đường dẫn /api/... |
| 4 | UseAuthentication | Giải mã cookie, dựng ClaimsPrincipal gồm mã, họ tên, vai trò, mã người cấp ngân sách. Chế độ Oidc còn xử lý /signin-oidc |
| 5 | UseAuthorization | Bộ lọc toàn cục bắt buộc đăng nhập; so chính sách của action. Chưa đăng nhập trả 401, sai vai trò trả 403 |
| 6 | Bộ lọc chống giả mạo | Request POST, PUT thiếu header X-XSRF-TOKEN hợp lệ bị trả 400 |
| 7 | Action của controller | Kiểm tra dữ liệu nhập, gọi service, bắt SqlException và dịch bằng LoiCSDL, trả JSON |
| 8 | MapFallbackToFile("index.html") | Đường dẫn không phải /api (ví dụ /han-muc) trả index.html để React Router hiện đúng trang khi bấm F5 |

### Bảng 5.9. Các chính sách phân quyền

| Chính sách | Điều kiện | Dùng cho |
|---|---|---|
| GhiGiaoDich | Vai trò SinhVien hoặc CapNganSach | Lịch sử giao dịch, báo cáo chi tiêu |
| QuanLyHanMuc | Vai trò CapNganSach, hoặc SinhVien không có người cấp ngân sách | Hạn mức, duyệt ngoại lệ, hủy giao dịch, báo cáo vi phạm |
| QuanTri | Vai trò QuanTri | Danh mục, người dùng |
| Roles = "SinhVien" | Vai trò SinhVien (đặt trực tiếp trên action) | Nhập giao dịch và dự báo |

### Bảng 5.10. Dịch lỗi SQL Server sang thông báo

| Mã lỗi | Nguyên nhân | Thông báo hiện cho người dùng |
|---|---|---|
| ≥ 50000 | THROW trong trigger, procedure | Giữ nguyên thông điệp, ví dụ "Hạn mức vừa bị người khác sửa..." |
| 547 | Vi phạm CHECK hoặc khóa ngoại | Theo tên ràng buộc, ví dụ CK_HanMuc_Ky: "Ngày kết thúc phải sau ngày bắt đầu." |
| 2627, 2601 | Trùng khóa UNIQUE | "Tên đăng nhập đã tồn tại.", "Tên danh mục đã tồn tại." |
| 1205 | Deadlock, SQL Server hủy transaction | "Hệ thống đang bận, vui lòng thử lại." |
| −2 | Hết thời gian chờ | "SQL Server phản hồi quá lâu, vui lòng thử lại." |
| 208, 2812 | Thiếu bảng, view hoặc procedure | Nhắc chạy lại file QuanLyHanMuc.sql |
| 18456, 4060 | Sai tài khoản SQL, sai tên database | Nhắc kiểm tra chuỗi kết nối |
| 2, 53, −1 | Không kết nối được máy chủ | Nhắc kiểm tra tên máy chủ và dịch vụ SQL Server |

### Bảng 5.11. Danh sách API của hệ thống

| Lệnh | Đường dẫn | Quyền | Use case | Việc làm |
|---|---|---|---|---|
| GET | /api/phien | Không cần đăng nhập | UC12 | Ai đang đăng nhập, cờ quyền cho menu, số thông báo chưa đọc; đặt cookie XSRF-TOKEN |
| POST | /api/dang-nhap | Không cần đăng nhập | UC12 | Chế độ Local: kiểm tra mật khẩu, tạo cookie phiên |
| GET | /xac-thuc/dang-nhap | Không cần đăng nhập | UC12 | Chế độ Oidc: chuyển trình duyệt sang Keycloak |
| POST | /xac-thuc/dang-xuat | Đã đăng nhập | UC12 | Xóa cookie phiên; chế độ Oidc đăng xuất khỏi Keycloak |
| GET | /api/trang-chu | Đã đăng nhập | UC08, UC09 | Hạn mức của tôi hoặc của các sinh viên tôi quản lý, số yêu cầu chờ duyệt |
| GET | /api/giao-dich/nhap | SinhVien | UC05 | Danh mục có hạn mức vào ngày chọn và hạn mức hiệu lực |
| GET | /api/giao-dich/danh-gia | SinhVien | UC05 | Dự báo kết quả khi đang gõ số tiền (HanMuc.DanhGiaGiaoDich) |
| POST | /api/giao-dich | SinhVien | UC05 | Ghi giao dịch qua sp_GhiNhanGiaoDich |
| GET | /api/giao-dich | GhiGiaoDich | UC06 | Lịch sử theo khoảng ngày, danh mục, trạng thái |
| GET | /api/giao-dich/{id} | QuanLyHanMuc | UC07 | Một giao dịch, để hiện trên trang hủy |
| POST | /api/giao-dich/{id}/huy | QuanLyHanMuc | UC07 | Hủy giao dịch qua sp_HuyGiaoDich |
| GET | /api/han-muc | QuanLyHanMuc | UC03 | Danh sách hạn mức lọc theo kỳ, sinh viên, trạng thái |
| GET | /api/han-muc/lua-chon | QuanLyHanMuc | UC01 | Sinh viên và danh mục để điền ô chọn trên form |
| GET | /api/han-muc/{id} | QuanLyHanMuc | UC02 | Một hạn mức kèm PhienBan |
| POST | /api/han-muc | QuanLyHanMuc | UC01 | Tạo hạn mức cho một hoặc tất cả sinh viên |
| PUT | /api/han-muc/{id} | QuanLyHanMuc | UC02 | Điều chỉnh, gửi kèm PhienBan |
| POST | /api/han-muc/{id}/khoa | QuanLyHanMuc | UC03 | Khóa hạn mức |
| POST | /api/han-muc/{id}/mo-khoa | QuanLyHanMuc | UC03 | Mở khóa, trạng thái tính lại theo số đã chi |
| GET | /api/ngoai-le | QuanLyHanMuc | UC09 | Yêu cầu đang chờ duyệt |
| POST | /api/ngoai-le/{ma} | QuanLyHanMuc | UC09 | Duyệt hoặc từ chối kèm ghi chú |
| GET | /api/thong-bao | Đã đăng nhập | UC08 | Thông báo của tôi |
| POST | /api/thong-bao/da-doc | Đã đăng nhập | UC08 | Đánh dấu một hoặc tất cả là đã đọc |
| GET | /api/bao-cao/chi-tieu | GhiGiaoDich | UC10 | Chi tiêu theo danh mục trong khoảng ngày |
| GET | /api/bao-cao/vi-pham | QuanLyHanMuc | UC11 | Các lần vượt hạn mức từ NhatKyViPham |
| GET | /api/danh-muc | QuanTri | UC04 | Danh sách danh mục |
| POST | /api/danh-muc | QuanTri | UC04 | Thêm hoặc sửa danh mục qua sp_LuuDanhMuc |
| GET | /api/nguoi-dung | QuanTri | UC13 | Danh sách người dùng |
| POST | /api/nguoi-dung | QuanTri | UC13 | Thêm hoặc sửa người dùng; chế độ Oidc ghi sang Keycloak |
| POST | /api/nguoi-dung/{id}/dat-lai-mat-khau | QuanTri | UC13 | Đặt lại mật khẩu về 123456 |

### Bảng 5.12. Hai tình huống đồng thời và cách xử lý

| Tình huống | Nguy cơ | Cách xử lý |
|---|---|---|
| Nhiều giao dịch cùng ghi vào một hạn mức | Hai request cùng đọc số đã chi 900.000 trên hạn mức cứng 1.000.000, cùng thấy còn đủ cho 100.000 và cùng ghi; tổng thành 1.100.000. | Khóa bi quan: sp_GhiNhanGiaoDich đọc dòng hạn mức với UPDLOCK, HOLDLOCK trong transaction [6]. Request sau phải chờ đến khi request trước COMMIT rồi đọc số đã chi mới. |
| Hai người cùng sửa một hạn mức | Người lưu sau ghi đè thay đổi của người lưu trước mà không biết. | Khóa lạc quan: cột PhienBan kiểu rowversion. React giữ PhienBan đọc lúc mở form và gửi lại khi lưu; khác giá trị hiện tại thì sp_DieuChinhHanMuc báo lỗi 50035. |

### Bảng 5.13. Kết quả ba bài kiểm thử đồng thời

| Bài kiểm thử | Thao tác chạy song song | Kết quả đã kiểm tra |
|---|---|---|
| HaiMuoiGiaoDichCungLuc_KhongVuotHanMucCung | 20 luồng cùng ghi 100.000 đ vào hạn mức cứng 1.000.000 đ | 10 giao dịch được ghi, 10 bị từ chối; số đã chi 1.000.000 đ; 10 giao dịch Hợp lệ; 10 dòng Từ chối trong NhatKyViPham; vw_DoiSoatHanMuc không có dòng lệch |
| GhiVaHuyXenKe_SoDaChiVanKhop | 10 lần hủy giao dịch 50.000 đ và 10 lần ghi 70.000 đ cùng lúc trên hạn mức 5.000.000 đ | Số đã chi 700.000 đ, đúng bằng tổng 10 giao dịch mới; không có dòng lệch |
| HaiNguoiCungSua_NguoiLuuSauBiBao | Hai lần lưu dùng cùng một PhienBan | Lần đầu lưu được hạn mức 1.200.000 đ; lần sau nhận lỗi 50035; hạn mức vẫn 1.200.000 đ |
