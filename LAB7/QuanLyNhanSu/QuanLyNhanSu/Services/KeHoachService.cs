using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>Ke hoach lam viec tuan theo phong ban, ca lam va lich phu troi.</summary>
    public class KeHoachService
    {
        public static readonly string[] CaLamHopLe = { "Sang", "Chieu", "Toi" };

        /// <summary>Ngay Thu 2 cua tuan ISO (tuan 1 la tuan chua ngay 4/1).</summary>
        public static DateTime NgayDauTuan(int tuan, int nam)
        {
            var ngay4Thang1 = new DateTime(nam, 1, 4);
            int thu = ((int)ngay4Thang1.DayOfWeek + 6) % 7; // Thu 2 = 0
            DateTime tuan1 = ngay4Thang1.AddDays(-thu);
            return tuan1.AddDays((tuan - 1) * 7);
        }

        public static DateTime NgayCuoiTuan(int tuan, int nam)
        {
            return NgayDauTuan(tuan, nam).AddDays(6);
        }

        public string Validate(KeHoachInput x)
        {
            if (x == null) return "Du lieu ke hoach khong hop le.";
            if (x.MaPB <= 0) return "Vui long chon phong ban.";
            if (x.TuanSo < 1 || x.TuanSo > 53) return "Tuan phai trong khoang 1-53.";
            if (x.Nam < 2000 || x.Nam > 2100) return "Nam khong hop le.";
            if (string.IsNullOrWhiteSpace(x.NguoiLap)) return "Vui long nhap nguoi lap ke hoach.";
            if (x.ChiTiet == null || x.ChiTiet.Count == 0)
                return "Ke hoach phai co it nhat mot phan cong.";

            DateTime dau = NgayDauTuan(x.TuanSo, x.Nam);
            DateTime cuoi = dau.AddDays(6);
            var daCo = new HashSet<string>();
            foreach (ChiTietKeHoachInput ct in x.ChiTiet)
            {
                if (ct.MaNV <= 0) return "Dong phan cong thieu nhan vien.";
                if (ct.Ngay.Date < dau.Date || ct.Ngay.Date > cuoi.Date)
                    return "Ngay " + ct.Ngay.ToString("dd/MM/yyyy") + " khong thuoc tuan "
                        + x.TuanSo + "/" + x.Nam + " (" + dau.ToString("dd/MM/yyyy")
                        + " - " + cuoi.ToString("dd/MM/yyyy") + ").";
                if (Array.IndexOf(CaLamHopLe, ct.CaLam) < 0)
                    return "Ca lam chi nhan Sang, Chieu hoac Toi.";
                string key = ct.MaNV + "|" + ct.Ngay.ToString("yyyyMMdd");
                if (!daCo.Add(key))
                    return "Nhan vien bi phan cong trung ngay " + ct.Ngay.ToString("dd/MM/yyyy") + ".";
            }
            return null;
        }

        public ServiceResult Luu(KeHoachInput x)
        {
            string err = Validate(x);
            if (err != null) return ServiceResult.Fail(err);

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        var check = new SqlCommand(@"SELECT TOP 1 MaKH, TrangThai FROM dbo.KeHoachTuan
                            WHERE MaPB=@pb AND TuanSo=@tuan AND Nam=@nam", conn, tran);
                        check.Parameters.AddWithValue("@pb", x.MaPB);
                        check.Parameters.AddWithValue("@tuan", x.TuanSo);
                        check.Parameters.AddWithValue("@nam", x.Nam);

                        int maKH = 0;
                        string trangThai = null;
                        using (SqlDataReader rd = check.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                maKH = rd.GetInt32(0);
                                trangThai = rd.GetString(1);
                            }
                        }
                        if (trangThai == "DaDuyet")
                            return ServiceResult.Fail("Ke hoach nay da duyet, khong duoc sua. Hay tao tuan khac.");

                        // Kiem tra nhan vien thuoc phong ban.
                        var hopLe = new HashSet<int>();
                        using (var cmd = new SqlCommand(@"SELECT MaNV FROM dbo.NhanVien
                            WHERE MaPB=@pb AND TrangThai IN (N'ThuViec', N'DangLam')", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@pb", x.MaPB);
                            using (SqlDataReader rd = cmd.ExecuteReader())
                                while (rd.Read()) hopLe.Add(rd.GetInt32(0));
                        }
                        foreach (ChiTietKeHoachInput ct in x.ChiTiet)
                            if (!hopLe.Contains(ct.MaNV))
                                return ServiceResult.Fail("Nhan vien MaNV=" + ct.MaNV
                                    + " khong thuoc phong ban da chon.");

                        if (maKH == 0)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.KeHoachTuan(MaPB, TuanSo, Nam, NgayLap, NguoiLap, TrangThai)
VALUES (@pb, @tuan, @nam, @ngay, @nguoi, N'Nhap');
SELECT SCOPE_IDENTITY();", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@pb", x.MaPB);
                                cmd.Parameters.AddWithValue("@tuan", x.TuanSo);
                                cmd.Parameters.AddWithValue("@nam", x.Nam);
                                cmd.Parameters.AddWithValue("@ngay", DateTime.Today);
                                cmd.Parameters.AddWithValue("@nguoi", x.NguoiLap.Trim());
                                maKH = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else
                        {
                            using (var cmd = new SqlCommand(@"UPDATE dbo.KeHoachTuan
                                SET NgayLap=@ngay, NguoiLap=@nguoi WHERE MaKH=@kh", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@ngay", DateTime.Today);
                                cmd.Parameters.AddWithValue("@nguoi", x.NguoiLap.Trim());
                                cmd.Parameters.AddWithValue("@kh", maKH);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("DELETE dbo.ChiTietKeHoach WHERE MaKH=@kh", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@kh", maKH);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        foreach (ChiTietKeHoachInput ct in x.ChiTiet)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.ChiTietKeHoach(MaKH, MaNV, Ngay, CaLam, LaPhuTroi, GhiChu)
VALUES (@kh, @nv, @ngay, @ca, @pt, @ghichu)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@kh", maKH);
                                cmd.Parameters.AddWithValue("@nv", ct.MaNV);
                                cmd.Parameters.AddWithValue("@ngay", ct.Ngay.Date);
                                cmd.Parameters.AddWithValue("@ca", ct.CaLam);
                                cmd.Parameters.AddWithValue("@pt", ct.LaPhuTroi);
                                cmd.Parameters.AddWithValue("@ghichu", (object)ct.GhiChu ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Da luu ke hoach tuan " + x.TuanSo + "/" + x.Nam
                            + " (" + x.ChiTiet.Count + " phan cong).", maKH);
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

        public ServiceResult Duyet(int maKH)
        {
            if (maKH <= 0) return ServiceResult.Fail("Chua chon ke hoach.");
            int n = Db.Execute(@"UPDATE dbo.KeHoachTuan SET TrangThai=N'DaDuyet'
                                 WHERE MaKH=@kh AND EXISTS
                                 (SELECT 1 FROM dbo.ChiTietKeHoach ct WHERE ct.MaKH=@kh)",
                new SqlParameter("@kh", maKH));
            if (n == 0) return ServiceResult.Fail("Khong tim thay ke hoach hoac ke hoach chua co phan cong.");
            return ServiceResult.Ok("Da duyet ke hoach. Ban cham cong co the theo doi.");
        }

        public DataTable DanhSachKeHoach()
        {
            const string sql = @"
SELECT k.MaKH, k.MaPB, pb.TenPB, k.TuanSo, k.Nam, k.NgayLap, k.NguoiLap, k.TrangThai,
       (SELECT COUNT(*) FROM dbo.ChiTietKeHoach ct WHERE ct.MaKH=k.MaKH) AS SoPhanCong
FROM dbo.KeHoachTuan k JOIN dbo.PhongBan pb ON pb.MaPB=k.MaPB
ORDER BY k.Nam DESC, k.TuanSo DESC";
            return Db.Query(sql);
        }

        public DataTable LayChiTiet(int maKH)
        {
            const string sql = @"
SELECT ct.MaCTKH, ct.MaKH, nv.MaNV, nv.MaNVCode, nv.HoTen, ct.Ngay, ct.CaLam, ct.LaPhuTroi, ct.GhiChu
FROM dbo.ChiTietKeHoach ct JOIN dbo.NhanVien nv ON nv.MaNV=ct.MaNV
WHERE ct.MaKH=@kh ORDER BY ct.Ngay, nv.MaNVCode";
            return Db.Query(sql, new SqlParameter("@kh", maKH));
        }

        public DataTable LayKeHoachTheoTuan(int maPB, int tuan, int nam)
        {
            const string sql = @"
SELECT TOP 1 MaKH, MaPB, TuanSo, Nam, NgayLap, NguoiLap, TrangThai
FROM dbo.KeHoachTuan WHERE MaPB=@pb AND TuanSo=@tuan AND Nam=@nam";
            return Db.Query(sql, new SqlParameter("@pb", maPB),
                new SqlParameter("@tuan", tuan), new SqlParameter("@nam", nam));
        }

        /// <summary>Danh sach nhan vien kem phan cong va cham cong thuc te theo ngay.</summary>
        public DataTable LayPhanCongTheoNgay(DateTime ngay)
        {
            const string sql = @"
SELECT ct.MaCTKH, nv.MaNV, nv.MaNVCode, nv.HoTen, pb.TenPB,
       ct.CaLam AS CaKeHoach, ct.LaPhuTroi,
       cc.MaChamCong, cc.GioVao, cc.GioRa, cc.TrangThai AS TrangThaiChamCong, cc.GhiChu
FROM dbo.ChiTietKeHoach ct
JOIN dbo.KeHoachTuan k ON k.MaKH=ct.MaKH
JOIN dbo.NhanVien nv ON nv.MaNV=ct.MaNV
JOIN dbo.PhongBan pb ON pb.MaPB=nv.MaPB
LEFT JOIN dbo.ChamCong cc ON cc.MaNV=ct.MaNV AND cc.Ngay=ct.Ngay
WHERE ct.Ngay=@ngay AND k.TrangThai=N'DaDuyet'
ORDER BY pb.TenPB, nv.MaNVCode";
            return Db.Query(sql, new SqlParameter("@ngay", ngay.Date));
        }

        public DateTime? NgayKeHoachGanNhat()
        {
            object v = Db.Scalar(@"
SELECT MAX(ct.Ngay) FROM dbo.ChiTietKeHoach ct
JOIN dbo.KeHoachTuan k ON k.MaKH=ct.MaKH
WHERE k.TrangThai=N'DaDuyet' AND ct.Ngay <= CONVERT(DATE, GETDATE())");
            if (v == null || v == DBNull.Value) return null;
            return Convert.ToDateTime(v);
        }

        public DataTable DanhSachNhanVienTheoPhong(int maPB)
        {
            const string sql = @"
SELECT MaNV, MaNVCode, HoTen FROM dbo.NhanVien
WHERE MaPB=@pb AND TrangThai IN (N'ThuViec', N'DangLam') ORDER BY MaNVCode";
            return Db.Query(sql, new SqlParameter("@pb", maPB));
        }
    }
}
