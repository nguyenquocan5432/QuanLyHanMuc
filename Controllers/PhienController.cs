using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Models;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class DangNhapYeuCau
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
    }

    [AllowAnonymous]
    public class PhienController : CoSoController
    {
        private readonly IAntiforgery _chongGiaMao;
        private readonly CauHinhXacThuc _xacThuc;

        public PhienController(IAntiforgery chongGiaMao, CauHinhXacThuc xacThuc)
        {
            _chongGiaMao = chongGiaMao;
            _xacThuc = xacThuc;
        }

        [HttpGet("api/phien")]
        public IActionResult Phien()
        {
            AntiforgeryTokenSet ma = _chongGiaMao.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", ma.RequestToken,
                new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Strict });

            string cheDo = _xacThuc.DungOidc ? "Oidc" : "Local";
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Ok(new { cheDo, nguoiDung = (object)null, soChuaDoc = 0 });

            int chuaDoc = 0;
            if (!User.IsInRole("QuanTri"))
            {
                try { chuaDoc = ThongBaoService.DemChuaDoc(Toi); } catch (SqlException) { }
            }

            return Ok(new
            {
                cheDo,
                nguoiDung = new
                {
                    ma = Toi,
                    hoTen = User.HoTen(),
                    vaiTro = User.VaiTro(),
                    tenVaiTro = User.TenVaiTro(),
                    duocNhapGiaoDich = User.DuocNhapGiaoDich(),
                    duocXemGiaoDich = User.DuocGhiGiaoDich(),
                    duocQuanLyHanMuc = User.DuocQuanLyHanMuc(),
                    laCapNganSach = User.IsInRole("CapNganSach"),
                    laQuanTri = User.IsInRole("QuanTri")
                },
                soChuaDoc = chuaDoc
            });
        }

        [HttpPost("api/dang-nhap")]
        public async Task<IActionResult> DangNhap(DangNhapYeuCau yeuCau)
        {
            if (_xacThuc.DungOidc)
                return Loi("Hệ thống đang dùng đăng nhập OIDC.");

            string loi;
            NguoiDungDangNhap nd;
            try
            {
                loi = TaiKhoanService.DangNhap(yeuCau.TenDangNhap, yeuCau.MatKhau, out nd);
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
            if (loi != null)
                return Loi(loi);

            ClaimsIdentity danhTinh = new ClaimsIdentity(
                NguoiDungHienTai.TaoClaims(nd), CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(danhTinh));
            return Ok(new { hoTen = nd.HoTen });
        }

        [HttpGet("xac-thuc/dang-nhap")]
        public IActionResult DangNhapOidc(string returnUrl = "/")
        {
            if (!_xacThuc.DungOidc)
                return Redirect("/dang-nhap");

            string ve = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
            return Challenge(new AuthenticationProperties { RedirectUri = ve }, TenScheme.Oidc);
        }

        [HttpPost("xac-thuc/dang-xuat")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DangXuat()
        {
            if (_xacThuc.DungOidc && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return SignOut(new AuthenticationProperties { RedirectUri = "/dang-nhap" },
                    CookieAuthenticationDefaults.AuthenticationScheme, TenScheme.Oidc);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/dang-nhap");
        }
    }
}
