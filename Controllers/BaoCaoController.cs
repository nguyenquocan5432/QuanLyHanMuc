using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    [Authorize(Policy = ChinhSach.GhiGiaoDich)]
    [Route("api/bao-cao")]
    public class BaoCaoController : CoSoController
    {
        [HttpGet("chi-tieu")]
        public IActionResult ChiTieu(DateTime? tu, DateTime? den, int? maSinhVien)
        {
            KhoangNgay(tu, den, out DateTime tuNgay, out DateTime denNgay);
            bool quanLy = User.DuocQuanLyHanMuc();
            DataTable sinhViens = null;

            if (!quanLy)
            {
                maSinhVien = Toi;
            }
            else
            {
                sinhViens = HanMucService.LaySinhVienDuocQuanLy(Toi);
                if (!maSinhVien.HasValue && sinhViens.Rows.Count > 0)
                    maSinhVien = (int)sinhViens.Rows[0]["MaNguoiDung"];
            }

            return Ok(new
            {
                tu = tuNgay,
                den = denNgay,
                maSinhVien,
                sinhViens = sinhViens == null ? null : Bang.DanhSach(sinhViens),
                chiTieu = maSinhVien.HasValue
                    ? Bang.DanhSach(BaoCaoService.ChiTieuTheoKy(Toi, maSinhVien.Value, tuNgay, denNgay))
                    : new List<Dictionary<string, object>>()
            });
        }

        [HttpGet("vi-pham")]
        [Authorize(Policy = ChinhSach.QuanLyHanMuc)]
        public IActionResult ViPham(DateTime? tu, DateTime? den, int? maSinhVien)
        {
            KhoangNgay(tu, den, out DateTime tuNgay, out DateTime denNgay);
            return Ok(new
            {
                tu = tuNgay,
                den = denNgay,
                maSinhVien,
                sinhViens = Bang.DanhSach(HanMucService.LaySinhVienDuocQuanLy(Toi)),
                viPham = Bang.DanhSach(BaoCaoService.ViPham(Toi, maSinhVien, tuNgay, denNgay))
            });
        }
    }
}
