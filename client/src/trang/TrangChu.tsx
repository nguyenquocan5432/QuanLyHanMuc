import { Link } from 'react-router-dom';
import { usePhien } from '../Phien';
import { useTai } from '../useTai';
import Thuoc from '../Thuoc';
import { lopTrangThai, ngay, rongThanh, soNgayGiua, tien, tyLe } from '../dinhDang';

const BIEU_TUONG: Record<string, JSX.Element> = {
  nhap: <path d="M12 5v14M5 12h14" />,
  lichsu: <><path d="M3 12a9 9 0 1 0 3-6.7L3 8" /><path d="M3 3v5h5" /><path d="M12 7v5l3 2" /></>,
  baocao: <path d="M4 20V10M10 20V4M16 20v-7M22 20H2" />,
  thongbao: <><path d="M6 8a6 6 0 1 1 12 0c0 7 3 9 3 9H3s3-2 3-9" /><path d="M10.3 21a1.94 1.94 0 0 0 3.4 0" /></>,
  hanmuc: <><rect x="3" y="9" width="18" height="6" rx="3" /><path d="M15 6v12" /></>,
  duyet: <><path d="M9 11l3 3L22 4" /><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11" /></>,
  vipham: <><path d="M10.3 3.9 1.8 18a2 2 0 0 0 1.7 3h17a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0z" /><path d="M12 9v4M12 17h.01" /></>,
  danhmuc: <path d="M3 7h18M3 12h18M3 17h12" />,
  nguoidung: <><circle cx="9" cy="8" r="4" /><path d="M2 21a7 7 0 0 1 14 0" /><path d="M17 4a4 4 0 0 1 0 8M22 21a7 7 0 0 0-4-6.3" /></>,
};

function OChucNang({ den, ten, bieuTuong, noiBat }: { den: string; ten: string; bieuTuong: string; noiBat?: boolean }) {
  return (
    <Link className={'o-chuc-nang ' + (noiBat ? 'noi-bat' : '')} to={den}>
      <span className="bieu-tuong" aria-hidden="true">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
          {BIEU_TUONG[bieuTuong]}
        </svg>
      </span>
      <b>{ten}</b>
    </Link>
  );
}

function DongHanMuc({ r, cheDo }: { r: any; cheDo: 'nhap' | 'sua' }) {
  const lop = lopTrangThai(r.trangThai);
  const daKhoa = r.trangThai === 'Đã khóa';
  return (
    <li className={'so-dong ' + lop}>
      <div className="so-ten"><b>{r.tenDanhMuc}</b></div>
      <div className="so-thuoc">
        <div className="so-thuoc-tren">
          <span className={'nhan ' + lop}>{r.trangThai}</span>
          <b>{tyLe(r.tyLeSuDung)}</b>
        </div>
        <Thuoc tyLeSuDung={r.tyLeSuDung} nguong={r.nguongCanhBao} />
        <div className="so-thuoc-duoi">Đã chi {tien(r.soTienDaChi)} trên {tien(r.soTienHanMuc)}</div>
      </div>
      <div className="so-con">
        {r.soConLai < 0
          ? <><span>Đã vượt</span><b className="am">{tien(-r.soConLai)}</b></>
          : <><span>Còn lại</span><b>{tien(r.soConLai)}</b></>}
      </div>
      <div className="so-hanh-dong">
        {!daKhoa && cheDo === 'nhap' && <Link className="nut nut-nho" to={'/giao-dich/nhap?maDanhMuc=' + r.maDanhMuc}>Nhập</Link>}
        {!daKhoa && cheDo === 'sua' && <Link className="nut nut-nho" to={'/han-muc/' + r.maHanMuc}>Sửa</Link>}
      </div>
    </li>
  );
}

function NhipChiTieu({ hanMuc, homNay }: { hanMuc: any[]; homNay: string }) {
  const tongHanMuc = hanMuc.reduce((t, r) => t + r.soTienHanMuc, 0);
  const tongDaChi = hanMuc.reduce((t, r) => t + r.soTienDaChi, 0);
  const batDau = hanMuc.map((r) => r.ngayBatDau as string).sort()[0];
  const ketThuc = hanMuc.map((r) => r.ngayKetThuc as string).sort().reverse()[0];
  const tongNgay = soNgayGiua(batDau, ketThuc) + 1;
  const daQua = Math.max(0, Math.min(tongNgay, soNgayGiua(batDau, homNay) + 1));
  const tyLeThoiGian = tongNgay > 0 ? (daQua * 100) / tongNgay : 0;
  const tyLeChi = tongHanMuc > 0 ? (tongDaChi * 100) / tongHanMuc : 0;
  const lech = tyLeChi - tyLeThoiGian;
  const conLai = tongHanMuc - tongDaChi;

  return (
    <section className="nhip" aria-label="Nhịp chi tiêu kỳ này">
      <div className="nhip-ky">
        <h2>Kỳ {ngay(batDau).substring(0, 5)} đến {ngay(ketThuc)}</h2>
      </div>
      <div className="nhip-so-sanh">
        <div className="nhip-dong">
          <span>Thời gian đã qua</span>
          <div className="thuoc thuoc-thoi-gian" style={{ ['--ty-le' as any]: rongThanh(tyLeThoiGian) }}><div className="thuoc-day"></div></div>
          <b>{Math.round(tyLeThoiGian)}%</b>
        </div>
        <div className={'nhip-dong ' + (tyLeChi > 100 ? 'tt-da-vuot' : lech > 10 ? 'tt-sap-vuot' : '')}>
          <span>Ngân sách đã dùng</span>
          <div className="thuoc" style={{ ['--ty-le' as any]: rongThanh(tyLeChi) }}><div className="thuoc-day"></div></div>
          <b>{Math.round(tyLeChi)}%</b>
        </div>
      </div>
      <dl className="nhip-tong">
        <div><dt>Tổng hạn mức</dt><dd>{tien(tongHanMuc)}</dd></div>
        <div><dt>Đã chi</dt><dd>{tien(tongDaChi)}</dd></div>
        <div><dt>Còn lại</dt><dd className={conLai < 0 ? 'am' : ''}>{tien(conLai)}</dd></div>
      </dl>
    </section>
  );
}

export default function TrangChu() {
  const { nguoiDung } = usePhien();
  const { duLieu, loi } = useTai('/api/trang-chu');
  if (!nguoiDung) return null;
  const nd = nguoiDung;

  const hanMuc: any[] | null = duLieu?.hanMucCuaToi ?? null;
  const hanMucSinhVien: any[] | null = duLieu?.hanMucSinhVien ?? null;

  const nhom: { ten: string; dong: any[] }[] = [];
  for (const r of hanMucSinhVien ?? []) {
    let n = nhom.find((x) => x.ten === r.sinhVien);
    if (!n) { n = { ten: r.sinhVien, dong: [] }; nhom.push(n); }
    n.dong.push(r);
  }

  return (
    <>
      <div className="chao">
        <div><h1>Xin chào, {nd.hoTen}</h1></div>
        {nd.duocNhapGiaoDich
          ? <Link className="nut nut-chinh" to="/giao-dich/nhap">Nhập giao dịch</Link>
          : nd.duocQuanLyHanMuc && <Link className="nut nut-chinh" to="/han-muc/tao">Thêm hạn mức</Link>}
      </div>

      {loi && <div className="hop-bao bao-do" role="alert">{loi}</div>}

      {nd.duocQuanLyHanMuc && duLieu && duLieu.soChoDuyet > 0 && (
        <div className="hop-bao bao-vang">
          <span><b>{duLieu.soChoDuyet}</b> giao dịch đang chờ duyệt</span>
          <Link to="/ngoai-le">Duyệt</Link>
        </div>
      )}

      {hanMuc && hanMuc.length > 0 && (
        <>
          <NhipChiTieu hanMuc={hanMuc} homNay={duLieu.homNay} />
          <section className="the so-han-muc">
            <div className="the-dau"><h2>Hạn mức từng danh mục</h2></div>
            <ul className="so-ds">
              {hanMuc.map((r) => <DongHanMuc key={r.maHanMuc} r={r} cheDo="nhap" />)}
            </ul>
          </section>
        </>
      )}
      {hanMuc && hanMuc.length === 0 && (
        <section className="the">
          <h2>Chưa có hạn mức cho kỳ này</h2>
          {nd.duocQuanLyHanMuc && <Link className="nut nut-chinh" to="/han-muc/tao">Thêm hạn mức</Link>}
        </section>
      )}

      {hanMucSinhVien && (
        <section className="the so-han-muc">
          <div className="the-dau">
            <h2>Sinh viên của bạn trong kỳ này</h2>
            <Link to="/han-muc">Xem tất cả hạn mức</Link>
          </div>
          {hanMucSinhVien.length === 0 && <p className="so-trong">Chưa có hạn mức trong kỳ này.</p>}
          {nhom.map((n) => (
            <div className="so-sinh-vien" key={n.ten}>
              <div className="so-sinh-vien-dau">
                <h3>{n.ten}</h3>
                <span className="ghi-chu">
                  Đã chi {tien(n.dong.reduce((t, r) => t + r.soTienDaChi, 0))} trên {tien(n.dong.reduce((t, r) => t + r.soTienHanMuc, 0))}
                </span>
              </div>
              <ul className="so-ds">
                {n.dong.map((r) => <DongHanMuc key={r.maHanMuc} r={r} cheDo="sua" />)}
              </ul>
            </div>
          ))}
        </section>
      )}

      <h2>Chức năng</h2>
      <nav className="luoi-o" aria-label="Chức năng">
        {nd.duocNhapGiaoDich && <OChucNang den="/giao-dich/nhap" ten="Nhập giao dịch" bieuTuong="nhap" noiBat />}
        {nd.duocQuanLyHanMuc && (
          <>
            <OChucNang den="/ngoai-le" ten="Duyệt ngoại lệ" bieuTuong="duyet" noiBat={!nd.duocNhapGiaoDich} />
            <OChucNang den="/han-muc" ten="Quản lý hạn mức" bieuTuong="hanmuc" />
          </>
        )}
        {nd.duocXemGiaoDich && (
          <>
            <OChucNang den="/giao-dich" ten="Lịch sử giao dịch" bieuTuong="lichsu" />
            <OChucNang den="/bao-cao" ten="Báo cáo chi tiêu" bieuTuong="baocao" />
          </>
        )}
        {nd.duocQuanLyHanMuc && <OChucNang den="/bao-cao?tab=vipham" ten="Báo cáo vi phạm" bieuTuong="vipham" />}
        {!nd.laQuanTri && <OChucNang den="/thong-bao" ten="Thông báo" bieuTuong="thongbao" />}
        {nd.laQuanTri && (
          <>
            <OChucNang den="/danh-muc" ten="Danh mục chi tiêu" bieuTuong="danhmuc" noiBat />
            <OChucNang den="/nguoi-dung" ten="Người dùng" bieuTuong="nguoidung" />
          </>
        )}
      </nav>
    </>
  );
}
