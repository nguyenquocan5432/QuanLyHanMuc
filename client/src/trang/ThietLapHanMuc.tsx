import { FormEvent, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from '../api';
import { useBao } from '../Bao';
import { useTai } from '../useTai';
import { ngay, ngayInput, tien } from '../dinhDang';

export default function ThietLapHanMuc() {
  const { id } = useParams();
  const laSua = !!id;
  const chuyen = useNavigate();
  const { bao } = useBao();
  const { duLieu: luaChon, loi: loiLuaChon } = useTai(laSua ? null : '/api/han-muc/lua-chon');
  const { duLieu: hienCo, loi: loiHienCo } = useTai(laSua ? '/api/han-muc/' + id : null);

  const [f, datF] = useState({
    maSinhVien: '', maDanhMuc: '', ngayBatDau: '', ngayKetThuc: '',
    soTienHanMuc: '1000000', loaiHanMuc: 'Cứng', nguongCanhBao: '80', apDungTatCa: false,
  });
  const [loi, datLoi] = useState<string | null>(null);
  const [dangGui, datDangGui] = useState(false);

  useEffect(() => {
    if (luaChon) {
      datF((cu) => ({
        ...cu,
        maSinhVien: String(luaChon.sinhViens[0]?.maNguoiDung ?? ''),
        maDanhMuc: String(luaChon.danhMucs[0]?.maDanhMuc ?? ''),
        ngayBatDau: ngayInput(luaChon.ngayBatDau),
        ngayKetThuc: ngayInput(luaChon.ngayKetThuc),
      }));
    }
  }, [luaChon]);

  useEffect(() => {
    if (hienCo) {
      datF((cu) => ({
        ...cu,
        soTienHanMuc: String(hienCo.soTienHanMuc),
        loaiHanMuc: hienCo.loaiHanMuc,
        nguongCanhBao: String(hienCo.nguongCanhBao),
      }));
    }
  }, [hienCo]);

  async function luu(e: FormEvent) {
    e.preventDefault();
    datDangGui(true);
    datLoi(null);
    const noiDung = {
      maSinhVien: +f.maSinhVien || 0,
      maDanhMuc: +f.maDanhMuc || 0,
      ngayBatDau: laSua ? ngayInput(hienCo.ngayBatDau) : f.ngayBatDau,
      ngayKetThuc: laSua ? ngayInput(hienCo.ngayKetThuc) : f.ngayKetThuc,
      soTienHanMuc: +f.soTienHanMuc || 0,
      loaiHanMuc: f.loaiHanMuc,
      nguongCanhBao: +f.nguongCanhBao || 0,
      apDungTatCa: f.apDungTatCa,
      phienBan: laSua ? hienCo.phienBan : null,
    };
    try {
      const kq = laSua ? await api.sua('/api/han-muc/' + id, noiDung) : await api.gui('/api/han-muc', noiDung);
      bao({ xanh: kq.thongBao, vang: kq.canhBao }, true);
      chuyen('/han-muc');
    } catch (ex) {
      datLoi((ex as Error).message);
    } finally {
      datDangGui(false);
    }
  }

  const loiTai = loiLuaChon ?? loiHienCo;
  const sanSang = laSua ? !!hienCo : !!luaChon;

  return (
    <>
      <h1>{laSua ? 'Điều chỉnh hạn mức' : 'Thiết lập hạn mức'}</h1>
      {(loi ?? loiTai) && <div className="hop-bao bao-do" role="alert">{loi ?? loiTai}</div>}

      {sanSang && (
        <form className="the hep" onSubmit={luu}>
          {laSua ? (
            <>
              <p>Sinh viên: <b>{hienCo.sinhVien}</b> · Danh mục: <b>{hienCo.tenDanhMuc}</b></p>
              <p>Kỳ: <b>{ngay(hienCo.ngayBatDau)} - {ngay(hienCo.ngayKetThuc)}</b> · Đã chi: <b>{tien(hienCo.soTienDaChi)}</b></p>
            </>
          ) : (
            <>
              <label>Sinh viên
                <select value={f.maSinhVien} disabled={f.apDungTatCa} onChange={(e) => datF({ ...f, maSinhVien: e.target.value })}>
                  {luaChon.sinhViens.map((s: any) => <option key={s.maNguoiDung} value={s.maNguoiDung}>{s.hoTen}</option>)}
                </select>
              </label>
              <label>Danh mục
                <select value={f.maDanhMuc} onChange={(e) => datF({ ...f, maDanhMuc: e.target.value })}>
                  {luaChon.danhMucs.map((d: any) => <option key={d.maDanhMuc} value={d.maDanhMuc}>{d.tenDanhMuc}</option>)}
                </select>
              </label>
              <div className="hang-nhap">
                <label>Từ ngày <input type="date" required value={f.ngayBatDau} onChange={(e) => datF({ ...f, ngayBatDau: e.target.value })} /></label>
                <label>Đến ngày <input type="date" required value={f.ngayKetThuc} onChange={(e) => datF({ ...f, ngayKetThuc: e.target.value })} /></label>
              </div>
            </>
          )}

          <label>Số tiền hạn mức (đ)
            <input type="number" min={1000} step={1000} required value={f.soTienHanMuc}
              onChange={(e) => datF({ ...f, soTienHanMuc: e.target.value })} />
          </label>

          <fieldset>
            <legend>Loại hạn mức</legend>
            {['Cứng', 'Mềm'].map((loai) => (
              <label className="chon" key={loai}>
                <input type="radio" name="loaiHanMuc" value={loai} checked={f.loaiHanMuc === loai}
                  onChange={() => datF({ ...f, loaiHanMuc: loai })} /> {loai}
              </label>
            ))}
          </fieldset>

          <label>Ngưỡng cảnh báo (%)
            <input type="number" min={1} max={100} step={1} required value={f.nguongCanhBao}
              onChange={(e) => datF({ ...f, nguongCanhBao: e.target.value })} />
          </label>

          {!laSua && luaChon.coSinhVienCuaToi && (
            <label className="chon">
              <input type="checkbox" checked={f.apDungTatCa} onChange={(e) => datF({ ...f, apDungTatCa: e.target.checked })} />
              {' '}Áp dụng cho tất cả sinh viên của tôi
            </label>
          )}

          <div className="thanh-nut">
            <button type="submit" className="nut nut-chinh" disabled={dangGui}>Lưu</button>
            <Link className="nut" to="/han-muc">Hủy bỏ</Link>
          </div>
        </form>
      )}
    </>
  );
}
