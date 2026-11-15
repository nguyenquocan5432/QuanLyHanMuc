using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Models;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class GhiGiaoDichYeuCau
    {
        public int MaDanhMuc { get; set; }
        public DateTime Ngay { get; set; }
        public decimal SoTien { get; set; }
        public string MoTa { get; set; }
    }

    public class HuyGiaoDichYeuCau
    {
        public string LyDo { get; set; }
    }

    [Authorize(Policy = ChinhSach.GhiGiaoDich)]
    [Route("api/giao-dich")]
    public class GiaoDichController : CoSoController
    {

        [HttpGet("nhap")]
        [Authorize(Roles = "SinhVien")]
        public IActionResult TrangNhap(int? maDanhMuc, DateTime? ngay)
        {
            DateTime ngayChon = (ngay ?? DateTime.Today).Date;
            HanMuc hanMuc = null;
            DataTable ganDay = null;
            if (maDanhMuc.HasValue)
            {
                hanMuc = GiaoDichService.LayHanMucHieuLuc(Toi, maDanhMuc.Value, ngayChon);
                if (hanMuc != null)
                    ganDay = GiaoDichService.LayGanDay(hanMuc.MaHanMuc);
            }
            return Ok(new
            {
                ngay = ngayChon,
                danhMucs = Bang.DanhSach(GiaoDichService.LayDanhMucCoHanMuc(Toi, ngayChon)),
                hanMuc = hanMuc == null ? null : new
                {
                    hanMuc.MaHanMuc,
                    hanMuc.TenDanhMuc,
                    hanMuc.SoTienHanMuc,
                    hanMuc.SoTienDaChi,
                    hanMuc.NguongCanhBao,
                    hanMuc.LoaiHanMuc,
                    hanMuc.TrangThai,
                    soConLai = hanMuc.SoConLai(),
                    tyLeSuDung = Math.Round(hanMuc.TyLeSuDung(), 2)
                },
                ganDay = ganDay == null ? null : Bang.DanhSach(ganDay)
            });
        }

        [HttpGet("danh-gia")]
        [Authorize(Roles = "SinhVien")]
        public IActionResult DanhGia(int maDanhMuc, DateTime ngay, decimal soTien)
        {
            HanMuc hm = GiaoDichService.LayHanMucHieuLuc(Toi, maDanhMuc, ngay.Date);
            if (hm == null)
                return Ok(new { mau = "xam", thongDiep = "Chưa có hạn mức cho danh mục này vào ngày đã chọn.", choGhi = false, nhanNut = "Ghi nhận" });

            if (soTien <= 0)
            {
                if (hm.DaKhoa)
                    return Ok(new { mau = "xam", thongDiep = "Hạn mức đang bị khóa, chưa thể ghi giao dịch.", choGhi = false, nhanNut = "Ghi nhận" });
                return Ok(new { mau = "", thongDiep = "", choGhi = false, nhanNut = "Ghi nhận" });
            }

            decimal tyLe = Math.Round(hm.TyLeDuKien(soTien), 2);
            switch (hm.DanhGiaGiaoDich(soTien))
            {
                case KetQuaDanhGia.DaKhoa:
                    return Ok(new { mau = "xam", thongDiep = "Hạn mức đang bị khóa, chưa thể ghi giao dịch.", choGhi = false, nhanNut = "Ghi nhận", tyLe });
                case KetQuaDanhGia.TuChoi:
                    return Ok(new { mau = "do", thongDiep = $"Giao dịch vượt hạn mức còn lại (còn {hm.SoConLai():N0} đ). Không thể ghi nhận.", choGhi = false, nhanNut = "Ghi nhận", tyLe });
                case KetQuaDanhGia.ChoDuyet:
                    return Ok(new { mau = "do-nhat", thongDiep = "Giao dịch vượt hạn mức, sẽ chuyển người cấp ngân sách duyệt.", choGhi = true, nhanNut = "Gửi chờ duyệt", tyLe });
                case KetQuaDanhGia.ChapNhanDungMuc:
                    return Ok(new { mau = "cam-dam", thongDiep = "Giao dịch này dùng hết hạn mức.", choGhi = true, nhanNut = "Ghi nhận", tyLe });
                case KetQuaDanhGia.ChapNhanCanhBao:
                    return Ok(new { mau = "cam", thongDiep = $"Sau giao dịch này bạn sẽ dùng {tyLe:0.##}% hạn mức.", choGhi = true, nhanNut = "Ghi nhận", tyLe });
                default:
                    return Ok(new { mau = "", thongDiep = "", choGhi = true, nhanNut = "Ghi nhận", tyLe });
            }
        }

        [HttpPost]
        [Authorize(Roles = "SinhVien")]
        public IActionResult GhiNhan(GhiGiaoDichYeuCau yeuCau)
        {
            try
            {
                KetQuaGhiNhan kq = GiaoDichService.GhiNhanGiaoDich(Toi, yeuCau.MaDanhMuc, yeuCau.SoTien, yeuCau.Ngay.Date, yeuCau.MoTa);
                string muc = kq.KetQua == "Chấp nhận" ? "thanh-cong"
                           : kq.DuocGhiNhan || kq.KetQua == "Chờ duyệt" ? "canh-bao"
                           : "loi";
                return Ok(new { ketQua = kq.KetQua, thongBao = kq.KetQua + ". " + kq.ThongDiep, muc, maGiaoDich = kq.MaGiaoDich });
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
        }

        [HttpGet]
        public IActionResult LichSu(DateTime? tu, DateTime? den, int? maDanhMuc, string trangThai)
        {
            KhoangNgay(tu, den, out DateTime tuNgay, out DateTime denNgay);
            DataTable ds = GiaoDichService.LayLichSu(Toi, tuNgay, denNgay, maDanhMuc, string.IsNullOrEmpty(trangThai) ? null : trangThai);
            return Ok(new
            {
                tu = tuNgay,
                den = denNgay,
                danhMucs = Bang.DanhSach(HanMucService.LayDanhMucDangDung()),
                giaoDichs = Bang.DanhSach(ds)
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = ChinhSach.QuanLyHanMuc)]
        public IActionResult MotGiaoDich(int id)
        {
            DataRow gd = GiaoDichService.LayGiaoDich(Toi, id);
            if (gd == null)
                return NotFound(new { loi = "Không tìm thấy giao dịch hoặc bạn không có quyền hủy giao dịch này." });
            return Ok(Bang.Dong(gd));
        }

        [HttpPost("{id:int}/huy")]
        [Authorize(Policy = ChinhSach.QuanLyHanMuc)]
        public IActionResult Huy(int id, HuyGiaoDichYeuCau yeuCau)
        {
            if (string.IsNullOrWhiteSpace(yeuCau.LyDo))
                return Loi("Hủy giao dịch phải ghi lý do (BR07).");
            try
            {
                GiaoDichService.HuyGiaoDich(Toi, id, yeuCau.LyDo.Trim());
                return Xong("Đã hủy giao dịch. Số đã chi của hạn mức đã được trừ lại.");
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
        }
    }
}
