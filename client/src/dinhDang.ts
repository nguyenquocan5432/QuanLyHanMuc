const soNguyen = new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 0 });
const soLe = new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 2 });

export function tien(giaTri: number | null | undefined): string {
  if (giaTri === null || giaTri === undefined) return '';
  return soNguyen.format(giaTri) + ' đ';
}

export function tyLe(giaTri: number | null | undefined): string {
  if (giaTri === null || giaTri === undefined) return '';
  return soLe.format(giaTri) + '%';
}

export function ngay(iso: string | null | undefined): string {
  if (!iso) return '';
  const [nam, thang, ngayTrongThang] = iso.substring(0, 10).split('-');
  return `${ngayTrongThang}/${thang}/${nam}`;
}

export function ngayGio(iso: string | null | undefined): string {
  if (!iso) return '';
  return ngay(iso) + ' ' + iso.substring(11, 16);
}

export function ngayInput(iso: string | null | undefined): string {
  return iso ? iso.substring(0, 10) : '';
}

export function lopTrangThai(trangThai: string | null | undefined): string {
  switch (trangThai) {
    case 'Sắp vượt': return 'tt-sap-vuot';
    case 'Đã vượt': return 'tt-da-vuot';
    case 'Đã khóa': return 'tt-da-khoa';
    case 'Chờ duyệt': return 'tt-cho-duyet';
    case 'Từ chối': return 'tt-tu-choi';
    case 'Đã hủy': return 'tt-da-huy';
    default: return '';
  }
}

export function rongThanh(phanTram: number | null | undefined): string {
  const t = Math.max(0, Math.min(100, phanTram ?? 0));
  return t + '%';
}

export function chuCaiDau(hoTen: string | null | undefined): string {
  if (!hoTen || !hoTen.trim()) return '?';
  const tu = hoTen.trim().split(/\s+/);
  return tu[tu.length - 1].substring(0, 1).toUpperCase();
}

export function tenVaiTro(vaiTro: string): string {
  switch (vaiTro) {
    case 'QuanTri': return 'Quản trị viên';
    case 'CapNganSach': return 'Người cấp ngân sách';
    case 'SinhVien': return 'Sinh viên';
    default: return vaiTro;
  }
}

export function soNgayGiua(a: string, b: string): number {
  const ngayA = Date.UTC(+a.substring(0, 4), +a.substring(5, 7) - 1, +a.substring(8, 10));
  const ngayB = Date.UTC(+b.substring(0, 4), +b.substring(5, 7) - 1, +b.substring(8, 10));
  return Math.round((ngayB - ngayA) / 86400000);
}
