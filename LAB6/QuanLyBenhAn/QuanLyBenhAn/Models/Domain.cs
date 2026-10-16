using System;

namespace QuanLyBenhAn.Models
{
    public enum LoaiKham : byte { DieuTri = 1, DinhKyQuy = 2 }
    public enum TrangThaiPhieu : byte { ChoKham = 0, DaKham = 1, DaThanhToan = 2, HoanTat = 3 }
    public enum TrangThaiLichHen : byte { DaHen = 0, DaKham = 1, Huy = 2 }

    public class BenhNhan
    {
        public int MaBN { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Anh3x4 { get; set; }
    }

    public class PhieuKhamInput
    {
        public int MaHoSo { get; set; }
        public int MaBS { get; set; }
        public int MaKhoa { get; set; }
        public string TrieuChung { get; set; }
        public decimal? ThanNhiet { get; set; }
        public string HuyetAp { get; set; }
        public string ChanDoan { get; set; }
        public LoaiKham LoaiKham { get; set; }
    }

    public class ChiTietToaInput
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string LieuLuong { get; set; }
        public int SoLuong { get; set; }
        public string CachDung { get; set; }
    }
}
