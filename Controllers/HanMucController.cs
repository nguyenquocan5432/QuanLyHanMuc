using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyHanMucWeb.Data;
using QuanLyHanMucWeb.Infrastructure;
using QuanLyHanMucWeb.Services;

namespace QuanLyHanMucWeb.Controllers
{
    public class HanMucYeuCau
    {
        public int MaSinhVien { get; set; }
        public int MaDanhMuc { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal SoTienHanMuc { get; set; }
        public string LoaiHanMuc { get; set; } = "Cứng";
        public decimal NguongCanhBao { get; set; } = 80;
        public bool ApDungTatCa { get; set; }
        public string PhienBan { get; set; }
    }

    [Authorize(Policy = ChinhSach.QuanLyHanMuc)]
    [Route("api/han-muc")]
    public class HanMucController : CoSoController
    {
        [HttpGet]
        public IActionResult DanhSach(string ky = "HienTai", int? maSinhVien = null, string trangThai = null)
        {
            LocKy locKy = ky == "TatCa" ? LocKy.TatCa : ky == "DaHetKy" ? LocKy.DaHetKy : LocKy.HienTai;
            DataTable ds = HanMucService.LayDanhSach(Toi, locKy, maSinhVien, string.IsNullOrEmpty(trangThai) ? null : trangThai);
            return Ok(new
            {
                sinhViens = Bang.DanhSach(HanMucService.LaySinhVienDuocQuanLy(Toi)),
                hanMucs = Bang.DanhSach(ds),
                homNay = DateTime.Today
            });
        }

        [HttpGet("lua-chon")]
        public IActionResult LuaChon()
        {
            DateTime dauThang = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return Ok(new
            {
                sinhViens = Bang.DanhSach(HanMucService.LaySinhVienDuocQuanLy(Toi)),
                danhMucs = Bang.DanhSach(HanMucService.LayDanhMucDangDung()),
                coSinhVienCuaToi = User.IsInRole("CapNganSach"),
                ngayBatDau = dauThang,
                ngayKetThuc = dauThang.AddMonths(1).AddDays(-1)
            });
        }

        [HttpGet("{id:int}")]
        public IActionResult MotHanMuc(int id)
        {
            DataRow hm = HanMucService.LayHanMuc(Toi, id);
            if (hm == null)
                return NotFound(new { loi = "Không tìm thấy hạn mức." });
            return Ok(Bang.Dong(hm));
        }

        [HttpPost]
        public IActionResult Tao(HanMucYeuCau f)
        {
            string loi = KiemTraNhapLieu(f);
            if (loi != null)
                return Loi(loi);

            try
            {
                if (f.ApDungTatCa && User.IsInRole("CapNganSach"))
                {
                    List<string> boQua = HanMucService.TaoChoTatCa(Toi, f.MaDanhMuc, f.NgayBatDau, f.NgayKetThuc,
                        f.SoTienHanMuc, f.LoaiHanMuc, f.NguongCanhBao, out int soDaTao);

                    return Xong("Đã tạo " + soDaTao + " hạn mức.",
                        boQua.Count > 0 ? "Bỏ qua " + boQua.Count + " sinh viên: " + string.Join("; ", boQua) : null);
                }

                HanMucService.TaoHanMuc(Toi, f.MaSinhVien, f.MaDanhMuc, f.NgayBatDau, f.NgayKetThuc,
                    f.SoTienHanMuc, f.LoaiHanMuc, f.NguongCanhBao);
                return Xong("Đã tạo hạn mức.");
            }
            catch (SqlException ex)
            {
                string thongBao = LoiCSDL.Dich(ex);
                if (ex.Number == 50001)
                    thongBao += " Hãy tìm hạn mức đó trong danh sách và bấm Sửa để điều chỉnh.";
                return Loi(thongBao);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Sua(int id, HanMucYeuCau f)
        {
            if (f.SoTienHanMuc <= 0 || f.NguongCanhBao < 1 || f.NguongCanhBao > 100)
                return Loi("Số tiền phải lớn hơn 0 và ngưỡng từ 1 đến 100.");

            byte[] phienBan;
            try { phienBan = Convert.FromBase64String(f.PhienBan ?? ""); }
            catch (FormatException) { return Loi("Thiếu phiên bản của hạn mức. Hãy mở lại trang sửa."); }

            try
            {
                HanMucService.DieuChinhHanMuc(Toi, id, f.SoTienHanMuc, f.LoaiHanMuc, f.NguongCanhBao, phienBan);
                return Xong("Đã điều chỉnh hạn mức.");
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
        }

        [HttpPost("{id:int}/khoa")]
        public IActionResult Khoa(int id) => DoiKhoa(id, true);

        [HttpPost("{id:int}/mo-khoa")]
        public IActionResult MoKhoa(int id) => DoiKhoa(id, false);

        private IActionResult DoiKhoa(int id, bool khoa)
        {
            try
            {
                string trangThai = HanMucService.KhoaMoKhoa(Toi, id, khoa);
                return Xong((khoa ? "Đã khóa hạn mức" : "Đã mở khóa hạn mức") + ". Trạng thái: " + trangThai + ".");
            }
            catch (SqlException ex)
            {
                return Loi(LoiCSDL.Dich(ex));
            }
        }

        private static string KiemTraNhapLieu(HanMucYeuCau f)
        {
            if (!f.ApDungTatCa && f.MaSinhVien <= 0) return "Chọn sinh viên.";
            if (f.MaDanhMuc <= 0) return "Chọn danh mục.";
            if (f.NgayKetThuc.Date <= f.NgayBatDau.Date) return "Ngày kết thúc phải sau ngày bắt đầu.";
            if (f.SoTienHanMuc <= 0) return "Số tiền hạn mức phải lớn hơn 0.";
            if (f.NguongCanhBao < 1 || f.NguongCanhBao > 100) return "Ngưỡng cảnh báo phải từ 1 đến 100.";
            if (f.LoaiHanMuc != "Cứng" && f.LoaiHanMuc != "Mềm") return "Chọn loại hạn mức.";
            return null;
        }
    }
}
