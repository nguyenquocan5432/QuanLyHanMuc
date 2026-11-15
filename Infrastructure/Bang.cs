using System.Data;

namespace QuanLyHanMucWeb.Infrastructure
{
    public static class Bang
    {
        public static List<Dictionary<string, object>> DanhSach(DataTable bang)
        {
            List<Dictionary<string, object>> ds = new List<Dictionary<string, object>>();
            if (bang == null) return ds;
            foreach (DataRow dong in bang.Rows)
                ds.Add(Dong(dong));
            return ds;
        }

        public static Dictionary<string, object> Dong(DataRow dong)
        {
            if (dong == null) return null;
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (DataColumn cot in dong.Table.Columns)
            {
                object giaTri = dong[cot];
                d[TenKhoa(cot.ColumnName)] = giaTri == DBNull.Value ? null : giaTri;
            }
            return d;
        }

        private static string TenKhoa(string tenCot)
        {
            return char.ToLowerInvariant(tenCot[0]) + tenCot.Substring(1);
        }
    }
}
