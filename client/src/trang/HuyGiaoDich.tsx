import { FormEvent, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from '../api';
import { useBao } from '../Bao';
import { useTai } from '../useTai';
import { ngay, tien } from '../dinhDang';

export default function HuyGiaoDich() {
  const { id } = useParams();
  const chuyen = useNavigate();
  const { bao } = useBao();
  const { duLieu: gd, loi } = useTai('/api/giao-dich/' + id);
  const [lyDo, datLyDo] = useState('');
  const [dangGui, datDangGui] = useState(false);

  async function xacNhan(e: FormEvent) {
    e.preventDefault();
    datDangGui(true);
    try {
      const kq = await api.gui(`/api/giao-dich/${id}/huy`, { lyDo });
      bao({ xanh: kq.thongBao }, true);
    } catch (ex) {
      bao({ do: (ex as Error).message }, true);
    }
    chuyen('/giao-dich');
  }

  return (
    <>
      <h1>Hủy giao dịch</h1>
      {loi && (
        <div className="the hep">
          <p className="chu-loi">{loi}</p>
          <Link className="nut" to="/giao-dich">Về lịch sử</Link>
        </div>
      )}
      {gd && (
        <div className="the hep">
          <p>
            <b>{ngay(gd.ngayGiaoDich)}</b> · {gd.sinhVien} · {gd.tenDanhMuc} · <b>{tien(gd.soTien)}</b> · trạng thái {gd.trangThai}
          </p>
          <form onSubmit={xacNhan}>
            <label>Lý do hủy
              <textarea rows={3} maxLength={255} required value={lyDo} onChange={(e) => datLyDo(e.target.value)}></textarea>
            </label>
            <div className="thanh-nut">
              <button type="submit" className="nut nut-do" disabled={dangGui || lyDo.trim().length === 0}>Xác nhận hủy</button>
              <Link className="nut" to="/giao-dich">Bỏ qua</Link>
            </div>
          </form>
        </div>
      )}
    </>
  );
}
