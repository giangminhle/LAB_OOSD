using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>Lap hop dong thue nhieu van phong + hoa don tra truoc 6 thang.</summary>
    public class HopDongService
    {
        public const int SoThangToiThieu = 6;
        public const int SoThangTraTruoc = 6;

        public string Validate(HopDongInput x)
        {
            if (x == null) return "Chưa nhập thông tin hợp đồng.";
            if (x.MaKH <= 0) return "Chưa chọn khách hàng.";
            if (x.DanhSachPhong == null || x.DanhSachPhong.Count == 0) return "Hợp đồng phải có ít nhất 1 văn phòng.";
            if (x.DanhSachPhong.Distinct().Count() != x.DanhSachPhong.Count) return "Một văn phòng bị chọn 2 lần.";
            if (x.SoThang < SoThangToiThieu) return "Thời gian thuê ít nhất " + SoThangToiThieu + " tháng.";
            if (x.SoThang > 60) return "Thời gian thuê tối đa 60 tháng.";
            if (x.NgayBatDau.Day != 1) return "Ngày bắt đầu thuê phải là ngày 1 của tháng.";
            if (x.NgayKy.Date > x.NgayBatDau.Date) return "Ngày ký phải trước hoặc bằng ngày bắt đầu thuê.";
            return null;
        }

        /// <summary>Ngay ket thuc = ngay bat dau + so thang - 1 ngay.</summary>
        public static DateTime TinhNgayKetThuc(DateTime batDau, int soThang)
        {
            return batDau.Date.AddMonths(soThang).AddDays(-1);
        }

        /// <summary>Tien tra truoc = tong gia thue cac phong x 6 thang.</summary>
        public static decimal TinhTienTraTruoc(IEnumerable<decimal> giaThuePhong)
        {
            return giaThuePhong.Sum() * SoThangTraTruoc;
        }

        public ServiceResult LapHopDong(HopDongInput x)
        {
            string err = Validate(x);
            if (err != null) return ServiceResult.Fail(err);

            DateTime ketThuc = TinhNgayKetThuc(x.NgayBatDau, x.SoThang);
            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // kiem tra phong con trong va lay gia
                        var gia = new Dictionary<int, decimal>();
                        foreach (int maPhong in x.DanhSachPhong)
                        {
                            using (var cmd = new SqlCommand(
                                "SELECT MaPhongCode, GiaThue, TrangThai FROM dbo.Phong WITH (UPDLOCK) WHERE MaPhong=@p", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@p", maPhong);
                                using (var rd = cmd.ExecuteReader())
                                {
                                    if (!rd.Read()) { rd.Close(); tran.Rollback(); return ServiceResult.Fail("Không tìm thấy phòng " + maPhong + "."); }
                                    string code = rd.GetString(0);
                                    decimal g = rd.GetDecimal(1);
                                    string tt = rd.GetString(2);
                                    rd.Close();
                                    if (tt != "Trong") { tran.Rollback(); return ServiceResult.Fail("Phòng " + code + " không còn trống."); }
                                    gia[maPhong] = g;
                                }
                            }
                        }

                        string soHD = MaSo.Sinh(conn, tran, "HopDongThue", "SoHD", "HD", x.NgayKy.Year);
                        int maHD;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.HopDongThue(SoHD, MaKH, NgayKy, NgayBatDau, SoThang, NgayKetThuc, NoiDung)
OUTPUT INSERTED.MaHD VALUES (@so, @kh, @ky, @bd, @st, @kt, @nd)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", soHD);
                            cmd.Parameters.AddWithValue("@kh", x.MaKH);
                            cmd.Parameters.AddWithValue("@ky", x.NgayKy.Date);
                            cmd.Parameters.AddWithValue("@bd", x.NgayBatDau.Date);
                            cmd.Parameters.AddWithValue("@st", x.SoThang);
                            cmd.Parameters.AddWithValue("@kt", ketThuc);
                            cmd.Parameters.AddWithValue("@nd", (object)x.NoiDung ?? DBNull.Value);
                            maHD = (int)cmd.ExecuteScalar();
                        }

                        foreach (var kv in gia)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.ChiTietHopDong(MaHD, MaPhong, GiaThue, SoThangThue, NgayKetThuc) VALUES (@hd, @p, @g, @st, @kt);
UPDATE dbo.Phong SET TrangThai = N'DangThue' WHERE MaPhong = @p;", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@hd", maHD);
                                cmd.Parameters.AddWithValue("@p", kv.Key);
                                cmd.Parameters.AddWithValue("@g", kv.Value);
                                cmd.Parameters.AddWithValue("@st", x.SoThang);
                                cmd.Parameters.AddWithValue("@kt", ketThuc);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // hoa don tra truoc 6 thang dau
                        decimal tienTraTruoc = TinhTienTraTruoc(gia.Values);
                        string soTT = MaSo.Sinh(conn, tran, "HoaDonTienThue", "SoHoaDon", "TT", x.NgayKy.Year);
                        using (var cmd = new SqlCommand(@"
INSERT dbo.HoaDonTienThue(SoHoaDon, MaHD, LoaiHoaDon, KyTu, KyDen, SoTien, HanThanhToan, DaThanhToan, NgayThanhToan)
VALUES (@so, @hd, N'TraTruoc6Thang', @tu, @den, @tien, @han, @da, @ngay)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", soTT);
                            cmd.Parameters.AddWithValue("@hd", maHD);
                            cmd.Parameters.AddWithValue("@tu", x.NgayBatDau.Date);
                            cmd.Parameters.AddWithValue("@den", TinhNgayKetThuc(x.NgayBatDau, SoThangTraTruoc));
                            cmd.Parameters.AddWithValue("@tien", tienTraTruoc);
                            cmd.Parameters.AddWithValue("@han", x.NgayBatDau.Date);
                            cmd.Parameters.AddWithValue("@da", x.DaTraTruoc);
                            cmd.Parameters.AddWithValue("@ngay", x.DaTraTruoc ? (object)x.NgayKy.Date : DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return ServiceResult.Ok("Đã lập hợp đồng " + soHD + " (" + gia.Count + " phòng, đến "
                            + ketThuc.ToString("dd/MM/yyyy") + "). Hóa đơn trả trước " + soTT + ": "
                            + tienTraTruoc.ToString("N0") + " đ.", maHD, soHD);
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

        public DataTable DanhSachKhachHang()
        {
            return Db.Query("SELECT MaKH, TenCongTy, MaSoThue, NguoiDaiDien, DienThoai FROM dbo.KhachHang ORDER BY TenCongTy");
        }

        public ServiceResult ThemKhachHang(string tenCongTy, string maSoThue, string daiDien, string dienThoai)
        {
            if (string.IsNullOrWhiteSpace(tenCongTy) || tenCongTy.Trim().Length < 3) return ServiceResult.Fail("Tên công ty không hợp lệ.");
            if (!KiemTra.LaChuoiSo(maSoThue, 10, 13)) return ServiceResult.Fail("Mã số thuế phải gồm 10-13 chữ số.");
            if (string.IsNullOrWhiteSpace(daiDien)) return ServiceResult.Fail("Chưa nhập người đại diện.");
            if (!KiemTra.LaChuoiSo(dienThoai, 9, 11)) return ServiceResult.Fail("Điện thoại phải gồm 9-11 chữ số.");
            try
            {
                object id = Db.Scalar(@"INSERT dbo.KhachHang(TenCongTy, MaSoThue, NguoiDaiDien, DienThoai)
OUTPUT INSERTED.MaKH VALUES (@t, @m, @d, @dt)",
                    new SqlParameter("@t", tenCongTy.Trim()), new SqlParameter("@m", maSoThue.Trim()),
                    new SqlParameter("@d", daiDien.Trim()), new SqlParameter("@dt", dienThoai.Trim()));
                return ServiceResult.Ok("Đã thêm khách hàng.", Convert.ToInt32(id));
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                return ServiceResult.Fail("Mã số thuế đã tồn tại.");
            }
        }

        public DataTable DanhSachHopDong()
        {
            return Db.Query(@"
SELECT h.MaHD, h.SoHD, k.TenCongTy, h.NgayBatDau, h.SoThang, h.NgayKetThuc, h.TrangThai,
       STUFF((SELECT N', ' + p.MaPhongCode FROM dbo.ChiTietHopDong c JOIN dbo.Phong p ON p.MaPhong = c.MaPhong
              WHERE c.MaHD = h.MaHD FOR XML PATH('')), 1, 2, '') AS Phong,
       (SELECT SUM(c.GiaThue) FROM dbo.ChiTietHopDong c WHERE c.MaHD = h.MaHD) AS TienThueThang
FROM dbo.HopDongThue h JOIN dbo.KhachHang k ON k.MaKH = h.MaKH
ORDER BY h.MaHD DESC");
        }
    }
}
