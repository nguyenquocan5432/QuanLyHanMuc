using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;

namespace QuanLyHanMucWeb.Services
{
    public static class BaoCaoService
    {
        public static DataTable ChiTieuTheoKy(int toi, int maSinhVien, DateTime tu, DateTime den)
        {
            return KetNoi.TruyVan(
                "SELECT dm.TenDanhMuc, " +
                "       SUM(h.SoTienHanMuc) AS SoTienHanMuc, " +
                "       SUM(ISNULL(g.DaChi, 0)) AS DaChi, " +
                "       SUM(h.SoTienHanMuc) - SUM(ISNULL(g.DaChi, 0)) AS ConLai, " +
                "       CAST(SUM(ISNULL(g.DaChi, 0)) * 100.0 / SUM(h.SoTienHanMuc) AS decimal(9,2)) AS TyLe, " +
                "       MIN(h.NguongCanhBao) AS Nguong " +
                "FROM dbo.HanMuc h " +
                "JOIN dbo.DanhMuc dm ON dm.MaDanhMuc = h.MaDanhMuc " +
                "OUTER APPLY (SELECT SUM(SoTien) AS DaChi FROM dbo.GiaoDich " +
                "             WHERE MaHanMuc = h.MaHanMuc AND TrangThai = N'Hợp lệ' " +
                "               AND NgayGiaoDich BETWEEN @Tu AND @Den) g " +
                "WHERE h.MaSinhVien = @MaSinhVien " +
                "  AND (h.MaSinhVien = @Toi OR dbo.fn_CoQuyenQuanLy(@Toi, h.MaSinhVien) = 1) " +
                "  AND h.NgayBatDau <= @Den AND h.NgayKetThuc >= @Tu " +
                "GROUP BY dm.TenDanhMuc " +
                "ORDER BY DaChi DESC",
                KetNoi.SoNguyen("@Toi", toi),
                KetNoi.SoNguyen("@MaSinhVien", maSinhVien),
                KetNoi.Ngay("@Tu", tu),
                KetNoi.Ngay("@Den", den));
        }

        public static DataTable ViPham(int toi, int? maSinhVien, DateTime tu, DateTime den)
        {
            StringBuilder sql = new StringBuilder(
                "SELECT vp.NgayXayRa, sv.HoTen AS SinhVien, dm.TenDanhMuc, vp.SoTienDinhChi, vp.SoConLaiLucDo, vp.KetQua " +
                "FROM dbo.NhatKyViPham vp " +
                "JOIN dbo.HanMuc    h  ON h.MaHanMuc = vp.MaHanMuc " +
                "JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien " +
                "JOIN dbo.DanhMuc   dm ON dm.MaDanhMuc = h.MaDanhMuc " +
                "WHERE dbo.fn_CoQuyenQuanLy(@Toi, h.MaSinhVien) = 1 " +
                "  AND vp.NgayXayRa >= @Tu AND vp.NgayXayRa < DATEADD(DAY, 1, @Den)");

            List<SqlParameter> thamSo = new List<SqlParameter>
            {
                KetNoi.SoNguyen("@Toi", toi),
                KetNoi.Ngay("@Tu", tu),
                KetNoi.Ngay("@Den", den)
            };
            if (maSinhVien.HasValue)
            {
                sql.Append(" AND h.MaSinhVien = @MaSinhVien");
                thamSo.Add(KetNoi.SoNguyen("@MaSinhVien", maSinhVien));
            }
            sql.Append(" ORDER BY vp.NgayXayRa DESC");
            return KetNoi.TruyVan(sql.ToString(), thamSo.ToArray());
        }
    }
}
