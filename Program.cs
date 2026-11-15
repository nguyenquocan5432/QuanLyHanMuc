using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

CultureInfo vi = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = vi;
CultureInfo.DefaultThreadCurrentUICulture = vi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

KetNoi.CauHinh(builder.Configuration.GetConnectionString("QuanLyHanMuc"));

CauHinhXacThuc xacThuc = builder.Configuration.GetSection("XacThuc").Get<CauHinhXacThuc>() ?? new CauHinhXacThuc();
builder.Services.AddSingleton(xacThuc);
KeycloakQuanTri.CauHinh(xacThuc.Keycloak);

builder.Services.AddControllersWithViews(options =>
{
    AuthorizationPolicy batBuocDangNhap = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(batBuocDangNhap));
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

AuthenticationBuilder xacThucBuilder = builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

if (xacThuc.DungOidc)
    CauHinhOidc.Them(xacThucBuilder, xacThuc);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ChinhSach.GhiGiaoDich, p => p.RequireRole("SinhVien", "CapNganSach"));
    options.AddPolicy(ChinhSach.QuanLyHanMuc, p => p.RequireAssertion(ctx => ctx.User.DuocQuanLyHanMuc()));
    options.AddPolicy(ChinhSach.QuanTri, p => p.RequireRole("QuanTri"));
});

WebApplication app = builder.Build();

app.UseExceptionHandler(nhanh => nhanh.Run(async context =>
{
    Exception loi = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    string thongBao = loi is SqlException sqlEx ? LoiCSDL.Dich(sqlEx) : "Đã có lỗi xảy ra. Vui lòng thử lại.";
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { loi = thongBao });
}));

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html").AllowAnonymous();

app.Run();

public partial class Program { }
