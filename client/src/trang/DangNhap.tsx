import { FormEvent, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { api } from '../api';
import { usePhien } from '../Phien';

export default function DangNhap() {
  const { cheDo, taiLai } = usePhien();
  const [thamSo] = useSearchParams();
  const chuyen = useNavigate();
  const [tenDangNhap, datTenDangNhap] = useState('');
  const [matKhau, datMatKhau] = useState('');
  const [loi, datLoi] = useState<string | null>(thamSo.get('loi'));
  const [dangGui, datDangGui] = useState(false);

  const returnUrl = thamSo.get('returnUrl');
  const ve = returnUrl && returnUrl.startsWith('/') && !returnUrl.startsWith('//') ? returnUrl : '/';

  async function gui(e: FormEvent) {
    e.preventDefault();
    datDangGui(true);
    datLoi(null);
    try {
      await api.gui('/api/dang-nhap', { tenDangNhap, matKhau });
      await taiLai();
      chuyen(ve, { replace: true });
    } catch (ex) {
      datLoi((ex as Error).message);
    } finally {
      datDangGui(false);
    }
  }

  return (
    <div className="trang-dang-nhap">
      <main className="noi-dung">
        <section className="khung-dang-nhap">
          <h1>Đăng nhập</h1>
          {cheDo === 'Oidc' ? (
            <>
              {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}
              <a className="nut nut-chinh nut-rong" href={'/xac-thuc/dang-nhap?returnUrl=' + encodeURIComponent(ve)}>
                Đăng nhập bằng tài khoản Keycloak
              </a>
            </>
          ) : (
            <form onSubmit={gui}>
              <label>Tên đăng nhập
                <input type="text" value={tenDangNhap} onChange={(e) => datTenDangNhap(e.target.value)}
                  maxLength={50} autoComplete="username" autoFocus required />
              </label>
              <label>Mật khẩu
                <input type="password" value={matKhau} onChange={(e) => datMatKhau(e.target.value)}
                  autoComplete="current-password" required />
              </label>
              {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}
              <button type="submit" className="nut nut-chinh nut-rong" disabled={dangGui}>Đăng nhập</button>
            </form>
          )}
        </section>
      </main>
    </div>
  );
}
