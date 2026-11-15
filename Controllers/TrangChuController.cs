using Microsoft.AspNetCore.Mvc;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class TrangChuController : CoSoController
    {
        [HttpGet("api/trang-chu")]
        public IActionResult TrangChu()
        {
            object hanMucCuaToi = null, hanMucSinhVien = null;
            int soChoDuyet = 0;

            if (User.DuocNhapGiaoDich())
                hanMucCuaToi = Bang.DanhSach(GiaoDichService.LayHanMucCuaToi(Toi));
            if (User.DuocQuanLyHanMuc())
                soChoDuyet = NgoaiLeService.LayDanhSachChoDuyet(Toi).Rows.Count;
            if (User.IsInRole("CapNganSach"))
                hanMucSinhVien = Bang.DanhSach(HanMucService.LayDanhSach(Toi, LocKy.HienTai, null, null));

            return Ok(new { hanMucCuaToi, hanMucSinhVien, soChoDuyet, homNay = DateTime.Today });
        }
    }
}
