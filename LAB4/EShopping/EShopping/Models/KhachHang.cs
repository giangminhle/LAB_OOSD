using System;

namespace EShopping.Models
{
    public class KhachHang
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SoCMND { get; set; }          // CMND/CCCD/Passport
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }         // mật khẩu thô, chỉ tồn tại trong bộ nhớ; DB lưu hash
        public string Email { get; set; }
    }
}
