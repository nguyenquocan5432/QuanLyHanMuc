using Microsoft.AspNetCore.Mvc;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class DaDocYeuCau
    {
        public int? Ma { get; set; }
    }

    [Route("api/thong-bao")]
    public class ThongBaoController : CoSoController
    {
        [HttpGet]
        public IActionResult DanhSach()
        {
            return Ok(Bang.DanhSach(ThongBaoService.LayDanhSach(Toi)));
        }

        [HttpPost("da-doc")]
        public IActionResult DaDoc(DaDocYeuCau yeuCau)
        {
            ThongBaoService.DanhDauDaDoc(Toi, yeuCau.Ma);
            return Ok(new { soChuaDoc = ThongBaoService.DemChuaDoc(Toi) });
        }
    }
}
