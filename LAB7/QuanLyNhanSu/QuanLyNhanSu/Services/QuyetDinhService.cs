using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Services
{
    /// <summary>Quyet dinh luong va quyet dinh chuc vu (tang luong / bo nhiem).</summary>
    public class QuyetDinhService
    {
        public string ValidateLuong(int maNV, decimal mucLuong, decimal phuCapLuong,
            decimal phuCapDocHai, DateTime ngayHieuLuc, DateTime? hopDongStart, string nguoiKy)
        {
            if (maNV <= 0) return "Vui long chon nhan vien.";
            if (mucLuong <= 0) return "Muc luong co ban phai lon hon 0.";
            if (phuCapLuong < 0 || phuCapDocHai < 0) return "Phu cap khong duoc am.";
            if (string.IsNullOrWhiteSpace(nguoiKy)) return "Vui long nhap nguoi ky.";
            if (hopDongStart.HasValue && ngayHieuLuc.Date < hopDongStart.Value.Date)
                return "Ngay hieu luc khong duoc truoc ngay hieu luc hop dong.";
            return null;
        }

        public ServiceResult LapQuyetDinhLuong(int maNV, decimal mucLuong, decimal phuCapLuong,
            decimal phuCapDocHai, DateTime ngayHieuLuc, string lyDo, string nguoiKy)
        {
            DateTime? hopDongStart = null;
            object v = Db.Scalar(@"SELECT TOP 1 NgayHieuLuc FROM dbo.HopDongLaoDong
                                   WHERE MaNV=@m AND TrangThai=N'HieuLuc' ORDER BY MaHD DESC",
                new SqlParameter("@m", maNV));
            if (v != null && v != DBNull.Value) hopDongStart = Convert.ToDateTime(v);

            string err = ValidateLuong(maNV, mucLuong, phuCapLuong, phuCapDocHai,
                ngayHieuLuc, hopDongStart, nguoiKy);
            if (err != null) return ServiceResult.Fail(err);

            string soQD = HopDongService.SinhMa("QDL", ngayHieuLuc);
            try
            {
                int id = Convert.ToInt32(Db.Scalar(@"
INSERT dbo.QuyetDinhLuong(SoQD, MaNV, MucLuongCoBan, PhuCapLuong, PhuCapDocHai,
                          NgayBanHanh, NgayHieuLuc, LyDo, NguoiKy)
VALUES (@so, @nv, @luong, @pcl, @pcdh, CONVERT(DATE, GETDATE()), @ngay, @lydo, @ky);
SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@so", soQD),
                    new SqlParameter("@nv", maNV),
                    new SqlParameter("@luong", mucLuong),
                    new SqlParameter("@pcl", phuCapLuong),
                    new SqlParameter("@pcdh", phuCapDocHai),
                    new SqlParameter("@ngay", ngayHieuLuc.Date),
                    new SqlParameter("@lydo", (object)lyDo ?? DBNull.Value),
                    new SqlParameter("@ky", nguoiKy.Trim())));
                return ServiceResult.Ok("Da lap quyet dinh luong " + soQD, id, soQD);
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        public string ValidateChucVu(int maNV, string chucVu, decimal phuCapChucVu,
            DateTime ngayHieuLuc, DateTime? hopDongStart, string nguoiKy)
        {
            if (maNV <= 0) return "Vui long chon nhan vien.";
            if (string.IsNullOrWhiteSpace(chucVu)) return "Vui long nhap chuc vu.";
            if (phuCapChucVu < 0) return "Phu cap chuc vu khong duoc am.";
            if (string.IsNullOrWhiteSpace(nguoiKy)) return "Vui long nhap nguoi ky.";
            if (hopDongStart.HasValue && ngayHieuLuc.Date < hopDongStart.Value.Date)
                return "Ngay hieu luc khong duoc truoc ngay hieu luc hop dong.";
            return null;
        }

        public ServiceResult LapQuyetDinhChucVu(int maNV, string chucVu, decimal phuCapChucVu,
            DateTime ngayHieuLuc, string lyDo, string nguoiKy)
        {
            DateTime? hopDongStart = null;
            int maPB = 0;
            DataTable dt = Db.Query(@"SELECT TOP 1 hd.NgayHieuLuc, nv.MaPB
                                      FROM dbo.HopDongLaoDong hd
                                      JOIN dbo.NhanVien nv ON nv.MaNV = hd.MaNV
                                      WHERE hd.MaNV=@m AND hd.TrangThai=N'HieuLuc'
                                      ORDER BY hd.MaHD DESC", new SqlParameter("@m", maNV));
            if (dt.Rows.Count > 0)
            {
                hopDongStart = Convert.ToDateTime(dt.Rows[0]["NgayHieuLuc"]);
                maPB = Convert.ToInt32(dt.Rows[0]["MaPB"]);
            }

            string err = ValidateChucVu(maNV, chucVu, phuCapChucVu, ngayHieuLuc, hopDongStart, nguoiKy);
            if (err != null) return ServiceResult.Fail(err);
            if (maPB == 0) return ServiceResult.Fail("Nhan vien chua co hop dong dang hieu luc.");

            string soQD = HopDongService.SinhMa("QDCV", ngayHieuLuc);
            try
            {
                int id = Convert.ToInt32(Db.Scalar(@"
INSERT dbo.QuyetDinhChucVu(SoQD, MaNV, MaPB, ChucVu, PhuCapChucVu,
                           NgayBanHanh, NgayHieuLuc, LyDo, NguoiKy)
VALUES (@so, @nv, @pb, @cv, @pc, CONVERT(DATE, GETDATE()), @ngay, @lydo, @ky);
SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@so", soQD),
                    new SqlParameter("@nv", maNV),
                    new SqlParameter("@pb", maPB),
                    new SqlParameter("@cv", chucVu.Trim()),
                    new SqlParameter("@pc", phuCapChucVu),
                    new SqlParameter("@ngay", ngayHieuLuc.Date),
                    new SqlParameter("@lydo", (object)lyDo ?? DBNull.Value),
                    new SqlParameter("@ky", nguoiKy.Trim())));
                return ServiceResult.Ok("Da lap quyet dinh chuc vu " + soQD, id, soQD);
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        public DataTable DanhSachTatCaQuyetDinh()
        {
            const string sql = @"
SELECT N'Luong' AS LoaiQD, q.SoQD, nv.MaNVCode, nv.HoTen, q.NgayHieuLuc,
       q.MucLuongCoBan AS GiaTriChinh, q.PhuCapLuong AS PhuCap1, q.PhuCapDocHai AS PhuCap2,
       q.LyDo, q.NguoiKy
FROM dbo.QuyetDinhLuong q JOIN dbo.NhanVien nv ON nv.MaNV = q.MaNV
UNION ALL
SELECT N'ChucVu' AS LoaiQD, q.SoQD, nv.MaNVCode, nv.HoTen, q.NgayHieuLuc,
       q.PhuCapChucVu AS GiaTriChinh, 0 AS PhuCap1, 0 AS PhuCap2,
       q.ChucVu + N' - ' + ISNULL(q.LyDo, N'') AS LyDo, q.NguoiKy
FROM dbo.QuyetDinhChucVu q JOIN dbo.NhanVien nv ON nv.MaNV = q.MaNV
ORDER BY NgayHieuLuc DESC, SoQD DESC";
            return Db.Query(sql);
        }

        public DataTable LayQuyetDinhLuongGanNhat(int maNV)
        {
            const string sql = @"
SELECT TOP 1 MaQDLuong, SoQD, MucLuongCoBan, PhuCapLuong, PhuCapDocHai, NgayHieuLuc
FROM dbo.QuyetDinhLuong WHERE MaNV=@m ORDER BY NgayHieuLuc DESC, MaQDLuong DESC";
            return Db.Query(sql, new SqlParameter("@m", maNV));
        }

        public DataTable LayQuyetDinhChucVuGanNhat(int maNV)
        {
            const string sql = @"
SELECT TOP 1 MaQDChucVu, SoQD, ChucVu, PhuCapChucVu, NgayHieuLuc
FROM dbo.QuyetDinhChucVu WHERE MaNV=@m ORDER BY NgayHieuLuc DESC, MaQDChucVu DESC";
            return Db.Query(sql, new SqlParameter("@m", maNV));
        }
    }
}
