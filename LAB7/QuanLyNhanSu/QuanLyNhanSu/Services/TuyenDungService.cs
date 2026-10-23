using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>
    /// Nghiep vu tuyen dung - thu viec: tiep nhan ho so, lap quyet dinh thu viec,
    /// theo doi danh sach dang thu viec. Cac ham Validate* la ham thuan (khong DB)
    /// de kiem thu offline.
    /// </summary>
    public class TuyenDungService
    {
        public const int SoNgayThuViecToiDa = 60;

        public string Validate(TuyenDungInput x)
        {
            if (x == null) return "Du lieu tiep nhan khong hop le.";
            if (string.IsNullOrWhiteSpace(x.HoTen) || x.HoTen.Trim().Length < 2)
                return "Ho ten phai co it nhat 2 ky tu.";
            if (!LaGioiTinhHopLe(x.GioiTinh))
                return "Gioi tinh chi nhan Nam, Nu hoac Khac.";
            if (x.NgaySinh.Date >= x.NgayBatDau.Date)
                return "Ngay sinh phai truoc ngay bat dau thu viec.";
            if (x.NgaySinh.Date > x.NgayBatDau.Date.AddYears(-18))
                return "Nhan vien phai du 18 tuoi tai ngay bat dau thu viec.";
            if (!LaCCCDHopLe(x.CCCD))
                return "CCCD phai gom dung 12 chu so.";
            if (!LaDienThoaiHopLe(x.DienThoai))
                return "Dien thoai phai gom 9-11 chu so (cho phep dau +).";
            if (!string.IsNullOrWhiteSpace(x.Email) && !x.Email.Contains("@"))
                return "Email khong hop le.";
            if (string.IsNullOrWhiteSpace(x.DiaChi))
                return "Vui long nhap dia chi.";
            if (x.MaPB <= 0)
                return "Vui long chon phong ban co nhu cau tuyen dung.";
            if (x.NgayBanHanh.Date > x.NgayBatDau.Date)
                return "Ngay ban hanh quyet dinh khong duoc sau ngay bat dau thu viec.";
            int soNgay = (x.NgayKetThuc.Date - x.NgayBatDau.Date).Days;
            if (soNgay <= 0 || soNgay > SoNgayThuViecToiDa)
                return "Thoi gian thu viec phai tu 1 den 60 ngay.";
            if (x.MucLuong <= 0)
                return "Muc luong thu viec phai lon hon 0.";
            if (string.IsNullOrWhiteSpace(x.NoiDung))
                return "Noi dung quyet dinh khong duoc de trong.";
            if (string.IsNullOrWhiteSpace(x.NguoiKy))
                return "Vui long nhap nguoi ky quyet dinh.";
            return null;
        }

        public static bool LaGioiTinhHopLe(string gioiTinh)
        {
            return gioiTinh == "Nam" || gioiTinh == "Nu" || gioiTinh == "Khac";
        }

        public static bool LaCCCDHopLe(string cccd)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return false;
            return Regex.IsMatch(cccd.Trim(), @"^\d{12}$");
        }

        public static bool LaDienThoaiHopLe(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;
            string s = sdt.Trim().Replace(" ", "");
            if (s.StartsWith("+")) s = s.Substring(1);
            return Regex.IsMatch(s, @"^\d{9,11}$");
        }

        public static string SinhSoQD(DateTime ngay)
        {
            return "QDTD-" + ngay.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss");
        }

        public ServiceResult TiepNhan(TuyenDungInput x)
        {
            string err = Validate(x);
            if (err != null) return ServiceResult.Fail(err);

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (var cmd = new SqlCommand("dbo.sp_TiepNhanThuViec", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HoTen", x.HoTen.Trim());
                    cmd.Parameters.AddWithValue("@GioiTinh", x.GioiTinh);
                    cmd.Parameters.AddWithValue("@NgaySinh", x.NgaySinh.Date);
                    cmd.Parameters.AddWithValue("@CCCD", x.CCCD.Trim());
                    cmd.Parameters.AddWithValue("@DienThoai", x.DienThoai.Trim());
                    cmd.Parameters.AddWithValue("@DiaChi", x.DiaChi.Trim());
                    cmd.Parameters.AddWithValue("@Email", (object)x.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaPB", x.MaPB);
                    cmd.Parameters.AddWithValue("@SoQD", string.IsNullOrWhiteSpace(x.SoQD) ? SinhSoQD(x.NgayBanHanh) : x.SoQD.Trim());
                    cmd.Parameters.AddWithValue("@NgayBanHanh", x.NgayBanHanh.Date);
                    cmd.Parameters.AddWithValue("@NgayBatDau", x.NgayBatDau.Date);
                    cmd.Parameters.AddWithValue("@NgayKetThuc", x.NgayKetThuc.Date);
                    cmd.Parameters.AddWithValue("@MucLuong", x.MucLuong);
                    cmd.Parameters.AddWithValue("@NoiDung", x.NoiDung.Trim());
                    cmd.Parameters.AddWithValue("@NguoiKy", x.NguoiKy.Trim());
                    var pNV = cmd.Parameters.Add("@MaNV", SqlDbType.Int); pNV.Direction = ParameterDirection.Output;
                    var pQD = cmd.Parameters.Add("@MaQDTD", SqlDbType.Int); pQD.Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    int maNV = pNV.Value == DBNull.Value ? 0 : Convert.ToInt32(pNV.Value);
                    string code = Convert.ToString(Db.Scalar("SELECT MaNVCode FROM dbo.NhanVien WHERE MaNV=@m",
                        new SqlParameter("@m", maNV)));
                    return ServiceResult.Ok("Tiep nhan thu viec thanh cong. Ma nhan vien: " + code, maNV, code);
                }
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail(ChuyenLoiSql(ex));
            }
        }

        private static string ChuyenLoiSql(SqlException ex)
        {
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                string m = ex.Message;
                if (m.Contains("CCCD")) return "CCCD da ton tai trong ho so nhan su.";
                if (m.Contains("DienThoai")) return "So dien thoai da duoc su dung.";
                if (m.Contains("SoQD")) return "So quyet dinh da ton tai.";
                return "Du lieu bi trung: " + ex.Message;
            }
            if (ex.Number >= 50000) return ex.Message;
            return "Loi CSDL: " + ex.Message;
        }

        public DataTable DanhSachPhongBan()
        {
            return Db.Query(@"SELECT MaPB, TenPB, DienThoai, TrangThai
                              FROM dbo.PhongBan WHERE TrangThai = 1 ORDER BY TenPB");
        }

        public DataTable DanhSachNhanVien(string trangThai = null)
        {
            string sql = @"
SELECT nv.MaNV, nv.MaNVCode, nv.HoTen, nv.GioiTinh, nv.NgaySinh, nv.CCCD,
       nv.DienThoai, nv.Email, nv.NgayVaoLam, pb.TenPB, nv.TrangThai
FROM dbo.NhanVien nv
JOIN dbo.PhongBan pb ON pb.MaPB = nv.MaPB
WHERE (@tt IS NULL OR nv.TrangThai = @tt)
ORDER BY nv.MaNVCode";
            return Db.Query(sql, new SqlParameter("@tt", (object)trangThai ?? DBNull.Value));
        }

        public DataTable DanhSachThuViec()
        {
            const string sql = @"
SELECT nv.MaNV, nv.MaNVCode, nv.HoTen, pb.TenPB,
       q.SoQD, q.NgayBatDauThuViec, q.NgayKetThucThuViec,
       q.MucLuongThuViec, DATEDIFF(DAY, CONVERT(DATE, GETDATE()), q.NgayKetThucThuViec) AS SoNgayConLai
FROM dbo.QuyetDinhTuyenDung q
JOIN dbo.NhanVien nv ON nv.MaNV = q.MaNV
JOIN dbo.PhongBan pb ON pb.MaPB = q.MaPB
WHERE q.TrangThai = N'HieuLuc' AND nv.TrangThai = N'ThuViec'
ORDER BY q.NgayKetThucThuViec";
            return Db.Query(sql);
        }

        /// <summary>Thong tin phuc vu form ky hop dong: ngay ket thuc thu viec, luong thu viec.</summary>
        public DataTable LayThongTinThuViec(int maNV)
        {
            const string sql = @"
SELECT TOP 1 nv.MaNV, nv.MaNVCode, nv.HoTen, nv.MaPB, pb.TenPB,
       q.MaQDTD, q.SoQD, q.NgayBatDauThuViec, q.NgayKetThucThuViec, q.MucLuongThuViec
FROM dbo.QuyetDinhTuyenDung q
JOIN dbo.NhanVien nv ON nv.MaNV = q.MaNV
JOIN dbo.PhongBan pb ON pb.MaPB = q.MaPB
WHERE q.MaNV = @m AND q.TrangThai = N'HieuLuc'
ORDER BY q.MaQDTD DESC";
            return Db.Query(sql, new SqlParameter("@m", maNV));
        }

        public DataTable DanhSachQuyetDinhTuyenDung()
        {
            const string sql = @"
SELECT q.MaQDTD, q.SoQD, nv.MaNVCode, nv.HoTen, pb.TenPB,
       q.NgayBanHanh, q.NgayBatDauThuViec, q.NgayKetThucThuViec,
       q.MucLuongThuViec, q.TrangThai
FROM dbo.QuyetDinhTuyenDung q
JOIN dbo.NhanVien nv ON nv.MaNV = q.MaNV
JOIN dbo.PhongBan pb ON pb.MaPB = q.MaPB
ORDER BY q.MaQDTD DESC";
            return Db.Query(sql);
        }
    }
}
