import { FormEvent, useState } from 'react';
import { api } from '../api';
import { useBao } from '../Bao';
import { useTai } from '../useTai';

const TRONG = { maDanhMuc: null as number | null, tenDanhMuc: '', moTa: '', hoatDong: true };

export default function DanhMuc() {
  const { bao } = useBao();
  const { duLieu, loi, taiLai } = useTai<any[]>('/api/danh-muc');
  const [f, datF] = useState(TRONG);

  async function luu(e: FormEvent) {
    e.preventDefault();
    try {
      const kq = await api.gui('/api/danh-muc', f);
      bao({ xanh: kq.thongBao });
      datF({ ...f, maDanhMuc: kq.ma });
      taiLai();
    } catch (ex) {
      bao({ do: (ex as Error).message });
    }
  }

  return (
    <>
      <h1>Danh mục chi tiêu</h1>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}
      <div className="hai-cot">
        <table className="bang">
          <thead><tr><th>Mã</th><th>Tên danh mục</th><th>Mô tả</th><th>Số hạn mức</th><th>Trạng thái</th></tr></thead>
          <tbody>
            {(duLieu ?? []).map((r) => (
              <tr key={r.maDanhMuc} className={(r.hoatDong ? '' : 'tt-da-khoa ') + (f.maDanhMuc === r.maDanhMuc ? 'dong-chon' : '')}>
                <td>{r.maDanhMuc}</td>
                <td>
                  <a href="#" onClick={(e) => {
                    e.preventDefault();
                    datF({ maDanhMuc: r.maDanhMuc, tenDanhMuc: r.tenDanhMuc, moTa: r.moTa ?? '', hoatDong: r.hoatDong });
                  }}>{r.tenDanhMuc}</a>
                </td>
                <td>{r.moTa}</td>
                <td>{r.soHanMuc}</td>
                <td>{r.hoatDong ? 'Đang dùng' : 'Ngừng dùng'}</td>
              </tr>
            ))}
          </tbody>
        </table>

        <form className="the" onSubmit={luu}>
          <h2>{f.maDanhMuc === null ? 'Thêm danh mục mới' : 'Sửa danh mục'}</h2>
          <label>Tên danh mục
            <input type="text" maxLength={100} required value={f.tenDanhMuc} onChange={(e) => datF({ ...f, tenDanhMuc: e.target.value })} />
          </label>
          <label>Mô tả
            <textarea rows={3} maxLength={255} value={f.moTa} onChange={(e) => datF({ ...f, moTa: e.target.value })}></textarea>
          </label>
          <label className="chon">
            <input type="checkbox" checked={f.hoatDong} onChange={(e) => datF({ ...f, hoatDong: e.target.checked })} />
            {' '}Đang dùng
          </label>
          <div className="thanh-nut">
            <button type="submit" className="nut nut-chinh">Lưu</button>
            <button type="button" className="nut" onClick={() => datF(TRONG)}>Thêm mới</button>
          </div>
        </form>
      </div>
    </>
  );
}
