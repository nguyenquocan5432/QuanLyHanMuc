import { api } from '../api';
import { usePhien } from '../Phien';
import { useTai } from '../useTai';
import { ngayGio } from '../dinhDang';

export default function ThongBao() {
  const { taiLai: taiLaiPhien } = usePhien();
  const { duLieu, loi, taiLai } = useTai<any[]>('/api/thong-bao');
  const ds = duLieu ?? [];
  const chuaDoc = ds.filter((r) => !r.daDoc).length;

  async function daDoc(ma: number | null) {
    await api.gui('/api/thong-bao/da-doc', { ma });
    taiLai();
    taiLaiPhien();
  }

  return (
    <>
      <div className="dau-trang">
        <h1>Thông báo</h1>
        {chuaDoc > 0 && <button type="button" className="nut" onClick={() => daDoc(null)}>Đánh dấu tất cả đã đọc</button>}
      </div>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}

      <table className="bang">
        <thead><tr><th>Ngày</th><th>Loại</th><th>Nội dung</th><th>Tình trạng gửi</th><th></th></tr></thead>
        <tbody>
          {ds.map((r) => (
            <tr key={r.maThongBao} className={r.daDoc ? '' : 'chua-doc'}>
              <td>{ngayGio(r.ngayTao)}</td>
              <td><span className={'nhan loai-' + String(r.loaiThongBao).replace(/ /g, '-').toLowerCase()}>{r.loaiThongBao}</span></td>
              <td>{r.noiDung}</td>
              <td>{r.tinhTrangGui}</td>
              <td>{!r.daDoc && <button type="button" className="nut nut-nho" onClick={() => daDoc(r.maThongBao)}>Đã đọc</button>}</td>
            </tr>
          ))}
          {duLieu && ds.length === 0 && <tr><td colSpan={5} className="ghi-chu">Chưa có thông báo.</td></tr>}
        </tbody>
      </table>
    </>
  );
}
