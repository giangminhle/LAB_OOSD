using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>
    /// Lap phieu gia han cho tung phong cua hop dong.
    /// Dieu kien: so thang gia han &lt;= 1/2 thoi gian thue, con trong han hop dong,
    /// khong no tien thue mat bang va da thanh toan du dich vu.
    /// </summary>
    public class GiaHanService
    {
        public static int SoThangGiaHanToiDa(int soThangThue)
        {
            return soThangThue / 2;
        }

        /// <summary>Ngay ket thuc moi sau khi gia han n thang (giu dung ngay cuoi thang).</summary>
        public static DateTime TinhNgayKetThucMoi(DateTime ngayKetThucCu, int soThang)
        {
            return ngayKetThucCu.Date.AddDays(1).AddMonths(soThang).AddDays(-1);
        }

        public string Validate(GiaHanInput x)
        {
            if (x == null || x.MaCTHD <= 0) return "Chưa chọn phòng cần gia hạn.";
            if (x.SoThangGiaHan < 1) return "Số tháng gia hạn phải từ 1 trở lên.";
            if (string.IsNullOrWhiteSpace(x.NguoiLap)) return "Chưa nhập người lập phiếu.";
            return null;
        }

        /// <summary>Kiem tra dieu kien gia han tren thong tin da doc tu CSDL.</summary>
        public static string KiemTraDieuKien(ThongTinGiaHan t, int soThangGiaHan, DateTime ngayLap)
        {
            if (t == null) return "Không tìm thấy phòng trong hợp đồng.";
            if (t.TrangThaiHD != "HieuLuc") return "Hợp đồng không còn hiệu lực.";
            if (ngayLap.Date > t.NgayKetThuc) return "Phòng " + t.MaPhongCode + " đã hết hạn thuê, không gia hạn được.";
            int toiDa = SoThangGiaHanToiDa(t.SoThangThue);
            if (soThangGiaHan > toiDa)
                return "Chỉ được gia hạn tối đa " + toiDa + " tháng (1/2 thời gian thuê " + t.SoThangThue + " tháng).";
            if (t.SoHoaDonThueNo > 0)
                return "Còn nợ " + t.SoHoaDonThueNo + " hóa đơn tiền thuê (" + t.TienThueNo.ToString("N0") + " đ).";
            if (t.SoHoaDonDichVuNo > 0)
                return "Chưa thanh toán " + t.SoHoaDonDichVuNo + " hóa đơn dịch vụ (" + t.TienDichVuNo.ToString("N0") + " đ).";
            return null;
        }

        public ThongTinGiaHan LayThongTin(int maCTHD, DateTime ngayLap)
        {
            using (SqlConnection conn = Db.OpenConnection())
                return LayThongTin(conn, null, maCTHD, ngayLap);
        }

        private static ThongTinGiaHan LayThongTin(SqlConnection conn, SqlTransaction tran, int maCTHD, DateTime ngayLap)
        {
            using (var cmd = new SqlCommand(@"
SELECT c.MaCTHD, c.MaHD, p.MaPhongCode, h.TrangThai, c.SoThangThue, c.NgayKetThuc,
       (SELECT COUNT(*) FROM dbo.HoaDonTienThue t WHERE t.MaHD = c.MaHD AND t.DaThanhToan = 0 AND t.HanThanhToan <= @ngay),
       (SELECT ISNULL(SUM(t.SoTien), 0) FROM dbo.HoaDonTienThue t WHERE t.MaHD = c.MaHD AND t.DaThanhToan = 0 AND t.HanThanhToan <= @ngay),
       (SELECT COUNT(*) FROM dbo.HoaDonDichVu d WHERE d.MaHD = c.MaHD AND d.DaThanhToan = 0),
       (SELECT ISNULL(SUM(d.TongTien), 0) FROM dbo.HoaDonDichVu d WHERE d.MaHD = c.MaHD AND d.DaThanhToan = 0)
FROM dbo.ChiTietHopDong c
JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD
JOIN dbo.Phong p ON p.MaPhong = c.MaPhong
WHERE c.MaCTHD = @id", conn, tran))
            {
                cmd.Parameters.AddWithValue("@id", maCTHD);
                cmd.Parameters.AddWithValue("@ngay", ngayLap.Date);
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;
                    return new ThongTinGiaHan
                    {
                        MaCTHD = rd.GetInt32(0),
                        MaHD = rd.GetInt32(1),
                        MaPhongCode = rd.GetString(2),
                        TrangThaiHD = rd.GetString(3),
                        SoThangThue = rd.GetInt32(4),
                        NgayKetThuc = rd.GetDateTime(5),
                        SoHoaDonThueNo = rd.GetInt32(6),
                        TienThueNo = rd.GetDecimal(7),
                        SoHoaDonDichVuNo = rd.GetInt32(8),
                        TienDichVuNo = rd.GetDecimal(9)
                    };
                }
            }
        }

        public ServiceResult LapPhieuGiaHan(GiaHanInput x)
        {
            string err = Validate(x);
            if (err != null) return ServiceResult.Fail(err);
            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        ThongTinGiaHan t = LayThongTin(conn, tran, x.MaCTHD, x.NgayLap);
                        err = KiemTraDieuKien(t, x.SoThangGiaHan, x.NgayLap);
                        if (err != null) { tran.Rollback(); return ServiceResult.Fail(err); }

                        DateTime moi = TinhNgayKetThucMoi(t.NgayKetThuc, x.SoThangGiaHan);
                        string so = MaSo.Sinh(conn, tran, "PhieuGiaHan", "SoPhieu", "GH", x.NgayLap.Year);
                        using (var cmd = new SqlCommand(@"
INSERT dbo.PhieuGiaHan(SoPhieu, MaCTHD, NgayLap, SoThangGiaHan, NgayKetThucCu, NgayKetThucMoi, NguoiLap)
VALUES (@so, @ct, @ngay, @st, @cu, @moi, @nguoi);
UPDATE dbo.ChiTietHopDong SET NgayKetThuc = @moi WHERE MaCTHD = @ct;
UPDATE dbo.HopDongThue SET NgayKetThuc = (SELECT MAX(NgayKetThuc) FROM dbo.ChiTietHopDong WHERE MaHD = @hd)
WHERE MaHD = @hd;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", so);
                            cmd.Parameters.AddWithValue("@ct", x.MaCTHD);
                            cmd.Parameters.AddWithValue("@ngay", x.NgayLap.Date);
                            cmd.Parameters.AddWithValue("@st", x.SoThangGiaHan);
                            cmd.Parameters.AddWithValue("@cu", t.NgayKetThuc);
                            cmd.Parameters.AddWithValue("@moi", moi);
                            cmd.Parameters.AddWithValue("@nguoi", x.NguoiLap.Trim());
                            cmd.Parameters.AddWithValue("@hd", t.MaHD);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Đã lập phiếu gia hạn " + so + ": phòng " + t.MaPhongCode + " thêm "
                            + x.SoThangGiaHan + " tháng, đến " + moi.ToString("dd/MM/yyyy") + ".", 0, so);
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
                return ServiceResult.Fail("Lỗi CSDL: " + ex.Message);
            }
        }

        public DataTable DanhSachPhongDangThue()
        {
            return Db.Query(@"
SELECT c.MaCTHD, h.SoHD, k.TenCongTy, p.MaPhongCode, c.SoThangThue, c.NgayKetThuc,
       h.SoHD + N' - ' + p.MaPhongCode + N' - ' + k.TenCongTy AS HienThi
FROM dbo.ChiTietHopDong c
JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD AND h.TrangThai = N'HieuLuc'
JOIN dbo.KhachHang k ON k.MaKH = h.MaKH
JOIN dbo.Phong p ON p.MaPhong = c.MaPhong
ORDER BY h.SoHD, p.MaPhongCode");
        }

        public DataTable DanhSachPhieu()
        {
            return Db.Query(@"
SELECT g.SoPhieu, h.SoHD, p.MaPhongCode, g.NgayLap, g.SoThangGiaHan, g.NgayKetThucCu, g.NgayKetThucMoi, g.NguoiLap
FROM dbo.PhieuGiaHan g
JOIN dbo.ChiTietHopDong c ON c.MaCTHD = g.MaCTHD
JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD
JOIN dbo.Phong p ON p.MaPhong = c.MaPhong
ORDER BY g.MaPGH DESC");
        }
    }
}
