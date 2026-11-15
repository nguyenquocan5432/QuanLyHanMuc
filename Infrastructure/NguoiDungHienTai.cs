using System.Security.Claims;

namespace QuanLyHanMucWeb.Infrastructure
{
    public static class ChinhSach
    {
        public const string GhiGiaoDich = "GhiGiaoDich";
        public const string QuanLyHanMuc = "QuanLyHanMuc";
        public const string QuanTri = "QuanTri";
    }

    public static class NguoiDungHienTai
    {
        public const string ClaimMaNguoiCap = "MaNguoiCapNganSach";

        public static List<Claim> TaoClaims(QuanLyHanMucWeb.Models.NguoiDungDangNhap nd)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nd.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, nd.HoTen),
                new Claim(ClaimTypes.Role, nd.VaiTro),
                new Claim(ClaimMaNguoiCap, nd.MaNguoiCapNganSach?.ToString() ?? "")
            };
        }

        public static int MaNguoiDung(this ClaimsPrincipal user)
        {
            string ma = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(ma) ? 0 : int.Parse(ma);
        }

        public static string HoTen(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string VaiTro(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static bool TuQuanLy(this ClaimsPrincipal user)
        {
            return user.IsInRole("SinhVien") && string.IsNullOrEmpty(user.FindFirst(ClaimMaNguoiCap)?.Value);
        }

        public static bool DuocNhapGiaoDich(this ClaimsPrincipal user)
        {
            return user.IsInRole("SinhVien");
        }

        public static bool DuocGhiGiaoDich(this ClaimsPrincipal user)
        {
            return user.IsInRole("SinhVien") || user.IsInRole("CapNganSach");
        }

        public static bool DuocQuanLyHanMuc(this ClaimsPrincipal user)
        {
            return user.IsInRole("CapNganSach") || user.TuQuanLy();
        }

        public static string TenVaiTro(this ClaimsPrincipal user)
        {
            switch (user.VaiTro())
            {
                case "QuanTri": return "Quản trị viên";
                case "CapNganSach": return "Người cấp ngân sách";
                case "SinhVien": return user.TuQuanLy() ? "Sinh viên (tự quản lý)" : "Sinh viên";
                default: return user.VaiTro();
            }
        }
    }
}
