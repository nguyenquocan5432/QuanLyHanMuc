import { CSSProperties } from 'react';
import { rongThanh, tyLe } from './dinhDang';

export default function Thuoc(props: {
  tyLeSuDung: number;
  nguong?: number;
  duKien?: number | null;
  mau?: string;
  lop?: string;
}) {
  const kieu = {
    '--ty-le': rongThanh(props.tyLeSuDung),
    ...(props.nguong !== undefined ? { '--nguong': rongThanh(props.nguong) } : {}),
    ...(props.duKien !== undefined ? { '--du-kien': rongThanh(props.duKien ?? 0) } : {}),
  } as CSSProperties;
  const moTa = 'Đã dùng ' + tyLe(props.tyLeSuDung) + ' hạn mức' +
    (props.nguong !== undefined ? ', ngưỡng cảnh báo ' + tyLe(props.nguong) : '');
  return (
    <div className={'thuoc ' + (props.lop ?? '')} style={kieu} role="img" aria-label={moTa} data-mau={props.mau || undefined}>
      {props.duKien !== undefined && <div className="thuoc-du-kien"></div>}
      <div className="thuoc-day"></div>
      {props.nguong !== undefined && <span className="thuoc-moc"></span>}
    </div>
  );
}
