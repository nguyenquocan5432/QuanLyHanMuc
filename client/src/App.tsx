import { useEffect } from 'react';
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import { usePhien, NguoiDung } from './Phien';
import BoCuc from './BoCuc';
import DangNhap from './trang/DangNhap';
import TrangChu from './trang/TrangChu';
import NhapGiaoDich from './trang/NhapGiaoDich';
import LichSu from './trang/LichSu';
import HuyGiaoDich from './trang/HuyGiaoDich';
import DanhSachHanMuc from './trang/DanhSachHanMuc';
import ThietLapHanMuc from './trang/ThietLapHanMuc';
import DuyetNgoaiLe from './trang/DuyetNgoaiLe';
import ThongBao from './trang/ThongBao';
import BaoCao from './trang/BaoCao';
import DanhMuc from './trang/DanhMuc';
import NguoiDungTrang from './trang/NguoiDung';
import KhongCoQuyen from './trang/KhongCoQuyen';

function CanQuyen({ dieuKien, children }: { dieuKien: (nd: NguoiDung) => boolean; children: JSX.Element }) {
  const { nguoiDung } = usePhien();
  if (!nguoiDung) return null;
  return dieuKien(nguoiDung) ? children : <Navigate to="/khong-co-quyen" replace />;
}

export default function App() {
  const { dangTai, nguoiDung, loi, taiLai } = usePhien();
  const viTri = useLocation();
  const chuyen = useNavigate();

  useEffect(() => {
    const chuaDangNhap = () => { taiLai(); };
    const khongCoQuyen = () => chuyen('/khong-co-quyen');
    window.addEventListener('chua-dang-nhap', chuaDangNhap);
    window.addEventListener('khong-co-quyen', khongCoQuyen);
    return () => {
      window.removeEventListener('chua-dang-nhap', chuaDangNhap);
      window.removeEventListener('khong-co-quyen', khongCoQuyen);
    };
  }, [taiLai, chuyen]);

  if (dangTai) return null;

  if (loi && !nguoiDung) {
    return (
      <div className="trang-dang-nhap">
        <main className="noi-dung">
          <div className="the hep">
            <h1>Đã có lỗi</h1>
            <p className="chu-loi">{loi}</p>
            <button className="nut" onClick={() => taiLai()}>Thử lại</button>
          </div>
        </main>
      </div>
    );
  }

  if (!nguoiDung) {
    if (viTri.pathname !== '/dang-nhap') {
      const ve = viTri.pathname + viTri.search;
      return <Navigate to={'/dang-nhap' + (ve !== '/' ? '?returnUrl=' + encodeURIComponent(ve) : '')} replace />;
    }
    return <DangNhap />;
  }

  if (viTri.pathname === '/dang-nhap') return <Navigate to="/" replace />;

  return (
    <BoCuc>
      <Routes>
        <Route path="/" element={<TrangChu />} />
        <Route path="/giao-dich/nhap" element={<CanQuyen dieuKien={(n) => n.duocNhapGiaoDich}><NhapGiaoDich /></CanQuyen>} />
        <Route path="/giao-dich" element={<CanQuyen dieuKien={(n) => n.duocXemGiaoDich}><LichSu /></CanQuyen>} />
        <Route path="/giao-dich/:id/huy" element={<CanQuyen dieuKien={(n) => n.duocQuanLyHanMuc}><HuyGiaoDich /></CanQuyen>} />
        <Route path="/han-muc" element={<CanQuyen dieuKien={(n) => n.duocQuanLyHanMuc}><DanhSachHanMuc /></CanQuyen>} />
        <Route path="/han-muc/tao" element={<CanQuyen dieuKien={(n) => n.duocQuanLyHanMuc}><ThietLapHanMuc /></CanQuyen>} />
        <Route path="/han-muc/:id" element={<CanQuyen dieuKien={(n) => n.duocQuanLyHanMuc}><ThietLapHanMuc /></CanQuyen>} />
        <Route path="/ngoai-le" element={<CanQuyen dieuKien={(n) => n.duocQuanLyHanMuc}><DuyetNgoaiLe /></CanQuyen>} />
        <Route path="/thong-bao" element={<CanQuyen dieuKien={(n) => !n.laQuanTri}><ThongBao /></CanQuyen>} />
        <Route path="/bao-cao" element={<CanQuyen dieuKien={(n) => n.duocXemGiaoDich}><BaoCao /></CanQuyen>} />
        <Route path="/danh-muc" element={<CanQuyen dieuKien={(n) => n.laQuanTri}><DanhMuc /></CanQuyen>} />
        <Route path="/nguoi-dung" element={<CanQuyen dieuKien={(n) => n.laQuanTri}><NguoiDungTrang /></CanQuyen>} />
        <Route path="/khong-co-quyen" element={<KhongCoQuyen />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BoCuc>
  );
}
