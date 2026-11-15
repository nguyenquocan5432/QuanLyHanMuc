import { FormEvent, useState } from 'react';
import { api } from '../api';
import { useBao } from '../Bao';
import { useTai } from '../useTai';
import { tenVaiTro } from '../dinhDang';

const CAC_VAI_TRO = [['SinhVien', 'Sinh viên'], ['CapNganSach', 'Người cấp ngân sách'], ['QuanTri', 'Quản trị viên']];
const TRONG = {
  maNguoiDung: null as number | null, tenDangNhap: '', hoTen: '', email: '', vaiTro: 'SinhVien', maNguoiCap: '0', hoatDong: true,
};

export default function NguoiDung() {
  const { bao } = useBao();
  const { duLieu, loi, taiLai } = useTai('/api/nguoi-dung');
  const [f, datF] = useState(TRONG);
  const dangSua = f.maNguoiDung !== null;

  async function luu(e: FormEvent) {
    e.preventDefault();
    try {
      const kq = await api.gui('/api/nguoi-dung', { ...f, maNguoiCap: f.vaiTro === 'SinhVien' ? +f.maNguoiCap : 0 });
      bao({ xanh: kq.thongBao, vang: kq.canhBao });
      datF({ ...f, maNguoiDung: kq.ma });
      taiLai();
    } catch (ex) {
      bao({ do: (ex as Error).message });
    }
  }

  async function datLaiMatKhau() {
    if (!window.confirm(`Đặt lại mật khẩu của ${f.tenDangNhap} về ${duLieu.matKhauMacDinh}?`)) return;
    try {
      const kq = await api.gui(`/api/nguoi-dung/${f.maNguoiDung}/dat-lai-mat-khau`);
      bao({ xanh: kq.thongBao });
    } catch (ex) {
      bao({ do: (ex as Error).message });
    }
  }

  return (
    <>
      <h1>Người dùng</h1>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}
      <div className="hai-cot">
        <table className="bang">
          <thead><tr><th>Tên đăng nhập</th><th>Họ tên</th><th>Vai trò</th><th>Người cấp ngân sách</th><th>Trạng thái</th></tr></thead>
          <tbody>
            {(duLieu?.nguoiDungs ?? []).map((r: any) => (
              <tr key={r.maNguoiDung} className={(r.hoatDong ? '' : 'tt-da-khoa ') + (f.maNguoiDung === r.maNguoiDung ? 'dong-chon' : '')}>
                <td>
                  <a href="#" onClick={(e) => {
                    e.preventDefault();
                    datF({
                      maNguoiDung: r.maNguoiDung, tenDangNhap: r.tenDangNhap, hoTen: r.hoTen, email: r.email ?? '',
                      vaiTro: r.vaiTro, maNguoiCap: String(r.maNguoiCapNganSach ?? 0), hoatDong: r.hoatDong,
                    });
                  }}>{r.tenDangNhap}</a>
                </td>
                <td>{r.hoTen}</td>
                <td>{tenVaiTro(r.vaiTro)}</td>
                <td>{r.nguoiCap ?? (r.vaiTro === 'SinhVien' ? 'Tự quản lý' : '')}</td>
                <td>{r.hoatDong ? 'Hoạt động' : 'Đã khóa'}</td>
              </tr>
            ))}
          </tbody>
        </table>

        <div>
          <form className="the" onSubmit={luu}>
            <h2>{dangSua ? 'Sửa người dùng' : 'Thêm người dùng mới'}</h2>
            <label>Tên đăng nhập
              <input type="text" maxLength={50} required pattern="[A-Za-z0-9_.]+" title="Chỉ chữ không dấu, số, dấu chấm, gạch dưới"
                value={f.tenDangNhap} disabled={dangSua} onChange={(e) => datF({ ...f, tenDangNhap: e.target.value })} />
            </label>
            <label>Họ tên
              <input type="text" maxLength={100} required value={f.hoTen} onChange={(e) => datF({ ...f, hoTen: e.target.value })} />
            </label>
            <label>Email
              <input type="email" maxLength={100} value={f.email} onChange={(e) => datF({ ...f, email: e.target.value })} />
            </label>
            <label>Vai trò
              <select value={f.vaiTro} onChange={(e) => datF({ ...f, vaiTro: e.target.value })}>
                {CAC_VAI_TRO.map(([ma, ten]) => <option key={ma} value={ma}>{ten}</option>)}
              </select>
            </label>
            <label>Người cấp ngân sách
              <select value={f.vaiTro === 'SinhVien' ? f.maNguoiCap : '0'} disabled={f.vaiTro !== 'SinhVien'}
                onChange={(e) => datF({ ...f, maNguoiCap: e.target.value })}>
                <option value="0">Tự quản lý</option>
                {(duLieu?.nguoiCaps ?? []).map((n: any) => <option key={n.maNguoiDung} value={n.maNguoiDung}>{n.hoTen}</option>)}
              </select>
            </label>
            <label className="chon">
              <input type="checkbox" checked={f.hoatDong} onChange={(e) => datF({ ...f, hoatDong: e.target.checked })} />
              {' '}Đang hoạt động
            </label>
            <div className="thanh-nut">
              <button type="submit" className="nut nut-chinh">Lưu</button>
              <button type="button" className="nut" onClick={() => datF(TRONG)}>Thêm mới</button>
            </div>
          </form>

          {dangSua && (
            <div className="the">
              <button type="button" className="nut" onClick={datLaiMatKhau}>Đặt lại mật khẩu</button>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
