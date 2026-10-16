using System;
using System.Collections.Generic;
using System.Text;
using QuanLyBenhAn.Models;

namespace QuanLyBenhAn.Services
{
    /// <summary>Kiem thu quy tac nghiep vu (chay offline, khong can DB).</summary>
    public static class RuleTests
    {
        public static string RunAll()
        {
            var sb = new StringBuilder();
            int pass = 0, fail = 0;
            Check(sb, ref pass, ref fail, "DK01_DangKyLanDau_HopLe",
                new BenhNhanService().ValidateDangKy("Nguyen Van A", new DateTime(1990, 1, 1), "123 Ly Thuong Kiet", "0901234567") == null);
            Check(sb, ref pass, ref fail, "DK02_ChanSDT_Sai",
                new BenhNhanService().ValidateDangKy("A", DateTime.Today, "DC", "abc") != null);
            Check(sb, ref pass, ref fail, "DK03_ChanNgaySinh_TuongLai",
                new BenhNhanService().ValidateDangKy("Nguyen Van A", DateTime.Today.AddDays(1), "DC", "0901234567") != null);
            Check(sb, ref pass, ref fail, "LP01_LePhiNam_HopLe",
                new BenhNhanService().ValidateLePhiNam(2026, 500000m) == null);
            Check(sb, ref pass, ref fail, "LP02_LePhiNam_Bang0_BiChan",
                new BenhNhanService().ValidateLePhiNam(2026, 0) != null);
            Check(sb, ref pass, ref fail, "Q01_LichDinhKy_CachNhau1Quy_HopLe",
                new BenhNhanService().ValidateLichDinhKy(new DateTime(2026, 1, 10), new DateTime(2026, 4, 15)) == null);
            Check(sb, ref pass, ref fail, "Q02_LichDinhKy_CachNhau2Tuan_BiChan",
                new BenhNhanService().ValidateLichDinhKy(new DateTime(2026, 1, 10), new DateTime(2026, 1, 25)) != null);
            Check(sb, ref pass, ref fail, "K01_PhieuKham_HopLe",
                new KhamBenhService().ValidatePhieuKham(new PhieuKhamInput
                { MaHoSo = 1, MaBS = 1, MaKhoa = 1, ThanNhiet = 37.5m, HuyetAp = "120/80" }) == null);
            Check(sb, ref pass, ref fail, "K02_ThanNhiet50_BiChan",
                new KhamBenhService().ValidatePhieuKham(new PhieuKhamInput
                { MaHoSo = 1, MaBS = 1, MaKhoa = 1, ThanNhiet = 50m }) != null);
            Check(sb, ref pass, ref fail, "XN01_XetNghiem_PhaiCoMaPhieu",
                new KhamBenhService().ValidateXetNghiem(0, "Mau") != null);
            Check(sb, ref pass, ref fail, "T01_ToaThuoc_PhaiCoThuoc",
                new KhamBenhService().ValidateToa(5, new List<ChiTietToaInput>()) != null);
            Check(sb, ref pass, ref fail, "T02_ToaThuoc_PhaiLienKetPhieu",
                new KhamBenhService().ValidateToa(0, new List<ChiTietToaInput>
                { new ChiTietToaInput { MaThuoc = 1, LieuLuong = "1 vien/lan", SoLuong = 10 } }) != null);
            Check(sb, ref pass, ref fail, "T03_ToaThuoc_HopLe",
                new KhamBenhService().ValidateToa(5, new List<ChiTietToaInput>
                { new ChiTietToaInput { MaThuoc = 1, LieuLuong = "1 vien/lan", SoLuong = 10 } }) == null);
            Check(sb, ref pass, ref fail, "HT01_HoanTat_PhaiThanhToan",
                new KhamBenhService().CanHoanTat(false, true) != null);
            Check(sb, ref pass, ref fail, "HT02_HoanTat_PhaiCoHoaDon",
                new KhamBenhService().CanHoanTat(true, false) != null);
            Check(sb, ref pass, ref fail, "HT03_HoanTat_HopLe",
                new KhamBenhService().CanHoanTat(true, true) == null);
            sb.AppendLine(string.Format("TONG: {0} pass / {1} fail tren {2} case.", pass, fail, pass + fail));
            return sb.ToString();
        }

        private static void Check(StringBuilder sb, ref int pass, ref int fail, string id, bool ok)
        {
            if (ok) { pass++; sb.AppendLine("PASS " + id); }
            else { fail++; sb.AppendLine("FAIL " + id); }
        }
    }
}
