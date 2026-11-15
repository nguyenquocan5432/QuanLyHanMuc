# Bảng biểu chương 2: Phân tích

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 2.1. Đặc tả use case UC01 – Thiết lập hạn mức

| Mục | Nội dung |
|---|---|
| Tác nhân | Người cấp ngân sách; sinh viên tự quản lý (chỉ cho chính mình) |
| Mô tả | Tạo hạn mức cho một sinh viên, một danh mục trong một kỳ. |
| Tiền điều kiện | Đã đăng nhập, có quyền quản lý hạn mức. Danh mục đang dùng. |
| Hậu điều kiện | Hạn mức mới ở trạng thái Hoạt động, số đã chi 0. Có một dòng "Tạo hạn mức" trong NhatKyHeThong. |
| Luồng chính | 1. Người dùng bấm "Thêm hạn mức".<br>2. Hệ thống hiện form: sinh viên, danh mục, từ ngày, đến ngày, số tiền, loại (Cứng/Mềm), ngưỡng cảnh báo (mặc định 80%).<br>3. Người dùng nhập và bấm Lưu.<br>4. Hệ thống gọi sp_TaoHanMuc: kiểm tra quyền với sinh viên (fn_CoQuyenQuanLy), kiểm tra danh mục đang dùng, INSERT.<br>5. Trigger TR_HanMuc_ChongTrungKy kiểm tra không trùng kỳ (BR01).<br>6. Hệ thống ghi nhật ký, chuyển về danh sách hạn mức và báo "Đã tạo hạn mức". |
| Luồng thay thế | A1. Ở bước 3 người cấp ngân sách chọn "Áp dụng cho tất cả sinh viên của tôi": hệ thống lặp bước 4–5 cho từng sinh viên, bỏ qua người bị trùng kỳ và liệt kê họ ở cuối. |
| Ngoại lệ | E1. Số tiền nhỏ hơn 1.000 đ hoặc ngưỡng ngoài 1–100: form báo lỗi, không gửi.<br>E2. Ngày kết thúc không sau ngày bắt đầu: CHECK CK_HanMuc_Ky từ chối.<br>E3. Trùng kỳ: lỗi 50001, giữ dữ liệu đã nhập và gợi ý tìm hạn mức đó trong danh sách để Sửa (UC02).<br>E4. Không có quyền với sinh viên: lỗi 50031. |
| Quy tắc | BR01, BR03, BR05, BR08 |
| Màn hình | ThietLapHanMuc.tsx (/han-muc/tao) |

### Bảng 2.2. Đặc tả use case UC02 – Điều chỉnh hạn mức

| Mục | Nội dung |
|---|---|
| Tác nhân | Người cấp ngân sách; sinh viên tự quản lý |
| Mô tả | Đổi số tiền, loại hạn mức hoặc ngưỡng cảnh báo của một hạn mức còn trong kỳ. Sinh viên, danh mục, kỳ không đổi. |
| Tiền điều kiện | Đã đăng nhập, có quyền với sinh viên của hạn mức. |
| Hậu điều kiện | Hạn mức có giá trị mới, trạng thái được tính lại theo số đã chi (trừ khi đang khóa). Nhật ký lưu giá trị cũ và mới. |
| Luồng chính | 1. Người dùng bấm Sửa ở một dòng trong danh sách hạn mức hoặc trang chủ.<br>2. Hệ thống hiện form với sinh viên, danh mục, kỳ, số đã chi ở dạng chỉ đọc và PhienBan ở trường ẩn.<br>3. Người dùng sửa và bấm Lưu.<br>4. sp_DieuChinhHanMuc khóa dòng hạn mức, so PhienBan, kiểm tra hạn mức cứng không nhỏ hơn số đã chi.<br>5. Hệ thống cập nhật, gọi fn_TinhTrangThai, ghi nhật ký, chuyển về danh sách. |
| Luồng thay thế | A1. Hạn mức đang khóa: giá trị được lưu, trạng thái vẫn là Đã khóa. |
| Ngoại lệ | E1. PhienBan đã đổi: lỗi 50035, báo hạn mức vừa bị người khác sửa.<br>E2. Hạn mức cứng nhỏ hơn số đã chi: lỗi 50037.<br>E3. Không có quyền: lỗi 50036. |
| Quy tắc | BR02, BR03, BR04, BR08 |
| Màn hình | ThietLapHanMuc.tsx (/han-muc/{id}) |

### Bảng 2.3. Đặc tả use case UC03 – Khóa / mở khóa hạn mức

| Mục | Nội dung |
|---|---|
| Tác nhân | Người cấp ngân sách; sinh viên tự quản lý |
| Mô tả | Chuyển hạn mức sang Đã khóa để chặn giao dịch mới, hoặc mở lại. |
| Tiền điều kiện | Đã đăng nhập, có quyền. Hạn mức còn trong kỳ (nút Khóa ẩn với hạn mức đã hết kỳ). |
| Hậu điều kiện | Trạng thái mới được lưu; nhật ký có trạng thái cũ và mới. |
| Luồng chính | 1. Người dùng bấm Khóa ở dòng hạn mức và xác nhận.<br>2. sp_KhoaMoKhoaHanMuc khóa dòng, kiểm tra quyền, đặt TrangThai = Đã khóa.<br>3. Hệ thống ghi nhật ký, tải lại danh sách. |
| Luồng thay thế | A1. Bấm Mở khóa: trạng thái = fn_TinhTrangThai(số đã chi, hạn mức, ngưỡng); trigger kiểm tra mở khóa không gây trùng kỳ. |
| Ngoại lệ | E1. Không có quyền: lỗi 50034.<br>E2. Mở khóa làm trùng kỳ với hạn mức khác đang mở: lỗi 50001. |
| Quy tắc | BR01, BR06, BR08 |
| Màn hình | DanhSachHanMuc.tsx (/han-muc) |

### Bảng 2.4. Đặc tả use case UC04 – Quản lý danh mục chi tiêu

| Mục | Nội dung |
|---|---|
| Tác nhân | Quản trị viên |
| Mô tả | Thêm, sửa tên và mô tả, ngừng dùng hoặc dùng lại danh mục. |
| Tiền điều kiện | Đăng nhập với vai trò QuanTri. |
| Hậu điều kiện | Danh mục được lưu; nhật ký có giá trị cũ và mới. |
| Luồng chính | 1. Quản trị viên mở trang Danh mục.<br>2. Hệ thống hiện bảng danh mục (mã, tên, mô tả, số hạn mức, trạng thái) và form thêm mới.<br>3. Quản trị viên nhập tên, mô tả, bấm Lưu.<br>4. sp_LuuDanhMuc kiểm tra vai trò, tên không trống, INSERT, ghi nhật ký. |
| Luồng thay thế | A1. Bấm tên một danh mục: dữ liệu đổ sang form, Lưu sẽ UPDATE. |
| Ngoại lệ | E1. Tên trống: lỗi 50042.<br>E2. Trùng tên: vi phạm UQ_DanhMuc_TenDanhMuc, LoiCSDL dịch thành câu tiếng Việt.<br>E3. Không phải quản trị viên: lỗi 50041. |
| Quy tắc | BR08 |
| Màn hình | DanhMuc.tsx (/danh-muc) |

### Bảng 2.5. Đặc tả use case UC05 – Ghi nhận giao dịch và kiểm tra hạn mức

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên (gồm sinh viên tự quản lý) |
| Mô tả | Sinh viên nhập một khoản chi. Hệ thống so với hạn mức còn lại rồi chấp nhận, chấp nhận kèm cảnh báo, từ chối hoặc chuyển chờ duyệt. |
| Tiền điều kiện | Đăng nhập với vai trò SinhVien. Có hạn mức chưa khóa cho danh mục vào ngày giao dịch. |
| Hậu điều kiện | Thành công: giao dịch Hợp lệ được lưu, số đã chi tăng, trạng thái hạn mức tính lại. Chờ duyệt: giao dịch lưu ở trạng thái Chờ duyệt, chưa cộng số đã chi. Từ chối: không lưu giao dịch. |
| Luồng chính | 1. Sinh viên mở Nhập giao dịch, chọn danh mục và ngày.<br>2. Hệ thống hiện hạn mức, đã chi, còn lại, tỷ lệ và 5 giao dịch gần nhất của danh mục.<br>3. Sinh viên gõ số tiền; trang gọi /api/giao-dich/danh-gia, dải cảnh báo đổi màu theo kết quả dự báo.<br>4. Sinh viên nhập mô tả, bấm Ghi nhận.<br>5. sp_GhiNhanGiaoDich mở transaction, đọc hạn mức với UPDLOCK, HOLDLOCK.<br>6. Tổng dự kiến dưới ngưỡng: lưu giao dịch Hợp lệ, cộng số đã chi trong cùng transaction.<br>7. Hệ thống báo "Đã ghi nhận" kèm tỷ lệ và số còn lại. |
| Luồng thay thế | A1. Từ ngưỡng đến dưới 100%: như bước 6, hạn mức sang Sắp vượt; nếu trạng thái vừa đổi thì tạo thông báo loại Ngưỡng.<br>A2. Đúng 100%: chấp nhận, hạn mức sang Đã vượt, thông báo loại Đã vượt.<br>A3. Vượt 100%, hạn mức mềm: lưu Chờ duyệt, tạo YeuCauNgoaiLe, ghi NhatKyViPham "Gửi chờ duyệt", báo người cấp ngân sách (hoặc chính sinh viên tự quản lý). |
| Ngoại lệ | E1. Số tiền ≤ 0: từ chối (BR07).<br>E2. Không có hạn mức hoặc hạn mức Đã khóa: từ chối, nêu lý do (BR06).<br>E3. Vượt 100%, hạn mức cứng: không lưu giao dịch, ghi NhatKyViPham "Từ chối", báo số còn lại.<br>E4. Hai giao dịch gửi cùng lúc: giao dịch đến sau chờ khóa, được kiểm tra với số đã chi mới. |
| Quy tắc | BR02, BR03, BR04, BR05, BR06, BR07 |
| Màn hình | NhapGiaoDich.tsx (/giao-dich/nhap) |

### Bảng 2.6. Đặc tả use case UC06 – Xem lịch sử giao dịch

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên; người cấp ngân sách |
| Mô tả | Xem giao dịch trong khoảng ngày, lọc theo danh mục và trạng thái. |
| Tiền điều kiện | Đã đăng nhập với vai trò SinhVien hoặc CapNganSach. |
| Hậu điều kiện | Không thay đổi dữ liệu. |
| Luồng chính | 1. Người dùng mở Lịch sử; mặc định từ đầu tháng đến hôm nay.<br>2. Người dùng chọn khoảng ngày, danh mục, trạng thái, bấm Xem.<br>3. Hệ thống hiện bảng giao dịch mới nhất trước, mỗi dòng tô màu theo trạng thái, và tổng tiền các giao dịch Hợp lệ. |
| Luồng thay thế | A1. Người quản lý hạn mức: bảng có thêm cột Sinh viên và nút Hủy ở dòng Hợp lệ (dẫn sang UC07). |
| Ngoại lệ | E1. Không có giao dịch: hiện "Không có giao dịch trong khoảng đã chọn". |
| Quy tắc | BR08 |
| Màn hình | LichSu.tsx (/giao-dich) |

### Bảng 2.7. Đặc tả use case UC07 – Hủy giao dịch

| Mục | Nội dung |
|---|---|
| Tác nhân | Người cấp ngân sách; sinh viên tự quản lý |
| Mô tả | Hủy một giao dịch Hợp lệ hoặc Chờ duyệt kèm lý do. |
| Tiền điều kiện | Có quyền với sinh viên của giao dịch; giao dịch đang Hợp lệ hoặc Chờ duyệt. |
| Hậu điều kiện | Giao dịch Đã hủy, có lý do, người hủy, ngày hủy. Số đã chi và trạng thái hạn mức được tính lại. |
| Luồng chính | 1. Người dùng bấm Hủy ở trang Lịch sử.<br>2. Hệ thống hiện thông tin giao dịch và ô Lý do hủy; nút Xác nhận hủy chỉ bật khi đã nhập lý do.<br>3. Người dùng nhập lý do, xác nhận.<br>4. sp_HuyGiaoDich khóa dòng giao dịch và hạn mức, kiểm tra trạng thái và quyền.<br>5. Giao dịch Hợp lệ: đổi sang Đã hủy, trừ số đã chi, tính lại trạng thái hạn mức.<br>6. Hệ thống ghi nhật ký, về trang Lịch sử. |
| Luồng thay thế | A1. Giao dịch đang Chờ duyệt: đổi sang Đã hủy, yêu cầu ngoại lệ đi kèm chuyển Từ chối với ghi chú "Giao dịch đã bị hủy". Trang Lịch sử hiện chỉ đặt nút Hủy ở dòng Hợp lệ; nhánh này được procedure hỗ trợ sẵn khi mở trực tiếp /giao-dich/{id}/huy. |
| Ngoại lệ | E1. Lý do trống: lỗi 50021.<br>E2. Giao dịch Đã hủy hoặc Từ chối: lỗi 50023.<br>E3. Không có quyền: lỗi 50024. |
| Quy tắc | BR02, BR07, BR08 |
| Màn hình | HuyGiaoDich.tsx (/giao-dich/{id}/huy) |

### Bảng 2.8. Đặc tả use case UC08 – Nhận và xem thông báo

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên; người cấp ngân sách |
| Mô tả | Xem thông báo về ngưỡng, vượt hạn mức, yêu cầu duyệt và kết quả duyệt. |
| Tiền điều kiện | Đã đăng nhập, không phải quản trị viên. |
| Hậu điều kiện | Thông báo được đánh dấu đã đọc nếu người dùng chọn. |
| Luồng chính | 1. Mỗi trang hiện chuông trên menu kèm số thông báo chưa đọc.<br>2. Người dùng bấm chuông.<br>3. Hệ thống hiện thông báo của người dùng, chưa đọc trước, mới nhất trước.<br>4. Người dùng bấm "Đã đọc" ở một dòng. |
| Luồng thay thế | A1. Bấm "Đánh dấu tất cả đã đọc": mọi thông báo chưa đọc của người dùng chuyển đã đọc. |
| Ngoại lệ | E1. Mã thông báo của người khác: câu UPDATE có điều kiện MaNguoiNhan nên không có dòng nào bị đổi. |
| Quy tắc | BR03, BR05 |
| Màn hình | ThongBao.tsx (/thong-bao), chuông trên thanh menu (BoCuc.tsx) |

### Bảng 2.9. Đặc tả use case UC09 – Duyệt ngoại lệ vượt hạn mức

| Mục | Nội dung |
|---|---|
| Tác nhân | Người cấp ngân sách; sinh viên tự quản lý |
| Mô tả | Duyệt hoặc từ chối giao dịch vượt hạn mức mềm. |
| Tiền điều kiện | Có yêu cầu Chờ duyệt của sinh viên mình quản lý. |
| Hậu điều kiện | Duyệt: giao dịch Hợp lệ, số đã chi cộng thêm, hạn mức thường sang Đã vượt. Từ chối: giao dịch Từ chối, số đã chi không đổi. Sinh viên nhận thông báo Kết quả duyệt. |
| Luồng chính | 1. Người dùng mở Duyệt ngoại lệ.<br>2. Hệ thống hiện từng yêu cầu: sinh viên, danh mục, số tiền, mô tả, ngày, số còn lại trước giao dịch, số tiền vượt.<br>3. Người dùng nhập ghi chú (không bắt buộc khi duyệt), bấm Duyệt.<br>4. sp_DuyetNgoaiLe khóa yêu cầu, giao dịch, hạn mức; kiểm tra yêu cầu còn chờ và quyền.<br>5. Hệ thống đổi giao dịch sang Hợp lệ, cộng số đã chi, tính lại trạng thái, đổi yêu cầu sang Đã duyệt, tạo thông báo cho sinh viên. |
| Luồng thay thế | A1. Bấm Từ chối: giao dịch và yêu cầu chuyển Từ chối, lý do được ghi vào thông báo. |
| Ngoại lệ | E1. Từ chối không ghi lý do: lỗi 50011.<br>E2. Yêu cầu đã xử lý: lỗi 50013.<br>E3. Không có quyền: lỗi 50014.<br>E4. Hạn mức đang khóa: nút Duyệt bị tắt; nếu vẫn gửi thì lỗi 50015. |
| Quy tắc | BR05, BR06, BR08 |
| Màn hình | DuyetNgoaiLe.tsx (/ngoai-le) |

### Bảng 2.10. Đặc tả use case UC10 – Báo cáo chi tiêu theo kỳ

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên; người cấp ngân sách |
| Mô tả | Tổng hợp hạn mức, đã chi, còn lại, tỷ lệ theo danh mục trong khoảng ngày. |
| Tiền điều kiện | Đã đăng nhập với vai trò SinhVien hoặc CapNganSach. |
| Hậu điều kiện | Không thay đổi dữ liệu. |
| Luồng chính | 1. Người dùng mở Báo cáo, tab Chi tiêu theo kỳ.<br>2. Người dùng chọn từ ngày, đến ngày, bấm Xem.<br>3. Hệ thống lấy các hạn mức có kỳ giao với khoảng đã chọn, cộng giao dịch Hợp lệ trong khoảng, nhóm theo danh mục.<br>4. Hệ thống hiện bảng và biểu đồ thanh so sánh hạn mức với đã chi. |
| Luồng thay thế | A1. Người cấp ngân sách chọn sinh viên cần xem trong danh sách sinh viên của mình. |
| Ngoại lệ | E1. Không có hạn mức trong khoảng: hiện "Không có hạn mức nào trong khoảng đã chọn". |
| Quy tắc | BR02, BR08 |
| Màn hình | BaoCao.tsx (/bao-cao) |

### Bảng 2.11. Đặc tả use case UC11 – Báo cáo vi phạm hạn mức

| Mục | Nội dung |
|---|---|
| Tác nhân | Người cấp ngân sách; sinh viên tự quản lý |
| Mô tả | Liệt kê các lần giao dịch vượt hạn mức trong khoảng ngày. |
| Tiền điều kiện | Có quyền quản lý hạn mức. |
| Hậu điều kiện | Không thay đổi dữ liệu. |
| Luồng chính | 1. Người dùng chọn tab Vi phạm hạn mức.<br>2. Người dùng chọn khoảng ngày, có thể chọn một sinh viên, bấm Xem.<br>3. Hệ thống hiện ngày giờ, sinh viên, danh mục, số tiền định chi, số còn lại lúc đó, kết quả (Từ chối hoặc Gửi chờ duyệt). |
| Luồng thay thế | A1. Không chọn sinh viên: hiện vi phạm của mọi sinh viên mình quản lý. |
| Ngoại lệ | E1. Không có vi phạm: hiện "Không có vi phạm trong khoảng đã chọn". |
| Quy tắc | BR04, BR05, BR08 |
| Màn hình | BaoCao.tsx (/bao-cao?tab=vipham) |

### Bảng 2.12. Đặc tả use case UC12 – Đăng nhập và phân quyền

| Mục | Nội dung |
|---|---|
| Tác nhân | Sinh viên, người cấp ngân sách, quản trị viên |
| Mô tả | Xác thực người dùng qua Keycloak (chế độ Oidc) hoặc bằng mật khẩu trong bảng NguoiDung (chế độ Local); tạo phiên làm việc theo vai trò. |
| Tiền điều kiện | Tài khoản tồn tại. |
| Hậu điều kiện | Cookie đăng nhập (HttpOnly, hạn 8 giờ) chứa mã, họ tên, vai trò, mã người cấp ngân sách. |
| Luồng chính | 1. Người dùng mở ứng dụng khi chưa đăng nhập; trang React chuyển tới /dang-nhap.<br>2. Người dùng bấm Đăng nhập, trình duyệt chuyển sang trang đăng nhập của Keycloak.<br>3. Người dùng nhập tên đăng nhập, mật khẩu trên trang Keycloak.<br>4. Keycloak trả mã xác thực về /signin-oidc; máy chủ đổi mã lấy id_token.<br>5. Hệ thống tìm người dùng theo preferred_username, kiểm tra HoatDong.<br>6. Hệ thống tạo cookie, chuyển về Trang chủ. |
| Luồng thay thế | A1. Chế độ Local: người dùng nhập tên đăng nhập, mật khẩu ngay trên trang /dang-nhap; hệ thống băm mật khẩu bằng SHA-256, so với MatKhauMaHoa.<br>A2. Đăng xuất: xóa cookie (chế độ Oidc đăng xuất luôn khỏi Keycloak), về trang đăng nhập.<br>A3. Gọi API không đúng vai trò: API trả 403, React chuyển tới /khong-co-quyen. |
| Ngoại lệ | E1. Sai tên hoặc mật khẩu: "Tên đăng nhập hoặc mật khẩu không đúng." (chế độ Oidc do Keycloak báo).<br>E2. Tài khoản ngừng hoạt động: "Tài khoản đã ngừng hoạt động."<br>E3. Đăng nhập Keycloak được nhưng không có trong bảng NguoiDung: về /dang-nhap kèm câu báo lỗi. |
| Quy tắc | BR08 |
| Màn hình | DangNhap.tsx (/dang-nhap), KhongCoQuyen.tsx (/khong-co-quyen); trang đăng nhập của Keycloak ở chế độ Oidc |

### Bảng 2.13. Đặc tả use case UC13 – Quản lý người dùng

| Mục | Nội dung |
|---|---|
| Tác nhân | Quản trị viên |
| Mô tả | Thêm, sửa người dùng; gán người cấp ngân sách; ngừng hoặc mở lại tài khoản; đặt lại mật khẩu. |
| Tiền điều kiện | Đăng nhập với vai trò QuanTri. |
| Hậu điều kiện | Tài khoản được lưu; nhật ký có giá trị cũ và mới. |
| Luồng chính | 1. Quản trị viên mở trang Người dùng.<br>2. Hệ thống hiện bảng người dùng và form thêm mới.<br>3. Quản trị viên nhập tên đăng nhập, họ tên, email, vai trò, người cấp ngân sách (chỉ bật khi vai trò là Sinh viên), bấm Lưu.<br>4. sp_LuuNguoiDung kiểm tra và INSERT; hệ thống ghi nhật ký, tải lại trang. |
| Luồng thay thế | A1. Bấm tên đăng nhập một dòng để sửa; tên đăng nhập không đổi được.<br>A2. Bấm Đặt lại mật khẩu và xác nhận: mật khẩu về giá trị mặc định. |
| Ngoại lệ | E1. Trùng tên đăng nhập: vi phạm UQ_NguoiDung_TenDangNhap.<br>E2. Người cấp không hợp lệ: 50044.<br>E3. Tự khóa hoặc tự bỏ quyền quản trị: 50046.<br>E4. Đổi vai trò người đang cấp ngân sách: 50047. |
| Quy tắc | BR08 |
| Màn hình | NguoiDung.tsx (/nguoi-dung) |

### Bảng 2.14. Các lớp phân tích

| Lớp | Ý nghĩa |
|---|---|
| NguoiDung | Mọi tài khoản: sinh viên, người cấp ngân sách, quản trị viên. Quan hệ tự thân "cấp ngân sách cho" nối sinh viên với người cấp ngân sách. |
| DanhMuc | Nhóm chi tiêu do quản trị viên định nghĩa, ví dụ Ăn uống, Đi lại. |
| HanMuc | Số tiền tối đa một sinh viên được chi cho một danh mục trong một kỳ, kèm loại, ngưỡng, số đã chi và trạng thái. |
| GiaoDich | Một khoản chi sinh viên đã nhập và được lưu (Hợp lệ, Chờ duyệt, Từ chối, Đã hủy). |
| YeuCauNgoaiLe | Yêu cầu duyệt cho một giao dịch vượt hạn mức mềm. |
| NhatKyViPham | Mỗi lần một khoản chi vượt hạn mức, gồm cả lần bị từ chối mà không tạo giao dịch. |
| ThongBao | Thông báo gửi tới một người dùng, có thể gắn với một hạn mức. |
| NhatKyHeThong | Ai làm gì, với dòng nào, lúc nào, giá trị trước và sau. |

### Bảng 2.15. Quan hệ giữa các lớp

| Mã | Quan hệ | Bội số | Ý nghĩa |
|---|---|---|---|
| R1 | NguoiDung – NguoiDung | 0..1 – 0..* | Một người cấp ngân sách cho nhiều sinh viên; sinh viên tự quản lý không có người cấp. |
| R2 | NguoiDung (sinh viên) – HanMuc | 1 – 0..* | Hạn mức thuộc về một sinh viên. |
| R3 | NguoiDung (người tạo) – HanMuc | 1 – 0..* | Người tạo hạn mức, dùng khi truy vết. |
| R4 | DanhMuc – HanMuc | 1 – 0..* | Hạn mức thuộc một danh mục. |
| R5 | HanMuc ◆– GiaoDich | 1 – 0..* | Hợp thành: giao dịch không tồn tại ngoài hạn mức của nó. |
| R6 | GiaoDich – YeuCauNgoaiLe | 1 – 0..1 | Mỗi giao dịch có tối đa một yêu cầu duyệt (UNIQUE MaGiaoDich). |
| R7 | NguoiDung (người duyệt) – YeuCauNgoaiLe | 0..1 – 0..* | Người đã duyệt hoặc từ chối. |
| R8 | NguoiDung (người hủy) – GiaoDich | 0..1 – 0..* | Người đã hủy giao dịch. |
| R9 | HanMuc ◆– NhatKyViPham | 1 – 0..* | Vi phạm gắn với hạn mức bị vượt. |
| R10 | NguoiDung – ThongBao | 1 – 0..* | Người nhận thông báo. |
| R11 | HanMuc – ThongBao | 0..1 – 0..* | Hạn mức mà thông báo nhắc tới. |
| R12 | NguoiDung – NhatKyHeThong | 1 – 0..* | Người thực hiện thao tác. |

### Bảng 2.16. Các tập giá trị

| Tên | Giá trị |
|---|---|
| VaiTro | QuanTri, CapNganSach, SinhVien |
| LoaiHanMuc | Cứng, Mềm |
| TrangThaiHanMuc | Hoạt động, Sắp vượt, Đã vượt, Đã khóa |
| TrangThaiGiaoDich | Hợp lệ, Chờ duyệt, Từ chối, Đã hủy |
| TrangThaiYeuCau | Chờ duyệt, Đã duyệt, Từ chối |
| KetQuaViPham | Từ chối, Gửi chờ duyệt |
| LoaiThongBao | Ngưỡng, Đã vượt, Yêu cầu duyệt, Kết quả duyệt |
| KetQuaDanhGia | ChapNhan, ChapNhanCanhBao, ChapNhanDungMuc, ChoDuyet, TuChoi, DaKhoa (enum trong Models/HanMuc.cs) |

### Bảng 2.17. Chuyển trạng thái lớp HanMuc

| Từ | Sự kiện, điều kiện | Đến | Nơi xử lý |
|---|---|---|---|
| (mới) | Tạo hạn mức | Hoạt động | sp_TaoHanMuc |
| Hoạt động | Ghi hoặc duyệt giao dịch, tỷ lệ ≥ ngưỡng | Sắp vượt | sp_GhiNhanGiaoDich, sp_DuyetNgoaiLe |
| Hoạt động, Sắp vượt | Tỷ lệ ≥ 100% | Đã vượt | sp_GhiNhanGiaoDich, sp_DuyetNgoaiLe |
| Sắp vượt, Đã vượt | Hủy giao dịch hoặc tăng hạn mức, tỷ lệ giảm | Hoạt động hoặc Sắp vượt | sp_HuyGiaoDich, sp_DieuChinhHanMuc |
| Đang mở | Khóa | Đã khóa | sp_KhoaMoKhoaHanMuc |
| Đã khóa | Mở khóa, tính lại theo số đã chi | Một trong ba trạng thái mở | sp_KhoaMoKhoaHanMuc |
