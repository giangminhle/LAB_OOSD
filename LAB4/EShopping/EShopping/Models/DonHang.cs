using System;

namespace EShopping.Models
{
    public class DonHang
    {
        public int MaDH { get; set; }
        public int MaKH { get; set; }
        public LoaiGiaoHang LoaiGiaoHang { get; set; }

        // Người nhận (có thể khác người mua)
        public string TenNguoiNhan { get; set; }
        public string DiaChiNhan { get; set; }
        public string DienThoaiNhan { get; set; }

        public decimal TongTienHang { get; set; }
        public decimal PhiGiaoHang { get; set; }
        public decimal PhiThe { get; set; }
        public decimal TongThanhToan { get; set; }

        public LoaiThe LoaiThe { get; set; }
        public string SoTheMask { get; set; }
        public string TenChuThe { get; set; }
        public DateTime NgayHetHanThe { get; set; }
        public DateTime ThoiDiemDat { get; set; }
    }

    /// <summary>Thông tin thẻ do người dùng nhập. Không lưu xuống DB (chỉ dùng để xác thực thanh toán).</summary>
    public class ThongTinThe
    {
        public LoaiThe LoaiThe { get; set; }
        public string SoThe { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string TenChuThe { get; set; }
        public string Csv { get; set; }
    }
}
