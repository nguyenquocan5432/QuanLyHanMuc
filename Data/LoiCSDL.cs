using Microsoft.Data.SqlClient;

namespace QuanLyHanMucWeb.Data
{
    public static class LoiCSDL
    {
        public static string Dich(SqlException ex)
        {
            if (ex.Number >= 50000)
                return ex.Message;

            switch (ex.Number)
            {
                case 547:
                    return DichRangBuoc(ex.Message);

                case 2627:
                case 2601:
                    if (ex.Message.Contains("UQ_NguoiDung_TenDangNhap")) return "Tên đăng nhập đã tồn tại.";
                    if (ex.Message.Contains("UQ_DanhMuc_TenDanhMuc")) return "Tên danh mục đã tồn tại.";
                    if (ex.Message.Contains("UQ_YeuCauNgoaiLe_MaGiaoDich")) return "Giao dịch này đã có yêu cầu duyệt.";
                    return "Dữ liệu bị trùng.";

                case 2812:
                    return "CSDL thiếu thủ tục cần dùng. Hãy chạy lại file CSDL/QuanLyHanMuc.sql.";

                case 208:
                    return "CSDL thiếu bảng hoặc view cần dùng. Hãy chạy lại file CSDL/QuanLyHanMuc.sql.";

                case 1205:
                    return "Hệ thống đang bận, vui lòng thử lại.";

                case -2:
                    return "SQL Server phản hồi quá lâu, vui lòng thử lại.";

                case 18456:
                    return "Không đăng nhập được SQL Server. Kiểm tra User ID và Password trong appsettings.json.";

                case 4060:
                    return "Không mở được database QuanLyHanMuc. Đã chạy file SQL tạo CSDL chưa?";

                case 2:
                case 53:
                case -1:
                    return "Không kết nối được SQL Server. Kiểm tra Server trong appsettings.json và dịch vụ SQL Server đang chạy.";

                default:
                    return "Lỗi cơ sở dữ liệu (mã " + ex.Number + "): " + ex.Message;
            }
        }

        private static string DichRangBuoc(string thongBao)
        {
            if (thongBao.Contains("CK_HanMuc_CungKhongVuot")) return "Giao dịch làm vượt hạn mức cứng.";
            if (thongBao.Contains("CK_HanMuc_Ky")) return "Ngày kết thúc phải sau ngày bắt đầu.";
            if (thongBao.Contains("CK_HanMuc_SoTien")) return "Số tiền hạn mức phải lớn hơn 0.";
            if (thongBao.Contains("CK_HanMuc_Nguong")) return "Ngưỡng cảnh báo phải từ 1 đến 100.";
            if (thongBao.Contains("CK_GiaoDich_SoTien")) return "Số tiền giao dịch phải lớn hơn 0.";
            if (thongBao.Contains("CK_GiaoDich_HuyPhaiCoLyDo")) return "Hủy giao dịch phải ghi lý do.";
            if (thongBao.Contains("CK_NguoiDung_VaiTro")) return "Vai trò không hợp lệ.";
            if (thongBao.Contains("CK_NguoiDung_KhongTuCap")) return "Không thể tự làm người cấp ngân sách cho chính mình.";
            if (thongBao.Contains("REFERENCE")) return "Dữ liệu đang được dùng ở nơi khác nên không xóa hoặc sửa được.";
            return "Dữ liệu không hợp lệ: " + thongBao;
        }
    }
}
