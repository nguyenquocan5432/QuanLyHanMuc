import { FormEvent, useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { thamSo } from '../api';
import { usePhien } from '../Phien';
import { useTai } from '../useTai';
import { ngayGio, ngayInput, rongThanh, tien, tyLe } from '../dinhDang';

export default function BaoCao() {
  const { nguoiDung } = usePhien();
  const [ts, datTs] = useSearchParams();
  const quanLy = !!nguoiDung?.duocQuanLyHanMuc;
  const tab = quanLy && ts.get('tab') === 'vipham' ? 'vipham' : 'chitieu';

  const { duLieu, loi } = useTai(
    (tab === 'vipham' ? '/api/bao-cao/vi-pham' : '/api/bao-cao/chi-tieu') +
    thamSo({ tu: ts.get('tu'), den: ts.get('den'), maSinhVien: ts.get('maSinhVien') }));
  const [loc, datLoc] = useState({ tu: '', den: '', maSinhVien: '' });

  useEffect(() => {
    if (duLieu) datLoc({ tu: ngayInput(duLieu.tu), den: ngayInput(duLieu.den), maSinhVien: String(duLieu.maSinhVien ?? '') });
  }, [duLieu]);

  function xem(e: FormEvent) {
    e.preventDefault();
    datTs(new URLSearchParams(thamSo({ tab: tab === 'vipham' ? 'vipham' : null, ...loc }).substring(1)));
  }

  const chiTieu: any[] = tab === 'chitieu' ? duLieu?.chiTieu ?? [] : [];
  const viPham: any[] = tab === 'vipham' ? duLieu?.viPham ?? [] : [];
  const lonNhat = Math.max(1, ...chiTieu.map((r) => Math.max(r.soTienHanMuc, r.daChi)));

  return (
    <>
      <h1>Báo cáo</h1>
      {quanLy && (
        <nav className="tab">
          <Link className={tab === 'chitieu' ? 'dang-chon' : ''} to="/bao-cao">Chi tiêu theo kỳ</Link>
          <Link className={tab === 'vipham' ? 'dang-chon' : ''} to="/bao-cao?tab=vipham">Vi phạm hạn mức</Link>
        </nav>
      )}
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}

      <form className="thanh-loc" onSubmit={xem}>
        <label>Từ <input type="date" value={loc.tu} onChange={(e) => datLoc({ ...loc, tu: e.target.value })} /></label>
        <label>Đến <input type="date" value={loc.den} onChange={(e) => datLoc({ ...loc, den: e.target.value })} /></label>
        {quanLy && duLieu?.sinhViens && (
          <label>Sinh viên
            <select value={loc.maSinhVien} onChange={(e) => datLoc({ ...loc, maSinhVien: e.target.value })}>
              {tab === 'vipham' && <option value="">Tất cả</option>}
              {duLieu.sinhViens.map((s: any) => <option key={s.maNguoiDung} value={s.maNguoiDung}>{s.hoTen}</option>)}
            </select>
          </label>
        )}
        <button type="submit" className="nut nut-chinh">Xem</button>
      </form>

      {tab === 'chitieu' ? (
        <div className="hai-cot">
          <table className="bang">
            <thead><tr><th>Danh mục</th><th className="so">Hạn mức</th><th className="so">Đã chi</th><th className="so">Còn lại</th><th className="so">Tỷ lệ</th></tr></thead>
            <tbody>
              {chiTieu.map((r) => (
                <tr key={r.tenDanhMuc}>
                  <td>{r.tenDanhMuc}</td>
                  <td className="so">{tien(r.soTienHanMuc)}</td>
                  <td className="so">{tien(r.daChi)}</td>
                  <td className="so">{tien(r.conLai)}</td>
                  <td className={'so ' + (r.tyLe > 100 ? 'tt-da-vuot' : r.tyLe >= r.nguong ? 'tt-sap-vuot' : '')}>{tyLe(r.tyLe)}</td>
                </tr>
              ))}
              {duLieu && chiTieu.length === 0 && (
                <tr><td colSpan={5} className="ghi-chu">Không có hạn mức nào trong khoảng đã chọn.</td></tr>
              )}
            </tbody>
          </table>

          <div className="the bieu-do">
            <div className="chu-giai"><span className="mau-han-muc"></span> Hạn mức <span className="mau-da-chi"></span> Đã chi</div>
            {chiTieu.map((r) => (
              <div className="nhom-cot" key={r.tenDanhMuc}>
                <div className="ten-cot">{r.tenDanhMuc}</div>
                <div className="cot cot-han-muc" style={{ width: rongThanh((r.soTienHanMuc * 100) / lonNhat) }}>{tien(r.soTienHanMuc)}</div>
                <div className={'cot cot-da-chi ' + (r.daChi > r.soTienHanMuc ? 'vuot' : '')} style={{ width: rongThanh((r.daChi * 100) / lonNhat) }}>{tien(r.daChi)}</div>
              </div>
            ))}
          </div>
        </div>
      ) : (
        <table className="bang">
          <thead><tr><th>Ngày</th><th>Sinh viên</th><th>Danh mục</th><th className="so">Số tiền định chi</th><th className="so">Còn lại lúc đó</th><th>Kết quả</th></tr></thead>
          <tbody>
            {viPham.map((r, i) => (
              <tr key={i} className={r.ketQua === 'Từ chối' ? 'tt-tu-choi' : 'tt-cho-duyet'}>
                <td>{ngayGio(r.ngayXayRa)}</td>
                <td>{r.sinhVien}</td>
                <td>{r.tenDanhMuc}</td>
                <td className="so">{tien(r.soTienDinhChi)}</td>
                <td className="so">{tien(r.soConLaiLucDo)}</td>
                <td>{r.ketQua}</td>
              </tr>
            ))}
            {duLieu && viPham.length === 0 && (
              <tr><td colSpan={6} className="ghi-chu">Không có vi phạm trong khoảng đã chọn.</td></tr>
            )}
          </tbody>
        </table>
      )}
    </>
  );
}
