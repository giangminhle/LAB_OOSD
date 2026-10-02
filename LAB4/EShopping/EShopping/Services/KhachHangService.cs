using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using EShopping.Data;
using EShopping.Models;

namespace EShopping.Services
{
    public class KhachHangService
    {
        /// <summary>Validate -> kiểm tra trùng -> băm mật khẩu -> lưu xuống Data.</summary>
        public ServiceResult DangKy(KhachHang kh)
        {
            string loi = Validate(kh);
            if (loi != null) return ServiceResult.Fail(loi);

            try
            {
                if (TonTai("TenDangNhap", kh.TenDangNhap))
                    return ServiceResult.Fail("Tên đăng nhập đã tồn tại.");
                if (TonTai("SoCMND", kh.SoCMND))
                    return ServiceResult.Fail("Số CMND/Passport đã được đăng ký.");

                const string sql = @"
INSERT INTO KhachHang (HoTen, NgaySinh, SoCMND, DiaChi, DienThoai, TenDangNhap, MatKhauHash, Email)
OUTPUT INSERTED.MaKH
VALUES (@HoTen, @NgaySinh, @SoCMND, @DiaChi, @DienThoai, @TenDangNhap, @MatKhauHash, @Email)";

                object id = Db.ExecuteScalar(sql,
                    new SqlParameter("@HoTen", kh.HoTen.Trim()),
                    new SqlParameter("@NgaySinh", kh.NgaySinh.Date),
                    new SqlParameter("@SoCMND", kh.SoCMND.Trim()),
                    new SqlParameter("@DiaChi", kh.DiaChi.Trim()),
                    new SqlParameter("@DienThoai", kh.DienThoai.Trim()),
                    new SqlParameter("@TenDangNhap", kh.TenDangNhap.Trim()),
                    new SqlParameter("@MatKhauHash", HashPassword(kh.MatKhau)),
                    new SqlParameter("@Email", string.IsNullOrWhiteSpace(kh.Email)
                                                ? (object)DBNull.Value : kh.Email.Trim()));

                return ServiceResult.Ok("Đăng ký tài khoản thành công!", Convert.ToInt32(id));
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi cơ sở dữ liệu: " + ex.Message);
            }
        }

        // ---------------- Nghiệp vụ ----------------
        private string Validate(KhachHang kh)
        {
            if (kh == null) return "Dữ liệu không hợp lệ.";
            if (string.IsNullOrWhiteSpace(kh.HoTen)) return "Vui lòng nhập họ tên.";
            if (kh.NgaySinh.Date >= DateTime.Today) return "Ngày sinh không hợp lệ.";
            if (kh.NgaySinh.Date > DateTime.Today.AddYears(-16)) return "Khách hàng phải từ 16 tuổi trở lên.";
            if (string.IsNullOrWhiteSpace(kh.SoCMND) || !Regex.IsMatch(kh.SoCMND.Trim(), @"^[A-Za-z0-9]{8,12}$"))
                return "Số CMND/Passport không hợp lệ (8-12 ký tự chữ/số).";
            if (string.IsNullOrWhiteSpace(kh.DiaChi)) return "Vui lòng nhập địa chỉ.";
            if (string.IsNullOrWhiteSpace(kh.DienThoai) || !Regex.IsMatch(kh.DienThoai.Trim(), @"^(0|\+84)\d{9,10}$"))
                return "Số điện thoại không hợp lệ.";
            if (string.IsNullOrWhiteSpace(kh.TenDangNhap) || !Regex.IsMatch(kh.TenDangNhap.Trim(), @"^[A-Za-z0-9_]{4,30}$"))
                return "Tên đăng nhập 4-30 ký tự (chữ, số, gạch dưới).";
            if (string.IsNullOrEmpty(kh.MatKhau) || kh.MatKhau.Length < 6)
                return "Mật khẩu tối thiểu 6 ký tự.";
            if (!string.IsNullOrWhiteSpace(kh.Email) &&
                !Regex.IsMatch(kh.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Email không hợp lệ.";
            return null;
        }

        private bool TonTai(string cot, string giaTri)
        {
            // 'cot' là hằng do code truyền vào (không phải input người dùng) nên an toàn khi nối chuỗi
            object n = Db.ExecuteScalar("SELECT COUNT(1) FROM KhachHang WHERE " + cot + " = @v",
                                        new SqlParameter("@v", giaTri.Trim()));
            return Convert.ToInt32(n) > 0;
        }

        /// <summary>PBKDF2 + salt ngẫu nhiên. Định dạng lưu: base64(salt).base64(hash)</summary>
        private static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
            }
        }
    }
}
