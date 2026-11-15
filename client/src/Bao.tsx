import { createContext, useCallback, useContext, useEffect, useRef, useState, ReactNode } from 'react';
import { useLocation } from 'react-router-dom';

export interface NoiDungBao {
  xanh?: string | null;
  vang?: string | null;
  do?: string | null;
}

interface Bao {
  noiDung: NoiDungBao;
  bao: (noiDung: NoiDungBao, choTrangSau?: boolean) => void;
  xoa: () => void;
}

const NguCanh = createContext<Bao>({ noiDung: {}, bao: () => {}, xoa: () => {} });

export function BaoProvider({ children }: { children: ReactNode }) {
  const [noiDung, datNoiDung] = useState<NoiDungBao>({});
  const giuQuaChuyenTrang = useRef(false);
  const viTri = useLocation();

  useEffect(() => {
    if (giuQuaChuyenTrang.current) {
      giuQuaChuyenTrang.current = false;
    } else {
      datNoiDung({});
    }
  }, [viTri.key]);

  const bao = useCallback((moi: NoiDungBao, choTrangSau = false) => {
    giuQuaChuyenTrang.current = choTrangSau;
    datNoiDung(moi);
  }, []);
  const xoa = useCallback(() => datNoiDung({}), []);

  return <NguCanh.Provider value={{ noiDung, bao, xoa }}>{children}</NguCanh.Provider>;
}

export function useBao(): Bao {
  return useContext(NguCanh);
}

export function HopBao() {
  const { noiDung } = useBao();
  return (
    <>
      {noiDung.xanh && <div className="hop-bao bao-xanh" role="status">{noiDung.xanh}</div>}
      {noiDung.vang && <div className="hop-bao bao-vang" role="status">{noiDung.vang}</div>}
      {noiDung.do && <div className="hop-bao bao-do" role="alert">{noiDung.do}</div>}
    </>
  );
}
