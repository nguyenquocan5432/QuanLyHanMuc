# Bảng biểu chương 6: Giao diện

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 6.1. Danh sách các màn hình

| STT | Màn hình | Trang React | Đường dẫn | Người dùng | Use case |
|---|---|---|---|---|---|
| 1 | Đăng nhập | DangNhap.tsx | /dang-nhap | Tất cả | UC12 |
| 2 | Trang chủ sinh viên | TrangChu.tsx | / | Sinh viên | UC08 |
| 3 | Trang chủ người cấp ngân sách | TrangChu.tsx | / | Người cấp ngân sách | UC09 |
| 4 | Nhập giao dịch | NhapGiaoDich.tsx | /giao-dich/nhap | Sinh viên | UC05 |
| 5 | Lịch sử giao dịch | LichSu.tsx | /giao-dich | Sinh viên, người cấp ngân sách | UC06 |
| 6 | Hủy giao dịch | HuyGiaoDich.tsx | /giao-dich/{id}/huy | Người quản lý hạn mức | UC07 |
| 7 | Danh sách hạn mức | DanhSachHanMuc.tsx | /han-muc | Người quản lý hạn mức | UC03 |
| 8 | Thiết lập, điều chỉnh hạn mức | ThietLapHanMuc.tsx | /han-muc/tao, /han-muc/{id} | Người quản lý hạn mức | UC01, UC02 |
| 9 | Duyệt ngoại lệ | DuyetNgoaiLe.tsx | /ngoai-le | Người quản lý hạn mức | UC09 |
| 10 | Thông báo | ThongBao.tsx | /thong-bao | Sinh viên, người cấp ngân sách | UC08 |
| 11 | Báo cáo chi tiêu theo kỳ | BaoCao.tsx | /bao-cao | Sinh viên, người cấp ngân sách | UC10 |
| 12 | Báo cáo vi phạm | BaoCao.tsx | /bao-cao?tab=vipham | Người quản lý hạn mức | UC11 |
| 13 | Danh mục chi tiêu | DanhMuc.tsx | /danh-muc | Quản trị viên | UC04 |
| 14 | Người dùng | NguoiDung.tsx | /nguoi-dung | Quản trị viên | UC13 |
| 15 | Không có quyền truy cập | KhongCoQuyen.tsx | /khong-co-quyen | Tất cả | UC12 |

### Bảng 6.2. Thành phần màn hình Đăng nhập

| Khu vực | Nội dung |
|---|---|
| Tên đăng nhập | Ô chữ, tối đa 50 ký tự, con trỏ đặt sẵn ở ô này |
| Mật khẩu | Ô mật khẩu, ký tự bị che |
| Thông báo lỗi | Hộp đỏ phía trên nút, chỉ hiện khi đăng nhập không thành công |
| Nút Đăng nhập | Gọi POST /api/dang-nhap kèm mã chống giả mạo |
| Chế độ Oidc | Hai ô nhập được thay bằng một nút Đăng nhập bằng tài khoản Keycloak |

### Bảng 6.3. Thành phần màn hình Trang chủ

| Khu vực | Nội dung |
|---|---|
| Lời chào, nút chính | Họ tên; nút Nhập giao dịch (sinh viên) hoặc Thêm hạn mức (người quản lý hạn mức) |
| Tổng quan kỳ | Chỉ có ở trang sinh viên: kỳ, hai thanh so sánh phần thời gian đã qua với phần ngân sách đã dùng, tổng hạn mức, đã chi, còn lại |
| Dải chờ duyệt | Chỉ có ở trang người quản lý hạn mức, khi có yêu cầu: số giao dịch chờ duyệt và liên kết Duyệt |
| Hạn mức từng danh mục | Mỗi dòng: tên danh mục, nhãn trạng thái, thanh tỷ lệ có vạch ngưỡng cảnh báo, đã chi trên hạn mức, số còn lại, nút Nhập hoặc Sửa |
| Chức năng | Các ô lối tắt tới những trang vai trò được mở |

### Bảng 6.4. Thành phần màn hình Nhập giao dịch

| Khu vực | Nội dung |
|---|---|
| Danh mục, Ngày | Danh mục chưa có hạn mức vào ngày đã chọn vẫn hiện nhưng bị mờ, ghi "(chưa có hạn mức)" |
| Số tiền | Ô số lớn, bước 1.000 đ, tối thiểu 1.000 đ; dòng nhỏ bên dưới đọc lại số đã gõ theo dạng 500.000 đ |
| Dải cảnh báo | Đổi màu và nội dung theo kết quả dự báo: không màu, cam, cam đậm, đỏ nhạt (sẽ chờ duyệt), đỏ (bị từ chối) |
| Mô tả | Ô chữ, tối đa 255 ký tự |
| Nút | Đóng (về trang chủ), Làm mới, Ghi nhận; nút Ghi nhận đổi nhãn thành Gửi chờ duyệt khi giao dịch vượt hạn mức mềm |
| Hạn mức hiện tại | Số còn lại, nhãn trạng thái, thanh tỷ lệ có phần dự kiến kẻ sọc, hạn mức, đã chi, loại, ngưỡng |
| Giao dịch gần đây | Năm giao dịch mới nhất của hạn mức đang chọn; giao dịch đã hủy bị gạch ngang |

### Bảng 6.5. Thành phần màn hình Lịch sử giao dịch

| Khu vực | Nội dung |
|---|---|
| Bộ lọc | Từ ngày, đến ngày (mặc định là tháng hiện tại), danh mục, trạng thái, nút Xem |
| Bảng giao dịch | Ngày, sinh viên (chỉ hiện với người quản lý hạn mức), danh mục, số tiền, mô tả, trạng thái, lý do hủy |
| Cột thao tác | Nút Hủy trên dòng Hợp lệ, chỉ hiện với người quản lý hạn mức |
| Dòng tổng | Tổng số tiền các giao dịch Hợp lệ trong kết quả lọc |

### Bảng 6.6. Thành phần màn hình Hủy giao dịch

| Khu vực | Nội dung |
|---|---|
| Thông tin giao dịch | Ngày, sinh viên, danh mục, số tiền, trạng thái hiện tại |
| Lý do hủy | Ô nhiều dòng, bắt buộc, tối đa 255 ký tự |
| Nút | Xác nhận hủy (màu đỏ), Bỏ qua |

### Bảng 6.7. Thành phần màn hình Danh sách hạn mức

| Khu vực | Nội dung |
|---|---|
| Bộ lọc | Kỳ (kỳ hiện tại, tất cả, đã hết kỳ), sinh viên, trạng thái, nút Lọc |
| Bảng hạn mức | Sinh viên, danh mục, kỳ, hạn mức, đã chi, còn lại, thanh tỷ lệ, loại, trạng thái |
| Cột thao tác | Sửa; Khóa hoặc Mở khóa tùy trạng thái |
| Nút Thêm hạn mức | Mở màn hình Thiết lập hạn mức |

### Bảng 6.8. Thành phần màn hình Thiết lập, điều chỉnh hạn mức

| Khu vực | Nội dung |
|---|---|
| Sinh viên | Danh sách sinh viên người dùng được quản lý; sinh viên tự quản lý chỉ thấy chính mình |
| Danh mục | Các danh mục đang dùng |
| Kỳ | Từ ngày, đến ngày, mặc định là tháng hiện tại |
| Số tiền hạn mức | Ô số, tối thiểu 1.000 đ |
| Loại hạn mức | Hai lựa chọn Cứng, Mềm |
| Ngưỡng cảnh báo | Ô số từ 1 đến 100, mặc định 80 |
| Áp dụng cho tất cả | Ô đánh dấu, chỉ có khi tạo mới: tạo cùng hạn mức cho mọi sinh viên của người cấp ngân sách |
| Nút | Lưu, Hủy bỏ |

### Bảng 6.9. Thành phần màn hình Duyệt ngoại lệ

| Khu vực | Nội dung |
|---|---|
| Thẻ yêu cầu | Sinh viên, danh mục, số tiền và mô tả, ngày giao dịch, số còn lại trước giao dịch, số vượt (chữ đỏ), thời điểm gửi |
| Ghi chú | Ô chữ, bắt buộc khi từ chối |
| Nút | Duyệt (xanh), Từ chối (đỏ) |

### Bảng 6.10. Thành phần màn hình Thông báo

| Khu vực | Nội dung |
|---|---|
| Nút đầu trang | Đánh dấu tất cả đã đọc |
| Bảng thông báo | Thời điểm, loại, nội dung, tình trạng gửi |
| Cột thao tác | Nút Đã đọc trên dòng chưa đọc |

### Bảng 6.11. Thành phần màn hình Báo cáo

| Khu vực | Nội dung |
|---|---|
| Tab | Chi tiêu theo kỳ; Vi phạm hạn mức (chỉ người quản lý hạn mức) |
| Bộ lọc | Từ ngày, đến ngày, sinh viên (người cấp ngân sách chọn; sinh viên chỉ có chính mình), nút Xem |
| Bảng chi tiêu | Danh mục, hạn mức, đã chi, còn lại, tỷ lệ; tỷ lệ từ ngưỡng trở lên tô cam, trên 100% tô đỏ |
| Biểu đồ | Cặp thanh ngang hạn mức và đã chi cho từng danh mục |
| Bảng vi phạm | Thời điểm, sinh viên, danh mục, số tiền định chi, số còn lại lúc đó, kết quả |

### Bảng 6.12. Thành phần màn hình Danh mục chi tiêu

| Khu vực | Nội dung |
|---|---|
| Bảng danh mục | Mã, tên (bấm để sửa), mô tả, số hạn mức đang dùng danh mục, trạng thái |
| Form | Tên danh mục (bắt buộc, tối đa 100 ký tự), mô tả, ô Đang dùng |
| Nút | Lưu, Thêm mới (xóa form để nhập danh mục mới) |

### Bảng 6.13. Thành phần màn hình Người dùng

| Khu vực | Nội dung |
|---|---|
| Bảng người dùng | Tên đăng nhập (bấm để sửa), họ tên, vai trò, người cấp ngân sách, trạng thái |
| Form | Tên đăng nhập, họ tên, email, vai trò, người cấp ngân sách (có lựa chọn Tự quản lý), ô Đang hoạt động |
| Nút | Lưu, Thêm mới; Đặt lại mật khẩu ở khung riêng khi đang sửa |

### Bảng 6.14. Thành phần màn hình Không có quyền truy cập

| Khu vực | Nội dung |
|---|---|
| Nội dung | Tiêu đề và một dòng "Tài khoản này không dùng được chức năng này." |
| Nút | Về trang chủ |
