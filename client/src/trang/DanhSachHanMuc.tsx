import { FormEvent, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { api, thamSo } from '../api';
import { useBao } from '../Bao';
import { useTai } from '../useTai';
import Thuoc from '../Thuoc';
import { lopTrangThai, ngay, ngayInput, tien, tyLe } from '../dinhDang';

const CAC_KY = [['HienTai', 'Kỳ hiện tại'], ['TatCa', 'Tất cả'], ['DaHetKy', 'Đã hết kỳ']];
const CAC_TRANG_THAI = ['Hoạt động', 'Sắp vượt', 'Đã vượt', 'Đã khóa'];

export default function DanhSachHanMuc() {
  const [ts, datTs] = useSearchParams();
  const { bao } = useBao();
  const ky = ts.get('ky') ?? 'HienTai';
  const { duLieu, loi, taiLai } = useTai('/api/han-muc' + thamSo({ ky, maSinhVien: ts.get('maSinhVien'), trangThai: ts.get('trangThai') }));
  const [loc, datLoc] = useState({ ky, maSinhVien: ts.get('maSinhVien') ?? '', trangThai: ts.get('trangThai') ?? '' });

  function locDanhSach(e: FormEvent) {
    e.preventDefault();
    datTs(new URLSearchParams(thamSo(loc).substring(1)));
  }

  async function doiKhoa(r: any, khoa: boolean) {
    if (khoa && !window.confirm(`Khóa hạn mức ${r.sinhVien} - ${r.tenDanhMuc}? Sinh viên sẽ không ghi được giao dịch mới cho danh mục này (BR06).`))
      return;
    try {
      const kq = await api.gui(`/api/han-muc/${r.maHanMuc}/${khoa ? 'khoa' : 'mo-khoa'}`);
      bao({ xanh: kq.thongBao });
    } catch (ex) {
      bao({ do: (ex as Error).message });
    }
    taiLai();
  }

  const ds: any[] = duLieu?.hanMucs ?? [];
  const homNay = duLieu ? ngayInput(duLieu.homNay) : '';

  return (
    <>
      <div className="dau-trang">
        <h1>Danh sách hạn mức</h1>
        <Link className="nut nut-chinh" to="/han-muc/tao">Thêm hạn mức</Link>
      </div>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}

      <form className="thanh-loc" onSubmit={locDanhSach}>
        <label>Kỳ
          <select value={loc.ky} onChange={(e) => datLoc({ ...loc, ky: e.target.value })}>
            {CAC_KY.map(([ma, ten]) => <option key={ma} value={ma}>{ten}</option>)}
          </select>
        </label>
        <label>Sinh viên
          <select value={loc.maSinhVien} onChange={(e) => datLoc({ ...loc, maSinhVien: e.target.value })}>
            <option value="">Tất cả</option>
            {(duLieu?.sinhViens ?? []).map((s: any) => <option key={s.maNguoiDung} value={s.maNguoiDung}>{s.hoTen}</option>)}
          </select>
        </label>
        <label>Trạng thái
          <select value={loc.trangThai} onChange={(e) => datLoc({ ...loc, trangThai: e.target.value })}>
            <option value="">Tất cả</option>
            {CAC_TRANG_THAI.map((t) => <option key={t}>{t}</option>)}
          </select>
        </label>
        <button type="submit" className="nut nut-chinh">Lọc</button>
      </form>

      <table className="bang">
        <thead>
          <tr>
            <th>Sinh viên</th><th>Danh mục</th><th>Kỳ</th>
            <th className="so">Hạn mức</th><th className="so">Đã chi</th><th className="so">Còn lại</th>
            <th>Tỷ lệ</th><th>Loại</th><th>Trạng thái</th><th></th>
          </tr>
        </thead>
        <tbody>
          {ds.map((r) => {
            const daKhoa = r.trangThai === 'Đã khóa';
            const hetKy = ngayInput(r.ngayKetThuc) < homNay;
            return (
              <tr key={r.maHanMuc} className={lopTrangThai(r.trangThai)}>
                <td>{r.sinhVien}</td>
                <td>{r.tenDanhMuc}</td>
                <td>{ngay(r.ngayBatDau)} - {ngay(r.ngayKetThuc)}</td>
                <td className="so">{tien(r.soTienHanMuc)}</td>
                <td className="so">{tien(r.soTienDaChi)}</td>
                <td className="so">{tien(r.soConLai)}</td>
                <td>
                  <Thuoc lop="thuoc-nho" tyLeSuDung={r.tyLeSuDung} nguong={r.nguongCanhBao} />
                  {tyLe(r.tyLeSuDung)}
                </td>
                <td>{r.loaiHanMuc}</td>
                <td><span className={'nhan ' + lopTrangThai(r.trangThai)}>{r.trangThai}</span></td>
                <td className="cot-nut">
                  {!hetKy && <Link className="nut nut-nho" to={'/han-muc/' + r.maHanMuc}>Sửa</Link>}
                  {daKhoa
                    ? <button type="button" className="nut nut-nho" onClick={() => doiKhoa(r, false)}>Mở khóa</button>
                    : !hetKy && <button type="button" className="nut nut-nho" onClick={() => doiKhoa(r, true)}>Khóa</button>}
                </td>
              </tr>
            );
          })}
          {duLieu && ds.length === 0 && (
            <tr><td colSpan={10} className="ghi-chu">Không có hạn mức phù hợp bộ lọc.</td></tr>
          )}
        </tbody>
      </table>
    </>
  );
}
