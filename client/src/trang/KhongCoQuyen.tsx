import { Link } from 'react-router-dom';

export default function KhongCoQuyen() {
  return (
    <div className="the">
      <h1>Không có quyền truy cập</h1>
      <p>Tài khoản này không dùng được chức năng này.</p>
      <Link className="nut" to="/">Về trang chủ</Link>
    </div>
  );
}
