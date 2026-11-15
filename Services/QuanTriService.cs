using System.Data;
using QuanLyHanMucWeb.Data;

namespace QuanLyHanMucWeb.Services
{
    public static class QuanTriService
    {
        public const string MatKhauMacDinh = "123456";

        public static DataTable LayDanhMuc()
        {
            return KetNoi.TruyVan(
                "SELECT dm.MaDanhMuc, dm.TenDanhMuc, dm.MoTa, dm.HoatDong, " +
                "       (SELECT COUNT(*) FROM dbo.HanMuc h WHERE h.MaDanhMuc = dm.MaDanhMuc) AS SoHanMuc " +
                "FROM dbo.DanhMuc dm ORDER BY dm.TenDanhMuc");
        }

        public static int LuuDanhMuc(int toi, int? maDanhMuc, string ten, string moTa, bool hoatDong)
        {
            DataTable kq = KetNoi.GoiProcedure("dbo.sp_LuuDanhMuc",
                KetNoi.SoNguyen("@MaNguoi", toi),
                KetNoi.SoNguyen("@MaDanhMuc", maDanhMuc),
                KetNoi.Chuoi("@TenDanhMuc", ten, 100),
                KetNoi.Chuoi("@MoTa", string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim(), 255),
                KetNoi.DungSai("@HoatDong", hoatDong));
            return Convert.ToInt32(kq.Rows[0]["MaDanhMuc"]);
        }

        public static DataTable LayNguoiDung()
        {
            return KetNoi.TruyVan(
                "SELECT nd.MaNguoiDung, nd.TenDangNhap, nd.HoTen, nd.Email, nd.VaiTro, nd.MaNguoiCapNganSach, " +
                "       nc.HoTen AS NguoiCap, nd.HoatDong " +
                "FROM dbo.NguoiDung nd " +
                "LEFT JOIN dbo.NguoiDung nc ON nc.MaNguoiDung = nd.MaNguoiCapNganSach " +
                "ORDER BY nd.VaiTro, nd.HoTen");
        }

        public static DataTable LayNguoiCapNganSach()
        {
            return KetNoi.TruyVan(
                "SELECT MaNguoiDung, HoTen FROM dbo.NguoiDung " +
                "WHERE VaiTro = N'CapNganSach' AND HoatDong = 1 ORDER BY HoTen");
        }

        public static int LuuNguoiDung(int toi, int? maNguoiDung, string tenDangNhap, string hoTen, string email,
                                       string vaiTro, int? maNguoiCap, bool hoatDong)
        {
            string matKhauBam = maNguoiDung.HasValue ? null : TaiKhoanService.BamMatKhau(MatKhauMacDinh);

            DataTable kq = KetNoi.GoiProcedure("dbo.sp_LuuNguoiDung",
                KetNoi.SoNguyen("@MaNguoi", toi),
                KetNoi.SoNguyen("@MaNguoiDung", maNguoiDung),
                KetNoi.ChuoiKhongDau("@TenDangNhap", tenDangNhap?.Trim(), 50),
                KetNoi.ChuoiKhongDau("@MatKhauMaHoa", matKhauBam, 200),
                KetNoi.Chuoi("@HoTen", hoTen?.Trim(), 100),
                KetNoi.ChuoiKhongDau("@Email", string.IsNullOrWhiteSpace(email) ? null : email.Trim(), 100),
                KetNoi.Chuoi("@VaiTro", vaiTro, 20),
                KetNoi.SoNguyen("@MaNguoiCapNganSach", maNguoiCap),
                KetNoi.DungSai("@HoatDong", hoatDong));
            return Convert.ToInt32(kq.Rows[0]["MaNguoiDung"]);
        }

        public static string LayTenDangNhap(int maNguoiDung)
        {
            return KetNoi.LayMotGiaTri(
                "SELECT TenDangNhap FROM dbo.NguoiDung WHERE MaNguoiDung = @Ma",
                KetNoi.SoNguyen("@Ma", maNguoiDung)) as string;
        }

        public static void DatLaiMatKhau(int toi, int maNguoiDung)
        {
            KetNoi.GoiProcedure("dbo.sp_DatLaiMatKhau",
                KetNoi.SoNguyen("@MaNguoi", toi),
                KetNoi.SoNguyen("@MaNguoiDung", maNguoiDung),
                KetNoi.ChuoiKhongDau("@MatKhauMaHoa", TaiKhoanService.BamMatKhau(MatKhauMacDinh), 200));
        }
    }
}
