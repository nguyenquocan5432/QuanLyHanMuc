export class LoiApi extends Error {
  constructor(message: string, public maTrangThai: number) {
    super(message);
  }
}

function docCookie(ten: string): string {
  const dong = document.cookie.split('; ').find((c) => c.startsWith(ten + '='));
  return dong ? decodeURIComponent(dong.substring(ten.length + 1)) : '';
}

export function maChongGiaMao(): string {
  return docCookie('XSRF-TOKEN');
}

async function goi<T>(phuongThuc: string, duongDan: string, noiDung?: unknown): Promise<T> {
  const headers: Record<string, string> = { Accept: 'application/json' };
  if (phuongThuc !== 'GET') {
    headers['Content-Type'] = 'application/json';
    headers['X-XSRF-TOKEN'] = maChongGiaMao();
  }
  const traLoi = await fetch(duongDan, {
    method: phuongThuc,
    headers,
    credentials: 'same-origin',
    body: phuongThuc === 'GET' ? undefined : JSON.stringify(noiDung ?? {}),
  });

  if (traLoi.status === 401) {
    window.dispatchEvent(new Event('chua-dang-nhap'));
    throw new LoiApi('Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.', 401);
  }
  if (traLoi.status === 403) {
    window.dispatchEvent(new Event('khong-co-quyen'));
    throw new LoiApi('Tài khoản này không dùng được chức năng này.', 403);
  }

  const chu = await traLoi.text();
  let duLieu: any = null;
  try {
    duLieu = chu ? JSON.parse(chu) : null;
  } catch {
    duLieu = null;
  }
  if (!traLoi.ok) {
    throw new LoiApi(duLieu?.loi ?? 'Yêu cầu không hợp lệ (mã ' + traLoi.status + ').', traLoi.status);
  }
  return duLieu as T;
}

export function thamSo(giaTri: Record<string, string | number | null | undefined>): string {
  const p = new URLSearchParams();
  for (const [k, v] of Object.entries(giaTri)) {
    if (v !== null && v !== undefined && v !== '') p.set(k, String(v));
  }
  const s = p.toString();
  return s ? '?' + s : '';
}

export const api = {
  lay: <T = any>(duongDan: string) => goi<T>('GET', duongDan),
  gui: <T = any>(duongDan: string, noiDung?: unknown) => goi<T>('POST', duongDan, noiDung),
  sua: <T = any>(duongDan: string, noiDung?: unknown) => goi<T>('PUT', duongDan, noiDung),
};
