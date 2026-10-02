using System.Data;
using System.Data.SqlClient;

namespace EShopping.Data
{
    /// <summary>
    /// Lớp truy cập dữ liệu (DAL). Chỉ lo kết nối + thực thi SQL, KHÔNG chứa nghiệp vụ.
    /// Luôn dùng SqlParameter để chống SQL Injection.
    /// </summary>
    public static class Db
    {
        // Đã sửa lại đúng tên Server, User (sa) và Password (25022005) của máy m
        public static string ConnectionString =
            @"Data Source=ADMIN-PC\SQLEXPRESS;Initial Catalog=eShoppingDb;User ID=sa;Password=25022005;";

        private static SqlCommand CreateCommand(SqlConnection conn, string sql, SqlParameter[] ps)
        {
            var cmd = new SqlCommand(sql, conn);
            if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
            return cmd;
        }

        /// <summary>SELECT -> DataTable</summary>
        public static DataTable ExecuteQuery(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = CreateCommand(conn, sql, ps))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>INSERT/UPDATE/DELETE -> số dòng bị ảnh hưởng</summary>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = CreateCommand(conn, sql, ps))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Trả về giá trị ô đầu tiên (COUNT, OUTPUT INSERTED.Id...)</summary>
        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = CreateCommand(conn, sql, ps))
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}