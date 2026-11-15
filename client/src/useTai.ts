import { useCallback, useEffect, useState } from 'react';
import { api } from './api';

export function useTai<T = any>(duongDan: string | null) {
  const [duLieu, datDuLieu] = useState<T | null>(null);
  const [loi, datLoi] = useState<string | null>(null);
  const [lan, datLan] = useState(0);

  useEffect(() => {
    if (!duongDan) return;
    let conDung = true;
    datLoi(null);
    api.lay<T>(duongDan)
      .then((d) => { if (conDung) datDuLieu(d); })
      .catch((e) => { if (conDung) datLoi((e as Error).message); });
    return () => { conDung = false; };
  }, [duongDan, lan]);

  const taiLai = useCallback(() => datLan((n) => n + 1), []);
  return { duLieu, loi, taiLai };
}
