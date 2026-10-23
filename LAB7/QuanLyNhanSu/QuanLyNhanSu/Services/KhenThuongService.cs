using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>
    /// Quyet dinh khen thuong / ky luat cho ca nhan hoac tap the phong ban.
    /// - Khen thuong nguon PhieuChi : tu dong tao phieu chi.
    /// - Ky luat nguon PhieuThu    : tu dong tao phieu thu nop phat.
    /// - Nguon Luong / TruLuong    : dua vao bang luong thang.
    /// </summary>
    public class KhenThuongService
    {
        public const string CaNhan = "CaNhan";
        public const string TapThe = "TapThe";
        public const string NguonPhieuChi = "PhieuChi";
        public const string NguonLuong = "Luong";
        public const string NguonPhieuThu = "PhieuThu";
        public const string NguonTruLuong = "TruLuong";

        public string ValidateKhenThuong(KhenThuongInput x)
        {
            if (x == null) return "Du lieu khen thuong khong hop le.";
            if (x.NgayBanHanh == DateTime.MinValue) return "Ngay ban hanh khong hop le.";
            if (x.DoiTuong != CaNhan && x.DoiTuong != TapThe)
                return "Doi tuong chi nhan CaNhan hoac TapThe.";
            if (x.DoiTuong == CaNhan && (!x.MaNV.HasValue || x.MaNV.Value <= 0 || x.MaPB.HasValue))
                return "Khen thuong ca nhan phai chon dung mot nhan vien.";
            if (x.DoiTuong == TapThe && (!x.MaPB.HasValue || x.MaPB.Value <= 0 || x.MaNV.HasValue))
                return "Khen thuong tap the phai chon dung mot phong ban.";
            if (string.IsNullOrWhiteSpace(x.LyDo)) return "Vui long nhap ly do khen thuong.";
            if (string.IsNullOrWhiteSpace(x.HinhThuc)) return "Vui long nhap hinh thuc khen thuong.";
            if (x.SoTien <= 0) return "So tien khen thuong phai lon hon 0.";
            if (x.NguonChiTra != NguonPhieuChi && x.NguonChiTra != NguonLuong)
                return "Nguon chi tra chi nhan PhieuChi hoac Luong.";
            if (string.IsNullOrWhiteSpace(x.NguoiKy)) return "Vui long nhap nguoi ky.";
            return null;
        }

        public string ValidateKyLuat(KyLuatInput x)
        {
            if (x == null) return "Du lieu ky luat khong hop le.";
            if (x.NgayBanHanh == DateTime.MinValue) return "Ngay ban hanh khong hop le.";
            if (x.DoiTuong != CaNhan && x.DoiTuong != TapThe)
                return "Doi tuong chi nhan CaNhan hoac TapThe.";
            if (x.DoiTuong == CaNhan && (!x.MaNV.HasValue || x.MaNV.Value <= 0 || x.MaPB.HasValue))
                return "Ky luat ca nhan phai chon dung mot nhan vien.";
            if (x.DoiTuong == TapThe && (!x.MaPB.HasValue || x.MaPB.Value <= 0 || x.MaNV.HasValue))
                return "Ky luat tap the phai chon dung mot phong ban.";
            if (string.IsNullOrWhiteSpace(x.LyDo)) return "Vui long nhap ly do ky luat.";
            if (string.IsNullOrWhiteSpace(x.HinhThuc)) return "Vui long nhap hinh thuc ky luat.";
            if (x.SoTienPhat <= 0) return "So tien phat phai lon hon 0.";
            if (x.NguonThu != NguonPhieuThu && x.NguonThu != NguonTruLuong)
                return "Nguon thu chi nhan PhieuThu hoac TruLuong.";
            if (string.IsNullOrWhiteSpace(x.NguoiKy)) return "Vui long nhap nguoi ky.";
            return null;
        }

        /// <summary>Chia thuong/phat tap the deu cho cac nhan vien, lam tron 0 dong.</summary>
        public static decimal ChiaDeu(decimal tongTien, int soNhanVien)
        {
            if (soNhanVien <= 0) return 0m;
            return Math.Round(tongTien / soNhanVien, 0, MidpointRounding.AwayFromZero);
        }

        public ServiceResult LapKhenThuong(KhenThuongInput x)
        {
            string err = ValidateKhenThuong(x);
            if (err != null) return ServiceResult.Fail(err);
            string soQD = HopDongService.SinhMa("QDKT", x.NgayBanHanh);

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maQD;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.QuyetDinhKhenThuong(SoQD, NgayBanHanh, DoiTuong, MaNV, MaPB, LyDo,
        HinhThucKhenThuong, SoTien, NguonChiTra, NguoiKy, TrangThai)
VALUES (@so, @ngay, @dt, @nv, @pb, @lydo, @ht, @tien, @nguon, @ky, N'HieuLuc');
SELECT SCOPE_IDENTITY();", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", soQD);
                            cmd.Parameters.AddWithValue("@ngay", x.NgayBanHanh.Date);
                            cmd.Parameters.AddWithValue("@dt", x.DoiTuong);
                            cmd.Parameters.AddWithValue("@nv", x.MaNV.HasValue ? (object)x.MaNV.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@pb", x.MaPB.HasValue ? (object)x.MaPB.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@lydo", x.LyDo.Trim());
                            cmd.Parameters.AddWithValue("@ht", x.HinhThuc.Trim());
                            cmd.Parameters.AddWithValue("@tien", x.SoTien);
                            cmd.Parameters.AddWithValue("@nguon", x.NguonChiTra);
                            cmd.Parameters.AddWithValue("@ky", x.NguoiKy.Trim());
                            maQD = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        string soPhieuChi = null;
                        if (x.NguonChiTra == NguonPhieuChi)
                        {
                            soPhieuChi = HopDongService.SinhMa("PC", x.NgayBanHanh);
                            using (var cmd = new SqlCommand(@"
INSERT dbo.PhieuChiKhenThuong(SoPhieu, MaQDKhenThuong, NgayChi, SoTien, NguoiChi, GhiChu)
VALUES (@so, @qd, @ngay, @tien, @nguoi, @ghichu)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@so", soPhieuChi);
                                cmd.Parameters.AddWithValue("@qd", maQD);
                                cmd.Parameters.AddWithValue("@ngay", x.NgayBanHanh.Date);
                                cmd.Parameters.AddWithValue("@tien", x.SoTien);
                                cmd.Parameters.AddWithValue("@nguoi", x.NguoiKy.Trim());
                                cmd.Parameters.AddWithValue("@ghichu", "Chi tu dong theo " + soQD);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        string msg = "Da lap quyet dinh khen thuong " + soQD + ".";
                        if (x.DoiTuong == TapThe)
                            msg += " Tap the duoc chia deu khi dua vao bang luong (neu nguon Luong).";
                        if (soPhieuChi != null) msg += " Da tao phieu chi " + soPhieuChi + ".";
                        return ServiceResult.Ok(msg, maQD, soQD);
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

        public ServiceResult LapKyLuat(KyLuatInput x)
        {
            string err = ValidateKyLuat(x);
            if (err != null) return ServiceResult.Fail(err);
            string soQD = HopDongService.SinhMa("QDKL", x.NgayBanHanh);

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maQD;
                        using (var cmd = new SqlCommand(@"
INSERT dbo.QuyetDinhKyLuat(SoQD, NgayBanHanh, DoiTuong, MaNV, MaPB, LyDo,
        HinhThucKyLuat, SoTienPhat, NguonThu, NguoiKy, TrangThai)
VALUES (@so, @ngay, @dt, @nv, @pb, @lydo, @ht, @tien, @nguon, @ky, N'HieuLuc');
SELECT SCOPE_IDENTITY();", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@so", soQD);
                            cmd.Parameters.AddWithValue("@ngay", x.NgayBanHanh.Date);
                            cmd.Parameters.AddWithValue("@dt", x.DoiTuong);
                            cmd.Parameters.AddWithValue("@nv", x.MaNV.HasValue ? (object)x.MaNV.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@pb", x.MaPB.HasValue ? (object)x.MaPB.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@lydo", x.LyDo.Trim());
                            cmd.Parameters.AddWithValue("@ht", x.HinhThuc.Trim());
                            cmd.Parameters.AddWithValue("@tien", x.SoTienPhat);
                            cmd.Parameters.AddWithValue("@nguon", x.NguonThu);
                            cmd.Parameters.AddWithValue("@ky", x.NguoiKy.Trim());
                            maQD = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        string soPhieuThu = null;
                        if (x.NguonThu == NguonPhieuThu)
                        {
                            soPhieuThu = HopDongService.SinhMa("PTNP", x.NgayBanHanh);
                            using (var cmd = new SqlCommand(@"
INSERT dbo.PhieuThuNopPhat(SoPhieu, MaQDKyLuat, NgayThu, SoTien, NguoiThu, GhiChu)
VALUES (@so, @qd, @ngay, @tien, @nguoi, @ghichu)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@so", soPhieuThu);
                                cmd.Parameters.AddWithValue("@qd", maQD);
                                cmd.Parameters.AddWithValue("@ngay", x.NgayBanHanh.Date);
                                cmd.Parameters.AddWithValue("@tien", x.SoTienPhat);
                                cmd.Parameters.AddWithValue("@nguoi", x.NguoiKy.Trim());
                                cmd.Parameters.AddWithValue("@ghichu", "Thu tu dong theo " + soQD);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        string msg = "Da lap quyet dinh ky luat " + soQD + ".";
                        if (soPhieuThu != null) msg += " Da tao phieu thu " + soPhieuThu + ".";
                        else msg += " Khoan phat se tru vao bang luong thang.";
                        return ServiceResult.Ok(msg, maQD, soQD);
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

        public DataTable DanhSachKhenThuong()
        {
            const string sql = @"
SELECT q.MaQDKhenThuong, q.SoQD, q.NgayBanHanh, q.DoiTuong,
       ISNULL(nv.HoTen, pb.TenPB) AS MucTieu, q.LyDo, q.HinhThucKhenThuong,
       q.SoTien, q.NguonChiTra, q.NguoiKy, q.TrangThai,
       pc.SoPhieu AS PhieuChi
FROM dbo.QuyetDinhKhenThuong q
LEFT JOIN dbo.NhanVien nv ON nv.MaNV=q.MaNV
LEFT JOIN dbo.PhongBan pb ON pb.MaPB=q.MaPB
LEFT JOIN dbo.PhieuChiKhenThuong pc ON pc.MaQDKhenThuong=q.MaQDKhenThuong
ORDER BY q.MaQDKhenThuong DESC";
            return Db.Query(sql);
        }

        public DataTable DanhSachKyLuat()
        {
            const string sql = @"
SELECT q.MaQDKyLuat, q.SoQD, q.NgayBanHanh, q.DoiTuong,
       ISNULL(nv.HoTen, pb.TenPB) AS MucTieu, q.LyDo, q.HinhThucKyLuat,
       q.SoTienPhat, q.NguonThu, q.NguoiKy, q.TrangThai,
       pt.SoPhieu AS PhieuThu
FROM dbo.QuyetDinhKyLuat q
LEFT JOIN dbo.NhanVien nv ON nv.MaNV=q.MaNV
LEFT JOIN dbo.PhongBan pb ON pb.MaPB=q.MaPB
LEFT JOIN dbo.PhieuThuNopPhat pt ON pt.MaQDKyLuat=q.MaQDKyLuat
ORDER BY q.MaQDKyLuat DESC";
            return Db.Query(sql);
        }
    }
}
