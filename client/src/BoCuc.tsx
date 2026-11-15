import { ReactNode } from 'react';
import { Link, NavLink, useLocation } from 'react-router-dom';
import { usePhien } from './Phien';
import { HopBao } from './Bao';
import { chuCaiDau } from './dinhDang';
import { maChongGiaMao } from './api';

export default function BoCuc({ children }: { children: ReactNode }) {
  const { nguoiDung, soChuaDoc } = usePhien();
  const viTri = useLocation();
  if (!nguoiDung) return null;

  const dangNhapGiaoDich = viTri.pathname === '/giao-dich/nhap';
  const dangLichSu = viTri.pathname.startsWith('/giao-dich') && !dangNhapGiaoDich;
  const lop = (dangChon: boolean) => (dangChon ? 'dang-chon' : '');

  return (
    <>
      <header className="thanh-tren">
        <Link className="ten-app" to="/">
          <svg className="logo" viewBox="0 0 32 32" aria-hidden="true">
            <rect width="32" height="32" rx="8" fill="#ffffff14" />
            <rect x="6" y="14" width="20" height="5" rx="2.5" fill="#ffffff33" />
            <rect x="6" y="14" width="13" height="5" rx="2.5" fill="#3FC1C5" />
            <rect x="21" y="10" width="2" height="13" rx="1" fill="#fff" />
          </svg>
          Kiểm soát hạn mức
        </Link>
        <nav className="menu" aria-label="Chức năng">
          {nguoiDung.duocNhapGiaoDich && <Link className={lop(dangNhapGiaoDich)} to="/giao-dich/nhap">Nhập giao dịch</Link>}
          {nguoiDung.duocXemGiaoDich && (
            <>
              <Link className={lop(dangLichSu)} to="/giao-dich">Lịch sử</Link>
              <NavLink className={({ isActive }) => lop(isActive)} to="/bao-cao">Báo cáo</NavLink>
            </>
          )}
          {nguoiDung.duocQuanLyHanMuc && (
            <>
              <NavLink className={({ isActive }) => lop(isActive)} to="/han-muc">Hạn mức</NavLink>
              <NavLink className={({ isActive }) => lop(isActive)} to="/ngoai-le">Duyệt ngoại lệ</NavLink>
            </>
          )}
          {nguoiDung.laQuanTri && (
            <>
              <NavLink className={({ isActive }) => lop(isActive)} to="/danh-muc">Danh mục</NavLink>
              <NavLink className={({ isActive }) => lop(isActive)} to="/nguoi-dung">Người dùng</NavLink>
            </>
          )}
        </nav>
        <div className="nguoi-dung">
          {!nguoiDung.laQuanTri && (
            <Link className="chuong" to="/thong-bao" aria-label="Thông báo"
              title={soChuaDoc > 0 ? soChuaDoc + ' thông báo chưa đọc' : 'Thông báo'}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                <path d="M6 8a6 6 0 1 1 12 0c0 7 3 9 3 9H3s3-2 3-9" /><path d="M10.3 21a1.94 1.94 0 0 0 3.4 0" />
              </svg>
              {soChuaDoc > 0 && <span className="so">{soChuaDoc}</span>}
            </Link>
          )}
          <span className="ho-so">
            <span className="chu-cai" aria-hidden="true">{chuCaiDau(nguoiDung.hoTen)}</span>
            <span className="ten">{nguoiDung.hoTen}<small>{nguoiDung.tenVaiTro}</small></span>
          </span>
          <form method="post" action="/xac-thuc/dang-xuat">
            <input type="hidden" name="__RequestVerificationToken" value={maChongGiaMao()} />
            <button type="submit" className="nut nut-nho nut-vien-trang">Đăng xuất</button>
          </form>
        </div>
      </header>
      <main className="noi-dung">
        <HopBao />
        {children}
      </main>
    </>
  );
}
