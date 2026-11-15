namespace QuanLyHanMucWeb.Models
{
    public enum KetQuaDanhGia
    {
        ChapNhan,
        ChapNhanCanhBao,
        ChapNhanDungMuc,
        ChoDuyet,
        TuChoi,
        DaKhoa
    }

    public class HanMuc
    {
        public int MaHanMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public decimal SoTienHanMuc { get; set; }
        public decimal SoTienDaChi { get; set; }
        public decimal NguongCanhBao { get; set; }
        public string LoaiHanMuc { get; set; }
        public string TrangThai { get; set; }

        public bool LaCung => LoaiHanMuc == "Cứng";
        public bool DaKhoa => TrangThai == "Đã khóa";

        public decimal TyLeSuDung() => SoTienDaChi * 100m / SoTienHanMuc;

        public decimal SoConLai() => SoTienHanMuc - SoTienDaChi;

        public decimal TyLeDuKien(decimal soTien) => (SoTienDaChi + soTien) * 100m / SoTienHanMuc;

        public KetQuaDanhGia DanhGiaGiaoDich(decimal soTien)
        {
            if (DaKhoa)
                return KetQuaDanhGia.DaKhoa;

            decimal tongDuKien = SoTienDaChi + soTien;

            if (tongDuKien > SoTienHanMuc)
                return LaCung ? KetQuaDanhGia.TuChoi : KetQuaDanhGia.ChoDuyet;

            if (tongDuKien == SoTienHanMuc)
                return KetQuaDanhGia.ChapNhanDungMuc;

            if (TyLeDuKien(soTien) >= NguongCanhBao)
                return KetQuaDanhGia.ChapNhanCanhBao;

            return KetQuaDanhGia.ChapNhan;
        }
    }

    public class KetQuaGhiNhan
    {
        public string KetQua { get; set; }
        public int? MaGiaoDich { get; set; }
        public decimal? TyLeDuKien { get; set; }
        public decimal? SoConLai { get; set; }
        public string ThongDiep { get; set; }

        public bool DuocGhiNhan => KetQua != null && KetQua.StartsWith("Chấp nhận");
    }

    public class NguoiDungDangNhap
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }
        public int? MaNguoiCapNganSach { get; set; }
    }
}
