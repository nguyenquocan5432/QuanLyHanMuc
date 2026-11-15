# Bảng biểu chương 1: Khảo sát và yêu cầu

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 1.1. Hạn chế của cách theo dõi chi tiêu hiện nay

| Hạn chế | Biểu hiện |
|---|---|
| Không phân loại theo danh mục | Ngân hàng và ví điện tử chỉ ghi từng giao dịch. Muốn biết đã chi bao nhiêu cho ăn uống, sinh viên phải tự lọc và cộng tay. |
| Không có điểm chặn trước khi chi | Hạn mức chỉ tồn tại trong ý định của người chi, và ý định này phải cạnh tranh với khuyến mãi và thanh toán một chạm. |
| Không cảnh báo trước khi chạm trần | Sinh viên chỉ nhận ra đã vượt khi số dư cạn, lúc đó chỉ còn cách cắt giảm các khoản sau. |
| Người cấp ngân sách không thấy được giữa kỳ | Phụ huynh chỉ can thiệp được sau khi tiền đã chi. Một khoản lớn trong đêm khuyến mãi không có cơ chế dừng lại để hỏi. |

### Bảng 1.2. So sánh cách theo dõi hiện nay với hệ thống đề xuất

| Tiêu chí | Ghi chú, ứng dụng ngân hàng | Bảng tính tự lập | Hệ thống đề xuất |
|---|---|---|---|
| Phân loại khoản chi | Không có, hoặc theo loại giao dịch của ngân hàng | Có, nếu người dùng tự nhập đều | Bắt buộc chọn danh mục khi nhập |
| Hạn mức theo danh mục | Không có | Tự viết công thức | Có, theo từng sinh viên, danh mục, kỳ |
| Kiểm tra trước khi ghi | Không | Không | Có, so với hạn mức ngay lúc nhập |
| Cảnh báo | Không | Tô màu nếu tự cài | Dải cảnh báo khi đang gõ, thông báo khi chạm ngưỡng |
| Người cấp ngân sách tham gia | Không | Phải gửi file | Duyệt khoản vượt hạn mức mềm, xem báo cáo |
| Lịch sử thay đổi | Không | Không | Nhật ký ghi người thực hiện, thời điểm, giá trị cũ và mới |

### Bảng 1.3. Các tác nhân của hệ thống

| Tác nhân | Vai trò | Mô tả |
|---|---|---|
| Sinh viên | SinhVien | Người chi tiêu. Ghi giao dịch, xem hạn mức còn lại, lịch sử, báo cáo và thông báo của chính mình. Không tự sửa hạn mức nếu đã có người cấp ngân sách. |
| Sinh viên tự quản lý | SinhVien | Sinh viên không có người cấp ngân sách. Làm được mọi việc của sinh viên, thêm vào đó tự thiết lập, điều chỉnh, khóa hạn mức và tự duyệt khoản vượt hạn mức mềm của mình. |
| Người cấp ngân sách | CapNganSach | Phụ huynh hoặc người giám hộ. Thiết lập và điều chỉnh hạn mức cho các sinh viên mình cấp ngân sách, duyệt ngoại lệ, hủy giao dịch, xem lịch sử và báo cáo. Không nhập giao dịch. |
| Quản trị viên | QuanTri | Quản lý danh mục chi tiêu và tài khoản người dùng. Không ghi giao dịch, không quản lý hạn mức. |

### Bảng 1.4. Các nhóm nghiệp vụ

| Nhóm | Nội dung | Use case | Loại |
|---|---|---|---|
| 1. Quản lý hạn mức | Tạo, sửa, khóa, mở khóa hạn mức theo sinh viên, danh mục, kỳ; quản lý danh mục chi tiêu | UC01–UC04 | Cốt lõi, ghi dữ liệu |
| 2. Theo dõi giao dịch | Ghi nhận giao dịch, cộng vào số đã chi của hạn mức, xem lịch sử, hủy giao dịch | UC05–UC07 | Cốt lõi, ghi dữ liệu |
| 3. Cảnh báo và ngoại lệ | Tạo thông báo khi chạm ngưỡng, duyệt khoản vượt hạn mức mềm | UC08, UC09 | Phát sinh từ nhóm 2 |
| 4. Báo cáo | Chi tiêu theo kỳ, vi phạm hạn mức | UC10, UC11 | Chỉ đọc |
| Dùng chung | Đăng nhập, phân quyền, quản lý người dùng | UC12, UC13 | Hỗ trợ |

### Bảng 1.5. Kết quả xử lý theo tổng chi dự kiến

| Giao dịch mới | Tổng chi dự kiến | Tỷ lệ | Hạn mức cứng | Hạn mức mềm | Trạng thái hạn mức sau xử lý |
|---|---|---|---|---|---|
| 100.000 | 1.500.000 | 75% | Chấp nhận | Chấp nhận | Hoạt động |
| 200.000 | 1.600.000 | 80% | Chấp nhận kèm cảnh báo | Chấp nhận kèm cảnh báo | Sắp vượt |
| 600.000 | 2.000.000 | 100% | Chấp nhận đúng mức | Chấp nhận đúng mức | Đã vượt |
| 700.000 | 2.100.000 | 105% | Từ chối | Chờ duyệt | Giữ nguyên |

### Bảng 1.6. Các module của hệ thống

| Mã | Module | Chức năng | Thành phần chính |
|---|---|---|---|
| M1 | Xác thực và phân quyền | Đăng nhập, đăng xuất, kiểm tra vai trò khi mở từng trang | PhienController, TaiKhoanService, NguoiDungHienTai, CauHinhOidc, ba chính sách trong Program.cs, DangNhap.tsx |
| M2 | Quản lý hạn mức | Thiết lập, điều chỉnh, khóa, mở khóa hạn mức; danh sách hạn mức | HanMucController, HanMucService, sp_TaoHanMuc, sp_DieuChinhHanMuc, sp_KhoaMoKhoaHanMuc |
| M3 | Ghi nhận và kiểm tra giao dịch | Nhập giao dịch, dự báo mức cảnh báo khi đang gõ, ghi giao dịch có khóa dòng | GiaoDichController, GiaoDichService, HanMuc.DanhGiaGiaoDich, sp_GhiNhanGiaoDich, NhapGiaoDich.tsx |
| M4 | Ngoại lệ và hủy giao dịch | Duyệt hoặc từ chối khoản vượt hạn mức mềm; hủy giao dịch kèm lý do | NgoaiLeController, NgoaiLeService, sp_DuyetNgoaiLe, sp_HuyGiaoDich |
| M5 | Thông báo | Đếm thông báo chưa đọc trên menu, xem và đánh dấu đã đọc | ThongBaoController, ThongBaoService, bảng ThongBao |
| M6 | Lịch sử và báo cáo | Lịch sử giao dịch có bộ lọc, báo cáo chi tiêu theo kỳ, báo cáo vi phạm | GiaoDichController.LichSu, BaoCaoController, BaoCaoService |
| M7 | Quản trị và lưu trữ | Danh mục, người dùng, nhật ký hệ thống, ràng buộc và trigger của CSDL | DanhMucController, NguoiDungController, QuanTriService, KeycloakQuanTri, KetNoi, LoiCSDL, trigger, view |

### Bảng 1.7. Danh sách use case

| Mã | Tên use case | Tác nhân | Module |
|---|---|---|---|
| UC01 | Thiết lập hạn mức | Người cấp ngân sách, sinh viên tự quản lý | M2 |
| UC02 | Điều chỉnh hạn mức | Người cấp ngân sách, sinh viên tự quản lý | M2 |
| UC03 | Khóa / mở khóa hạn mức | Người cấp ngân sách, sinh viên tự quản lý | M2 |
| UC04 | Quản lý danh mục chi tiêu | Quản trị viên | M7 |
| UC05 | Ghi nhận giao dịch và kiểm tra hạn mức | Sinh viên (kể cả tự quản lý) | M3 |
| UC06 | Xem lịch sử giao dịch | Sinh viên, người cấp ngân sách | M6 |
| UC07 | Hủy giao dịch | Người cấp ngân sách, sinh viên tự quản lý | M4 |
| UC08 | Nhận và xem thông báo | Sinh viên, người cấp ngân sách | M5 |
| UC09 | Duyệt ngoại lệ vượt hạn mức | Người cấp ngân sách, sinh viên tự quản lý | M4 |
| UC10 | Báo cáo chi tiêu theo kỳ | Sinh viên, người cấp ngân sách | M6 |
| UC11 | Báo cáo vi phạm hạn mức | Người cấp ngân sách, sinh viên tự quản lý | M6 |
| UC12 | Đăng nhập và phân quyền | Tất cả | M1 |
| UC13 | Quản lý người dùng | Quản trị viên | M7 |

### Bảng 1.8. Quy tắc nghiệp vụ

| Mã | Quy tắc | Nơi kiểm tra |
|---|---|---|
| BR01 | Một hạn mức xác định bởi bộ ba (sinh viên, danh mục, kỳ). Không có hai hạn mức chưa khóa trùng sinh viên, danh mục mà kỳ giao nhau. | Trigger TR_HanMuc_ChongTrungKy (mã lỗi 50001) |
| BR02 | Tỷ lệ sử dụng = số đã chi ÷ số tiền hạn mức × 100%. Số đã chi chỉ cộng giao dịch Hợp lệ. | View vw_TinhTrangHanMuc, phương thức HanMuc.TyLeSuDung() |
| BR03 | Tỷ lệ đạt hoặc vượt ngưỡng cảnh báo (mặc định 80%, chỉnh được 1–100%) thì hạn mức chuyển "Sắp vượt" và sinh viên nhận thông báo. | Hàm fn_TinhTrangThai, sp_GhiNhanGiaoDich |
| BR04 | Giao dịch làm tổng chi vượt 100% hạn mức bị chặn. Tổng chi bằng đúng 100% vẫn được chấp nhận. | sp_GhiNhanGiaoDich, trigger TR_GiaoDich_BaoVe (50004), CHECK CK_HanMuc_CungKhongVuot |
| BR05 | Hạn mức cứng từ chối giao dịch ở BR04. Hạn mức mềm chuyển giao dịch đó thành yêu cầu chờ người quản lý duyệt. | sp_GhiNhanGiaoDich, sp_DuyetNgoaiLe |
| BR06 | Hạn mức đang khóa không nhận giao dịch mới và không duyệt được ngoại lệ. | sp_GhiNhanGiaoDich, trigger TR_GiaoDich_BaoVe (50003), sp_DuyetNgoaiLe (50015) |
| BR07 | Số tiền giao dịch lớn hơn 0. Giao dịch đã ghi không sửa số tiền, chỉ được hủy kèm lý do. | CHECK CK_GiaoDich_SoTien, CK_GiaoDich_HuyPhaiCoLyDo, sp_HuyGiaoDich (50021) |
| BR08 | Chỉ người quản lý hạn mức được thiết lập, điều chỉnh, khóa hạn mức, hủy giao dịch và duyệt ngoại lệ. Sinh viên chỉ ghi giao dịch của chính mình. | Hàm fn_CoQuyenQuanLy trong các SP, chính sách phân quyền trên API |

### Bảng 1.9. Yêu cầu phi chức năng

| Yêu cầu | Nội dung | Cách đáp ứng |
|---|---|---|
| Đúng đắn dữ liệu | Số đã chi của mỗi hạn mức luôn bằng tổng giao dịch Hợp lệ. | Ghi giao dịch và cập nhật số đã chi trong cùng một transaction; view vw_DoiSoatHanMuc để đối soát (cột ChenhLech phải bằng 0). |
| Xử lý đồng thời | Hai giao dịch của cùng hạn mức gửi cùng lúc không làm tổng vượt hạn mức cứng; hai người sửa cùng hạn mức không ghi đè nhau. | Khóa dòng UPDLOCK, HOLDLOCK khi ghi giao dịch; cột rowversion PhienBan khi điều chỉnh hạn mức. Kiểm thử bằng 20 luồng ghi song song (mục 7.2.2). |
| Bảo mật | Chỉ người đúng vai trò mới gọi được API; chống SQL injection và giả mạo yêu cầu. | Đăng nhập OIDC qua Keycloak; trình duyệt chỉ giữ cookie HttpOnly, không giữ token; chính sách phân quyền trên API; SqlCommand có tham số; mã chống giả mạo trong header X-XSRF-TOKEN của mọi request ghi. |
| Truy vết | Biết ai làm gì, lúc nào, giá trị trước và sau. | Bảng NhatKyHeThong, giá trị lưu dạng JSON bằng FOR JSON. |
| Hiệu năng | Trang nhập giao dịch tìm hạn mức hiệu lực nhanh khi dữ liệu nhiều kỳ. | Chỉ mục IX_HanMuc_TimHieuLuc có INCLUDE; chỉ mục có lọc cho yêu cầu chờ duyệt. |
| Dễ dùng | Biết trước kết quả trước khi bấm Ghi nhận. | Dải cảnh báo đổi màu ngay khi gõ số tiền; số tiền, ngày hiển thị theo định dạng Việt Nam. |
| Bảo trì | Sửa một phần không kéo theo phần khác. | Giao diện React tách khỏi API; phía máy chủ chia tầng Controller, Service, Data; mỗi UC có service và SP riêng. |
| Triển khai | Chạy được trên máy khác mà không cài tay từng thành phần. | docker compose dựng web, SQL Server, Keycloak; script CSDL tự chạy ở lần đầu. |

### Bảng 1.10. Các quyết định triển khai

| Vấn đề | Quyết định | Lý do |
|---|---|---|
| Nền tảng | Giao diện React gọi Web API ASP.NET Core (.NET 8), thay cho WinForms như thiết kế giữa kỳ | Phụ huynh và sinh viên ở hai nơi khác nhau, dùng trình duyệt không phải cài đặt; phía máy chủ vẫn viết bằng C#. Đây là bộ công nghệ đề bài gợi ý. |
| Hệ quản trị CSDL | SQL Server 2022 thay cho PostgreSQL trong gợi ý của đề | CSDL đã thiết kế trên SQL Server từ giữa kỳ; trigger, stored procedure và gợi ý khóa viết bằng T-SQL. |
| Đăng nhập | OpenID Connect với Keycloak khi chạy Docker; chế độ Local (kiểm tra mật khẩu trong bảng NguoiDung) khi chạy dotnet run | Đề yêu cầu OIDC. Chế độ Local giúp chạy thử nhanh không cần Keycloak. Vai trò và người cấp ngân sách vẫn lấy từ bảng NguoiDung trong cả hai chế độ. |
| Nơi giữ phiên đăng nhập | Máy chủ giữ token, trình duyệt chỉ có cookie HttpOnly | Mã JavaScript trên trang không đọc được phiên đăng nhập. |
| Đóng gói | Docker Compose với ba dịch vụ web, csdl, keycloak | Một lệnh dựng đủ môi trường giống nhau trên mọi máy. |
| Nơi đặt nghiệp vụ ghi | Mọi thao tác ghi đi qua stored procedure | Kiểm tra quyền, kiểm tra hạn mức và ghi dữ liệu nằm trong một transaction ở gần dữ liệu nhất. |
| Chốt chặn cuối | Trigger và CHECK trên bảng | Dữ liệu vẫn đúng khi ai đó ghi thẳng vào bảng hoặc mã C# có lỗi. |
| Ghi giao dịch đồng thời | Khóa bi quan UPDLOCK, HOLDLOCK | Giao dịch đến sau phải chờ và được kiểm tra lại với số đã chi mới. |
| Sửa hạn mức đồng thời | Khóa lạc quan bằng cột rowversion | Người sửa mở form lâu; khóa dòng suốt thời gian đó là không hợp lý. |
| Kiểu số tiền | decimal(18,0) | Đồng Việt Nam không có phần lẻ; float làm tròn sai khi cộng dồn. |
| Xóa dữ liệu | Không xóa người dùng, danh mục; dùng cột HoatDong | Hạn mức và giao dịch là lịch sử cần giữ; khóa ngoại không xóa dây chuyền. |
| Thông báo | Ghi vào bảng ThongBao, hiện trên chuông ở menu | Không phụ thuộc máy chủ thư khi demo; cột TinhTrangGui để dành cho kênh email sau này. |
| Ai nhập giao dịch | Chỉ vai trò SinhVien | Người cấp ngân sách không chi tiêu thay sinh viên; chặn ở cả menu và API. |
