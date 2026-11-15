using System.Data;
using QuanLyHanMucWeb.Data;

namespace QuanLyHanMucWeb.Services
{
    public static class NgoaiLeService
    {
        public static DataTable LayDanhSachChoDuyet(int toi)
        {
            return KetNoi.TruyVan(
                "SELECT yc.MaYeuCau, sv.HoTen AS SinhVien, dm.TenDanhMuc, gd.SoTien, gd.MoTa, gd.NgayGiaoDich, " +
                "       h.SoTienHanMuc - h.SoTienDaChi               AS ConLaiTruocGiaoDich, " +
                "       gd.SoTien - (h.SoTienHanMuc - h.SoTienDaChi) AS VuotBaoNhieu, " +
                "       yc.NgayYeuCau, " +
                "       CAST(CASE WHEN h.TrangThai = N'Đã khóa' THEN 1 ELSE 0 END AS bit) AS HanMucDaKhoa " +
                "FROM dbo.YeuCauNgoaiLe yc " +
                "JOIN dbo.GiaoDich  gd ON gd.MaGiaoDich = yc.MaGiaoDich " +
                "JOIN dbo.HanMuc    h  ON h.MaHanMuc = gd.MaHanMuc " +
                "JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien " +
                "JOIN dbo.DanhMuc   dm ON dm.MaDanhMuc = h.MaDanhMuc " +
                "WHERE yc.TrangThai = N'Chờ duyệt' " +
                "  AND dbo.fn_CoQuyenQuanLy(@Toi, h.MaSinhVien) = 1 " +
                "ORDER BY yc.NgayYeuCau",
                KetNoi.SoNguyen("@Toi", toi));
        }

        public static string XuLy(int toi, int maYeuCau, bool dongY, string ghiChu)
        {
            DataTable kq = KetNoi.GoiProcedure("dbo.sp_DuyetNgoaiLe",
                KetNoi.SoNguyen("@MaYeuCau", maYeuCau),
                KetNoi.SoNguyen("@MaNguoiDuyet", toi),
                KetNoi.DungSai("@DongY", dongY),
                KetNoi.Chuoi("@GhiChu", string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim(), 255));

            return kq.Rows[0]["ThongDiep"] as string;
        }
    }
}
