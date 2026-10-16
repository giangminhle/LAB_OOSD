using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>
    /// Nghiep vu dang ky benh nhan lan dau + le phi nam + cap ho so.
    /// Cac ham Validate* la thuan (pure) de kiem thu khong can DB.
    /// </summary>
    public class BenhNhanService
    {
        public const decimal LePhiNamMacDinh = 500000m;   // gia dinh, de 1 cho de chinh
        public const int KhoangCachQuyToiThieuNgay = 80;  // lich dinh ky theo quy: >= ~80 ngay

        public string ValidateDangKy(string hoTen, DateTime ngaySinh, string diaChi, string dienThoai)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || hoTen.Trim().Length < 2)
                return "Ho ten khong hop le.";
            if (ngaySinh.Date > DateTime.Today)
                return "Ngay sinh khong the la ngay trong tuong lai.";
            if (string.IsNullOrWhiteSpace(diaChi))
                return "Vui long nhap dia chi.";
            if (!LaSdtHopLe(dienThoai))
                return "So dien thoai chi gom 9-11 chu so (cho phep dau +).";
            return null;
        }

        public static bool LaSdtHopLe(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;
            string s = sdt.Trim().Replace(" ", "");
            if (s.StartsWith("+")) s = s.Substring(1);
            if (s.Length < 9 || s.Length > 11) return false;
            return Regex.IsMatch(s, @"^\d+$");
        }

        public string ValidateLePhiNam(int nam, decimal soTien)
        {
            if (nam < 2000 || nam > 2100) return "Nam le phi khong hop le.";
            if (soTien <= 0) return "Le phi nam phai > 0.";
            return null;
        }

        /// <summary>Kiem tra lich dinh ky theo quy: moi dot cach nhau it nhat ~80 ngay.</summary>
        public string ValidateLichDinhKy(DateTime lanCuoi, DateTime lanHenMoi)
        {
            if (lanHenMoi <= lanCuoi) return "Ngay hen moi phai sau lan kham truoc.";
            if ((lanHenMoi - lanCuoi).TotalDays < KhoangCachQuyToiThieuNgay)
                return "Lich dinh ky theo quy: moi dot cach nhau it nhat 80 ngay.";
            return null;
        }

        /// <summary>Dang ky tron goi trong 1 TRANSACTION (goi stored procedure).</summary>
        public ServiceResult DangKyLanDau(string hoTen, DateTime ngaySinh, string diaChi,
            string dienThoai, string anh3x4, int nam, decimal soTien, string nguoiThu)
        {
            string err = ValidateDangKy(hoTen, ngaySinh, diaChi, dienThoai)
                      ?? ValidateLePhiNam(nam, soTien);
            if (err != null) return ServiceResult.Fail(err);

            try
            {
                using (var conn = new SqlConnection(Db.ConnectionString))
                using (var cmd = new SqlCommand("dbo.sp_DangKyBenhNhan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HoTen", hoTen.Trim());
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh.Date);
                    cmd.Parameters.AddWithValue("@DiaChi", diaChi.Trim());
                    cmd.Parameters.AddWithValue("@DienThoai", dienThoai.Trim());
                    cmd.Parameters.AddWithValue("@Anh3x4", (object)anh3x4 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    cmd.Parameters.AddWithValue("@SoTien", soTien);
                    cmd.Parameters.AddWithValue("@NguoiThu", (object)nguoiThu ?? DBNull.Value);
                    var pBN = cmd.Parameters.Add("@MaBN", SqlDbType.Int); pBN.Direction = ParameterDirection.Output;
                    var pHS = cmd.Parameters.Add("@MaHoSo", SqlDbType.Int); pHS.Direction = ParameterDirection.Output;
                    var pCode = cmd.Parameters.Add("@MaHSCode", SqlDbType.VarChar, 20); pCode.Direction = ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int maBN = pBN.Value == DBNull.Value ? 0 : (int)pBN.Value;
                    string code = pCode.Value == DBNull.Value ? "" : (string)pCode.Value;
                    return ServiceResult.Ok("Dang ky thanh cong. Ma ho so: " + code, maBN);
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return ServiceResult.Fail("So dien thoai da dang ky ho so truoc do.");
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        public DataTable TraCuuHoSo(string tuKhoa)
        {
            const string sql = @"
SELECT TOP 50 hs.MaHoSo, hs.MaHSCode, bn.MaBN, bn.HoTen, bn.NgaySinh, bn.DienThoai, hs.NgayCap
FROM dbo.HoSoBenhAn hs JOIN dbo.BenhNhan bn ON bn.MaBN = hs.MaBN
WHERE hs.MaHSCode LIKE @k OR bn.HoTen LIKE @k OR bn.DienThoai LIKE @k
ORDER BY hs.MaHoSo DESC";
            return Db.ExecuteQuery(sql, new SqlParameter("@k", "%" + (tuKhoa ?? "").Trim() + "%"));
        }

        public DataTable LichSuKham(int maHoSo)
        {
            const string sql = @"
SELECT pk.MaPhieuKham, pk.NgayGioKham, bs.HoTen AS BacSi, k.TenKhoa,
       pk.TrieuChung, pk.ChanDoan, pk.LoaiKham, pk.TrangThai,
       (SELECT COUNT(*) FROM dbo.XetNghiem x WHERE x.MaPhieuKham = pk.MaPhieuKham) AS SoXN,
       (SELECT COUNT(*) FROM dbo.ToaThuoc t WHERE t.MaPhieuKham = pk.MaPhieuKham) AS SoToa
FROM dbo.PhieuKham pk
JOIN dbo.BacSi bs ON bs.MaBS = pk.MaBS
JOIN dbo.Khoa k ON k.MaKhoa = pk.MaKhoa
WHERE pk.MaHoSo = @hs ORDER BY pk.NgayGioKham DESC";
            return Db.ExecuteQuery(sql, new SqlParameter("@hs", maHoSo));
        }
    }
}
