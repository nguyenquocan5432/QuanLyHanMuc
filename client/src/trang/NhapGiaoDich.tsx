import { FormEvent, useEffect, useRef, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { api, thamSo } from '../api';
import { useBao } from '../Bao';
import { usePhien } from '../Phien';
import { useTai } from '../useTai';
import Thuoc from '../Thuoc';
import { lopTrangThai, ngay, ngayInput, tien, tyLe } from '../dinhDang';

interface DuBao {
  mau: string;
  thongDiep: string;
  choGhi: boolean;
  nhanNut: string;
  tyLe?: number;
}

export default function NhapGiaoDich() {
  const [ts, datTs] = useSearchParams();
  const { bao } = useBao();
  const { taiLai: taiLaiPhien } = usePhien();
  const maDanhMuc = ts.get('maDanhMuc') ?? '';
  const ngayChon = ts.get('ngay') ?? '';

  const { duLieu, loi, taiLai } = useTai('/api/giao-dich/nhap' + thamSo({ maDanhMuc, ngay: ngayChon }));
  const [soTien, datSoTien] = useState('');
  const [moTa, datMoTa] = useState('');
  const [duBao, datDuBao] = useState<DuBao | null>(null);
  const [dangGui, datDangGui] = useState(false);
  const hen = useRef<number>();

  const hm = duLieu?.hanMuc ?? null;
  const ngayTrang = duLieu ? ngayInput(duLieu.ngay) : ngayChon;
  const soTienSo = parseInt(soTien, 10) || 0;

  useEffect(() => {
    if (!maDanhMuc || !duLieu) { datDuBao(null); return; }
    window.clearTimeout(hen.current);
    hen.current = window.setTimeout(() => {
      api.lay<DuBao>('/api/giao-dich/danh-gia' + thamSo({ maDanhMuc, ngay: ngayTrang, soTien: soTienSo }))
        .then(datDuBao)
        .catch(() => datDuBao({ mau: '', thongDiep: '', choGhi: soTienSo > 0, nhanNut: 'Ghi nhận' }));
    }, 300);
    return () => window.clearTimeout(hen.current);
  }, [maDanhMuc, ngayTrang, soTienSo, duLieu]);

  function doiLuaChon(moi: { maDanhMuc?: string; ngay?: string }) {
    datSoTien('');
    datTs(new URLSearchParams(thamSo({ maDanhMuc: moi.maDanhMuc ?? maDanhMuc, ngay: moi.ngay ?? ngayTrang }).substring(1)));
  }

  function lamMoi() {
    datSoTien('');
    datMoTa('');
    taiLai();
  }

  async function ghi(e: FormEvent) {
    e.preventDefault();
    datDangGui(true);
    try {
      const kq = await api.gui('/api/giao-dich', { maDanhMuc: +maDanhMuc, ngay: ngayTrang, soTien: soTienSo, moTa });
      bao(kq.muc === 'thanh-cong' ? { xanh: kq.thongBao } : kq.muc === 'canh-bao' ? { vang: kq.thongBao } : { do: kq.thongBao });
      lamMoi();
      taiLaiPhien();
    } catch (ex) {
      bao({ do: (ex as Error).message });
    } finally {
      datDangGui(false);
    }
  }

  const danhMucs: any[] = duLieu?.danhMucs ?? [];
  const coHanMucNao = danhMucs.some((d) => d.coHanMuc);
  const lop = hm ? lopTrangThai(hm.trangThai) : '';
  const coDuKien = soTienSo > 0 && typeof duBao?.tyLe === 'number';

  return (
    <>
      <div className="dau-trang"><h1>Nhập giao dịch</h1></div>
      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}

      <div className="hai-cot">
        <section className="the">
          <div className="hang-nhap">
            <label className="rong">Danh mục
              <select value={maDanhMuc} onChange={(e) => doiLuaChon({ maDanhMuc: e.target.value })}>
                <option value="">Chọn danh mục</option>
                {danhMucs.map((d) => (
                  <option key={d.maDanhMuc} value={d.maDanhMuc} disabled={!d.coHanMuc}>
                    {d.tenDanhMuc}{d.coHanMuc ? '' : ' (chưa có hạn mức)'}
                  </option>
                ))}
              </select>
            </label>
            <label>Ngày
              <input type="date" value={ngayTrang} onChange={(e) => e.target.value && doiLuaChon({ ngay: e.target.value })} />
            </label>
          </div>

          {duLieu && !coHanMucNao && <div className="hop-bao bao-vang">Chưa có hạn mức vào ngày này.</div>}

          <form onSubmit={ghi}>
            <label htmlFor="txtSoTien" className="nhan-o">Số tiền</label>
            <div className="o-tien">
              <input type="number" id="txtSoTien" min={1000} step={1000} inputMode="numeric" autoComplete="off" placeholder="0"
                value={soTien} onChange={(e) => datSoTien(e.target.value)} disabled={!hm} />
              <span className="don-vi">đ</span>
            </div>
            <p className="ghi-chu" aria-live="polite">{soTienSo > 0 ? tien(soTienSo) : ''}</p>

            {duBao?.thongDiep && (
              <div className={'dai-canh-bao ' + (duBao.mau ? 'dai-' + duBao.mau : '')} role="status">{duBao.thongDiep}</div>
            )}

            <label>Mô tả
              <input type="text" maxLength={255} value={moTa} onChange={(e) => datMoTa(e.target.value)} />
            </label>
            <div className="thanh-nut">
              <Link className="nut" to="/">Đóng</Link>
              <button type="button" className="nut" onClick={lamMoi}>Làm mới</button>
              <button type="submit" className="nut nut-chinh" disabled={dangGui || !duBao?.choGhi}>
                {duBao?.nhanNut ?? 'Ghi nhận'}
              </button>
            </div>
          </form>
        </section>

        <div>
          <section className={'the han-muc-hien-tai ' + lop}>
            <div className="the-dau">
              <h2>Hạn mức hiện tại</h2>
              {hm && <span className={'nhan ' + lop}>{hm.trangThai}</span>}
            </div>
            {!hm ? <p className="ghi-chu">Chưa chọn danh mục.</p> : (
              <>
                <div className="so-lon">
                  <span className="ghi-chu">{hm.soConLai < 0 ? 'Đã vượt' : 'Còn lại'}</span>
                  <b className={hm.soConLai < 0 ? 'am' : ''}>{tien(Math.abs(hm.soConLai))}</b>
                </div>
                <Thuoc tyLeSuDung={hm.tyLeSuDung} nguong={hm.nguongCanhBao}
                  duKien={coDuKien ? duBao!.tyLe : 0} mau={coDuKien ? duBao!.mau || 'xanh' : undefined} />
                <dl className="bang-so">
                  <div><dt>Hạn mức</dt><dd>{tien(hm.soTienHanMuc)}</dd></div>
                  <div><dt>Đã chi</dt><dd>{tien(hm.soTienDaChi)}</dd></div>
                  <div><dt>Loại hạn mức</dt><dd>{hm.loaiHanMuc}</dd></div>
                  <div><dt>Ngưỡng cảnh báo</dt><dd>{tyLe(hm.nguongCanhBao)}</dd></div>
                </dl>
              </>
            )}
          </section>

          {duLieu?.ganDay && (
            <section className="the">
              <h2>Giao dịch gần đây</h2>
              {duLieu.ganDay.length === 0 ? <p className="ghi-chu">Chưa có giao dịch.</p> : (
                <ul className="ds-gan-day">
                  {duLieu.ganDay.map((r: any, i: number) => (
                    <li key={i} className={lopTrangThai(r.trangThai)}>
                      <div>
                        <b>{r.moTa || '(trống)'}</b>
                        <span>{ngay(r.ngayGiaoDich)}{r.trangThai === 'Hợp lệ' ? '' : ', ' + r.trangThai.toLowerCase()}</span>
                      </div>
                      <span className="so-tien">{tien(r.soTien)}</span>
                    </li>
                  ))}
                </ul>
              )}
            </section>
          )}
        </div>
      </div>
    </>
  );
}
