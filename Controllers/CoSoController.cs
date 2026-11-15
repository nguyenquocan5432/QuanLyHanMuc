using Microsoft.AspNetCore.Mvc;
using QuanLyHanMucWeb.Infrastructure;

namespace QuanLyHanMucWeb.Controllers
{
    [ApiController]
    public abstract class CoSoController : ControllerBase
    {
        protected int Toi => User.MaNguoiDung();

        protected IActionResult Loi(string thongBao) => BadRequest(new { loi = thongBao });

        protected IActionResult Xong(string thongBao, string canhBao = null) =>
            Ok(new { thongBao, canhBao });

        protected static void KhoangNgay(DateTime? tu, DateTime? den, out DateTime tuNgay, out DateTime denNgay)
        {
            DateTime homNay = DateTime.Today;
            tuNgay = (tu ?? new DateTime(homNay.Year, homNay.Month, 1)).Date;
            denNgay = (den ?? tuNgay.AddMonths(1).AddDays(-1)).Date;
        }
    }
}
