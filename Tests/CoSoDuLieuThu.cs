using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuanLyHanMucWeb.Data;
using Xunit;

namespace QuanLyHanMucWeb.Tests
{
    public class CoSoDuLieuThu
    {
        public const string TenDatabase = "QuanLyHanMuc_Test";

        public string ChuoiKetNoi { get; }
        public int MaQuanTri { get; }
        public int MaPhuHuynh { get; }
        public int MaKhoa { get; }
        public int MaHa { get; }
        public int MaBinh { get; }

        public CoSoDuLieuThu()
        {
            string goc = TimThuMucGoc();
            string chuoiGoc = Environment.GetEnvironmentVariable("QLHM_TEST_SQL");
            if (string.IsNullOrWhiteSpace(chuoiGoc))
            {
                IConfiguration cauHinh = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(goc, "appsettings.json"), optional: false)
                    .Build();
                chuoiGoc = cauHinh.GetConnectionString("QuanLyHanMuc");
            }

            SqlConnectionStringBuilder master = new SqlConnectionStringBuilder(chuoiGoc) { InitialCatalog = "master", Pooling = false };
            ChayScript(master.ConnectionString, Path.Combine(goc, "CSDL", "QuanLyHanMuc.sql"));

            ChuoiKetNoi = new SqlConnectionStringBuilder(chuoiGoc) { InitialCatalog = TenDatabase }.ConnectionString;
            KetNoi.CauHinh(ChuoiKetNoi);

            MaQuanTri = MaTheoTen("admin");
            MaPhuHuynh = MaTheoTen("phuhuynh1");
            MaKhoa = MaTheoTen("sinhvien1");
            MaHa = MaTheoTen("sinhvien2");
            MaBinh = MaTheoTen("tuquanly1");
        }

        public static string TimThuMucGoc()
        {
            DirectoryInfo thuMuc = new DirectoryInfo(AppContext.BaseDirectory);
            while (thuMuc != null && !File.Exists(Path.Combine(thuMuc.FullName, "QuanLyHanMucWeb.csproj")))
                thuMuc = thuMuc.Parent;
            if (thuMuc == null)
                throw new InvalidOperationException("Không tìm thấy thư mục chứa QuanLyHanMucWeb.csproj.");
            return thuMuc.FullName;
        }

        private static void ChayScript(string chuoiMaster, string duongDanScript)
        {
            string script = File.ReadAllText(duongDanScript);

            Match demo = Regex.Match(script, @"^\s*SET\s+NOEXEC\s+ON", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (demo.Success) script = script.Substring(0, demo.Index);

            script = Regex.Replace(script, @"\bQuanLyHanMuc\b", TenDatabase);

            using SqlConnection kn = new SqlConnection(chuoiMaster);
            kn.Open();
            foreach (string lo in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(lo)) continue;
                using SqlCommand lenh = new SqlCommand(lo, kn) { CommandTimeout = 120 };
                lenh.ExecuteNonQuery();
            }
        }

        private static int MaTheoTen(string tenDangNhap)
        {
            return (int)KetNoi.LayMotGiaTri("SELECT MaNguoiDung FROM dbo.NguoiDung WHERE TenDangNhap = @Ten",
                KetNoi.ChuoiKhongDau("@Ten", tenDangNhap, 50));
        }

        public int TaoDanhMuc(string tienTo)
        {
            string ten = tienTo + " " + Guid.NewGuid().ToString("N").Substring(0, 8);
            return (int)KetNoi.LayMotGiaTri(
                "INSERT INTO dbo.DanhMuc (TenDanhMuc) OUTPUT INSERTED.MaDanhMuc VALUES (@Ten)",
                KetNoi.Chuoi("@Ten", ten, 100));
        }

        public static DateTime DauThang => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public static DateTime CuoiThang => DauThang.AddMonths(1).AddDays(-1);

        public decimal SoDaChi(int maHanMuc)
        {
            return (decimal)KetNoi.LayMotGiaTri("SELECT SoTienDaChi FROM dbo.HanMuc WHERE MaHanMuc = @Ma", KetNoi.SoNguyen("@Ma", maHanMuc));
        }

        public int MaYeuCau(int maGiaoDich)
        {
            return (int)KetNoi.LayMotGiaTri("SELECT MaYeuCau FROM dbo.YeuCauNgoaiLe WHERE MaGiaoDich = @Ma", KetNoi.SoNguyen("@Ma", maGiaoDich));
        }

        public string TrangThaiHanMuc(int maHanMuc)
        {
            return (string)KetNoi.LayMotGiaTri("SELECT TrangThai FROM dbo.HanMuc WHERE MaHanMuc = @Ma", KetNoi.SoNguyen("@Ma", maHanMuc));
        }

        public byte[] PhienBan(int maHanMuc)
        {
            return (byte[])KetNoi.LayMotGiaTri("SELECT PhienBan FROM dbo.HanMuc WHERE MaHanMuc = @Ma", KetNoi.SoNguyen("@Ma", maHanMuc));
        }

        public int Dem(string bang, string dieuKien, int maHanMuc)
        {
            return (int)KetNoi.LayMotGiaTri($"SELECT COUNT(*) FROM dbo.{bang} WHERE MaHanMuc = @Ma AND {dieuKien}", KetNoi.SoNguyen("@Ma", maHanMuc));
        }

        public int SoDongLech()
        {
            return (int)KetNoi.LayMotGiaTri("SELECT COUNT(*) FROM dbo.vw_DoiSoatHanMuc WHERE ChenhLech <> 0");
        }
    }

    [CollectionDefinition("CSDL")]
    public class NhomCSDL : ICollectionFixture<CoSoDuLieuThu> { }
}
