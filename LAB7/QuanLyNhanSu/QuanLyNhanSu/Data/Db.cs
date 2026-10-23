using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyNhanSu.Data
{
    /// <summary>
    /// Lop Data: chi lo ket noi va thuc thi SQL co tham so.
    /// Toan bo nghiep vu + validate nam o lop Services.
    /// </summary>
    public static class Db
    {
        private static string _connectionString;

        /// <summary>
        /// Connection string lay tu App.config. Co the doi tam thoi tu FrmKetNoi.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_connectionString)) return _connectionString;
                ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["QuanLyNhanSuDb"];
                if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
                    return cs.ConnectionString;
                return @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyNhanSu;User ID=sa;Password=25022005;";
            }
            set { _connectionString = value; }
        }

        public static SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static DataTable Query(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static bool TestConnection(out string message)
        {
            try
            {
                object v = Scalar("SELECT DB_NAME()");
                message = "Ket noi thanh cong. CSDL: " + Convert.ToString(v);
                return true;
            }
            catch (Exception ex)
            {
                message = "Khong ket noi duoc: " + ex.Message;
                return false;
            }
        }
    }
}
