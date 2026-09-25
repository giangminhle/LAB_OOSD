using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public class ThongKeService
    {
        public DataTable TongHop(DateTime tu, DateTime den) => Db.Query(
            "SELECT " +
            "(SELECT COUNT(*) FROM PhieuDatPhong WHERE CAST(NgayLap AS date) BETWEEN @tu AND @den) SoPhieuDat," +
            "(SELECT COUNT(*) FROM PhieuDatPhong WHERE TrangThai=N'Đang ở') DangO," +
            "(SELECT COUNT(*) FROM HoaDon WHERE CAST(NgayLap AS date) BETWEEN @tu AND @den) SoHoaDon," +
            "(SELECT ISNULL(SUM(TongTien),0) FROM HoaDon WHERE CAST(NgayLap AS date) BETWEEN @tu AND @den) DoanhThuHoaDon," +
            "(SELECT ISNULL(SUM(TongTien),0) FROM PhieuDenBu WHERE CAST(NgayLap AS date) BETWEEN @tu AND @den) TongDenBu",
            new SqlParameter("@tu", tu.Date), new SqlParameter("@den", den.Date));

        public DataTable DichVu(DateTime tu, DateTime den) => Db.Query(
            "SELECT d.MaDV, d.TenDV, SUM(c.SoLuong) TongSoLuong, SUM(c.ThanhTien) TongTien " +
            "FROM PhieuSuDungDV p JOIN ChiTietPhieuSuDungDV c ON p.SoPhieuSDDV = c.SoPhieuSDDV " +
            "JOIN DichVu d ON c.MaDV = d.MaDV WHERE p.NgaySuDung BETWEEN @tu AND @den " +
            "GROUP BY d.MaDV, d.TenDV ORDER BY TongTien DESC",
            new SqlParameter("@tu", tu.Date), new SqlParameter("@den", den.Date));
    }
}
