using System.Data;
using QuanLyHanMucWeb.Data;

namespace QuanLyHanMucWeb.Services
{
    public static class ThongBaoService
    {
        public static int DemChuaDoc(int toi)
        {
            object kq = KetNoi.LayMotGiaTri(
                "SELECT COUNT(*) FROM dbo.ThongBao WHERE MaNguoiNhan = @Toi AND DaDoc = 0",
                KetNoi.SoNguyen("@Toi", toi));
            return Convert.ToInt32(kq);
        }

        public static DataTable LayDanhSach(int toi)
        {
            return KetNoi.TruyVan(
                "SELECT MaThongBao, NgayTao, LoaiThongBao, NoiDung, TinhTrangGui, DaDoc " +
                "FROM dbo.ThongBao WHERE MaNguoiNhan = @Toi " +
                "ORDER BY DaDoc, NgayTao DESC, MaThongBao DESC",
                KetNoi.SoNguyen("@Toi", toi));
        }

        public static void DanhDauDaDoc(int toi, int? maThongBao)
        {
            if (maThongBao.HasValue)
                KetNoi.ThucThi(
                    "UPDATE dbo.ThongBao SET DaDoc = 1 WHERE MaThongBao = @Ma AND MaNguoiNhan = @Toi",
                    KetNoi.SoNguyen("@Ma", maThongBao), KetNoi.SoNguyen("@Toi", toi));
            else
                KetNoi.ThucThi(
                    "UPDATE dbo.ThongBao SET DaDoc = 1 WHERE MaNguoiNhan = @Toi AND DaDoc = 0",
                    KetNoi.SoNguyen("@Toi", toi));
        }
    }
}
