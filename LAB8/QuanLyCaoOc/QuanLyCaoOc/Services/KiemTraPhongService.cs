using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>
    /// Kiem tra tinh trang phong 1 nam / lan de len ke hoach tu sua.
    /// Neu phong tu y thay doi ket cau thi lap phieu phat cho hop dong dang thue.
    /// </summary>
    public class KiemTraPhongService
    {
        public string Validate(KiemTraInput x)
        {
            if (x == null || x.MaPhong <= 0) return "Chưa chọn phòng.";
            if (x.TinhTrang != "Tot" && x.TinhTrang != "CanSua" && x.TinhTrang != "HuHong")
                return "Tình trạng chỉ gồm Tot, CanSua, HuHong.";
            if (string.IsNullOrWhiteSpace(x.NguoiKiemTra)) return "Chưa nhập người kiểm tra.";
            if (x.TinhTrang != "Tot" && string.IsNullOrWhiteSpace(x.KeHoachTuSua))
                return "Phòng cần sửa hoặc hư hỏng phải có kế hoạch tu sửa.";
            if (x.CoThayDoiKetCau)
            {
                if (x.SoTienPhat <= 0) return "Phòng thay đổi kết cấu phải có số tiền phạt > 0.";
                if (string.IsNullOrWhiteSpace(x.LyDoPhat)) return "Chưa nhập lý do phạt.";
            }
            return null;
        }

        /// <summary>Moi phong chi kiem tra 1 lan / nam.</summary>
        public static bool DaKiemTraTrongNam(DateTime? lanTruoc, DateTime ngayKiemTra)
        {
            return lanTruoc.HasValue && lanTruoc.Value.Year == ngayKiemTra.Year;
        }

        public ServiceResult LapKiemTra(KiemTraInput x)
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
                        using (var chk = new SqlCommand("SELECT MAX(NgayKiemTra) FROM dbo.KiemTraPhong WHERE MaPhong=@p", conn, tran))
                        {
                            chk.Parameters.AddWithValue("@p", x.MaPhong);
                            object v = chk.ExecuteScalar();
                            DateTime? truoc = v == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(v);
                            if (DaKiemTraTrongNam(truoc, x.NgayKiemTra))
                            {
                                tran.Rollback();
                                return ServiceResult.Fail("Phòng đã được kiểm tra trong năm " + x.NgayKiemTra.Year + ".");
                            }
                        }

                        int? maHD = null;
                        if (x.CoThayDoiKetCau)
                        {
                            using (var cmd = new SqlCommand(@"
SELECT TOP 1 c.MaHD FROM dbo.ChiTietHopDong c JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD
WHERE c.MaPhong = @p AND h.TrangThai = N'HieuLuc' AND h.NgayBatDau <= @ngay AND c.NgayKetThuc >= @ngay", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@p", x.MaPhong);
                                cmd.Parameters.AddWithValue("@ngay", x.NgayKiemTra.Date);
                                object v = cmd.ExecuteScalar();
                                if (v == null)
                                {
                                    tran.Rollback();
                                    return ServiceResult.Fail("Phòng không có khách thuê, không lập phiếu phạt được.");
                                }
                                maHD = Convert.ToInt32(v);
                            }
                        }

                        int maKT;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.KiemTraPhong(MaPhong, NgayKiemTra, TinhTrang, CoThayDoiKetCau, MoTa, KeHoachTuSua, NguoiKiemTra)
OUTPUT INSERTED.MaKT VALUES (@p, @ngay, @tt, @kc, @mt, @kh, @nguoi)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@p", x.MaPhong);
                            cmd.Parameters.AddWithValue("@ngay", x.NgayKiemTra.Date);
                            cmd.Parameters.AddWithValue("@tt", x.TinhTrang);
                            cmd.Parameters.AddWithValue("@kc", x.CoThayDoiKetCau);
                            cmd.Parameters.AddWithValue("@mt", (object)x.MoTa ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@kh", string.IsNullOrWhiteSpace(x.KeHoachTuSua) ? (object)DBNull.Value : x.KeHoachTuSua.Trim());
                            cmd.Parameters.AddWithValue("@nguoi", x.NguoiKiemTra.Trim());
                            maKT = (int)cmd.ExecuteScalar();
                        }

                        string msg = "Đã lưu kết quả kiểm tra phòng.";
                        if (maHD.HasValue)
                        {
                            string so = MaSo.Sinh(conn, tran, "PhieuPhat", "SoPhieu", "PP", x.NgayKiemTra.Year);
                            using (var cmd = new SqlCommand(@"
INSERT dbo.PhieuPhat(SoPhieu, MaKT, MaHD, LyDo, SoTien, NgayLap) VALUES (@so, @kt, @hd, @ld, @tien, @ngay)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@so", so);
                                cmd.Parameters.AddWithValue("@kt", maKT);
                                cmd.Parameters.AddWithValue("@hd", maHD.Value);
                                cmd.Parameters.AddWithValue("@ld", x.LyDoPhat.Trim());
                                cmd.Parameters.AddWithValue("@tien", x.SoTienPhat);
                                cmd.Parameters.AddWithValue("@ngay", x.NgayKiemTra.Date);
                                cmd.ExecuteNonQuery();
                            }
                            msg += " Lập phiếu phạt " + so + ": " + x.SoTienPhat.ToString("N0") + " đ.";
                        }
                        if (x.TinhTrang == "HuHong")
                        {
                            using (var cmd = new SqlCommand(
                                "UPDATE dbo.Phong SET TrangThai = N'SuaChua' WHERE MaPhong = @p AND TrangThai = N'Trong'", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@p", x.MaPhong);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return ServiceResult.Ok(msg, maKT);
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
                return ServiceResult.Fail("Phòng đã được kiểm tra trong năm này.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi CSDL: " + ex.Message);
            }
        }

        public DataTable DanhSachKiemTra()
        {
            return Db.Query(@"
SELECT k.MaKT, p.MaPhongCode, k.NgayKiemTra, k.TinhTrang, k.CoThayDoiKetCau, k.MoTa, k.KeHoachTuSua,
       f.SoPhieu AS PhieuPhat, f.SoTien AS TienPhat
FROM dbo.KiemTraPhong k
JOIN dbo.Phong p ON p.MaPhong = k.MaPhong
LEFT JOIN dbo.PhieuPhat f ON f.MaKT = k.MaKT
ORDER BY k.NgayKiemTra DESC, p.MaPhongCode");
        }

        /// <summary>Danh sach phong chua kiem tra trong nam.</summary>
        public DataTable PhongChuaKiemTra(int nam)
        {
            return Db.Query(@"
SELECT p.MaPhong, p.MaPhongCode, p.TrangThai FROM dbo.Phong p
WHERE NOT EXISTS (SELECT 1 FROM dbo.KiemTraPhong k WHERE k.MaPhong = p.MaPhong AND k.Nam = @nam)
ORDER BY p.MaPhongCode", new SqlParameter("@nam", nam));
        }
    }
}
