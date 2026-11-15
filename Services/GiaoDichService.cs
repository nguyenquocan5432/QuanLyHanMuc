using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Models;

namespace QuanLyHanMucWeb.Services
{
    public static class GiaoDichService
    {
        public static DataTable LayDanhMucCoHanMuc(int toi, DateTime ngay)
        {
            return KetNoi.TruyVan(
                "SELECT dm.MaDanhMuc, dm.TenDanhMuc, " +
                "       CAST(CASE WHEN x.MaDanhMuc IS NULL THEN 0 ELSE 1 END AS bit) AS CoHanMuc " +
                "FROM dbo.DanhMuc dm " +
                "LEFT JOIN (SELECT DISTINCT MaDanhMuc FROM dbo.HanMuc " +
                "           WHERE MaSinhVien = @Toi AND @Ngay BETWEEN NgayBatDau AND NgayKetThuc) x " +
                "       ON x.MaDanhMuc = dm.MaDanhMuc " +
                "WHERE dm.HoatDong = 1 OR x.MaDanhMuc IS NOT NULL " +
                "ORDER BY CoHanMuc DESC, dm.TenDanhMuc",
                KetNoi.SoNguyen("@Toi", toi),
                KetNoi.Ngay("@Ngay", ngay));
        }

        public static HanMuc LayHanMucHieuLuc(int toi, int maDanhMuc, DateTime ngay)
        {
            DataTable bang = KetNoi.TruyVan(
                "SELECT TOP (1) MaHanMuc, TenDanhMuc, SoTienHanMuc, SoTienDaChi, NguongCanhBao, LoaiHanMuc, TrangThai " +
                "FROM dbo.vw_TinhTrangHanMuc " +
                "WHERE MaSinhVien = @Toi AND MaDanhMuc = @MaDanhMuc " +
                "  AND @Ngay BETWEEN NgayBatDau AND NgayKetThuc " +
                "ORDER BY CASE WHEN TrangThai = N'Đã khóa' THEN 1 ELSE 0 END",
                KetNoi.SoNguyen("@Toi", toi),
                KetNoi.SoNguyen("@MaDanhMuc", maDanhMuc),
                KetNoi.Ngay("@Ngay", ngay));

            if (bang.Rows.Count == 0)
                return null;

            DataRow d = bang.Rows[0];
            return new HanMuc
            {
                MaHanMuc = (int)d["MaHanMuc"],
                TenDanhMuc = (string)d["TenDanhMuc"],
                SoTienHanMuc = (decimal)d["SoTienHanMuc"],
                SoTienDaChi = (decimal)d["SoTienDaChi"],
                NguongCanhBao = (decimal)d["NguongCanhBao"],
                LoaiHanMuc = (string)d["LoaiHanMuc"],
                TrangThai = (string)d["TrangThai"]
            };
        }

        public static DataTable LayHanMucCuaToi(int toi)
        {
            return KetNoi.TruyVan(
                "SELECT MaHanMuc, MaDanhMuc, TenDanhMuc, SoTienHanMuc, SoTienDaChi, SoConLai, TyLeSuDung, NguongCanhBao, " +
                "       LoaiHanMuc, TrangThai, NgayBatDau, NgayKetThuc " +
                "FROM dbo.vw_TinhTrangHanMuc " +
                "WHERE MaSinhVien = @Toi AND CAST(GETDATE() AS date) BETWEEN NgayBatDau AND NgayKetThuc " +
                "ORDER BY TyLeSuDung DESC",
                KetNoi.SoNguyen("@Toi", toi));
        }

        public static DataTable LayGanDay(int maHanMuc)
        {
            return KetNoi.TruyVan(
                "SELECT TOP (5) NgayGiaoDich, SoTien, TrangThai, MoTa FROM dbo.GiaoDich " +
                "WHERE MaHanMuc = @MaHanMuc ORDER BY NgayGiaoDich DESC, MaGiaoDich DESC",
                KetNoi.SoNguyen("@MaHanMuc", maHanMuc));
        }

        public static KetQuaGhiNhan GhiNhanGiaoDich(int toi, int maDanhMuc, decimal soTien, DateTime ngay, string moTa)
        {
            DataTable kq = KetNoi.GoiProcedure("dbo.sp_GhiNhanGiaoDich",
                KetNoi.SoNguyen("@MaSinhVien", toi),
                KetNoi.SoNguyen("@MaDanhMuc", maDanhMuc),
                KetNoi.SoTien("@SoTien", soTien),
                KetNoi.Ngay("@NgayGiaoDich", ngay),
                KetNoi.Chuoi("@MoTa", string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim(), 255));

            DataRow d = kq.Rows[0];
            return new KetQuaGhiNhan
            {
                KetQua = d["KetQua"] as string,
                MaGiaoDich = d["MaGiaoDich"] == DBNull.Value ? (int?)null : Convert.ToInt32(d["MaGiaoDich"]),
                TyLeDuKien = d["TyLeDuKien"] == DBNull.Value ? (decimal?)null : (decimal)d["TyLeDuKien"],
                SoConLai = d["SoConLai"] == DBNull.Value ? (decimal?)null : (decimal)d["SoConLai"],
                ThongDiep = d["ThongDiep"] as string
            };
        }

        public static DataTable LayLichSu(int toi, DateTime tu, DateTime den, int? maDanhMuc, string trangThai)
        {
            StringBuilder sql = new StringBuilder(
                "SELECT g.MaGiaoDich, g.NgayGiaoDich, sv.HoTen AS SinhVien, dm.TenDanhMuc, g.SoTien, g.MoTa, " +
                "       g.TrangThai, g.LyDoHuy, h.MaSinhVien " +
                "FROM dbo.GiaoDich g " +
                "JOIN dbo.HanMuc h     ON h.MaHanMuc = g.MaHanMuc " +
                "JOIN dbo.DanhMuc dm   ON dm.MaDanhMuc = h.MaDanhMuc " +
                "JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien " +
                "WHERE (h.MaSinhVien = @Toi OR dbo.fn_CoQuyenQuanLy(@Toi, h.MaSinhVien) = 1) " +
                "  AND g.NgayGiaoDich BETWEEN @Tu AND @Den");

            List<SqlParameter> thamSo = new List<SqlParameter>
            {
                KetNoi.SoNguyen("@Toi", toi),
                KetNoi.Ngay("@Tu", tu),
                KetNoi.Ngay("@Den", den)
            };

            if (maDanhMuc.HasValue)
            {
                sql.Append(" AND h.MaDanhMuc = @MaDanhMuc");
                thamSo.Add(KetNoi.SoNguyen("@MaDanhMuc", maDanhMuc));
            }
            if (!string.IsNullOrEmpty(trangThai))
            {
                sql.Append(" AND g.TrangThai = @TrangThai");
                thamSo.Add(KetNoi.Chuoi("@TrangThai", trangThai, 20));
            }

            sql.Append(" ORDER BY g.NgayGiaoDich DESC, g.MaGiaoDich DESC");
            return KetNoi.TruyVan(sql.ToString(), thamSo.ToArray());
        }

        public static DataRow LayGiaoDich(int toi, int maGiaoDich)
        {
            DataTable bang = KetNoi.TruyVan(
                "SELECT g.MaGiaoDich, g.NgayGiaoDich, sv.HoTen AS SinhVien, dm.TenDanhMuc, g.SoTien, g.MoTa, g.TrangThai " +
                "FROM dbo.GiaoDich g " +
                "JOIN dbo.HanMuc h     ON h.MaHanMuc = g.MaHanMuc " +
                "JOIN dbo.DanhMuc dm   ON dm.MaDanhMuc = h.MaDanhMuc " +
                "JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien " +
                "WHERE g.MaGiaoDich = @Ma AND dbo.fn_CoQuyenQuanLy(@Toi, h.MaSinhVien) = 1",
                KetNoi.SoNguyen("@Ma", maGiaoDich),
                KetNoi.SoNguyen("@Toi", toi));
            return bang.Rows.Count == 0 ? null : bang.Rows[0];
        }

        public static void HuyGiaoDich(int toi, int maGiaoDich, string lyDo)
        {
            KetNoi.GoiProcedure("dbo.sp_HuyGiaoDich",
                KetNoi.SoNguyen("@MaGiaoDich", maGiaoDich),
                KetNoi.SoNguyen("@MaNguoiHuy", toi),
                KetNoi.Chuoi("@LyDo", lyDo, 255));
        }
    }
}
