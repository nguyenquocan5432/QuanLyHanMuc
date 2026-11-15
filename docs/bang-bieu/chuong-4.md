# Bảng biểu chương 4: Cơ sở dữ liệu

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 4.1. Cấu trúc bảng NguoiDung

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaNguoiDung | int IDENTITY | PK |  | Mã người dùng |
| TenDangNhap | varchar(50) |  | NOT NULL, UQ_NguoiDung_TenDangNhap | Tên đăng nhập, chữ không dấu |
| MatKhauMaHoa | varchar(200) |  | NOT NULL | Mật khẩu băm SHA-256, chuỗi hex in hoa |
| HoTen | nvarchar(100) |  | NOT NULL | Họ tên |
| Email | varchar(100) |  | NULL | Email liên hệ |
| VaiTro | nvarchar(20) |  | CK_NguoiDung_VaiTro: QuanTri, CapNganSach, SinhVien | Vai trò |
| MaNguoiCapNganSach | int | FK | NULL; CK_NguoiDung_KhongTuCap; quản trị viên luôn NULL | Người cấp ngân sách của sinh viên; NULL là tự quản lý |
| HoatDong | bit |  | DEFAULT 1 | Tài khoản còn dùng |
| NgayTao | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm tạo |

### Bảng 4.2. Cấu trúc bảng DanhMuc

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaDanhMuc | int IDENTITY | PK |  | Mã danh mục |
| TenDanhMuc | nvarchar(100) |  | NOT NULL, UQ_DanhMuc_TenDanhMuc | Tên danh mục |
| MoTa | nvarchar(255) |  | NULL | Mô tả |
| HoatDong | bit |  | DEFAULT 1 | Còn dùng khi tạo hạn mức mới |

### Bảng 4.3. Cấu trúc bảng HanMuc

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaHanMuc | int IDENTITY | PK |  | Mã hạn mức |
| MaSinhVien | int | FK | NOT NULL | Sinh viên được cấp hạn mức |
| MaDanhMuc | int | FK | NOT NULL | Danh mục |
| NgayBatDau | date |  | NOT NULL | Ngày đầu kỳ |
| NgayKetThuc | date |  | CK_HanMuc_Ky: sau ngày bắt đầu | Ngày cuối kỳ |
| SoTienHanMuc | decimal(18,0) |  | CK_HanMuc_SoTien: > 0 | Số tiền tối đa |
| SoTienDaChi | decimal(18,0) |  | DEFAULT 0; CK_HanMuc_DaChi: ≥ 0 | Tổng giao dịch Hợp lệ |
| NguongCanhBao | decimal(5,2) |  | DEFAULT 80; CK_HanMuc_Nguong: 1–100 | Ngưỡng cảnh báo (%) |
| LoaiHanMuc | nvarchar(10) |  | CK_HanMuc_Loai: Cứng, Mềm | Loại hạn mức |
| TrangThai | nvarchar(20) |  | DEFAULT Hoạt động; CK_HanMuc_TrangThai | Hoạt động, Sắp vượt, Đã vượt, Đã khóa |
| MaNguoiTao | int | FK | NOT NULL | Người tạo |
| NgayTao | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm tạo |
| PhienBan | rowversion |  | Tự đổi mỗi lần dòng bị sửa | Dùng cho khóa lạc quan |

### Bảng 4.4. Cấu trúc bảng GiaoDich

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaGiaoDich | int IDENTITY | PK |  | Mã giao dịch |
| MaHanMuc | int | FK | NOT NULL | Hạn mức chứa giao dịch |
| SoTien | decimal(18,0) |  | CK_GiaoDich_SoTien: > 0 | Số tiền |
| NgayGiaoDich | date |  | Trong kỳ của hạn mức (trigger) | Ngày chi |
| MoTa | nvarchar(255) |  | NULL | Mô tả |
| TrangThai | nvarchar(20) |  | CK_GiaoDich_TrangThai | Hợp lệ, Chờ duyệt, Từ chối, Đã hủy |
| NgayTao | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm ghi |
| LyDoHuy | nvarchar(255) |  | NULL | Lý do hủy |
| MaNguoiHuy | int | FK | NULL | Người hủy |
| NgayHuy | datetime2(0) |  | NULL | Thời điểm hủy |

### Bảng 4.5. Cấu trúc bảng YeuCauNgoaiLe

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaYeuCau | int IDENTITY | PK |  | Mã yêu cầu |
| MaGiaoDich | int | FK | UQ_YeuCauNgoaiLe_MaGiaoDich | Giao dịch cần duyệt (quan hệ 1–1) |
| MaNguoiDuyet | int | FK | NULL | Người duyệt hoặc từ chối |
| TrangThai | nvarchar(20) |  | DEFAULT Chờ duyệt; CK_YeuCauNgoaiLe_TrangThai | Chờ duyệt, Đã duyệt, Từ chối |
| GhiChu | nvarchar(255) |  | NULL | Ghi chú, lý do từ chối |
| NgayYeuCau | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm tạo yêu cầu |
| NgayDuyet | datetime2(0) |  | NULL | Thời điểm xử lý |

### Bảng 4.6. Cấu trúc bảng NhatKyViPham

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaViPham | int IDENTITY | PK |  | Mã vi phạm |
| MaHanMuc | int | FK | NOT NULL | Hạn mức bị vượt |
| SoTienDinhChi | decimal(18,0) |  | CK_NhatKyViPham_SoTien: > 0 | Số tiền sinh viên định chi |
| SoConLaiLucDo | decimal(18,0) |  |  | Số còn lại lúc đó, có thể âm với hạn mức mềm đã vượt |
| KetQua | nvarchar(30) |  | CK_NhatKyViPham_KetQua: Từ chối, Gửi chờ duyệt | Kết quả xử lý |
| NgayXayRa | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm |

### Bảng 4.7. Cấu trúc bảng ThongBao

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaThongBao | int IDENTITY | PK |  | Mã thông báo |
| MaNguoiNhan | int | FK | NOT NULL | Người nhận |
| MaHanMuc | int | FK | NULL | Hạn mức liên quan |
| LoaiThongBao | nvarchar(30) |  | CK_ThongBao_Loai | Ngưỡng, Đã vượt, Yêu cầu duyệt, Kết quả duyệt |
| NoiDung | nvarchar(500) |  | NOT NULL | Nội dung |
| TinhTrangGui | nvarchar(20) |  | DEFAULT Trong ứng dụng; CK_ThongBao_TinhTrangGui | Đã gửi, Lỗi, Trong ứng dụng |
| DaDoc | bit |  | DEFAULT 0 | Đã đọc chưa |
| NgayTao | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm tạo |

### Bảng 4.8. Cấu trúc bảng NhatKyHeThong

| Cột | Kiểu dữ liệu | Khóa | Ràng buộc, mặc định | Ý nghĩa |
|---|---|---|---|---|
| MaNhatKy | int IDENTITY | PK |  | Mã nhật ký |
| MaNguoiDung | int | FK | NOT NULL | Người thực hiện |
| HanhDong | nvarchar(50) |  | NOT NULL | Ví dụ: Tạo hạn mức, Hủy giao dịch |
| TenThucThe | varchar(50) |  | NOT NULL | Tên bảng bị tác động |
| MaThucThe | int |  | NOT NULL | Khóa chính của dòng bị tác động |
| GiaTriCu | nvarchar(max) |  | NULL | Giá trị trước, dạng JSON |
| GiaTriMoi | nvarchar(max) |  | NULL | Giá trị sau, dạng JSON |
| NgayGio | datetime2(0) |  | DEFAULT SYSDATETIME() | Thời điểm |

### Bảng 4.9. Trigger và hàm

| Đối tượng | Nội dung | Mã lỗi |
|---|---|---|
| TR_HanMuc_ChongTrungKy | Hai hạn mức chưa khóa cùng sinh viên, danh mục không được có kỳ giao nhau. Hai kỳ [A1, A2] và [B1, B2] giao nhau khi A1 ≤ B2 và B1 ≤ A2. Bỏ qua lệnh UPDATE chỉ đổi số tiền. | 50001 |
| TR_GiaoDich_BaoVe | (1) Ngày giao dịch nằm trong kỳ của hạn mức. (2) Giao dịch vừa chuyển sang Hợp lệ không thuộc hạn mức đang khóa. (3) Với hạn mức cứng, tổng giao dịch Hợp lệ không vượt số tiền hạn mức; dòng hạn mức được đọc với UPDLOCK, HOLDLOCK. | 50002, 50003, 50004 |
| fn_TinhTrangThai | Trả về Đã vượt khi đã chi ≥ hạn mức; Sắp vượt khi đã chi × 100 ≥ hạn mức × ngưỡng; còn lại Hoạt động. |  |
| fn_CoQuyenQuanLy | Trả về 1 khi người gọi là người cấp ngân sách của sinh viên, hoặc chính là sinh viên tự quản lý. |  |

### Bảng 4.10. Các chỉ mục

| Chỉ mục | Cột | Phục vụ |
|---|---|---|
| IX_HanMuc_TimHieuLuc | (MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc) INCLUDE (TrangThai, SoTienHanMuc, SoTienDaChi) | Tìm hạn mức hiệu lực khi nhập giao dịch; trigger chống trùng kỳ |
| IX_GiaoDich_LichSu | (MaHanMuc, NgayGiaoDich) INCLUDE (SoTien, TrangThai) | Lịch sử, giao dịch gần đây, báo cáo chi tiêu |
| IX_YeuCauNgoaiLe_ChoDuyet | (NgayYeuCau) INCLUDE (MaGiaoDich) WHERE TrangThai = Chờ duyệt | Trang Duyệt ngoại lệ, cũ nhất trước |
| IX_ThongBao_ChuaDoc | (MaNguoiNhan, DaDoc) | Số trên chuông thông báo |
| IX_NhatKyViPham_BaoCao | (MaHanMuc, NgayXayRa) | Báo cáo vi phạm |

### Bảng 4.11. Các view

| View | Nội dung |
|---|---|
| vw_TinhTrangHanMuc | Hạn mức kèm tên sinh viên, tên danh mục, số còn lại và tỷ lệ sử dụng tính sẵn. Dùng cho danh sách hạn mức và trang chủ. |
| vw_DoiSoatHanMuc | So SoTienDaChi của từng hạn mức với tổng giao dịch Hợp lệ. Cột ChenhLech phải luôn bằng 0; dùng để kiểm tra sau demo. |

### Bảng 4.12. Các stored procedure

| Procedure | UC | Việc chính | Mã lỗi |
|---|---|---|---|
| sp_TaoHanMuc | UC01 | Kiểm tra quyền và danh mục, INSERT hạn mức, ghi nhật ký | 50031, 50032 |
| sp_DieuChinhHanMuc | UC02 | So PhienBan, chặn hạn mức cứng nhỏ hơn số đã chi, tính lại trạng thái | 50033, 50035, 50036, 50037 |
| sp_KhoaMoKhoaHanMuc | UC03 | Đổi trạng thái khóa, tính lại khi mở | 50033, 50034 |
| sp_LuuDanhMuc | UC04 | Thêm hoặc sửa danh mục | 50041, 50042, 50043 |
| sp_GhiNhanGiaoDich | UC05 | Khóa dòng hạn mức, đánh giá, ghi giao dịch hoặc vi phạm, tạo yêu cầu và thông báo | Trả kết quả dạng dòng |
| sp_HuyGiaoDich | UC07 | Hủy kèm lý do, trừ số đã chi hoặc đóng yêu cầu | 50021–50024 |
| sp_DuyetNgoaiLe | UC09 | Duyệt hoặc từ chối, cập nhật hạn mức, báo sinh viên | 50011–50015 |
| sp_LuuNguoiDung | UC13 | Thêm hoặc sửa người dùng | 50041, 50043–50047 |
| sp_DatLaiMatKhau | UC13 | Đặt lại mật khẩu đã băm | 50041, 50043 |

### Bảng 4.13. Tài khoản mẫu

| Tên đăng nhập | Họ tên | Vai trò | Ghi chú |
|---|---|---|---|
| admin | Quản trị viên | QuanTri |  |
| phuhuynh1 | Lê Văn Hùng | CapNganSach | Cấp ngân sách cho Khoa và Hà |
| sinhvien1 | Lê Minh Khoa | SinhVien | Người cấp: Lê Văn Hùng |
| sinhvien2 | Trần Thu Hà | SinhVien | Người cấp: Lê Văn Hùng |
| tuquanly1 | Nguyễn An Bình | SinhVien | Tự quản lý |

### Bảng 4.14. Hạn mức mẫu

| Mã | Sinh viên | Danh mục | Loại | Hạn mức | Đã chi | Trạng thái |
|---|---|---|---|---|---|---|
| 1 | Khoa | Mua sắm trực tuyến | Cứng | 2.000.000 | 1.400.000 | Hoạt động (70%) |
| 2 | Khoa | Ăn uống | Mềm | 1.500.000 | 1.380.000 | Sắp vượt (92%) |
| 3 | Hà | Đi lại | Cứng | 500.000 | 0 | Hoạt động |
| 4 | Khoa | Đi lại | Mềm | 300.000 | 0 | Đã khóa |
| 5 | Bình | Ăn uống | Mềm | 1.000.000 | 0 | Hoạt động |
