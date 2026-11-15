using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;

namespace QuanLyHanMucWeb.Services
{
    public enum LocKy
    {
        HienTai,
        TatCa,
        DaHetKy
    }

    public static class HanMucService
    {
        public static DataTable LayDanhSach(int toi, LocKy ky, int? maSinhVien, string trangThai)
        {
            StringBuilder sql = new StringBuilder(
                "SELECT MaHanMuc, MaSinhVien, SinhVien, TenDanhMuc, NgayBatDau, NgayKetThuc, " +
                "       SoTienHanMuc, SoTienDaChi, SoConLai, TyLeSuDung, NguongCanhBao, LoaiHanMuc, TrangThai " +
                "FROM dbo.vw_TinhTrangHanMuc " +
                "WHERE dbo.fn_CoQuyenQuanLy(@Toi, MaSinhVien) = 1");

            List<SqlParameter> thamSo = new List<SqlParameter> { KetNoi.SoNguyen("@Toi", toi) };

            if (ky == LocKy.HienTai)
                sql.Append(" AND CAST(GETDATE() AS date) BETWEEN NgayBatDau AND NgayKetThuc");
            else if (ky == LocKy.DaHetKy)
                sql.Append(" AND NgayKetThuc < CAST(GETDATE() AS date)");

            if (maSinhVien.HasValue)
            {
                sql.Append(" AND MaSinhVien = @MaSinhVien");
                thamSo.Add(KetNoi.SoNguyen("@MaSinhVien", maSinhVien));
            }
            if (!string.IsNullOrEmpty(trangThai))
            {
                sql.Append(" AND TrangThai = @TrangThai");
                thamSo.Add(KetNoi.Chuoi("@TrangThai", trangThai, 20));
            }

            sql.Append(" ORDER BY SinhVien, TenDanhMuc, NgayBatDau");
            return KetNoi.TruyVan(sql.ToString(), thamSo.ToArray());
        }

        public static DataTable LaySinhVienDuocQuanLy(int toi)
        {
            return KetNoi.TruyVan(
                "SELECT MaNguoiDung, HoTen FROM dbo.NguoiDung " +
                "WHERE HoatDong = 1 AND VaiTro IN (N'SinhVien', N'CapNganSach') " +
                "  AND dbo.fn_CoQuyenQuanLy(@Toi, MaNguoiDung) = 1 " +
                "ORDER BY HoTen",
                KetNoi.SoNguyen("@Toi", toi));
        }

        public static DataTable LaySinhVienCuaToi(int toi)
        {
            return KetNoi.TruyVan(
                "SELECT MaNguoiDung, HoTen FROM dbo.NguoiDung " +
                "WHERE HoatDong = 1 AND MaNguoiCapNganSach = @Toi ORDER BY HoTen",
                KetNoi.SoNguyen("@Toi", toi));
        }

        public static DataTable LayDanhMucDangDung()
        {
            return KetNoi.TruyVan(
                "SELECT MaDanhMuc, TenDanhMuc FROM dbo.DanhMuc WHERE HoatDong = 1 ORDER BY TenDanhMuc");
        }

        public static DataRow LayHanMuc(int toi, int maHanMuc)
        {
            DataTable bang = KetNoi.TruyVan(
                "SELECT h.MaHanMuc, h.MaSinhVien, sv.HoTen AS SinhVien, h.MaDanhMuc, dm.TenDanhMuc, " +
                "       h.NgayBatDau, h.NgayKetThuc, h.SoTienHanMuc, h.SoTienDaChi, h.NguongCanhBao, " +
                "       h.LoaiHanMuc, h.TrangThai, h.PhienBan " +
                "FROM dbo.HanMuc h " +
                "JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien " +
                "JOIN dbo.DanhMuc dm ON dm.MaDanhMuc = h.MaDanhMuc " +
                "WHERE h.MaHanMuc = @MaHanMuc AND dbo.fn_CoQuyenQuanLy(@Toi, h.MaSinhVien) = 1",
                KetNoi.SoNguyen("@MaHanMuc", maHanMuc),
                KetNoi.SoNguyen("@Toi", toi));
            return bang.Rows.Count == 0 ? null : bang.Rows[0];
        }

        public static int TaoHanMuc(int toi, int maSinhVien, int maDanhMuc, DateTime ngayBatDau, DateTime ngayKetThuc,
                                    decimal soTien, string loai, decimal nguong)
        {
            DataTable kq = KetNoi.GoiProcedure("dbo.sp_TaoHanMuc",
                KetNoi.SoNguyen("@MaNguoiTao", toi),
                KetNoi.SoNguyen("@MaSinhVien", maSinhVien),
                KetNoi.SoNguyen("@MaDanhMuc", maDanhMuc),
                KetNoi.Ngay("@NgayBatDau", ngayBatDau),
                KetNoi.Ngay("@NgayKetThuc", ngayKetThuc),
                KetNoi.SoTien("@SoTienHanMuc", soTien),
                KetNoi.Chuoi("@LoaiHanMuc", loai, 10),
                KetNoi.TyLe("@NguongCanhBao", nguong));

            return Convert.ToInt32(kq.Rows[0]["MaHanMuc"]);
        }

        public static List<string> TaoChoTatCa(int toi, int maDanhMuc, DateTime ngayBatDau, DateTime ngayKetThuc,
                                               decimal soTien, string loai, decimal nguong, out int soDaTao)
        {
            List<string> danhSachLoi = new List<string>();
            soDaTao = 0;
            foreach (DataRow sv in LaySinhVienCuaToi(toi).Rows)
            {
                try
                {
                    TaoHanMuc(toi, (int)sv["MaNguoiDung"], maDanhMuc, ngayBatDau, ngayKetThuc, soTien, loai, nguong);
                    soDaTao++;
                }
                catch (SqlException ex)
                {
                    danhSachLoi.Add(sv["HoTen"] + ": " + LoiCSDL.Dich(ex));
                }
            }
            return danhSachLoi;
        }

        public static void DieuChinhHanMuc(int toi, int maHanMuc, decimal soTien, string loai, decimal nguong, byte[] phienBan)
        {
            KetNoi.GoiProcedure("dbo.sp_DieuChinhHanMuc",
                KetNoi.SoNguyen("@MaHanMuc", maHanMuc),
                KetNoi.SoNguyen("@MaNguoi", toi),
                KetNoi.SoTien("@SoTienHanMuc", soTien),
                KetNoi.Chuoi("@LoaiHanMuc", loai, 10),
                KetNoi.TyLe("@NguongCanhBao", nguong),
                KetNoi.PhienBan("@PhienBan", phienBan));
        }

        public static string KhoaMoKhoa(int toi, int maHanMuc, bool khoa)
        {
            DataTable kq = KetNoi.GoiProcedure("dbo.sp_KhoaMoKhoaHanMuc",
                KetNoi.SoNguyen("@MaHanMuc", maHanMuc),
                KetNoi.SoNguyen("@MaNguoi", toi),
                KetNoi.DungSai("@Khoa", khoa));
            return (string)kq.Rows[0]["TrangThai"];
        }
    }
}
