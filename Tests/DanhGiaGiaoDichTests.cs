using QuanLyHanMucWeb.Models;
using Xunit;

namespace QuanLyHanMucWeb.Tests
{
    public class DanhGiaGiaoDichTests
    {
        private static HanMuc Tao(string loai, decimal daChi, string trangThai = "Hoạt động", decimal nguong = 80)
        {
            return new HanMuc { SoTienHanMuc = 2000000, SoTienDaChi = daChi, NguongCanhBao = nguong, LoaiHanMuc = loai, TrangThai = trangThai };
        }

        [Fact]
        public void DuoiNguong_ChapNhan()
        {
            Assert.Equal(KetQuaDanhGia.ChapNhan, Tao("Cứng", 1400000).DanhGiaGiaoDich(100000));
        }

        [Fact]
        public void ChamNguong_ChapNhanKemCanhBao()
        {
            Assert.Equal(KetQuaDanhGia.ChapNhanCanhBao, Tao("Cứng", 1400000).DanhGiaGiaoDich(200000));
        }

        [Fact]
        public void DungMotTram_ChapNhanDungMuc()
        {
            Assert.Equal(KetQuaDanhGia.ChapNhanDungMuc, Tao("Cứng", 1400000).DanhGiaGiaoDich(600000));
        }

        [Fact]
        public void VuotHanMucCung_TuChoi()
        {
            Assert.Equal(KetQuaDanhGia.TuChoi, Tao("Cứng", 1400000).DanhGiaGiaoDich(601000));
        }

        [Fact]
        public void VuotHanMucMem_ChoDuyet()
        {
            Assert.Equal(KetQuaDanhGia.ChoDuyet, Tao("Mềm", 1400000).DanhGiaGiaoDich(601000));
        }

        [Fact]
        public void HanMucDaKhoa_KhongGhiDuoc()
        {
            Assert.Equal(KetQuaDanhGia.DaKhoa, Tao("Mềm", 0, "Đã khóa").DanhGiaGiaoDich(1000));
        }

        [Theory]
        [InlineData(1400000, 70)]
        [InlineData(1380000, 69)]
        [InlineData(0, 0)]
        public void TyLeSuDung_TinhDung(int daChi, int mongDoi)
        {
            Assert.Equal((decimal)mongDoi, Tao("Cứng", daChi).TyLeSuDung());
        }

        [Fact]
        public void SoConLai_AmKhiHanMucMemDaVuot()
        {
            Assert.Equal(-130000m, Tao("Mềm", 2130000).SoConLai());
        }
    }
}
