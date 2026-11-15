import { FormEvent, useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { thamSo } from '../api';
import { usePhien } from '../Phien';
import { useTai } from '../useTai';
import { lopTrangThai, ngay, ngayInput, tien } from '../dinhDang';

const CAC_TRANG_THAI = ['Hợp lệ', 'Chờ duyệt', 'Từ chối', 'Đã hủy'];

export default function LichSu() {
  const { nguoiDung } = usePhien();
  const [ts, datTs] = useSearchParams();
  const { duLieu, loi } = useTai('/api/giao-dich' + thamSo({
    tu: ts.get('tu'), den: ts.get('den'), maDanhMuc: ts.get('maDanhMuc'), trangThai: ts.get('trangThai'),
  }));
  const [loc, datLoc] = useState({ tu: '', den: '', maDanhMuc: '', trangThai: '' });

  useEffect(() => {
    if (duLieu) {
      datLoc({ tu: ngayInput(duLieu.tu), den: ngayInput(duLieu.den), maDanhMuc: ts.get('maDanhMuc') ?? '', trangThai: ts.get('trangThai') ?? '' });
    }
  }, [duLieu]);

  function xem(e: FormEvent) {
    e.preventDefault();
    datTs(new URLSearchParams(thamSo(loc).substring(1)));
  }

  const quanLy = !!nguoiDung?.duocQuanLyHanMuc;
  const ds: any[] = duLieu?.giaoDichs ?? [];
  const tongHopLe = ds.filter((r) => r.trangThai === 'Hợp lệ').reduce((t, r) => t + r.soTien, 0);

  return (
    <>
      <h1>Lịch sử giao dịch</h1>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}

      <form className="thanh-loc" onSubmit={xem}>
        <label>Từ <input type="date" value={loc.tu} onChange={(e) => datLoc({ ...loc, tu: e.target.value })} /></label>
        <label>Đến <input type="date" value={loc.den} onChange={(e) => datLoc({ ...loc, den: e.target.value })} /></label>
        <label>Danh mục
          <select value={loc.maDanhMuc} onChange={(e) => datLoc({ ...loc, maDanhMuc: e.target.value })}>
            <option value="">Tất cả</option>
            {(duLieu?.danhMucs ?? []).map((d: any) => <option key={d.maDanhMuc} value={d.maDanhMuc}>{d.tenDanhMuc}</option>)}
          </select>
        </label>
        <label>Trạng thái
          <select value={loc.trangThai} onChange={(e) => datLoc({ ...loc, trangThai: e.target.value })}>
            <option value="">Tất cả</option>
            {CAC_TRANG_THAI.map((t) => <option key={t}>{t}</option>)}
          </select>
        </label>
        <button type="submit" className="nut nut-chinh">Xem</button>
      </form>

      <table className="bang">
        <thead>
          <tr>
            <th>Ngày</th>
            {quanLy && <th>Sinh viên</th>}
            <th>Danh mục</th><th className="so">Số tiền</th><th>Mô tả</th><th>Trạng thái</th><th>Lý do hủy</th>
            {quanLy && <th></th>}
          </tr>
        </thead>
        <tbody>
          {ds.map((r) => (
            <tr key={r.maGiaoDich} className={lopTrangThai(r.trangThai)}>
              <td>{ngay(r.ngayGiaoDich)}</td>
              {quanLy && <td>{r.sinhVien}</td>}
              <td>{r.tenDanhMuc}</td>
              <td className="so">{tien(r.soTien)}</td>
              <td>{r.moTa}</td>
              <td><span className={'nhan ' + lopTrangThai(r.trangThai)}>{r.trangThai}</span></td>
              <td>{r.lyDoHuy}</td>
              {quanLy && (
                <td>{r.trangThai === 'Hợp lệ' && <Link className="nut nut-nho" to={`/giao-dich/${r.maGiaoDich}/huy`}>Hủy</Link>}</td>
              )}
            </tr>
          ))}
          {duLieu && ds.length === 0 && (
            <tr><td colSpan={9} className="ghi-chu">Không có giao dịch trong khoảng đã chọn.</td></tr>
          )}
        </tbody>
      </table>
      <p className="tong">Tổng hợp lệ: <b>{tien(tongHopLe)}</b></p>
    </>
  );
}
