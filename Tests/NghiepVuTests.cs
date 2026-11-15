using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Models;
using QuanLyHanMucWeb.Services;
using Xunit;

namespace QuanLyHanMucWeb.Tests
{
    [Collection("CSDL")]
    public class NghiepVuTests
    {
        private readonly CoSoDuLieuThu _db;

        public NghiepVuTests(CoSoDuLieuThu db)
        {
            _db = db;
        }

        private int TaoHanMuc(int maDanhMuc, decimal soTien, string loai, decimal nguong = 80)
        {
            return HanMucService.TaoHanMuc(_db.MaPhuHuynh, _db.MaKhoa, maDanhMuc,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, soTien, loai, nguong);
        }

        private KetQuaGhiNhan Ghi(int maDanhMuc, decimal soTien)
        {
            return GiaoDichService.GhiNhanGiaoDich(_db.MaKhoa, maDanhMuc, soTien, DateTime.Today, "test");
        }

        [Fact]
        public void HanMucCung_DiHetCacMuc()
        {
            int dm = _db.TaoDanhMuc("Cứng");
            int hm = TaoHanMuc(dm, 1000000, "Cứng");

            Assert.Equal("Chấp nhận", Ghi(dm, 700000).KetQua);
            Assert.Equal("Chấp nhận kèm cảnh báo", Ghi(dm, 100000).KetQua);
            Assert.Equal("Sắp vượt", _db.TrangThaiHanMuc(hm));
            Assert.Equal("Từ chối", Ghi(dm, 300000).KetQua);
            Assert.Equal("Chấp nhận đúng mức", Ghi(dm, 200000).KetQua);
            Assert.Equal("Đã vượt", _db.TrangThaiHanMuc(hm));
            Assert.Equal("Từ chối", Ghi(dm, 1000).KetQua);

            Assert.Equal(1000000, _db.SoDaChi(hm));
            Assert.Equal(2, _db.Dem("NhatKyViPham", "KetQua = N'Từ chối'", hm));
            Assert.Equal(3, _db.Dem("GiaoDich", "TrangThai = N'Hợp lệ'", hm));
        }

        [Fact]
        public void HanMucMem_VuotThiChoDuyet_DuyetXongMoiCong()
        {
            int dm = _db.TaoDanhMuc("Mềm");
            int hm = TaoHanMuc(dm, 1000000, "Mềm");

            Assert.Equal("Chấp nhận", Ghi(dm, 600000).KetQua);
            KetQuaGhiNhan kq = Ghi(dm, 500000);
            Assert.Equal("Chờ duyệt", kq.KetQua);
            Assert.Equal(600000, _db.SoDaChi(hm));

            int maYeuCau = _db.MaYeuCau(kq.MaGiaoDich.Value);
            DataRow yeuCau = NgoaiLeService.LayDanhSachChoDuyet(_db.MaPhuHuynh).Select("MaYeuCau = " + maYeuCau)[0];
            Assert.Equal(100000m, (decimal)yeuCau["VuotBaoNhieu"]);

            SqlException saiQuyen = Assert.Throws<SqlException>(() => NgoaiLeService.XuLy(_db.MaKhoa, maYeuCau, true, null));
            Assert.Equal(50014, saiQuyen.Number);

            NgoaiLeService.XuLy(_db.MaPhuHuynh, maYeuCau, true, "Đồng ý");
            Assert.Equal(1100000, _db.SoDaChi(hm));
            Assert.Equal("Đã vượt", _db.TrangThaiHanMuc(hm));

            SqlException daXuLy = Assert.Throws<SqlException>(() => NgoaiLeService.XuLy(_db.MaPhuHuynh, maYeuCau, true, null));
            Assert.Equal(50013, daXuLy.Number);
        }

        [Fact]
        public void TuChoiNgoaiLe_PhaiCoLyDo_VaKhongCongSoDaChi()
        {
            int dm = _db.TaoDanhMuc("Mềm");
            int hm = TaoHanMuc(dm, 500000, "Mềm");
            KetQuaGhiNhan kq = Ghi(dm, 600000);
            int maYeuCau = _db.MaYeuCau(kq.MaGiaoDich.Value);

            SqlException thieuLyDo = Assert.Throws<SqlException>(() => NgoaiLeService.XuLy(_db.MaPhuHuynh, maYeuCau, false, ""));
            Assert.Equal(50011, thieuLyDo.Number);

            NgoaiLeService.XuLy(_db.MaPhuHuynh, maYeuCau, false, "Khoản này không cần thiết");
            Assert.Equal(0, _db.SoDaChi(hm));
            Assert.Equal(1, _db.Dem("GiaoDich", "TrangThai = N'Từ chối'", hm));
        }

        [Fact]
        public void TaoHanMucTrungKy_BiChan()
        {
            int dm = _db.TaoDanhMuc("Trùng kỳ");
            TaoHanMuc(dm, 1000000, "Cứng");
            SqlException loi = Assert.Throws<SqlException>(() => TaoHanMuc(dm, 500000, "Mềm"));
            Assert.Equal(50001, loi.Number);
        }

        [Fact]
        public void SinhVienCoNguoiCap_KhongTuTaoHanMuc()
        {
            int dm = _db.TaoDanhMuc("Quyền");
            SqlException loi = Assert.Throws<SqlException>(() => HanMucService.TaoHanMuc(_db.MaKhoa, _db.MaKhoa, dm,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, 500000, "Cứng", 80));
            Assert.Equal(50031, loi.Number);
        }

        [Fact]
        public void SinhVienTuQuanLy_TuTaoHanMucDuoc()
        {
            int dm = _db.TaoDanhMuc("Tự quản lý");
            int hm = HanMucService.TaoHanMuc(_db.MaBinh, _db.MaBinh, dm,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, 500000, "Mềm", 80);
            Assert.True(hm > 0);
        }

        [Fact]
        public void KhoaHanMuc_ChanGiaoDich_MoKhoaTinhLaiTrangThai()
        {
            int dm = _db.TaoDanhMuc("Khóa");
            int hm = TaoHanMuc(dm, 1000000, "Cứng");
            Ghi(dm, 850000);

            SqlException saiQuyen = Assert.Throws<SqlException>(() => HanMucService.KhoaMoKhoa(_db.MaKhoa, hm, true));
            Assert.Equal(50034, saiQuyen.Number);

            Assert.Equal("Đã khóa", HanMucService.KhoaMoKhoa(_db.MaPhuHuynh, hm, true));
            Assert.Equal("Từ chối", Ghi(dm, 10000).KetQua);
            Assert.Equal("Sắp vượt", HanMucService.KhoaMoKhoa(_db.MaPhuHuynh, hm, false));
        }

        [Fact]
        public void HaHanMucCungDuoiSoDaChi_BiChan()
        {
            int dm = _db.TaoDanhMuc("Hạ");
            int hm = TaoHanMuc(dm, 1000000, "Cứng");
            Ghi(dm, 700000);
            SqlException loi = Assert.Throws<SqlException>(() =>
                HanMucService.DieuChinhHanMuc(_db.MaPhuHuynh, hm, 600000, "Cứng", 80, _db.PhienBan(hm)));
            Assert.Equal(50037, loi.Number);
        }

        [Fact]
        public void HuyGiaoDich_TruLaiSoDaChi_HuyLanHaiBiChan()
        {
            int dm = _db.TaoDanhMuc("Hủy");
            int hm = TaoHanMuc(dm, 1000000, "Cứng");
            Ghi(dm, 500000);
            int ma = Ghi(dm, 400000).MaGiaoDich.Value;

            SqlException thieuLyDo = Assert.Throws<SqlException>(() => GiaoDichService.HuyGiaoDich(_db.MaPhuHuynh, ma, " "));
            Assert.Equal(50021, thieuLyDo.Number);

            GiaoDichService.HuyGiaoDich(_db.MaPhuHuynh, ma, "Nhập nhầm số tiền");
            Assert.Equal(500000, _db.SoDaChi(hm));
            Assert.Equal("Hoạt động", _db.TrangThaiHanMuc(hm));

            SqlException daHuy = Assert.Throws<SqlException>(() => GiaoDichService.HuyGiaoDich(_db.MaPhuHuynh, ma, "lần hai"));
            Assert.Equal(50023, daHuy.Number);
        }

        [Fact]
        public void GhiThangVaoBang_VanBiTriggerChan()
        {
            int dm = _db.TaoDanhMuc("Trigger");
            int hm = TaoHanMuc(dm, 1000000, "Cứng");
            Ghi(dm, 900000);

            SqlException loi = Assert.Throws<SqlException>(() => QuanLyHanMucWeb.Data.KetNoi.ThucThi(
                "INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, TrangThai) VALUES (@Ma, 200000, CAST(GETDATE() AS date), N'Hợp lệ')",
                QuanLyHanMucWeb.Data.KetNoi.SoNguyen("@Ma", hm)));
            Assert.Equal(50004, loi.Number);
            Assert.Equal(900000, _db.SoDaChi(hm));
        }

        [Fact]
        public void SauMoiBaiTest_SoDaChiKhopTongGiaoDich()
        {
            Assert.Equal(0, _db.SoDongLech());
        }
    }
}
