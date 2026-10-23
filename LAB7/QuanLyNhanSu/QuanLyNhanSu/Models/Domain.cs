using System;
using System.Collections.Generic;

namespace QuanLyNhanSu.Models
{
    /// <summary>Cac DTO dau vao cho Service (khong phu thuoc UI).</summary>
    public class TuyenDungInput
    {
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string CCCD { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public int MaPB { get; set; }
        public string SoQD { get; set; }
        public DateTime NgayBanHanh { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal MucLuong { get; set; }
        public string NoiDung { get; set; }
        public string NguoiKy { get; set; }
    }

    public class HopDongInput
    {
        public int MaNV { get; set; }
        public string SoHD { get; set; }
        public string LoaiHD { get; set; }
        public DateTime NgayKy { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public decimal MucLuong { get; set; }
        public decimal PhuCapLuong { get; set; }
        public decimal PhuCapDocHai { get; set; }
        public string ChucVu { get; set; }
        public decimal PhuCapChucVu { get; set; }
        public string NguoiKy { get; set; }
    }

    public class ChiTietKeHoachInput
    {
        public int MaNV { get; set; }
        public DateTime Ngay { get; set; }
        public string CaLam { get; set; }
        public bool LaPhuTroi { get; set; }
        public string GhiChu { get; set; }
    }

    public class KeHoachInput
    {
        public int MaPB { get; set; }
        public int TuanSo { get; set; }
        public int Nam { get; set; }
        public string NguoiLap { get; set; }
        public List<ChiTietKeHoachInput> ChiTiet { get; set; }

        public KeHoachInput()
        {
            ChiTiet = new List<ChiTietKeHoachInput>();
        }
    }

    public class ChamCongInput
    {
        public int MaNV { get; set; }
        public DateTime Ngay { get; set; }
        public int? MaCTKH { get; set; }
        public string CaLam { get; set; }
        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }

    public class ChiTietPhuTroiInput
    {
        public int MaNV { get; set; }
        public decimal SoGio { get; set; }
        public decimal HeSo { get; set; }
        public string GhiChu { get; set; }

        public ChiTietPhuTroiInput()
        {
            HeSo = 1.5m;
        }
    }

    public class PhuTroiInput
    {
        public DateTime NgayPhieu { get; set; }
        public string HinhThuc { get; set; }
        public string NguoiLap { get; set; }
        public string GhiChu { get; set; }
        public List<ChiTietPhuTroiInput> ChiTiet { get; set; }

        public PhuTroiInput()
        {
            ChiTiet = new List<ChiTietPhuTroiInput>();
        }
    }

    public class KhenThuongInput
    {
        public DateTime NgayBanHanh { get; set; }
        public string DoiTuong { get; set; }
        public int? MaNV { get; set; }
        public int? MaPB { get; set; }
        public string LyDo { get; set; }
        public string HinhThuc { get; set; }
        public decimal SoTien { get; set; }
        public string NguonChiTra { get; set; }
        public string NguoiKy { get; set; }
    }

    public class KyLuatInput
    {
        public DateTime NgayBanHanh { get; set; }
        public string DoiTuong { get; set; }
        public int? MaNV { get; set; }
        public int? MaPB { get; set; }
        public string LyDo { get; set; }
        public string HinhThuc { get; set; }
        public decimal SoTienPhat { get; set; }
        public string NguonThu { get; set; }
        public string NguoiKy { get; set; }
    }

    public class LuongRow
    {
        public int MaNV { get; set; }
        public string MaNVCode { get; set; }
        public string HoTen { get; set; }
        public int MaPB { get; set; }
        public string TenPB { get; set; }
        public string LoaiHD { get; set; }
        public decimal MucLuongCoBan { get; set; }
        public int SoNgayCong { get; set; }
        public decimal GioPhuTroi { get; set; }
        public decimal TienPhuTroi { get; set; }
        public decimal PhuCapChucVu { get; set; }
        public decimal PhuCapLuong { get; set; }
        public decimal PhuCapDocHai { get; set; }
        public decimal TienKhenThuong { get; set; }
        public decimal TienKyLuat { get; set; }
        public decimal BHXH { get; set; }
        public decimal BHYT { get; set; }
        public decimal BHTN { get; set; }
        public decimal ThucLinh { get; set; }
    }

    public class LuongKetQua
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int MaBangLuong { get; set; }
        public string TrangThai { get; set; }
        public List<LuongRow> Rows { get; set; }
        public decimal TongChiPhi { get; set; }

        public LuongKetQua()
        {
            Rows = new List<LuongRow>();
        }
    }
}
