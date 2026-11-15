namespace QuanLyHanMucWeb.Infrastructure
{
    public static class GiaoDien
    {
        public static string Tien(object giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value) return "";
            return Convert.ToDecimal(giaTri).ToString("N0") + " đ";
        }

        public static string TyLe(object giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value) return "";
            return Convert.ToDecimal(giaTri).ToString("0.##") + "%";
        }

        public static string Ngay(object giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value) return "";
            return Convert.ToDateTime(giaTri).ToString("dd/MM/yyyy");
        }

        public static string NgayGio(object giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value) return "";
            return Convert.ToDateTime(giaTri).ToString("dd/MM/yyyy HH:mm");
        }

        public static string NgayInput(DateTime ngay) => ngay.ToString("yyyy-MM-dd");

        public static string LopTrangThai(object trangThai)
        {
            switch (trangThai as string)
            {
                case "Sắp vượt": return "tt-sap-vuot";
                case "Đã vượt": return "tt-da-vuot";
                case "Đã khóa": return "tt-da-khoa";
                case "Chờ duyệt": return "tt-cho-duyet";
                case "Từ chối": return "tt-tu-choi";
                case "Đã hủy": return "tt-da-huy";
                default: return "";
            }
        }

        public static string RongThanh(object tyLe)
        {
            decimal t = tyLe == null || tyLe == DBNull.Value ? 0 : Convert.ToDecimal(tyLe);
            if (t > 100) t = 100;
            if (t < 0) t = 0;
            return t.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + "%";
        }

        public static int SoNgayConLai(object ngayKetThuc)
        {
            if (ngayKetThuc == null || ngayKetThuc == DBNull.Value) return 0;
            int soNgay = (Convert.ToDateTime(ngayKetThuc).Date - DateTime.Today).Days + 1;
            return Math.Max(0, soNgay);
        }

        public static string ChuCaiDau(string hoTen)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) return "?";
            string[] tu = hoTen.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return tu[tu.Length - 1].Substring(0, 1).ToUpper();
        }

        public static string TenVaiTro(object vaiTro)
        {
            switch (vaiTro as string)
            {
                case "QuanTri": return "Quản trị viên";
                case "CapNganSach": return "Người cấp ngân sách";
                case "SinhVien": return "Sinh viên";
                default: return vaiTro as string;
            }
        }
    }
}
