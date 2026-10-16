using System.Data;
using System.Data.SqlClient;

namespace QuanLyBenhAn.Data
{
    /// <summary>DAL: chi lo ket noi + thuc thi SQL co tham so. Nghiep vu nam o Services.</summary>
    public static class Db
    {
        // Sua cho dung SQL Server cua may truoc khi chay.
        public static string ConnectionString =
            @"Data Source=ADMIN-PC\SQLEXPRESS;Initial Catalog=QuanLyBenhAn;User ID=sa;Password=25022005;";

        public static DataTable ExecuteQuery(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
