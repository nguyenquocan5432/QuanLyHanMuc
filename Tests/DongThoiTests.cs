using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Models;
using QuanLyHanMucWeb.Services;
using Xunit;

namespace QuanLyHanMucWeb.Tests
{
    [Collection("CSDL")]
    public class DongThoiTests
    {
        private readonly CoSoDuLieuThu _db;

        public DongThoiTests(CoSoDuLieuThu db)
        {
            _db = db;
        }

        [Fact]
        public async Task HaiMuoiGiaoDichCungLuc_KhongVuotHanMucCung()
        {
            int dm = _db.TaoDanhMuc("Đồng thời");
            int hm = HanMucService.TaoHanMuc(_db.MaPhuHuynh, _db.MaKhoa, dm,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, 1000000, "Cứng", 80);

            const int soLuong = 20;
            using ManualResetEventSlim xuatPhat = new ManualResetEventSlim(false);
            Task<KetQuaGhiNhan>[] viec = Enumerable.Range(0, soLuong).Select(i => Task.Run(() =>
            {
                xuatPhat.Wait();
                return GiaoDichService.GhiNhanGiaoDich(_db.MaKhoa, dm, 100000, DateTime.Today, "song song " + i);
            })).ToArray();
            xuatPhat.Set();
            KetQuaGhiNhan[] ketQua = await Task.WhenAll(viec);

            int duocGhi = ketQua.Count(k => k.DuocGhiNhan);
            int tuChoi = ketQua.Count(k => k.KetQua == "Từ chối");

            Assert.Equal(10, duocGhi);
            Assert.Equal(10, tuChoi);
            Assert.Equal(1000000, _db.SoDaChi(hm));
            Assert.Equal(10, _db.Dem("GiaoDich", "TrangThai = N'Hợp lệ'", hm));
            Assert.Equal(10, _db.Dem("NhatKyViPham", "KetQua = N'Từ chối'", hm));
            Assert.Equal(0, _db.SoDongLech());
        }

        [Fact]
        public async Task GhiVaHuyXenKe_SoDaChiVanKhop()
        {
            int dm = _db.TaoDanhMuc("Ghi hủy");
            int hm = HanMucService.TaoHanMuc(_db.MaPhuHuynh, _db.MaKhoa, dm,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, 5000000, "Cứng", 80);

            List<int> maCu = new List<int>();
            for (int i = 0; i < 10; i++)
                maCu.Add(GiaoDichService.GhiNhanGiaoDich(_db.MaKhoa, dm, 50000, DateTime.Today, "cũ " + i).MaGiaoDich.Value);

            List<Task> viec = new List<Task>();
            foreach (int ma in maCu)
                viec.Add(Task.Run(() => GiaoDichService.HuyGiaoDich(_db.MaPhuHuynh, ma, "hủy song song")));
            for (int i = 0; i < 10; i++)
                viec.Add(Task.Run(() => GiaoDichService.GhiNhanGiaoDich(_db.MaKhoa, dm, 70000, DateTime.Today, "mới")));
            await Task.WhenAll(viec);

            Assert.Equal(700000, _db.SoDaChi(hm));
            Assert.Equal(0, _db.SoDongLech());
        }

        [Fact]
        public void HaiNguoiCungSua_NguoiLuuSauBiBao()
        {
            int dm = _db.TaoDanhMuc("Lạc quan");
            int hm = HanMucService.TaoHanMuc(_db.MaPhuHuynh, _db.MaKhoa, dm,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, 1000000, "Mềm", 80);

            byte[] phienBanA = _db.PhienBan(hm);
            byte[] phienBanB = _db.PhienBan(hm);

            HanMucService.DieuChinhHanMuc(_db.MaPhuHuynh, hm, 1200000, "Mềm", 90, phienBanA);

            SqlException loi = Assert.Throws<SqlException>(() =>
                HanMucService.DieuChinhHanMuc(_db.MaPhuHuynh, hm, 800000, "Mềm", 70, phienBanB));
            Assert.Equal(50035, loi.Number);

            Assert.Equal(1200000m, (decimal)HanMucService.LayHanMuc(_db.MaPhuHuynh, hm)["SoTienHanMuc"]);
        }
    }
}
