using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>Cham cong theo ke hoach tuan; moi nhan vien 1 ban ghi/ngay.</summary>
    public class ChamCongService
    {
        public static readonly string[] TrangThaiHopLe = { "DiLam", "DiTre", "Vang", "CoPhep" };

        public string Validate(ChamCongInput x)
        {
            if (x == null) return "Du lieu cham cong khong hop le.";
            if (x.MaNV <= 0) return "Vui long chon nhan vien.";
            if (x.Ngay == DateTime.MinValue) return "Ngay cham cong khong hop le.";
            if (Array.IndexOf(TrangThaiHopLe, x.TrangThai) < 0)
                return "Trang thai chi nhan DiLam, DiTre, Vang hoac CoPhep.";
            if (string.IsNullOrWhiteSpace(x.CaLam)) return "Vui long chon ca lam.";

            bool coMat = x.TrangThai == "DiLam" || x.TrangThai == "DiTre";
            if (coMat)
            {
                if (!x.GioVao.HasValue || !x.GioRa.HasValue)
                    return "Khi co mat phai nhap gio vao va gio ra.";
                if (x.GioRa.Value <= x.GioVao.Value)
                    return "Gio ra phai sau gio vao.";
                decimal gio = TinhSoGioLam(x.GioVao.Value, x.GioRa.Value);
                if (gio <= 0 || gio > 16) return "So gio lam phai trong khoang 0-16.";
            }
            else
            {
                if (x.GioVao.HasValue || x.GioRa.HasValue)
                    return "Vang/co phep khong duoc nhap gio vao, gio ra.";
            }
            return null;
        }

        /// <summary>Gio lam thuc te = gio ra - gio vao - 1 gio nghi trua neu ca > 6 gio.</summary>
        public static decimal TinhSoGioLam(TimeSpan vao, TimeSpan ra)
        {
            double h = (ra - vao).TotalHours;
            if (h > 6) h -= 1;
            return Math.Round((decimal)h, 2, MidpointRounding.AwayFromZero);
        }

        public static bool LaTrangThaiCoCong(string trangThai)
        {
            return trangThai == "DiLam" || trangThai == "DiTre";
        }

        public ServiceResult Luu(ChamCongInput x)
        {
            string err = Validate(x);
            if (err != null) return ServiceResult.Fail(err);

            decimal soGio = (x.TrangThai == "DiLam" || x.TrangThai == "DiTre")
                ? TinhSoGioLam(x.GioVao.Value, x.GioRa.Value) : 0m;

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maCC = 0;
                        using (var cmd = new SqlCommand("SELECT MaChamCong FROM dbo.ChamCong WHERE MaNV=@nv AND Ngay=@ngay", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@nv", x.MaNV);
                            cmd.Parameters.AddWithValue("@ngay", x.Ngay.Date);
                            object v = cmd.ExecuteScalar();
                            if (v != null) maCC = Convert.ToInt32(v);
                        }

                        if (maCC == 0)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.ChamCong(MaNV, Ngay, MaCTKH, CaLam, GioVao, GioRa, SoGioLam, TrangThai, GhiChu)
VALUES (@nv, @ngay, @ctkh, @ca, @vao, @ra, @gio, @tt, @ghichu);
SELECT SCOPE_IDENTITY();", conn, tran))
                            {
                                AddParams(cmd, x, soGio);
                                maCC = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else
                        {
                            using (var cmd = new SqlCommand(@"
UPDATE dbo.ChamCong SET MaCTKH=@ctkh, CaLam=@ca, GioVao=@vao, GioRa=@ra,
       SoGioLam=@gio, TrangThai=@tt, GhiChu=@ghichu WHERE MaChamCong=@ma", conn, tran))
                            {
                                AddParams(cmd, x, soGio);
                                cmd.Parameters.AddWithValue("@ma", maCC);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Da luu cham cong ngay " + x.Ngay.ToString("dd/MM/yyyy")
                            + " (" + x.TrangThai + ").", maCC);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        private static void AddParams(SqlCommand cmd, ChamCongInput x, decimal soGio)
        {
            cmd.Parameters.AddWithValue("@nv", x.MaNV);
            cmd.Parameters.AddWithValue("@ngay", x.Ngay.Date);
            cmd.Parameters.AddWithValue("@ctkh", x.MaCTKH.HasValue ? (object)x.MaCTKH.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ca", x.CaLam);
            cmd.Parameters.AddWithValue("@vao", x.GioVao.HasValue ? (object)x.GioVao.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ra", x.GioRa.HasValue ? (object)x.GioRa.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@gio", soGio);
            cmd.Parameters.AddWithValue("@tt", x.TrangThai);
            cmd.Parameters.AddWithValue("@ghichu", (object)x.GhiChu ?? DBNull.Value);
        }

        public DataTable LayTheoNgay(DateTime ngay)
        {
            return new KeHoachService().LayPhanCongTheoNgay(ngay);
        }

        public DataTable DanhSachThang(int thang, int nam)
        {
            const string sql = @"
SELECT cc.MaChamCong, nv.MaNVCode, nv.HoTen, cc.Ngay, cc.CaLam,
       cc.GioVao, cc.GioRa, cc.SoGioLam, cc.TrangThai, cc.GhiChu
FROM dbo.ChamCong cc JOIN dbo.NhanVien nv ON nv.MaNV=cc.MaNV
WHERE MONTH(cc.Ngay)=@thang AND YEAR(cc.Ngay)=@nam
ORDER BY cc.Ngay, nv.MaNVCode";
            return Db.Query(sql, new SqlParameter("@thang", thang), new SqlParameter("@nam", nam));
        }

        public int DemNgayCong(int maNV, int thang, int nam)
        {
            object v = Db.Scalar(@"
SELECT COUNT(*) FROM dbo.ChamCong
WHERE MaNV=@nv AND MONTH(Ngay)=@thang AND YEAR(Ngay)=@nam
  AND TrangThai IN (N'DiLam', N'DiTre')",
                new SqlParameter("@nv", maNV), new SqlParameter("@thang", thang), new SqlParameter("@nam", nam));
            return v == null ? 0 : Convert.ToInt32(v);
        }
    }
}
