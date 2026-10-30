using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>
    /// Tien thue hang thang (tu thang thu 7, tra dau thang) va hoa don dich vu (thanh toan cuoi thang).
    /// </summary>
    public class ThanhToanService
    {
        /// <summary>Han tra tien thue hang thang: ngay 5 dau thang.</summary>
        public const int NgayHanTienThue = 5;
        /// <summary>Han tra dich vu: ngay 5 cua thang ke tiep.</summary>
        public const int NgayHanDichVu = 5;

        /// <summary>Thang (thang/nam) la thang thu may cua hop dong. Thang dau tien = 1.</summary>
        public static int ThangThu(DateTime ngayBatDau, int thang, int nam)
        {
            return (nam * 12 + thang) - (ngayBatDau.Year * 12 + ngayBatDau.Month) + 1;
        }

        /// <summary>Kiem tra co duoc lap hoa don tien thue hang thang hay khong.</summary>
        public static string ValidateHoaDonThang(DateTime ngayBatDau, DateTime ngayKetThuc, string trangThai,
            int thang, int nam)
        {
            if (thang < 1 || thang > 12) return "Tháng không hợp lệ.";
            if (trangThai != "HieuLuc") return "Hợp đồng không còn hiệu lực.";
            int thu = ThangThu(ngayBatDau, thang, nam);
            if (thu < 1) return "Tháng này chưa bắt đầu thuê.";
            if (thu <= HopDongService.SoThangTraTruoc) return "Tháng thứ " + thu + " đã nằm trong 6 tháng trả trước.";
            if (new DateTime(nam, thang, 1) > ngayKetThuc) return "Tháng này đã hết thời hạn thuê.";
            return null;
        }

        public ServiceResult LapHoaDonTienThueThang(int maHD, int thang, int nam)
        {
            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        DateTime bd, kt; string tt;
                        using (var cmd = new SqlCommand("SELECT NgayBatDau, NgayKetThuc, TrangThai FROM dbo.HopDongThue WHERE MaHD=@hd", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@hd", maHD);
                            using (var rd = cmd.ExecuteReader())
                            {
                                if (!rd.Read()) { rd.Close(); tran.Rollback(); return ServiceResult.Fail("Không tìm thấy hợp đồng."); }
                                bd = rd.GetDateTime(0); kt = rd.GetDateTime(1); tt = rd.GetString(2);
                            }
                        }
                        string err = ValidateHoaDonThang(bd, kt, tt, thang, nam);
                        if (err != null) { tran.Rollback(); return ServiceResult.Fail(err); }

                        DateTime kyTu = new DateTime(nam, thang, 1);
                        DateTime kyDen = kyTu.AddMonths(1).AddDays(-1);
                        decimal tien;
                        using (var cmd = new SqlCommand(
                            "SELECT ISNULL(SUM(GiaThue), 0) FROM dbo.ChiTietHopDong WHERE MaHD=@hd AND NgayKetThuc >= @tu", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@hd", maHD);
                            cmd.Parameters.AddWithValue("@tu", kyTu);
                            tien = Convert.ToDecimal(cmd.ExecuteScalar());
                        }
                        if (tien <= 0) { tran.Rollback(); return ServiceResult.Fail("Không còn phòng nào thuê trong tháng này."); }

                        string so = MaSo.Sinh(conn, tran, "HoaDonTienThue", "SoHoaDon", "TT", nam);
                        using (var cmd = new SqlCommand(@"
INSERT dbo.HoaDonTienThue(SoHoaDon, MaHD, LoaiHoaDon, KyTu, KyDen, SoTien, HanThanhToan)
VALUES (@so, @hd, N'HangThang', @tu, @den, @tien, @han)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", so);
                            cmd.Parameters.AddWithValue("@hd", maHD);
                            cmd.Parameters.AddWithValue("@tu", kyTu);
                            cmd.Parameters.AddWithValue("@den", kyDen);
                            cmd.Parameters.AddWithValue("@tien", tien);
                            cmd.Parameters.AddWithValue("@han", new DateTime(nam, thang, NgayHanTienThue));
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Đã lập hóa đơn tiền thuê " + so + " tháng " + thang + "/" + nam
                            + " (tháng thứ " + ThangThu(bd, thang, nam) + "): " + tien.ToString("N0") + " đ.", 0, so);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                return ServiceResult.Fail("Tháng " + thang + "/" + nam + " đã có hóa đơn tiền thuê.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi CSDL: " + ex.Message);
            }
        }

        public string ValidateDichVu(HoaDonDichVuInput x)
        {
            if (x == null || x.MaHD <= 0) return "Chưa chọn hợp đồng.";
            if (x.Thang < 1 || x.Thang > 12) return "Tháng không hợp lệ.";
            if (x.ChiTiet == null || x.ChiTiet.Count == 0) return "Hóa đơn dịch vụ phải có ít nhất 1 dòng.";
            if (x.ChiTiet.Any(c => c.SoLuong <= 0)) return "Số lượng dịch vụ phải lớn hơn 0.";
            if (x.ChiTiet.GroupBy(c => new { c.MaPhong, c.MaDV }).Any(g => g.Count() > 1))
                return "Một dịch vụ của một phòng bị nhập 2 lần.";
            // dich vu thanh toan cuoi thang: chi lap tu ngay cuoi thang tro di
            DateTime cuoiThang = new DateTime(x.Nam, x.Thang, 1).AddMonths(1).AddDays(-1);
            if (x.NgayLap.Date < cuoiThang) return "Chỉ lập hóa đơn dịch vụ từ ngày cuối tháng " + x.Thang + "/" + x.Nam + ".";
            return null;
        }

        public static decimal ThanhTien(decimal soLuong, decimal donGia)
        {
            return Math.Round(soLuong * donGia, 0, MidpointRounding.AwayFromZero);
        }

        public ServiceResult LapHoaDonDichVu(HoaDonDichVuInput x)
        {
            string err = ValidateDichVu(x);
            if (err != null) return ServiceResult.Fail(err);
            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        string so = MaSo.Sinh(conn, tran, "HoaDonDichVu", "SoHoaDon", "DV", x.Nam);
                        DateTime han = new DateTime(x.Nam, x.Thang, 1).AddMonths(1).AddDays(NgayHanDichVu - 1);
                        int maHDDV;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.HoaDonDichVu(SoHoaDon, MaHD, Thang, Nam, HanThanhToan) OUTPUT INSERTED.MaHDDV
VALUES (@so, @hd, @t, @n, @han)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", so);
                            cmd.Parameters.AddWithValue("@hd", x.MaHD);
                            cmd.Parameters.AddWithValue("@t", x.Thang);
                            cmd.Parameters.AddWithValue("@n", x.Nam);
                            cmd.Parameters.AddWithValue("@han", han);
                            maHDDV = (int)cmd.ExecuteScalar();
                        }

                        decimal tong = 0;
                        foreach (var c in x.ChiTiet)
                        {
                            // phong phai thuoc hop dong
                            using (var chk = new SqlCommand("SELECT COUNT(*) FROM dbo.ChiTietHopDong WHERE MaHD=@hd AND MaPhong=@p", conn, tran))
                            {
                                chk.Parameters.AddWithValue("@hd", x.MaHD);
                                chk.Parameters.AddWithValue("@p", c.MaPhong);
                                if ((int)chk.ExecuteScalar() == 0)
                                {
                                    tran.Rollback();
                                    return ServiceResult.Fail("Phòng " + c.MaPhong + " không thuộc hợp đồng này.");
                                }
                            }
                            decimal donGia;
                            using (var g = new SqlCommand("SELECT DonGia FROM dbo.DichVu WHERE MaDV=@dv", conn, tran))
                            {
                                g.Parameters.AddWithValue("@dv", c.MaDV);
                                donGia = Convert.ToDecimal(g.ExecuteScalar());
                            }
                            decimal thanhTien = ThanhTien(c.SoLuong, donGia);
                            tong += thanhTien;
                            using (var cmd = new SqlCommand(@"
INSERT dbo.ChiTietHoaDonDichVu(MaHDDV, MaPhong, MaDV, SoLuong, DonGia, ThanhTien)
VALUES (@hddv, @p, @dv, @sl, @dg, @tt)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@hddv", maHDDV);
                                cmd.Parameters.AddWithValue("@p", c.MaPhong);
                                cmd.Parameters.AddWithValue("@dv", c.MaDV);
                                cmd.Parameters.AddWithValue("@sl", c.SoLuong);
                                cmd.Parameters.AddWithValue("@dg", donGia);
                                cmd.Parameters.AddWithValue("@tt", thanhTien);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        using (var cmd = new SqlCommand("UPDATE dbo.HoaDonDichVu SET TongTien=@t WHERE MaHDDV=@id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@t", tong);
                            cmd.Parameters.AddWithValue("@id", maHDDV);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Đã lập hóa đơn dịch vụ " + so + " tháng " + x.Thang + "/" + x.Nam
                            + ": " + tong.ToString("N0") + " đ, hạn " + han.ToString("dd/MM/yyyy") + ".", maHDDV, so);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                return ServiceResult.Fail("Tháng " + x.Thang + "/" + x.Nam + " đã có hóa đơn dịch vụ.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi CSDL: " + ex.Message);
            }
        }

        /// <summary>Thanh toan hoa don: loai = "Thue" hoac "DichVu".</summary>
        public ServiceResult ThanhToan(string loai, string soHoaDon, DateTime ngay)
        {
            string bang = loai == "Thue" ? "HoaDonTienThue" : loai == "DichVu" ? "HoaDonDichVu" : null;
            if (bang == null) return ServiceResult.Fail("Loại hóa đơn không hợp lệ.");
            if (string.IsNullOrWhiteSpace(soHoaDon)) return ServiceResult.Fail("Chưa chọn hóa đơn.");
            int n = Db.Execute("UPDATE dbo." + bang + " SET DaThanhToan=1, NgayThanhToan=@n WHERE SoHoaDon=@so AND DaThanhToan=0",
                new SqlParameter("@n", ngay.Date), new SqlParameter("@so", soHoaDon.Trim()));
            return n == 1 ? ServiceResult.Ok("Đã thanh toán hóa đơn " + soHoaDon + ".")
                          : ServiceResult.Fail("Hóa đơn không tồn tại hoặc đã thanh toán.");
        }

        public DataTable DanhSachHoaDon(int maHD)
        {
            return Db.Query(@"
SELECT N'Thue' AS Loai, SoHoaDon, LoaiHoaDon AS NoiDung, KyTu, KyDen, SoTien, HanThanhToan, DaThanhToan, NgayThanhToan
FROM dbo.HoaDonTienThue WHERE MaHD=@hd
UNION ALL
SELECT N'DichVu', SoHoaDon, N'Dịch vụ T' + CAST(Thang AS NVARCHAR) + N'/' + CAST(Nam AS NVARCHAR),
       DATEFROMPARTS(Nam, Thang, 1), EOMONTH(DATEFROMPARTS(Nam, Thang, 1)), TongTien, HanThanhToan, DaThanhToan, NgayThanhToan
FROM dbo.HoaDonDichVu WHERE MaHD=@hd
ORDER BY KyTu, Loai DESC", new SqlParameter("@hd", maHD));
        }

        public DataTable DanhSachDichVu()
        {
            return Db.Query("SELECT MaDV, TenDV, DonVi, DonGia FROM dbo.DichVu ORDER BY MaDV");
        }

        public DataTable PhongCuaHopDong(int maHD)
        {
            return Db.Query(@"SELECT c.MaPhong, p.MaPhongCode, c.GiaThue, c.NgayKetThuc
FROM dbo.ChiTietHopDong c JOIN dbo.Phong p ON p.MaPhong = c.MaPhong WHERE c.MaHD=@hd ORDER BY p.MaPhongCode",
                new SqlParameter("@hd", maHD));
        }
    }
}
