using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using QuanLyHanMucWeb.Infrastructure;

namespace QuanLyHanMucWeb.Services
{
    public static class KeycloakQuanTri
    {
        private static CauHinhKeycloak _cauHinh;
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public static void CauHinh(CauHinhKeycloak cauHinh)
        {
            _cauHinh = cauHinh;
        }

        public static bool DaCauHinh => _cauHinh != null && !string.IsNullOrEmpty(_cauHinh.AdminUrl);

        private static string GocQuanTri => _cauHinh.AdminUrl.TrimEnd('/') + "/admin/realms/" + _cauHinh.Realm;

        private static async Task<string> LayToken()
        {
            string url = _cauHinh.AdminUrl.TrimEnd('/') + "/realms/master/protocol/openid-connect/token";
            FormUrlEncodedContent form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "admin-cli",
                ["username"] = _cauHinh.AdminUser,
                ["password"] = _cauHinh.AdminPassword
            });
            using HttpResponseMessage tl = await _http.PostAsync(url, form);
            tl.EnsureSuccessStatusCode();
            using JsonDocument json = JsonDocument.Parse(await tl.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("access_token").GetString();
        }

        private static HttpRequestMessage TaoYeuCau(HttpMethod phuongThuc, string url, string token, object noiDung = null)
        {
            HttpRequestMessage yc = new HttpRequestMessage(phuongThuc, url);
            yc.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (noiDung != null)
                yc.Content = JsonContent.Create(noiDung);
            return yc;
        }

        private static async Task<string> TimMa(string token, string tenDangNhap)
        {
            string url = GocQuanTri + "/users?exact=true&username=" + Uri.EscapeDataString(tenDangNhap);
            using HttpResponseMessage tl = await _http.SendAsync(TaoYeuCau(HttpMethod.Get, url, token));
            tl.EnsureSuccessStatusCode();
            using JsonDocument json = JsonDocument.Parse(await tl.Content.ReadAsStringAsync());
            foreach (JsonElement nd in json.RootElement.EnumerateArray())
                return nd.GetProperty("id").GetString();
            return null;
        }

        public static async Task DongBoNguoiDung(string tenDangNhap, string hoTen, string email, bool hoatDong, string matKhauBanDau)
        {
            string token = await LayToken();
            string ma = await TimMa(token, tenDangNhap);

            string[] tu = (hoTen ?? "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string ten = tu.Length > 0 ? tu[tu.Length - 1] : tenDangNhap;
            string ho = tu.Length > 1 ? string.Join(' ', tu, 0, tu.Length - 1) : ten;
            string thu = string.IsNullOrWhiteSpace(email) ? tenDangNhap + "@hanmuc.local" : email.Trim();

            if (ma == null)
            {
                object moi = new
                {
                    username = tenDangNhap,
                    enabled = hoatDong,
                    firstName = ten,
                    lastName = ho,
                    email = thu,
                    emailVerified = true,
                    credentials = new[] { new { type = "password", value = matKhauBanDau, temporary = false } }
                };
                using HttpResponseMessage tl = await _http.SendAsync(TaoYeuCau(HttpMethod.Post, GocQuanTri + "/users", token, moi));
                tl.EnsureSuccessStatusCode();
            }
            else
            {
                object sua = new { enabled = hoatDong, firstName = ten, lastName = ho, email = thu, emailVerified = true };
                using HttpResponseMessage tl = await _http.SendAsync(TaoYeuCau(HttpMethod.Put, GocQuanTri + "/users/" + ma, token, sua));
                tl.EnsureSuccessStatusCode();
            }
        }

        public static async Task DatLaiMatKhau(string tenDangNhap, string matKhauMoi)
        {
            string token = await LayToken();
            string ma = await TimMa(token, tenDangNhap);
            if (ma == null)
                throw new InvalidOperationException("Keycloak chưa có tài khoản " + tenDangNhap + ".");

            object matKhau = new { type = "password", value = matKhauMoi, temporary = false };
            using HttpResponseMessage tl = await _http.SendAsync(
                TaoYeuCau(HttpMethod.Put, GocQuanTri + "/users/" + ma + "/reset-password", token, matKhau));
            tl.EnsureSuccessStatusCode();
        }
    }
}
