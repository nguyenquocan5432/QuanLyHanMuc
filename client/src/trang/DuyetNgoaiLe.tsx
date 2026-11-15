import { useState } from 'react';
import { api } from '../api';
import { useBao } from '../Bao';
import { usePhien } from '../Phien';
import { useTai } from '../useTai';
import { ngay, ngayGio, tien } from '../dinhDang';

function TheYeuCau({ r, xuLy }: { r: any; xuLy: (maYeuCau: number, dongY: boolean, ghiChu: string) => void }) {
  const [ghiChu, datGhiChu] = useState('');
  return (
    <div className="the the-yeu-cau">
      <div className="luoi-hai-cot">
        <div>Sinh viên: <b>{r.sinhVien}</b></div>
        <div>Danh mục: <b>{r.tenDanhMuc}</b></div>
        <div>Số tiền: <b>{tien(r.soTien)}</b> {r.moTa && <small>({r.moTa})</small>}</div>
        <div>Ngày giao dịch: {ngay(r.ngayGiaoDich)}</div>
        <div>Còn lại trước giao dịch: {tien(r.conLaiTruocGiaoDich)}</div>
        <div>Vượt: <b className="chu-loi">{tien(r.vuotBaoNhieu)}</b></div>
        <div className="ghi-chu">Yêu cầu lúc {ngayGio(r.ngayYeuCau)}</div>
      </div>
      {r.hanMucDaKhoa && <p className="chu-loi">Hạn mức đang khóa, không duyệt được.</p>}
      <div className="hang-nhap">
        <label className="rong">Ghi chú (bắt buộc khi từ chối)
          <input type="text" maxLength={255} value={ghiChu} onChange={(e) => datGhiChu(e.target.value)} />
        </label>
        <button type="button" className="nut nut-xanh" disabled={r.hanMucDaKhoa} onClick={() => xuLy(r.maYeuCau, true, ghiChu)}>Duyệt</button>
        <button type="button" className="nut nut-do" onClick={() => xuLy(r.maYeuCau, false, ghiChu)}>Từ chối</button>
      </div>
    </div>
  );
}

export default function DuyetNgoaiLe() {
  const { bao } = useBao();
  const { taiLai: taiLaiPhien } = usePhien();
  const { duLieu, loi, taiLai } = useTai<any[]>('/api/ngoai-le');

  async function xuLy(maYeuCau: number, dongY: boolean, ghiChu: string) {
    if (!dongY && !ghiChu.trim()) {
      bao({ do: 'Từ chối phải ghi lý do.' });
      return;
    }
    try {
      const kq = await api.gui('/api/ngoai-le/' + maYeuCau, { dongY, ghiChu });
      bao({ xanh: kq.thongBao });
    } catch (ex) {
      bao({ do: (ex as Error).message });
    }
    taiLai();
    taiLaiPhien();
  }

  return (
    <>
      <h1>Duyệt ngoại lệ vượt hạn mức</h1>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}
      {duLieu && duLieu.length === 0 && <div className="the ghi-chu">Không có yêu cầu chờ duyệt.</div>}
      {(duLieu ?? []).map((r) => <TheYeuCau key={r.maYeuCau} r={r} xuLy={xuLy} />)}
    </>
  );
}
