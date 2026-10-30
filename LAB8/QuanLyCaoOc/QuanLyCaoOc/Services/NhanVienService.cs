using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>Cong ty bao cao nhan vien moi cho ban quan ly; cap the giu xe.</summary>
    public class NhanVienService
    {
        public string Validate(NhanVienInput x)
        {
            if (x == null) return "Chưa nhập thông tin nhân viên.";
            if (string.IsNullOrWhiteSpace(x.HoTen) || x.HoTen.Trim().Length < 3) return "Họ tên phải có ít nhất 3 ký tự.";
            if (!KiemTra.LaChuoiSo(x.CCCD, 12, 12)) return "CCCD phải gồm đúng 12 chữ số.";
            if (x.MaKH <= 0) return "Chưa chọn công ty.";
            if (x.MaPhong <= 0) return "Chưa chọn phòng làm việc.";
            if (string.IsNullOrWhiteSpace(x.ChucVu)) return "Chưa nhập chức vụ.";
            if (!string.IsNullOrWhiteSpace(x.DienThoai) && !KiemTra.LaChuoiSo(x.DienThoai, 9, 11))
                return "Điện thoại phải gồm 9-11 chữ số.";
            return null;
        }

        public ServiceResult BaoCaoNhanVienMoi(NhanVienInput x)
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
                        // phong phai dang duoc chinh cong ty nay thue
                        using (var chk = new SqlCommand(@"
SELECT COUNT(*) FROM dbo.ChiTietHopDong c JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD
WHERE h.MaKH = @kh AND c.MaPhong = @p AND h.TrangThai = N'HieuLuc' AND c.NgayKetThuc >= @ngay", conn, tran))
                        {
                            chk.Parameters.AddWithValue("@kh", x.MaKH);
                            chk.Parameters.AddWithValue("@p", x.MaPhong);
                            chk.Parameters.AddWithValue("@ngay", x.NgayBaoCao.Date);
                            if ((int)chk.ExecuteScalar() == 0)
                            {
                                tran.Rollback();
                                return ServiceResult.Fail("Công ty không thuê phòng này, không thể báo cáo nhân viên.");
                            }
                        }
                        string ma = MaSo.SinhMa(conn, tran, "NhanVienVanPhong", "MaNVCode", "NV");
                        int id;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.NhanVienVanPhong(MaNVCode, HoTen, CCCD, HinhAnh, MaKH, MaPhong, ChucVu, DienThoai, NgayBaoCao)
OUTPUT INSERTED.MaNV VALUES (@ma, @ten, @cccd, @anh, @kh, @p, @cv, @dt, @ngay)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@ma", ma);
                            cmd.Parameters.AddWithValue("@ten", x.HoTen.Trim());
                            cmd.Parameters.AddWithValue("@cccd", x.CCCD.Trim());
                            cmd.Parameters.AddWithValue("@anh", string.IsNullOrWhiteSpace(x.HinhAnh) ? (object)DBNull.Value : x.HinhAnh.Trim());
                            cmd.Parameters.AddWithValue("@kh", x.MaKH);
                            cmd.Parameters.AddWithValue("@p", x.MaPhong);
                            cmd.Parameters.AddWithValue("@cv", x.ChucVu.Trim());
                            cmd.Parameters.AddWithValue("@dt", string.IsNullOrWhiteSpace(x.DienThoai) ? (object)DBNull.Value : x.DienThoai.Trim());
                            cmd.Parameters.AddWithValue("@ngay", x.NgayBaoCao.Date);
                            id = (int)cmd.ExecuteScalar();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Đã ghi nhận nhân viên mới " + ma + " - " + x.HoTen.Trim() + ".", id, ma);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return ServiceResult.Fail("CCCD đã được đăng ký cho nhân viên khác.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi CSDL: " + ex.Message);
            }
        }

        public string ValidateTheXe(TheXeInput x)
        {
            if (x == null || x.MaNV <= 0) return "Chưa chọn nhân viên.";
            if (x.LoaiXe != "XeMay" && x.LoaiXe != "OTo") return "Loại xe chỉ gồm XeMay hoặc OTo.";
            if (!KiemTra.LaBienSo(x.BienSo)) return "Biển số không đúng định dạng (VD 59X1-234.56).";
            return null;
        }

        public ServiceResult CapTheGiuXe(TheXeInput x)
        {
            string err = ValidateTheXe(x);
            if (err != null) return ServiceResult.Fail(err);
            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (var chk = new SqlCommand(
                            "SELECT TrangThai FROM dbo.NhanVienVanPhong WHERE MaNV=@nv", conn, tran))
                        {
                            chk.Parameters.AddWithValue("@nv", x.MaNV);
                            object tt = chk.ExecuteScalar();
                            if (tt == null || Convert.ToString(tt) != "DangLam")
                            {
                                tran.Rollback();
                                return ServiceResult.Fail("Nhân viên không tồn tại hoặc đã nghỉ việc.");
                            }
                        }
                        string ma = MaSo.SinhMa(conn, tran, "TheGiuXe", "MaTheCode", "TX");
                        using (var cmd = new SqlCommand(@"
INSERT dbo.TheGiuXe(MaTheCode, MaNV, LoaiXe, BienSo, MauXe, NgayCap) VALUES (@ma, @nv, @lx, @bs, @mau, @ngay)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@ma", ma);
                            cmd.Parameters.AddWithValue("@nv", x.MaNV);
                            cmd.Parameters.AddWithValue("@lx", x.LoaiXe);
                            cmd.Parameters.AddWithValue("@bs", x.BienSo.Trim().ToUpperInvariant());
                            cmd.Parameters.AddWithValue("@mau", string.IsNullOrWhiteSpace(x.MauXe) ? (object)DBNull.Value : x.MauXe.Trim());
                            cmd.Parameters.AddWithValue("@ngay", x.NgayCap.Date);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Đã cấp thẻ giữ xe " + ma + ".", 0, ma);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return ServiceResult.Fail("Biển số đã có thẻ hoặc nhân viên đã có thẻ đang hiệu lực.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi CSDL: " + ex.Message);
            }
        }

        public DataTable DanhSachNhanVien()
        {
            return Db.Query(@"
SELECT n.MaNV, n.MaNVCode, n.HoTen, n.CCCD, k.TenCongTy, n.ChucVu, p.MaPhongCode, t.SoTang, n.HinhAnh,
       (SELECT TOP 1 x.MaTheCode + N' (' + x.BienSo + N')' FROM dbo.TheGiuXe x
         WHERE x.MaNV = n.MaNV AND x.TrangThai = N'HieuLuc') AS TheGiuXe
FROM dbo.NhanVienVanPhong n
JOIN dbo.KhachHang k ON k.MaKH = n.MaKH
JOIN dbo.Phong p ON p.MaPhong = n.MaPhong
JOIN dbo.Tang t ON t.MaTang = p.MaTang
WHERE n.TrangThai = N'DangLam'
ORDER BY n.MaNVCode");
        }

        public DataTable PhongCuaCongTy(int maKH)
        {
            return Db.Query(@"
SELECT DISTINCT p.MaPhong, p.MaPhongCode FROM dbo.ChiTietHopDong c
JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD AND h.TrangThai = N'HieuLuc'
JOIN dbo.Phong p ON p.MaPhong = c.MaPhong
WHERE h.MaKH = @kh ORDER BY p.MaPhongCode", new SqlParameter("@kh", maKH));
        }
    }
}
