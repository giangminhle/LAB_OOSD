using System;
using System.Data.SqlClient;

namespace QuanLyCaoOc.Services
{
    /// <summary>Sinh so chung tu dang PREFIX + Nam + "-" + 4 so, VD TT2026-0009.</summary>
    internal static class MaSo
    {
        public static string Sinh(SqlConnection conn, SqlTransaction tran, string bang, string cot,
            string prefix, int nam)
        {
            // bang/cot la hang so trong code, khong lay tu nguoi dung
            string dau = prefix + nam + "-";
            string sql = "SELECT ISNULL(MAX(CAST(RIGHT(" + cot + ", 4) AS INT)), 0) FROM dbo." + bang +
                         " WHERE " + cot + " LIKE @dau";
            using (var cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@dau", dau + "%");
                int so = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                return dau + so.ToString("0000");
            }
        }

        /// <summary>Ma tang dan khong theo nam, VD NV0007, TX0005.</summary>
        public static string SinhMa(SqlConnection conn, SqlTransaction tran, string bang, string cot, string prefix)
        {
            string sql = "SELECT ISNULL(MAX(CAST(SUBSTRING(" + cot + ", " + (prefix.Length + 1) +
                         ", 10) AS INT)), 0) FROM dbo." + bang;
            using (var cmd = new SqlCommand(sql, conn, tran))
            {
                int so = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                return prefix + so.ToString("0000");
            }
        }
    }
}
