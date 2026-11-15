# Bảng biểu chương 3: Thiết kế lớp

Các bảng trích từ báo cáo cuối kỳ, đúng số thứ tự trong báo cáo.

### Bảng 3.1. Hiện thực các phương thức của lớp thực thể

| Lớp | Phương thức | Hiện thực |
|---|---|---|
| NguoiDung | dangNhap, bamMatKhau | TaiKhoanService.DangNhap, TaiKhoanService.BamMatKhau |
|  | coQuyenQuanLy | Hàm dbo.fn_CoQuyenQuanLy |
|  | luu, datLaiMatKhau | sp_LuuNguoiDung, sp_DatLaiMatKhau |
| DanhMuc | layDanhSachDangDung, luu | HanMucService.LayDanhMucDangDung, sp_LuuDanhMuc |
| HanMuc | taoMoi | sp_TaoHanMuc |
|  | tyLeSuDung, soConLai, tyLeDuKien, danhGiaGiaoDich | Lớp Models.HanMuc (C#) |
|  | tinhTrangThai | Hàm dbo.fn_TinhTrangThai |
|  | dieuChinh, khoa, moKhoa | sp_DieuChinhHanMuc, sp_KhoaMoKhoaHanMuc |
| GiaoDich | ghiNhan, huy | sp_GhiNhanGiaoDich, sp_HuyGiaoDich |
|  | layLichSu, layGanDay | GiaoDichService.LayLichSu, GiaoDichService.LayGanDay |
| YeuCauNgoaiLe | taoMoi, duyet, tuChoi | sp_GhiNhanGiaoDich (tạo), sp_DuyetNgoaiLe (duyệt, từ chối) |
| NhatKyViPham | ghiNhan, layBaoCao | sp_GhiNhanGiaoDich, BaoCaoService.ViPham |
| ThongBao | tao, demChuaDoc, layDanhSach, danhDauDaDoc | Các procedure ghi nghiệp vụ; ThongBaoService |
| NhatKyHeThong | ghi | Câu INSERT cuối mỗi procedure ghi dữ liệu |
