using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace QuanLyCaoOc.Data
{
    /// <summary>
    /// Lop Data: quan ly ket noi va thuc thi SQL co tham so.
    /// Toan bo nghiep vu va validate nam o lop Services.
    /// </summary>
    public static class Db
    {
        private static string _connectionString;

        /// <summary>
        /// Connection string lay tu App.config. Co the doi tu FrmKetNoi.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_connectionString)) return _connectionString;
                ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["QuanLyCaoOcDb"];
                if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
                    return cs.ConnectionString;
                return @"Data Source=.;Initial Catalog=QuanLyCaoOc;User ID=sa;Password=25022005;MultipleActiveResultSets=True;";
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
                message = "Kết nối thành công! CSDL hiện tại: " + Convert.ToString(v);
                return true;
            }
            catch (Exception ex)
            {
                message = "Không kết nối được: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Chay file script .sql duoc chia thanh cac batch boi tu khoa GO.
        /// </summary>
        public static bool ExecuteScriptFile(string filePath, out string message)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    message = "Không tìm thấy file: " + filePath;
                    return false;
                }

                string script = File.ReadAllText(filePath);
                string[] batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                using (var conn = OpenConnection())
                {
                    foreach (var batch in batches)
                    {
                        string sql = batch.Trim();
                        if (string.IsNullOrWhiteSpace(sql)) continue;
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.CommandTimeout = 120;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                message = "Chạy script thành công: " + Path.GetFileName(filePath);
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi khi chạy script " + Path.GetFileName(filePath) + ": " + ex.Message;
                return false;
            }
        }
    }
}
