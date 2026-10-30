using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyCaoOc.Data;

namespace QuanLyCaoOc.Services
{
    /// <summary>Tang, phong va gia thue (tham khao vi tri, dien tich, gia ca).</summary>
    public class PhongService
    {
        /// <summary>Don gia co ban VND / m2 / thang (tang 1).</summary>
        public const decimal DonGiaM2 = 250000m;

        /// <summary>Gia thue = dien tich x don gia m2 x he so tang, lam tron den nghin dong.</summary>
        public static decimal TinhGiaThue(decimal dienTich, decimal heSoTang)
        {
            if (dienTich <= 0 || heSoTang <= 0) return 0m;
            decimal gia = dienTich * DonGiaM2 * heSoTang;
            return Math.Round(gia / 1000m, 0, MidpointRounding.AwayFromZero) * 1000m;
        }

        /// <summary>He so gia theo do cao: tang 1 = 1.00, moi tang tang them 0.05.</summary>
        public static decimal HeSoTheoTang(int soTang)
        {
            if (soTang < 1) return 0m;
            return 1m + (soTang - 1) * 0.05m;
        }

        public DataTable DanhSachPhong(string trangThai = null)
        {
            string sql = @"
SELECT p.MaPhong, p.MaPhongCode, t.SoTang, p.DienTich, p.Huong, p.GiaThue, p.TrangThai,
       (SELECT TOP 1 k.TenCongTy FROM dbo.ChiTietHopDong c
          JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD AND h.TrangThai = N'HieuLuc'
          JOIN dbo.KhachHang k ON k.MaKH = h.MaKH
         WHERE c.MaPhong = p.MaPhong ORDER BY c.NgayKetThuc DESC) AS KhachThue
FROM dbo.Phong p JOIN dbo.Tang t ON t.MaTang = p.MaTang
WHERE (@tt IS NULL OR p.TrangThai = @tt)
ORDER BY t.SoTang, p.MaPhongCode";
            return Db.Query(sql, new SqlParameter("@tt", (object)trangThai ?? DBNull.Value));
        }

        public DataTable DanhSachTang()
        {
            return Db.Query("SELECT MaTang, SoTang, HeSoGia, MoTa FROM dbo.Tang ORDER BY SoTang");
        }

        public DataTable PhongTrong()
        {
            return Db.Query(@"SELECT MaPhong, MaPhongCode + N' - ' + CAST(CAST(DienTich AS INT) AS NVARCHAR) + N'm2 - '
                + FORMAT(GiaThue, 'N0') + N'đ' AS HienThi, GiaThue
                FROM dbo.Phong WHERE TrangThai = N'Trong' ORDER BY MaPhongCode");
        }
    }
}
