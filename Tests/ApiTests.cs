using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using QuanLyHanMucWeb.Services;
using Xunit;

namespace QuanLyHanMucWeb.Tests
{
    [Collection("CSDL")]
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _ungDung;
        private readonly CoSoDuLieuThu _db;

        public ApiTests(CoSoDuLieuThu db, WebApplicationFactory<Program> ungDung)
        {
            _db = db;
            _ungDung = ungDung.WithWebHostBuilder(b =>
            {
                b.UseSetting("ConnectionStrings:QuanLyHanMuc", db.ChuoiKetNoi);
                b.UseSetting("XacThuc:CheDo", "Local");
            });
            _ungDung.CreateClient().Dispose();
            QuanLyHanMucWeb.Data.KetNoi.CauHinh(db.ChuoiKetNoi);
        }

        private async Task<(HttpClient client, string ma)> MoTrinhDuyet()
        {
            HttpClient client = _ungDung.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            string ma = await LayMaChongGiaMao(client);
            return (client, ma);
        }

        private static async Task<string> LayMaChongGiaMao(HttpClient client)
        {
            HttpResponseMessage tl = await client.GetAsync("/api/phien");
            tl.EnsureSuccessStatusCode();
            foreach (string cookie in tl.Headers.GetValues("Set-Cookie"))
            {
                if (cookie.StartsWith("XSRF-TOKEN="))
                    return Uri.UnescapeDataString(cookie.Substring("XSRF-TOKEN=".Length).Split(';')[0]);
            }
            throw new InvalidOperationException("Không thấy cookie XSRF-TOKEN.");
        }

        private static HttpRequestMessage Gui(string duongDan, object noiDung, string ma)
        {
            HttpRequestMessage yc = new HttpRequestMessage(HttpMethod.Post, duongDan) { Content = JsonContent.Create(noiDung) };
            if (ma != null) yc.Headers.Add("X-XSRF-TOKEN", ma);
            return yc;
        }

        private async Task<(HttpClient client, string ma)> DangNhap(string tenDangNhap)
        {
            (HttpClient client, string ma) = await MoTrinhDuyet();
            HttpResponseMessage tl = await client.SendAsync(Gui("/api/dang-nhap", new { tenDangNhap, matKhau = "123456" }, ma));
            Assert.Equal(HttpStatusCode.OK, tl.StatusCode);
            return (client, await LayMaChongGiaMao(client));
        }

        [Fact]
        public async Task ChuaDangNhap_GoiApi_Tra401()
        {
            (HttpClient client, _) = await MoTrinhDuyet();
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/trang-chu")).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/han-muc")).StatusCode);
        }

        [Fact]
        public async Task SaiMatKhau_BaoCungMotCau()
        {
            (HttpClient client, string ma) = await MoTrinhDuyet();
            HttpResponseMessage saiMatKhau = await client.SendAsync(Gui("/api/dang-nhap", new { tenDangNhap = "sinhvien1", matKhau = "sai" }, ma));
            HttpResponseMessage saiTen = await client.SendAsync(Gui("/api/dang-nhap", new { tenDangNhap = "khongco", matKhau = "123456" }, ma));

            Assert.Equal(HttpStatusCode.BadRequest, saiMatKhau.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, saiTen.StatusCode);
            Assert.Equal(await saiMatKhau.Content.ReadAsStringAsync(), await saiTen.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ThieuMaChongGiaMao_BiTuChoi()
        {
            (HttpClient client, _) = await DangNhap("sinhvien1");
            HttpResponseMessage tl = await client.SendAsync(Gui("/api/thong-bao/da-doc", new { }, ma: null));
            Assert.Equal(HttpStatusCode.BadRequest, tl.StatusCode);
        }

        [Fact]
        public async Task PhienTraVeDungQuyenTheoVaiTro()
        {
            (HttpClient sinhVien, _) = await DangNhap("sinhvien1");
            JsonElement sv = (await sinhVien.GetFromJsonAsync<JsonElement>("/api/phien")).GetProperty("nguoiDung");
            Assert.True(sv.GetProperty("duocNhapGiaoDich").GetBoolean());
            Assert.False(sv.GetProperty("duocQuanLyHanMuc").GetBoolean());

            (HttpClient tuQuanLy, _) = await DangNhap("tuquanly1");
            JsonElement tql = (await tuQuanLy.GetFromJsonAsync<JsonElement>("/api/phien")).GetProperty("nguoiDung");
            Assert.True(tql.GetProperty("duocNhapGiaoDich").GetBoolean());
            Assert.True(tql.GetProperty("duocQuanLyHanMuc").GetBoolean());
        }

        [Fact]
        public async Task SinhVienCoNguoiCap_KhongVaoDuocQuanLyHanMuc()
        {
            (HttpClient client, string ma) = await DangNhap("sinhvien1");
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/han-muc")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/ngoai-le")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/bao-cao/vi-pham")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await client.SendAsync(Gui("/api/giao-dich/1/huy", new { lyDo = "x" }, ma))).StatusCode);
        }

        [Fact]
        public async Task NguoiCapNganSach_KhongNhapDuocGiaoDich()
        {
            (HttpClient client, string ma) = await DangNhap("phuhuynh1");
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/giao-dich/nhap")).StatusCode);
            HttpResponseMessage tl = await client.SendAsync(
                Gui("/api/giao-dich", new { maDanhMuc = 1, ngay = DateTime.Today.ToString("yyyy-MM-dd"), soTien = 1000, moTa = "x" }, ma));
            Assert.Equal(HttpStatusCode.Forbidden, tl.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/han-muc")).StatusCode);
        }

        [Fact]
        public async Task QuanTriVien_ChiVaoDuocDanhMucVaNguoiDung()
        {
            (HttpClient client, _) = await DangNhap("admin");
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/danh-muc")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/nguoi-dung")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/giao-dich")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/han-muc")).StatusCode);

            (HttpClient sinhVien, _) = await DangNhap("sinhvien1");
            Assert.Equal(HttpStatusCode.Forbidden, (await sinhVien.GetAsync("/api/nguoi-dung")).StatusCode);
        }

        [Fact]
        public async Task SinhVienGhiGiaoDichQuaApi_NhanKetQuaCuaProcedure()
        {
            (HttpClient client, string ma) = await DangNhap("tuquanly1");
            int maDanhMuc = _db.TaoDanhMuc("API");
            HanMucService.TaoHanMuc(_db.MaBinh, _db.MaBinh, maDanhMuc,
                CoSoDuLieuThu.DauThang, CoSoDuLieuThu.CuoiThang, 1000000, "Mềm", 80);

            HttpResponseMessage tl = await client.SendAsync(
                Gui("/api/giao-dich", new { maDanhMuc, ngay = DateTime.Today.ToString("yyyy-MM-dd"), soTien = 25000, moTa = "Bữa trưa" }, ma));
            Assert.Equal(HttpStatusCode.OK, tl.StatusCode);
            JsonElement kq = await tl.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("Chấp nhận", kq.GetProperty("ketQua").GetString());
            Assert.Equal("thanh-cong", kq.GetProperty("muc").GetString());
        }
    }
}
