using System.Data;
using System.Security.Cryptography;
using System.Text;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Models;

namespace QuanLyHanMucWeb.Services
{
    public static class TaiKhoanService
    {
        public static string DangNhap(string tenDangNhap, string matKhau, out NguoiDungDangNhap nguoiDung)
        {
            nguoiDung = null;
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrEmpty(matKhau))
                return "Nhập tên đăng nhập và mật khẩu.";

            DataTable bang = KetNoi.TruyVan(
                "SELECT MaNguoiDung, HoTen, VaiTro, MaNguoiCapNganSach, MatKhauMaHoa, HoatDong " +
                "FROM dbo.NguoiDung WHERE TenDangNhap = @TenDangNhap",
                KetNoi.ChuoiKhongDau("@TenDangNhap", tenDangNhap.Trim(), 50));

            const string saiThongTin = "Tên đăng nhập hoặc mật khẩu không đúng.";

            if (bang.Rows.Count == 0)
                return saiThongTin;

            DataRow d = bang.Rows[0];
            if (!string.Equals(BamMatKhau(matKhau), (string)d["MatKhauMaHoa"], StringComparison.OrdinalIgnoreCase))
                return saiThongTin;

            if (!(bool)d["HoatDong"])
                return "Tài khoản đã ngừng hoạt động.";

            nguoiDung = new NguoiDungDangNhap
            {
                MaNguoiDung = (int)d["MaNguoiDung"],
                HoTen = (string)d["HoTen"],
                VaiTro = (string)d["VaiTro"],
                MaNguoiCapNganSach = d["MaNguoiCapNganSach"] == DBNull.Value ? (int?)null : (int)d["MaNguoiCapNganSach"]
            };
            return null;
        }

        public static string LayTheoTenDangNhap(string tenDangNhap, out NguoiDungDangNhap nguoiDung)
        {
            nguoiDung = null;
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return "Máy chủ đăng nhập không trả về tên đăng nhập.";

            DataTable bang = KetNoi.TruyVan(
                "SELECT MaNguoiDung, HoTen, VaiTro, MaNguoiCapNganSach, HoatDong " +
                "FROM dbo.NguoiDung WHERE TenDangNhap = @TenDangNhap",
                KetNoi.ChuoiKhongDau("@TenDangNhap", tenDangNhap.Trim(), 50));

            if (bang.Rows.Count == 0)
                return "Tài khoản \"" + tenDangNhap + "\" chưa được khai báo trong hệ thống.";

            DataRow d = bang.Rows[0];
            if (!(bool)d["HoatDong"])
                return "Tài khoản đã ngừng hoạt động.";

            nguoiDung = new NguoiDungDangNhap
            {
                MaNguoiDung = (int)d["MaNguoiDung"],
                HoTen = (string)d["HoTen"],
                VaiTro = (string)d["VaiTro"],
                MaNguoiCapNganSach = d["MaNguoiCapNganSach"] == DBNull.Value ? (int?)null : (int)d["MaNguoiCapNganSach"]
            };
            return null;
        }

        public static string BamMatKhau(string matKhau)
        {
            byte[] bam = SHA256.HashData(Encoding.UTF8.GetBytes(matKhau));
            return Convert.ToHexString(bam);
        }
    }
}
