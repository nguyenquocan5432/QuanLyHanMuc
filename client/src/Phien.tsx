import { createContext, useCallback, useContext, useEffect, useState, ReactNode } from 'react';
import { api } from './api';

export interface NguoiDung {
  ma: number;
  hoTen: string;
  vaiTro: string;
  tenVaiTro: string;
  duocNhapGiaoDich: boolean;
  duocXemGiaoDich: boolean;
  duocQuanLyHanMuc: boolean;
  laCapNganSach: boolean;
  laQuanTri: boolean;
}

interface Phien {
  dangTai: boolean;
  cheDo: 'Local' | 'Oidc';
  nguoiDung: NguoiDung | null;
  soChuaDoc: number;
  loi: string | null;
  taiLai: () => Promise<void>;
}

const NguCanh = createContext<Phien>({
  dangTai: true, cheDo: 'Local', nguoiDung: null, soChuaDoc: 0, loi: null, taiLai: async () => {},
});

export function PhienProvider({ children }: { children: ReactNode }) {
  const [trangThai, datTrangThai] = useState<Omit<Phien, 'taiLai'>>({
    dangTai: true, cheDo: 'Local', nguoiDung: null, soChuaDoc: 0, loi: null,
  });

  const taiLai = useCallback(async () => {
    try {
      const p = await api.lay('/api/phien');
      datTrangThai({ dangTai: false, cheDo: p.cheDo, nguoiDung: p.nguoiDung, soChuaDoc: p.soChuaDoc, loi: null });
    } catch (e) {
      datTrangThai((cu) => ({ ...cu, dangTai: false, loi: (e as Error).message }));
    }
  }, []);

  useEffect(() => { taiLai(); }, [taiLai]);

  return <NguCanh.Provider value={{ ...trangThai, taiLai }}>{children}</NguCanh.Provider>;
}

export function usePhien(): Phien {
  return useContext(NguCanh);
}
