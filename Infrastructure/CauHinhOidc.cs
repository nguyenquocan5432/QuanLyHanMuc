using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using QuanLyHanMucWeb.Models;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Infrastructure
{
    public static class CauHinhOidc
    {
        public static void Them(AuthenticationBuilder xacThuc, CauHinhXacThuc cauHinh)
        {
            xacThuc.AddOpenIdConnect(TenScheme.Oidc, o =>
            {
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.Authority = cauHinh.Oidc.Authority;
                if (!string.IsNullOrEmpty(cauHinh.Oidc.MetadataAddress))
                    o.MetadataAddress = cauHinh.Oidc.MetadataAddress;
                o.ClientId = cauHinh.Oidc.ClientId;
                o.ClientSecret = cauHinh.Oidc.ClientSecret;
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.UsePkce = true;
                o.ResponseMode = OpenIdConnectResponseMode.Query;
                o.NonceCookie.SameSite = SameSiteMode.Lax;
                o.CorrelationCookie.SameSite = SameSiteMode.Lax;
                o.RequireHttpsMetadata = false;
                o.SaveTokens = true;
                o.MapInboundClaims = false;
                o.Scope.Clear();
                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.CallbackPath = "/signin-oidc";
                o.SignedOutCallbackPath = "/signout-callback-oidc";

                o.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        string tenDangNhap = context.Principal?.FindFirst("preferred_username")?.Value;
                        string loi = TaiKhoanService.LayTheoTenDangNhap(tenDangNhap, out NguoiDungDangNhap nd);
                        if (loi != null)
                        {
                            context.Fail(loi);
                            return Task.CompletedTask;
                        }
                        ClaimsIdentity danhTinh = new ClaimsIdentity(
                            NguoiDungHienTai.TaoClaims(nd), CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Principal = new ClaimsPrincipal(danhTinh);
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        string loi = context.Failure?.Message ?? "Đăng nhập không thành công.";
                        context.Response.Redirect("/dang-nhap?loi=" + Uri.EscapeDataString(loi));
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
