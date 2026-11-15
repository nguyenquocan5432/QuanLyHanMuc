using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyHanMucWeb.Data
{
    public static class KetNoi
    {
        private static string _chuoiKetNoi;

        public static void CauHinh(string chuoiKetNoi)
        {
            _chuoiKetNoi = chuoiKetNoi;
        }

        public static SqlConnection TaoKetNoi()
        {
            return new SqlConnection(_chuoiKetNoi);
        }

        public static DataTable GoiProcedure(string tenProcedure, params SqlParameter[] thamSo)
        {
            using (SqlConnection kn = TaoKetNoi())
            using (SqlCommand lenh = new SqlCommand(tenProcedure, kn))
            {
                lenh.CommandType = CommandType.StoredProcedure;
                lenh.Parameters.AddRange(thamSo);

                DataTable bang = new DataTable();
                kn.Open();
                using (SqlDataReader doc = lenh.ExecuteReader())
                {
                    bang.Load(doc);
                }
                return bang;
            }
        }

        public static DataTable TruyVan(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection kn = TaoKetNoi())
            using (SqlCommand lenh = new SqlCommand(sql, kn))
            {
                lenh.Parameters.AddRange(thamSo);

                DataTable bang = new DataTable();
                kn.Open();
                using (SqlDataReader doc = lenh.ExecuteReader())
                {
                    bang.Load(doc);
                }
                return bang;
            }
        }

        public static object LayMotGiaTri(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection kn = TaoKetNoi())
            using (SqlCommand lenh = new SqlCommand(sql, kn))
            {
                lenh.Parameters.AddRange(thamSo);
                kn.Open();
                return lenh.ExecuteScalar();
            }
        }

        public static int ThucThi(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection kn = TaoKetNoi())
            using (SqlCommand lenh = new SqlCommand(sql, kn))
            {
                lenh.Parameters.AddRange(thamSo);
                kn.Open();
                return lenh.ExecuteNonQuery();
            }
        }

        public static SqlParameter SoNguyen(string ten, int? giaTri)
        {
            return new SqlParameter(ten, SqlDbType.Int) { Value = (object)giaTri ?? DBNull.Value };
        }

        public static SqlParameter SoTien(string ten, decimal? giaTri)
        {
            return new SqlParameter(ten, SqlDbType.Decimal)
            {
                Precision = 18,
                Scale = 0,
                Value = (object)giaTri ?? DBNull.Value
            };
        }

        public static SqlParameter TyLe(string ten, decimal giaTri)
        {
            return new SqlParameter(ten, SqlDbType.Decimal) { Precision = 5, Scale = 2, Value = giaTri };
        }

        public static SqlParameter Chuoi(string ten, string giaTri, int doDai)
        {
            return new SqlParameter(ten, SqlDbType.NVarChar, doDai) { Value = (object)giaTri ?? DBNull.Value };
        }

        public static SqlParameter ChuoiKhongDau(string ten, string giaTri, int doDai)
        {
            return new SqlParameter(ten, SqlDbType.VarChar, doDai) { Value = (object)giaTri ?? DBNull.Value };
        }

        public static SqlParameter Ngay(string ten, DateTime? giaTri)
        {
            return new SqlParameter(ten, SqlDbType.Date)
            {
                Value = giaTri.HasValue ? (object)giaTri.Value.Date : DBNull.Value
            };
        }

        public static SqlParameter DungSai(string ten, bool giaTri)
        {
            return new SqlParameter(ten, SqlDbType.Bit) { Value = giaTri };
        }

        public static SqlParameter PhienBan(string ten, byte[] giaTri)
        {
            return new SqlParameter(ten, SqlDbType.Binary, 8) { Value = (object)giaTri ?? DBNull.Value };
        }
    }
}
