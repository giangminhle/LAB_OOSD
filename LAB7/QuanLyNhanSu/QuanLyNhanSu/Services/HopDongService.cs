using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>
    /// Nghiep vu hop dong lao dong va bao hiem.
    /// DaiHan  -> khong xac dinh thoi han, bat buoc BHXH + BHYT.
    /// NganHan -> toi da 12 thang, khong lap bao hiem.
    /// </summary>
    public class HopDongService
    {
        public const string DaiHan = "DaiHan";
        public const string NganHan = "NganHan";
        public const decimal TyLeBHXHNV = 8.00m;
        public const decimal TyLeBHYTNV = 1.50m;

        /// <summary>Kiem tra hop dong; probationEnd lay tu quyet dinh thu viec.</summary>
        public string Validate(HopDongInput x, DateTime? probationEnd)
        {
            if (x == null) return "Du lieu hop dong khong hop le.";
            if (x.MaNV <= 0) return "Vui long chon nhan vien.";
            if (string.IsNullOrWhiteSpace(x.SoHD)) return "So hop dong khong duoc de trong.";
            if (x.LoaiHD != DaiHan && x.LoaiHD != NganHan)
                return "Loai hop dong chi nhan DaiHan hoac NganHan.";
            if (x.NgayKy.Date > x.NgayHieuLuc.Date)
                return "Ngay ky khong duoc sau ngay hieu luc hop dong.";
            if (x.MucLuong <= 0) return "Muc luong hop dong phai lon hon 0.";
            if (x.PhuCapLuong < 0 || x.PhuCapDocHai < 0 || x.PhuCapChucVu < 0)
                return "Phu cap khong duoc am.";
            if (string.IsNullOrWhiteSpace(x.ChucVu))
                return "Vui long nhap chuc vu khi ky hop dong.";
            if (string.IsNullOrWhiteSpace(x.NguoiKy))
                return "Vui long nhap nguoi ky hop dong.";

            if (probationEnd.HasValue && x.NgayHieuLuc.Date < probationEnd.Value.Date)
                return "Ngay hieu luc hop dong phai sau ngay ket thuc thu viec ("
                    + probationEnd.Value.ToString("dd/MM/yyyy") + ").";

            if (x.LoaiHD == DaiHan)
            {
                if (x.NgayHetHan.HasValue)
                    return "Hop dong dai han (khong xac dinh thoi han) khong co ngay het han.";
            }
            else
            {
                if (!x.NgayHetHan.HasValue)
                    return "Hop dong ngan han phai co ngay het han.";
                if (x.NgayHetHan.Value.Date <= x.NgayHieuLuc.Date)
                    return "Ngay het han phai sau ngay hieu luc.";
                if (x.NgayHetHan.Value.Date > x.NgayHieuLuc.Date.AddMonths(12))
                    return "Hop dong ngan han toi da 12 thang.";
            }
            return null;
        }

        public static bool CanLapBaoHiem(string loaiHD)
        {
            return loaiHD == DaiHan;
        }

        public static string SinhMa(string prefix, DateTime ngay)
        {
            return prefix + "-" + ngay.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmssfff");
        }

        /// <summary>Ky hop dong + 2 quyet dinh + bao hiem trong 1 transaction (stored procedure).</summary>
        public ServiceResult KyHopDong(HopDongInput x)
        {
            DateTime? probationEnd = null;
            if (x != null && x.MaNV > 0)
            {
                object v = Db.Scalar(@"SELECT TOP 1 NgayKetThucThuViec
                                       FROM dbo.QuyetDinhTuyenDung
                                       WHERE MaNV=@m AND TrangThai=N'HieuLuc'
                                       ORDER BY MaQDTD DESC", new SqlParameter("@m", x.MaNV));
                if (v != null && v != DBNull.Value) probationEnd = Convert.ToDateTime(v);
            }

            string err = Validate(x, probationEnd);
            if (err != null) return ServiceResult.Fail(err);

            string soHD = string.IsNullOrWhiteSpace(x.SoHD) ? SinhMa("HD", x.NgayKy) : x.SoHD.Trim();
            string soQDL = SinhMa("QDL", x.NgayKy);
            string soQDCV = SinhMa("QDCV", x.NgayKy);

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (var cmd = new SqlCommand("dbo.sp_KyHopDongChinhThuc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNV", x.MaNV);
                    cmd.Parameters.AddWithValue("@SoHD", soHD);
                    cmd.Parameters.AddWithValue("@LoaiHD", x.LoaiHD);
                    cmd.Parameters.AddWithValue("@NgayKy", x.NgayKy.Date);
                    cmd.Parameters.AddWithValue("@NgayHieuLuc", x.NgayHieuLuc.Date);
                    cmd.Parameters.AddWithValue("@NgayHetHan",
                        x.NgayHetHan.HasValue ? (object)x.NgayHetHan.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@MucLuong", x.MucLuong);
                    cmd.Parameters.AddWithValue("@SoQDLuong", soQDL);
                    cmd.Parameters.AddWithValue("@PhuCapLuong", x.PhuCapLuong);
                    cmd.Parameters.AddWithValue("@PhuCapDocHai", x.PhuCapDocHai);
                    cmd.Parameters.AddWithValue("@SoQDChucVu", soQDCV);
                    cmd.Parameters.AddWithValue("@ChucVu", x.ChucVu.Trim());
                    cmd.Parameters.AddWithValue("@PhuCapChucVu", x.PhuCapChucVu);
                    cmd.Parameters.AddWithValue("@NguoiKy", x.NguoiKy.Trim());
                    var pHD = cmd.Parameters.Add("@MaHD", SqlDbType.Int);
                    pHD.Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    int maHD = pHD.Value == DBNull.Value ? 0 : Convert.ToInt32(pHD.Value);
                    string msg = "Ky hop dong " + soHD + " thanh cong.";
                    if (CanLapBaoHiem(x.LoaiHD))
                        msg += " Da lap ho so BHXH (8%) va BHYT (1,5%).";
                    else
                        msg += " Hop dong ngan han khong lap bao hiem.";
                    return ServiceResult.Ok(msg, maHD, soHD);
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return ServiceResult.Fail("So hop dong/quyet dinh da ton tai hoac nhan vien da co hop dong hieu luc.");
                if (ex.Number >= 50000) return ServiceResult.Fail(ex.Message);
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        public DataTable DanhSachHopDong()
        {
            const string sql = @"
SELECT hd.MaHD, hd.SoHD, nv.MaNVCode, nv.HoTen, pb.TenPB, hd.LoaiHD,
       hd.NgayKy, hd.NgayHieuLuc, hd.NgayHetHan, hd.MucLuong, hd.TrangThai
FROM dbo.HopDongLaoDong hd
JOIN dbo.NhanVien nv ON nv.MaNV = hd.MaNV
JOIN dbo.PhongBan pb ON pb.MaPB = nv.MaPB
ORDER BY hd.MaHD DESC";
            return Db.Query(sql);
        }

        public DataTable DanhSachBaoHiem()
        {
            const string sql = @"
SELECT bh.MaBH, nv.MaNVCode, nv.HoTen, bh.LoaiBH, bh.SoSo,
       bh.NgayCap, bh.NgayHetHan, bh.MucDong, bh.TyLeNhanVien, bh.TyLeCongTy, bh.TrangThai
FROM dbo.BaoHiem bh
JOIN dbo.NhanVien nv ON nv.MaNV = bh.MaNV
ORDER BY nv.MaNVCode, bh.LoaiBH";
            return Db.Query(sql);
        }

        public DataTable LayHopDongHieuLuc(int maNV)
        {
            const string sql = @"
SELECT TOP 1 hd.MaHD, hd.SoHD, hd.LoaiHD, hd.NgayHieuLuc, hd.NgayHetHan, hd.MucLuong
FROM dbo.HopDongLaoDong hd
WHERE hd.MaNV=@m AND hd.TrangThai=N'HieuLuc'
ORDER BY hd.MaHD DESC";
            return Db.Query(sql, new SqlParameter("@m", maNV));
        }
    }
}
