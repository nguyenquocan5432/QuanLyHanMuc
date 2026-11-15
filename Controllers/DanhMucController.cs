using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class DanhMucYeuCau
    {
        public int? MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }
        public bool HoatDong { get; set; } = true;
    }

    [Authorize(Policy = ChinhSach.QuanTri)]
    [Route("api/danh-muc")]
    public class DanhMucController : CoSoController
    {
        [HttpGet]
        public IActionResult DanhSach()
        {
            return Ok(Bang.DanhSach(QuanTriService.LayDanhMuc()));
        }

        [HttpPost]
        public IActionResult Luu(DanhMucYeuCau yeuCau)
        {
            try
            {
                int ma = QuanTriService.LuuDanhMuc(Toi, yeuCau.MaDanhMuc, yeuCau.TenDanhMuc, yeuCau.MoTa, yeuCau.HoatDong);
                return Ok(new { ma, thongBao = yeuCau.MaDanhMuc.HasValue ? "Đã lưu danh mục." : "Đã thêm danh mục." });
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
        }
    }
}
