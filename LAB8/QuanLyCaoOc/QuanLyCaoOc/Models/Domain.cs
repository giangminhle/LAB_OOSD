using System;
using System.Collections.Generic;

namespace QuanLyCaoOc.Models
{
    /// <summary>Du lieu nhap de lap hop dong thue nhieu van phong.</summary>
    public class HopDongInput
    {
        public int MaKH { get; set; }
        public List<int> DanhSachPhong { get; set; } = new List<int>();
        public DateTime NgayKy { get; set; }
        public DateTime NgayBatDau { get; set; }
        public int SoThang { get; set; }
        public string NoiDung { get; set; }
        /// <summary>Khach tra truoc 6 thang ngay khi ky.</summary>
        public bool DaTraTruoc { get; set; }
    }

    public class ChiTietDichVuInput
    {
        public int MaPhong { get; set; }
        public int MaDV { get; set; }
        public decimal SoLuong { get; set; }
    }

    /// <summary>Hoa don dich vu (dien, nuoc, internet) lap cuoi thang.</summary>
    public class HoaDonDichVuInput
    {
        public int MaHD { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public DateTime NgayLap { get; set; }
        public List<ChiTietDichVuInput> ChiTiet { get; set; } = new List<ChiTietDichVuInput>();
    }

    public class GiaHanInput
    {
        public int MaCTHD { get; set; }
        public int SoThangGiaHan { get; set; }
        public DateTime NgayLap { get; set; }
        public string NguoiLap { get; set; }
    }

    /// <summary>Thong tin hop dong dung de kiem tra dieu kien gia han.</summary>
    public class ThongTinGiaHan
    {
        public int MaCTHD { get; set; }
        public int MaHD { get; set; }
        public string MaPhongCode { get; set; }
        public string TrangThaiHD { get; set; }
        public int SoThangThue { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SoHoaDonThueNo { get; set; }
        public decimal TienThueNo { get; set; }
        public int SoHoaDonDichVuNo { get; set; }
        public decimal TienDichVuNo { get; set; }
    }

    public class NhanVienInput
    {
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public string HinhAnh { get; set; }
        public int MaKH { get; set; }
        public int MaPhong { get; set; }
        public string ChucVu { get; set; }
        public string DienThoai { get; set; }
        public DateTime NgayBaoCao { get; set; }
    }

    public class TheXeInput
    {
        public int MaNV { get; set; }
        public string LoaiXe { get; set; }
        public string BienSo { get; set; }
        public string MauXe { get; set; }
        public DateTime NgayCap { get; set; }
    }

    public class KiemTraInput
    {
        public int MaPhong { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string TinhTrang { get; set; }
        public bool CoThayDoiKetCau { get; set; }
        public string MoTa { get; set; }
        public string KeHoachTuSua { get; set; }
        public string NguoiKiemTra { get; set; }
        public decimal SoTienPhat { get; set; }
        public string LyDoPhat { get; set; }
    }
}
