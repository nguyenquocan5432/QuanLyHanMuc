using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class XuLyNgoaiLeYeuCau
    {
        public bool DongY { get; set; }
        public string GhiChu { get; set; }
    }

    [Authorize(Policy = ChinhSach.QuanLyHanMuc)]
    [Route("api/ngoai-le")]
    public class NgoaiLeController : CoSoController
    {
        [HttpGet]
        public IActionResult ChoDuyet()
        {
            return Ok(Bang.DanhSach(NgoaiLeService.LayDanhSachChoDuyet(Toi)));
        }

        [HttpPost("{maYeuCau:int}")]
        public IActionResult XuLy(int maYeuCau, XuLyNgoaiLeYeuCau yeuCau)
        {
            if (!yeuCau.DongY && string.IsNullOrWhiteSpace(yeuCau.GhiChu))
                return Loi("Từ chối phải ghi lý do.");
            try
            {
                return Xong(NgoaiLeService.XuLy(Toi, maYeuCau, yeuCau.DongY, yeuCau.GhiChu));
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
        }
    }
}
