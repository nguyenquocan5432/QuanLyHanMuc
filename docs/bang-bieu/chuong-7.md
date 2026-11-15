# Bảng biểu chương 7: Triển khai và kiểm thử

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 7.1. Môi trường phát triển

| Thành phần | Sử dụng |
|---|---|
| Hệ điều hành | Windows (64-bit) |
| Máy chủ | C#, .NET 8 SDK; thư viện Microsoft.Data.SqlClient 5.2.2 và Microsoft.AspNetCore.Authentication.OpenIdConnect 8.0.11 tải qua NuGet |
| Giao diện | React 18, TypeScript, Vite; Node.js 20 và npm chỉ cần khi sửa giao diện, bản build có sẵn trong wwwroot |
| Công cụ lập trình | Visual Studio Code; chạy ứng dụng bằng dotnet run, kiểm thử bằng dotnet test trong Terminal |
| Hệ quản trị CSDL | SQL Server 2022, instance BEHIHI\MSSQLSERVER01; SQL Server Management Studio để chạy script và xem dữ liệu |
| Đóng gói | Docker Desktop 4.99 (WSL 2), Docker Compose |
| Mô hình hóa | draw.io cho toàn bộ biểu đồ trong báo cáo (file SoDo_HeThongKiemSoatHanMuc.drawio) |
| Trình duyệt | Google Chrome, Microsoft Edge |

### Bảng 7.2. Môi trường triển khai

| Thành phần | Yêu cầu |
|---|---|
| Cách 1: Docker | Máy cài Docker Desktop; ổ chứa dữ liệu Docker còn trống khoảng 10 GB cho các image SQL Server, Keycloak, .NET, Node |
| Cách 2: dotnet run | Máy Windows cài .NET 8 SDK và SQL Server 2016 SP1 trở lên; database tạo bằng file CSDL/QuanLyHanMuc.sql; chuỗi kết nối trong appsettings.json |
| Địa chỉ | Cách 1: ứng dụng http://localhost:8080, Keycloak http://localhost:8081, SQL Server localhost,14333. Cách 2: http://localhost:5080 |
| Người dùng | Trình duyệt trên máy tính hoặc điện thoại; cần Internet để tải phông chữ từ Google Fonts, không có thì trình duyệt dùng phông thay thế |

### Bảng 7.3. Cấu trúc thư mục của project QuanLyHanMucWeb

| Thư mục, file | Nội dung |
|---|---|
| Controllers/ | 10 API controller: CoSoController (lớp cha), PhienController, TrangChuController, GiaoDichController, HanMucController, NgoaiLeController, ThongBaoController, BaoCaoController, DanhMucController, NguoiDungController |
| Services/ | TaiKhoanService, HanMucService, GiaoDichService, NgoaiLeService, ThongBaoService, BaoCaoService, QuanTriService, KeycloakQuanTri |
| Models/HanMuc.cs | Lớp HanMuc với DanhGiaGiaoDich, enum KetQuaDanhGia, KetQuaGhiNhan, NguoiDungDangNhap |
| Data/ | KetNoi (mở kết nối, tạo tham số, gọi procedure), LoiCSDL (dịch lỗi SQL) |
| Infrastructure/ | NguoiDungHienTai (đọc claim, tên chính sách), Bang (DataTable sang JSON), XacThuc và CauHinhOidc (đăng nhập OIDC) |
| Program.cs | Khai báo dịch vụ, chính sách phân quyền và thứ tự middleware |
| client/ | Mã nguồn React: src/App.tsx (bảng đường dẫn), src/BoCuc.tsx (khung trang), src/api.ts, src/trang/ (13 trang) |
| wwwroot/ | Bản build của React do npm run build sinh ra; máy chủ .NET phục vụ thẳng thư mục này |
| CSDL/QuanLyHanMuc.sql | Script tạo database, bảng, trigger, view, procedure, dữ liệu mẫu và phần demo kiểm thử bằng tay |
| Tests/ | Project xUnit: CoSoDuLieuThu (dựng database test), DanhGiaGiaoDichTests, NghiepVuTests, DongThoiTests, ApiTests |
| keycloak/hanmuc-realm.json | Realm hanmuc nạp vào Keycloak khi chạy Docker |
| Dockerfile, docker-compose.yml | Đóng gói và dựng toàn bộ hệ thống |
| appsettings.json | Chuỗi kết nối SQL Server khi chạy dotnet run; không đưa vào image Docker |

### Bảng 7.4. Các bước chạy thử hệ thống bằng Docker

| Bước | Thao tác | Kết quả |
|---|---|---|
| 1 | Mở Docker Desktop, chờ dòng Engine running | Docker sẵn sàng |
| 2 | Chép .env.example thành .env, điền mật khẩu, rồi chạy docker compose up --build | Lần đầu tải image và build; csdl-khoi-tao chạy script rồi dừng; web in "Content root path: /app" |
| 3 | Mở http://localhost:8080, bấm Đăng nhập | Trình duyệt chuyển sang trang Keycloak ở localhost:8081 |
| 4 | Đăng nhập tuquanly1 / 123456 | Về trang chủ của Nguyễn An Bình, hạn mức Ăn uống 1.000.000 đ |
| 5 | Đăng xuất, đăng nhập sinhvien1, gõ thẳng /han-muc | Trang Không có quyền truy cập |

### Bảng 7.5. Kết quả chạy thử chức năng trên Docker

| STT | Thao tác | Kết quả quan sát được |
|---|---|---|
| 1 | docker compose up --build (đã có file .env) | Bốn container được dựng; csdl-khoi-tao tạo database rồi dừng; web in "Content root path: /app" |
| 2 | Đăng nhập tuquanly1 qua trang Keycloak | Về trang chủ của Nguyễn An Bình, vai trò Sinh viên (tự quản lý), hạn mức Ăn uống 1.000.000 đ, đã chi 0 đ |
| 3 | Ghi 850.000 đ rồi 200.000 đ vào Ăn uống, duyệt giao dịch thứ hai | Đã chi 1.050.000 đ (105%), trạng thái Đã vượt, còn lại -50.000 đ, chuông có 3 thông báo |
| 4 | Đăng nhập sinhvien1 | Menu chỉ có Nhập giao dịch, Lịch sử, Báo cáo; hạn mức Ăn uống 92% Sắp vượt, Mua sắm trực tuyến 70%, Đi lại Đã khóa không có nút Nhập |
| 5 | sinhvien1 gõ thẳng /han-muc | Trang Không có quyền truy cập ở đường dẫn /khong-co-quyen |

### Bảng 7.6. Một số lỗi thường gặp khi chạy bằng Docker

| Hiện tượng | Cách xử lý |
|---|---|
| Cài Docker Desktop báo "There is not enough space on the disk" | Ổ C: thiếu chỗ. Dù cài vào ổ khác, bộ cài vẫn giải nén tạm và chạy WSL trên ổ C:, nên cần dọn ổ C: trước |
| PowerShell báo lỗi khi gõ cd /d D:\... | Tham số /d chỉ có trong Command Prompt. Trong PowerShell gõ cd D:\... là đủ |
| "no configuration file provided: not found" | Đang đứng sai thư mục; phải chạy lệnh ở thư mục có file docker-compose.yml |
| Bấm Đăng nhập báo lỗi ngay sau khi chạy | Keycloak chưa khởi động xong (lần đầu mất khoảng một phút); chờ thêm rồi thử lại |
| Cổng 8080 hoặc 8081 đã bị dùng | Tắt chương trình đang dùng cổng đó, hoặc sửa cổng bên trái dấu hai chấm trong docker-compose.yml |

### Bảng 7.7. Các nhóm kiểm thử tự động

| Lớp kiểm thử | Loại | Số ca | Nội dung |
|---|---|---|---|
| DanhGiaGiaoDichTests | Đơn vị | 10 | Lớp HanMuc không cần CSDL: các mức 75%, đúng 80%, đúng 100%, vượt cứng, vượt mềm, hạn mức khóa; tỷ lệ sử dụng (3 bộ số); số còn lại âm |
| NghiepVuTests | Tích hợp | 11 | Gọi service xuống procedure và trigger thật: hạn mức cứng đi hết các mức, hạn mức mềm chờ duyệt rồi duyệt, từ chối phải có lý do, trùng kỳ (50001), sinh viên có người cấp không tự tạo hạn mức (50031), khóa và mở khóa, hạ hạn mức dưới số đã chi (50037), hủy giao dịch (50021, 50023), INSERT thẳng vào bảng bị trigger chặn (50004), đối soát |
| DongThoiTests | Đồng thời | 3 | 20 giao dịch 100.000 đ ghi cùng lúc vào hạn mức cứng 1.000.000 đ; 10 lần hủy và 10 lần ghi xen kẽ; hai người cùng sửa một hạn mức |
| ApiTests | API | 8 | Chạy cả ứng dụng trong bộ nhớ bằng WebApplicationFactory: chưa đăng nhập trả 401, sai mật khẩu và sai tên trả cùng câu báo, thiếu mã chống giả mạo trả 400, cờ quyền theo vai trò, sinh viên có người cấp và phụ huynh và quản trị viên bị 403 ở API không thuộc vai trò, ghi giao dịch qua API |

### Bảng 7.8. Chi tiết 32 ca kiểm thử tự động

| STT | Tên ca kiểm thử | Dữ liệu và thao tác | Kết quả mong đợi |
|---|---|---|---|
| 1 | DuoiNguong_ChapNhan | Cứng 2.000.000, đã chi 1.400.000, ghi 100.000 (75%) | ChapNhan |
| 2 | ChamNguong_ChapNhanKemCanhBao | Như trên, ghi 200.000 (đúng 80%) | ChapNhanCanhBao |
| 3 | DungMotTram_ChapNhanDungMuc | Như trên, ghi 600.000 (đúng 100%) | ChapNhanDungMuc (BR04) |
| 4 | VuotHanMucCung_TuChoi | Hạn mức cứng, ghi 601.000 | TuChoi |
| 5 | VuotHanMucMem_ChoDuyet | Hạn mức mềm, ghi 601.000 | ChoDuyet |
| 6 | HanMucDaKhoa_KhongGhiDuoc | Hạn mức Đã khóa, ghi 1.000 | DaKhoa (BR06) |
| 7–9 | TyLeSuDung_TinhDung | Đã chi 1.400.000; 1.380.000; 0 trên 2.000.000 | 70; 69; 0 |
| 10 | SoConLai_AmKhiHanMucMemDaVuot | Mềm 2.000.000, đã chi 2.130.000 | −130.000 |
| 11 | HanMucCung_DiHetCacMuc | Cứng 1.000.000: ghi 700.000, 100.000, 300.000, 200.000, 1.000 | Chấp nhận; kèm cảnh báo; Từ chối; đúng mức; Từ chối. Đã chi 1.000.000, 2 dòng vi phạm, 3 giao dịch Hợp lệ |
| 12 | HanMucMem_VuotThiChoDuyet_DuyetXongMoiCong | Mềm 1.000.000: ghi 600.000 rồi 500.000; sinh viên tự duyệt; phụ huynh duyệt hai lần | Chờ duyệt, đã chi giữ 600.000, vượt 100.000; lỗi 50014; đã chi 1.100.000, Đã vượt; lỗi 50013 |
| 13 | TuChoiNgoaiLe_PhaiCoLyDo_VaKhongCongSoDaChi | Mềm 500.000, ghi 600.000; từ chối không lý do, rồi có lý do | Lỗi 50011; sau đó đã chi vẫn 0, giao dịch Từ chối |
| 14 | TaoHanMucTrungKy_BiChan | Tạo hai hạn mức cùng sinh viên, danh mục, kỳ | Lỗi 50001 từ trigger (BR01) |
| 15 | SinhVienCoNguoiCap_KhongTuTaoHanMuc | sinhvien1 tự tạo hạn mức cho mình | Lỗi 50031 (BR08) |
| 16 | SinhVienTuQuanLy_TuTaoHanMucDuoc | tuquanly1 tự tạo hạn mức cho mình | Tạo được, trả mã hạn mức |
| 17 | KhoaHanMuc_ChanGiaoDich_MoKhoaTinhLaiTrangThai | Đã chi 85%; sinh viên khóa; phụ huynh khóa, sinh viên ghi, phụ huynh mở | Lỗi 50034; Từ chối; mở ra Sắp vượt, không về Hoạt động |
| 18 | HaHanMucCungDuoiSoDaChi_BiChan | Đã chi 700.000, hạ hạn mức cứng xuống 600.000 | Lỗi 50037 |
| 19 | HuyGiaoDich_TruLaiSoDaChi_HuyLanHaiBiChan | Ghi 500.000 và 400.000; hủy không lý do; hủy có lý do; hủy lại | Lỗi 50021; đã chi về 500.000, Hoạt động; lỗi 50023 |
| 20 | GhiThangVaoBang_VanBiTriggerChan | Đã chi 900.000, INSERT thẳng 200.000 Hợp lệ | Lỗi 50004, đã chi giữ 900.000 |
| 21 | SauMoiBaiTest_SoDaChiKhopTongGiaoDich | Đếm dòng ChenhLech khác 0 trong vw_DoiSoatHanMuc | 0 |
| 22 | HaiMuoiGiaoDichCungLuc_KhongVuotHanMucCung | 20 luồng cùng ghi 100.000 vào hạn mức cứng 1.000.000 | 10 chấp nhận, 10 từ chối, đã chi đúng 1.000.000, không lệch |
| 23 | GhiVaHuyXenKe_SoDaChiVanKhop | 10 lần hủy giao dịch 50.000 chạy song song với 10 lần ghi 70.000 | Đã chi 700.000, không lệch |
| 24 | HaiNguoiCungSua_NguoiLuuSauBiBao | Hai người đọc cùng PhienBan, lần lượt lưu | Người đầu lưu được; người sau lỗi 50035; hạn mức giữ 1.200.000 |
| 25 | ChuaDangNhap_GoiApi_Tra401 | GET /api/trang-chu, /api/han-muc khi chưa đăng nhập | 401 |
| 26 | SaiMatKhau_BaoCungMotCau | Sai mật khẩu; tên không tồn tại | Cả hai 400 với cùng một nội dung |
| 27 | ThieuMaChongGiaMao_BiTuChoi | POST /api/thong-bao/da-doc không có X-XSRF-TOKEN | 400 |
| 28 | PhienTraVeDungQuyenTheoVaiTro | GET /api/phien với sinhvien1 và tuquanly1 | Chỉ tuquanly1 có duocQuanLyHanMuc = true |
| 29 | SinhVienCoNguoiCap_KhongVaoDuocQuanLyHanMuc | sinhvien1 gọi hạn mức, ngoại lệ, báo cáo vi phạm, hủy giao dịch | 403 ở cả bốn |
| 30 | NguoiCapNganSach_KhongNhapDuocGiaoDich | phuhuynh1 gọi /api/giao-dich/nhap và POST /api/giao-dich | 403; /api/han-muc vẫn 200 |
| 31 | QuanTriVien_ChiVaoDuocDanhMucVaNguoiDung | admin gọi danh mục, người dùng, giao dịch, hạn mức; sinhvien1 gọi người dùng | 200, 200, 403, 403; 403 |
| 32 | SinhVienGhiGiaoDichQuaApi_NhanKetQuaCuaProcedure | tuquanly1 ghi 25.000 vào hạn mức mềm 1.000.000 qua POST /api/giao-dich | 200, ketQua "Chấp nhận", muc "thanh-cong" |

### Bảng 7.9. Kịch bản kiểm thử bằng tay trên CSDL

| Khối | Nội dung | Kết quả mong đợi |
|---|---|---|
| [0] | Tình trạng ban đầu của dữ liệu mẫu | HM1 70% Hoạt động, HM2 92% Sắp vượt, HM4 Đã khóa |
| A1 | Thêm người dùng vai trò GiaoVien | Msg 547, CK_NguoiDung_VaiTro |
| A2 | Thêm tên đăng nhập sinhvien1 lần nữa | Msg 2627, UQ_NguoiDung_TenDangNhap |
| A3 | Xóa danh mục đang có hạn mức | Msg 547, FK_HanMuc_DanhMuc |
| A4 | Ghi thẳng số đã chi của HM1 vượt 2.000.000 | Msg 547, CK_HanMuc_CungKhongVuot |
| A5 | Ngày kết thúc trước ngày bắt đầu | Msg 547, CK_HanMuc_Ky |
| A6, A7 | Hủy không có lý do; số tiền giao dịch bằng 0 | Msg 547, CK_GiaoDich_HuyPhaiCoLyDo; CK_GiaoDich_SoTien |
| B1 | Tạo hạn mức thứ hai cùng kỳ cho Khoa, Mua sắm trực tuyến (BR01) | Msg 50001 |
| B2, B3 | Giao dịch ngoài kỳ; giao dịch vào hạn mức đang khóa (BR06) | Msg 50002; Msg 50003 |
| B4 | INSERT thẳng 700.000 vào HM1, bỏ qua procedure (BR04) | Msg 50004 |
| B5 | Đối soát sau các lỗi trên | Cột ChenhLech toàn 0 |
| C1 | Khoa chi 700.000 vào HM1 (lên 105%) | Từ chối, còn 600.000 đ; có dòng NhatKyViPham, không có dòng GiaoDich mới |
| C2, C3 | Chi 100.000 rồi thêm 100.000 | Chấp nhận 75%; Chấp nhận kèm cảnh báo 80%, HM1 Sắp vượt, có thông báo Ngưỡng |
| C4, C5 | Chi thêm 400.000 (đúng 100%), rồi 1.000 | Chấp nhận đúng mức, HM1 Đã vượt; sau đó Từ chối, còn 0 đ |
| C6 | Số tiền 0; danh mục chưa có hạn mức | Hai lần Từ chối |
| D1, D2 | Chi 250.000 vào HM2 mềm (lên 1.630.000) | Chờ duyệt, số đã chi vẫn 1.380.000; một yêu cầu vượt 130.000, phụ huynh có thông báo |
| D3 | Sinh viên tự duyệt yêu cầu của mình (BR08) | Msg 50014 |
| D4, D5 | Phụ huynh duyệt; duyệt lại lần nữa | HM2 1.630.000 (108,67%) Đã vượt; Msg 50013 |
| D6, D7 | Từ chối không ghi lý do; từ chối có lý do | Msg 50011; Từ chối, số đã chi không đổi |
| E1 | Hủy giao dịch không ghi lý do | Msg 50021 |
| E2, E3 | Hủy giao dịch 400.000 của HM1; hủy lại lần nữa | HM1 1.600.000, 80% Sắp vượt; Msg 50023 |
| F1, F2 | Sinh viên Hà tự khóa hạn mức; phụ huynh khóa, Hà ghi giao dịch, phụ huynh mở lại | Msg 50034; Đã khóa, Từ chối, Hoạt động |
| G1, G2 | Phụ huynh tạo hạn mức Học tập cho Khoa; Bình tự tạo cho mình; Khoa tự tạo | Hai lệnh đầu thành công; lệnh cuối Msg 50031 |
| G3, G4 | Nâng hạn mức Đi lại của Hà từ 500.000 lên 600.000, xem nhật ký | Nhật ký có giá trị trước 500000, sau 600000 |
| G5 | Hạ hạn mức cứng HM1 xuống dưới số đã chi | Msg 50037 |
| G6 | Lưu bằng PhienBan cũ | Msg 50035 |
| G7, G8 | Sinh viên thêm danh mục; quản trị viên tự khóa tài khoản mình | Msg 50041; Msg 50046 |
| H1–H4 | Hai cửa sổ SSMS cùng ghi vào HM3 | Cửa sổ 2 chờ khóa đến khi cửa sổ 1 COMMIT rồi đọc số liệu mới |
| I1 | Hai người cùng sửa ngưỡng HM2 | Người sửa sau cập nhật 0 dòng; ngưỡng giữ 90 của người sửa trước |
| K1 | Tìm hạn mức hiệu lực | Kế hoạch thực thi là Index Seek trên IX_HanMuc_TimHieuLuc |
| L1–L3 | Báo cáo chi tiêu, báo cáo vi phạm, đối soát cuối buổi | ChenhLech toàn 0 |
