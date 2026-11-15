using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class NguoiDungYeuCau
    {
        public int? MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }
        public int? MaNguoiCap { get; set; }
        public bool HoatDong { get; set; } = true;
    }

    [Authorize(Policy = ChinhSach.QuanTri)]
    [Route("api/nguoi-dung")]
    public class NguoiDungController : CoSoController
    {
        private readonly CauHinhXacThuc _xacThuc;

        public NguoiDungController(CauHinhXacThuc xacThuc)
        {
            _xacThuc = xacThuc;
        }

        private bool DongBoKeycloak => _xacThuc.DungOidc && KeycloakQuanTri.DaCauHinh;

        [HttpGet]
        public IActionResult DanhSach()
        {
            return Ok(new
            {
                nguoiDungs = Bang.DanhSach(QuanTriService.LayNguoiDung()),
                nguoiCaps = Bang.DanhSach(QuanTriService.LayNguoiCapNganSach()),
                matKhauMacDinh = QuanTriService.MatKhauMacDinh
            });
        }

        [HttpPost]
        public async Task<IActionResult> Luu(NguoiDungYeuCau yeuCau)
        {
            if (string.IsNullOrWhiteSpace(yeuCau.TenDangNhap) || string.IsNullOrWhiteSpace(yeuCau.HoTen))
                return Loi("Nhập tên đăng nhập và họ tên.");

            int? nguoiCap = yeuCau.MaNguoiCap.HasValue && yeuCau.MaNguoiCap.Value > 0 ? yeuCau.MaNguoiCap : null;
            int ma;
            try
            {
                ma = QuanTriService.LuuNguoiDung(Toi, yeuCau.MaNguoiDung, yeuCau.TenDangNhap, yeuCau.HoTen, yeuCau.Email,
                    yeuCau.VaiTro, nguoiCap, yeuCau.HoatDong);
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }

            string thongBao = yeuCau.MaNguoiDung.HasValue
                ? "Đã lưu người dùng."
                : "Đã thêm người dùng. Mật khẩu ban đầu: " + QuanTriService.MatKhauMacDinh;

            string canhBao = null;
            if (DongBoKeycloak)
            {
                try
                {
                    await KeycloakQuanTri.DongBoNguoiDung(yeuCau.TenDangNhap.Trim(), yeuCau.HoTen, yeuCau.Email,
                        yeuCau.HoatDong, QuanTriService.MatKhauMacDinh);
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                {
                    canhBao = "Đã lưu trong CSDL nhưng chưa ghi được sang Keycloak: " + ex.Message;
                }
            }
            return Ok(new { ma, thongBao, canhBao });
        }

        [HttpPost("{id:int}/dat-lai-mat-khau")]
        public async Task<IActionResult> DatLaiMatKhau(int id)
        {
            string tenDangNhap;
            try
            {
                QuanTriService.DatLaiMatKhau(Toi, id);
                tenDangNhap = QuanTriService.LayTenDangNhap(id);
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }

            if (DongBoKeycloak && !string.IsNullOrWhiteSpace(tenDangNhap))
            {
                try
                {
                    await KeycloakQuanTri.DatLaiMatKhau(tenDangNhap.Trim(), QuanTriService.MatKhauMacDinh);
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is InvalidOperationException)
                {
                    return Loi("Chưa đặt lại được mật khẩu trên Keycloak: " + ex.Message);
                }
            }
            return Xong("Đã đặt lại mật khẩu về " + QuanTriService.MatKhauMacDinh + ".");
        }
    }
}
