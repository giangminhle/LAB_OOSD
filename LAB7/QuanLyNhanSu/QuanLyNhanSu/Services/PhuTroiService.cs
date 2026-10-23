using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>Phieu ghi nhan gio phu troi (tang ca, ngay le, ngay nghi), he so 1.5.</summary>
    public class PhuTroiService
    {
        public const decimal HeSoPhuTroi = 1.5m;
        public static readonly string[] HinhThucHopLe = { "TangCa", "NgayLe", "NgayNghi" };

        public string Validate(PhuTroiInput x)
        {
            if (x == null) return "Du lieu phieu phu troi khong hop le.";
            if (x.NgayPhieu == DateTime.MinValue) return "Ngay phieu khong hop le.";
            if (Array.IndexOf(HinhThucHopLe, x.HinhThuc) < 0)
                return "Hinh thuc phu troi chi nhan TangCa, NgayLe hoac NgayNghi.";
            if (string.IsNullOrWhiteSpace(x.NguoiLap)) return "Vui long nhap nguoi lap phieu.";
            if (x.ChiTiet == null || x.ChiTiet.Count == 0)
                return "Phieu phu troi phai co it nhat mot nhan vien.";
            var daCo = new HashSet<int>();
            foreach (ChiTietPhuTroiInput ct in x.ChiTiet)
            {
                if (ct.MaNV <= 0) return "Dong chi tiet thieu nhan vien.";
                if (!daCo.Add(ct.MaNV)) return "Moi nhan vien chi duoc ghi mot lan trong phieu.";
                if (ct.SoGio <= 0 || ct.SoGio > 12)
                    return "So gio phu troi phai lon hon 0 va khong qua 12 gio/nguoi.";
                if (ct.HeSo != HeSoPhuTroi)
                    return "He so phu troi bat buoc la 1.5.";
            }
            return null;
        }

        /// <summary>Tien phu troi = luong thang / 26 / 8 * 1.5 * so gio.</summary>
        public static decimal TinhTienPhuTroi(decimal mucLuongCoBan, decimal soGio)
        {
            if (mucLuongCoBan <= 0 || soGio <= 0) return 0m;
            decimal tien = mucLuongCoBan / 26m / 8m * HeSoPhuTroi * soGio;
            return Math.Round(tien, 0, MidpointRounding.AwayFromZero);
        }

        public static string SinhSoPhieu(DateTime ngay)
        {
            return "PT-" + ngay.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmssfff");
        }

        public ServiceResult Luu(PhuTroiInput x)
        {
            string err = Validate(x);
            if (err != null) return ServiceResult.Fail(err);
            string soPhieu = SinhSoPhieu(x.NgayPhieu);

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maPhieu;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.PhieuPhuTroi(SoPhieu, NgayPhieu, HinhThuc, NguoiLap, TrangThai, GhiChu)
VALUES (@so, @ngay, @ht, @nguoi, N'Nhap', @ghichu);
SELECT SCOPE_IDENTITY();", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", soPhieu);
                            cmd.Parameters.AddWithValue("@ngay", x.NgayPhieu.Date);
                            cmd.Parameters.AddWithValue("@ht", x.HinhThuc);
                            cmd.Parameters.AddWithValue("@nguoi", x.NguoiLap.Trim());
                            cmd.Parameters.AddWithValue("@ghichu", (object)x.GhiChu ?? DBNull.Value);
                            maPhieu = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        foreach (ChiTietPhuTroiInput ct in x.ChiTiet)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.ChiTietPhuTroi(MaPhieuPT, MaNV, SoGio, HeSo, GhiChu)
VALUES (@p, @nv, @gio, @hs, @ghichu)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@p", maPhieu);
                                cmd.Parameters.AddWithValue("@nv", ct.MaNV);
                                cmd.Parameters.AddWithValue("@gio", ct.SoGio);
                                cmd.Parameters.AddWithValue("@hs", ct.HeSo);
                                cmd.Parameters.AddWithValue("@ghichu", (object)ct.GhiChu ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Da lap phieu phu troi " + soPhieu + ".", maPhieu, soPhieu);
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

        public ServiceResult Duyet(int maPhieu)
        {
            int n = Db.Execute(@"UPDATE dbo.PhieuPhuTroi SET TrangThai=N'DaDuyet'
                                 WHERE MaPhieuPT=@p AND EXISTS
                                 (SELECT 1 FROM dbo.ChiTietPhuTroi ct WHERE ct.MaPhieuPT=@p)",
                new SqlParameter("@p", maPhieu));
            if (n == 0) return ServiceResult.Fail("Khong tim thay phieu hoac phieu chua co chi tiet.");
            return ServiceResult.Ok("Da duyet phieu phu troi.");
        }

        public DataTable DanhSachPhieu()
        {
            const string sql = @"
SELECT p.MaPhieuPT, p.SoPhieu, p.NgayPhieu, p.HinhThuc, p.NguoiLap, p.TrangThai,
       (SELECT COUNT(*) FROM dbo.ChiTietPhuTroi ct WHERE ct.MaPhieuPT=p.MaPhieuPT) AS SoNhanVien,
       (SELECT ISNULL(SUM(ct.SoGio),0) FROM dbo.ChiTietPhuTroi ct WHERE ct.MaPhieuPT=p.MaPhieuPT) AS TongGio
FROM dbo.PhieuPhuTroi p ORDER BY p.MaPhieuPT DESC";
            return Db.Query(sql);
        }

        public DataTable LayChiTiet(int maPhieu)
        {
            const string sql = @"
SELECT ct.MaCTPT, nv.MaNV, nv.MaNVCode, nv.HoTen, ct.SoGio, ct.HeSo, ct.GhiChu
FROM dbo.ChiTietPhuTroi ct JOIN dbo.NhanVien nv ON nv.MaNV=ct.MaNV
WHERE ct.MaPhieuPT=@p ORDER BY nv.MaNVCode";
            return Db.Query(sql, new SqlParameter("@p", maPhieu));
        }
    }
}
