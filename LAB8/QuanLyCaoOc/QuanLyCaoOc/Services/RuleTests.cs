using System;
using System.Collections.Generic;
using System.Text;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>
    /// Kiem thu quy tac nghiep vu offline (khong can CSDL).
    /// Dung chung cho nut "Kiem thu quy tac" va project console Tests.
    /// </summary>
    public static class RuleTests
    {
        public static string RunAll()
        {
            var sb = new StringBuilder();
            int pass = 0, fail = 0;

            // --- Gia thue phong theo dien tich va do cao ---
            Check(sb, ref pass, ref fail, "P01_Gia_80m2_Tang1_20Trieu", PhongService.TinhGiaThue(80, 1.00m) == 20000000m);
            Check(sb, ref pass, ref fail, "P02_Gia_50m2_Tang2_13125000", PhongService.TinhGiaThue(50, 1.05m) == 13125000m);
            Check(sb, ref pass, ref fail, "P03_HeSo_Tang5_1_20", PhongService.HeSoTheoTang(5) == 1.20m);
            Check(sb, ref pass, ref fail, "P04_Gia_DienTich0_Bang0", PhongService.TinhGiaThue(0, 1m) == 0m);

            // --- Hop dong thue ---
            var hd = new HopDongService();
            Check(sb, ref pass, ref fail, "HD01_HopDong_HopLe", hd.Validate(HopDongHopLe()) == null);
            Check(sb, ref pass, ref fail, "HD02_ChanThieuKhachHang", hd.Validate(HD(x => x.MaKH = 0)) != null);
            Check(sb, ref pass, ref fail, "HD03_ChanKhongCoPhong", hd.Validate(HD(x => x.DanhSachPhong.Clear())) != null);
            Check(sb, ref pass, ref fail, "HD04_ChanTrungPhong", hd.Validate(HD(x => x.DanhSachPhong.Add(4))) != null);
            Check(sb, ref pass, ref fail, "HD05_ChanThue5Thang", hd.Validate(HD(x => x.SoThang = 5)) != null);
            Check(sb, ref pass, ref fail, "HD06_Thue6Thang_HopLe", hd.Validate(HD(x => x.SoThang = 6)) == null);
            Check(sb, ref pass, ref fail, "HD07_ChanBatDauNgay15", hd.Validate(HD(x => x.NgayBatDau = new DateTime(2026, 11, 15))) != null);
            Check(sb, ref pass, ref fail, "HD08_ChanNgayKySauBatDau", hd.Validate(HD(x => x.NgayKy = new DateTime(2026, 11, 2))) != null);
            Check(sb, ref pass, ref fail, "HD09_KetThuc_12Thang_31_12_2026",
                HopDongService.TinhNgayKetThuc(new DateTime(2026, 1, 1), 12) == new DateTime(2026, 12, 31));
            Check(sb, ref pass, ref fail, "HD10_TraTruoc_2Phong_210Trieu",
                HopDongService.TinhTienTraTruoc(new[] { 20000000m, 15000000m }) == 210000000m);

            // --- Tien thue hang thang tu thang thu 7 ---
            DateTime bd = new DateTime(2026, 1, 1), kt = new DateTime(2026, 12, 31);
            Check(sb, ref pass, ref fail, "TT01_Thang7_2026_LaThangThu7", ThanhToanService.ThangThu(bd, 7, 2026) == 7);
            Check(sb, ref pass, ref fail, "TT02_ChanThang6_DaTraTruoc",
                ThanhToanService.ValidateHoaDonThang(bd, kt, "HieuLuc", 6, 2026) != null);
            Check(sb, ref pass, ref fail, "TT03_Thang7_HopLe",
                ThanhToanService.ValidateHoaDonThang(bd, kt, "HieuLuc", 7, 2026) == null);
            Check(sb, ref pass, ref fail, "TT04_ChanSauNgayKetThuc",
                ThanhToanService.ValidateHoaDonThang(bd, kt, "HieuLuc", 1, 2027) != null);
            Check(sb, ref pass, ref fail, "TT05_ChanHopDongDaKetThuc",
                ThanhToanService.ValidateHoaDonThang(bd, kt, "KetThuc", 8, 2026) != null);

            // --- Hoa don dich vu cuoi thang ---
            var tt = new ThanhToanService();
            Check(sb, ref pass, ref fail, "DV01_HoaDonDichVu_HopLe", tt.ValidateDichVu(DichVuHopLe()) == null);
            Check(sb, ref pass, ref fail, "DV02_ChanLapTruocCuoiThang",
                tt.ValidateDichVu(DV(x => x.NgayLap = new DateTime(2026, 10, 20))) != null);
            Check(sb, ref pass, ref fail, "DV03_ChanSoLuong0", tt.ValidateDichVu(DV(x => x.ChiTiet[0].SoLuong = 0)) != null);
            Check(sb, ref pass, ref fail, "DV04_ChanTrungDong",
                tt.ValidateDichVu(DV(x => x.ChiTiet.Add(new ChiTietDichVuInput { MaPhong = 1, MaDV = 1, SoLuong = 5 }))) != null);
            Check(sb, ref pass, ref fail, "DV05_Dien1200kWh_4200000", ThanhToanService.ThanhTien(1200, 3500) == 4200000m);

            // --- Gia han ---
            Check(sb, ref pass, ref fail, "GH01_ToiDa_Thue12_La6_Thue7_La3",
                GiaHanService.SoThangGiaHanToiDa(12) == 6 && GiaHanService.SoThangGiaHanToiDa(7) == 3);
            DateTime ngay = new DateTime(2026, 10, 30);
            Check(sb, ref pass, ref fail, "GH02_DuDieuKien_HopLe", GiaHanService.KiemTraDieuKien(GH(), 6, ngay) == null);
            Check(sb, ref pass, ref fail, "GH03_ChanQuaNuaThoiGianThue", GiaHanService.KiemTraDieuKien(GH(), 7, ngay) != null);
            Check(sb, ref pass, ref fail, "GH04_ChanConNoTienThue",
                GiaHanService.KiemTraDieuKien(GH(t => { t.SoHoaDonThueNo = 1; t.TienThueNo = 26250000; }), 3, ngay) != null);
            Check(sb, ref pass, ref fail, "GH05_ChanChuaTraDichVu",
                GiaHanService.KiemTraDieuKien(GH(t => { t.SoHoaDonDichVuNo = 1; t.TienDichVuNo = 6150000; }), 3, ngay) != null);
            Check(sb, ref pass, ref fail, "GH06_ChanDaHetHanThue",
                GiaHanService.KiemTraDieuKien(GH(t => t.NgayKetThuc = new DateTime(2026, 9, 30)), 3, ngay) != null);
            Check(sb, ref pass, ref fail, "GH07_NgayKetThucMoi_DungCuoiThang",
                GiaHanService.TinhNgayKetThucMoi(new DateTime(2026, 11, 30), 3) == new DateTime(2027, 2, 28)
                && GiaHanService.TinhNgayKetThucMoi(new DateTime(2027, 2, 28), 3) == new DateTime(2027, 5, 31));
            Check(sb, ref pass, ref fail, "GH08_ChanSoThang0", new GiaHanService().Validate(new GiaHanInput
            { MaCTHD = 1, SoThangGiaHan = 0, NgayLap = ngay, NguoiLap = "BQL" }) != null);

            // --- Nhan vien van phong, the giu xe ---
            var nv = new NhanVienService();
            Check(sb, ref pass, ref fail, "NV01_NhanVien_HopLe", nv.Validate(NhanVienHopLe()) == null);
            Check(sb, ref pass, ref fail, "NV02_ChanCCCD_11So", nv.Validate(NV(x => x.CCCD = "07909000100")) != null);
            Check(sb, ref pass, ref fail, "NV03_ChanHoTenNgan", nv.Validate(NV(x => x.HoTen = "A")) != null);
            Check(sb, ref pass, ref fail, "TX01_TheXe_HopLe", nv.ValidateTheXe(TheXe("XeMay", "59X1-234.56")) == null);
            Check(sb, ref pass, ref fail, "TX02_ChanBienSoSai", nv.ValidateTheXe(TheXe("XeMay", "ABC123")) != null);
            Check(sb, ref pass, ref fail, "TX03_ChanLoaiXe_KhongHopLe", nv.ValidateTheXe(TheXe("XeTai", "51A-123.45")) != null);

            // --- Kiem tra phong, phat thay doi ket cau ---
            var ktp = new KiemTraPhongService();
            Check(sb, ref pass, ref fail, "KT01_KiemTra_HopLe", ktp.Validate(KiemTraHopLe()) == null);
            Check(sb, ref pass, ref fail, "KT02_ChanCanSua_ThieuKeHoach",
                ktp.Validate(KT(x => { x.TinhTrang = "CanSua"; x.KeHoachTuSua = ""; })) != null);
            Check(sb, ref pass, ref fail, "KT03_ChanThayDoiKetCau_KhongPhat",
                ktp.Validate(KT(x => { x.CoThayDoiKetCau = true; x.SoTienPhat = 0; x.LyDoPhat = "Dap vach"; })) != null);
            Check(sb, ref pass, ref fail, "KT04_MotNamMotLan",
                KiemTraPhongService.DaKiemTraTrongNam(new DateTime(2026, 6, 15), new DateTime(2026, 12, 1))
                && !KiemTraPhongService.DaKiemTraTrongNam(new DateTime(2025, 6, 10), new DateTime(2026, 6, 10)));

            sb.AppendLine("TONG: " + pass + " pass / " + fail + " fail tren " + (pass + fail) + " case.");
            return sb.ToString();
        }

        private static void Check(StringBuilder sb, ref int pass, ref int fail, string id, bool ok)
        {
            if (ok) { pass++; sb.AppendLine("PASS " + id); }
            else { fail++; sb.AppendLine("FAIL " + id); }
        }

        // ---------- du lieu mau ----------
        private static HopDongInput HopDongHopLe()
        {
            return new HopDongInput
            {
                MaKH = 2, DanhSachPhong = new List<int> { 4, 7 },
                NgayKy = new DateTime(2026, 10, 25), NgayBatDau = new DateTime(2026, 11, 1), SoThang = 12
            };
        }

        private static HopDongInput HD(Action<HopDongInput> f) { var x = HopDongHopLe(); f(x); return x; }

        private static HoaDonDichVuInput DichVuHopLe()
        {
            return new HoaDonDichVuInput
            {
                MaHD = 1, Thang = 10, Nam = 2026, NgayLap = new DateTime(2026, 10, 31),
                ChiTiet = new List<ChiTietDichVuInput>
                {
                    new ChiTietDichVuInput { MaPhong = 1, MaDV = 1, SoLuong = 1000 },
                    new ChiTietDichVuInput { MaPhong = 1, MaDV = 3, SoLuong = 1 }
                }
            };
        }

        private static HoaDonDichVuInput DV(Action<HoaDonDichVuInput> f) { var x = DichVuHopLe(); f(x); return x; }

        private static ThongTinGiaHan GH(Action<ThongTinGiaHan> f = null)
        {
            var t = new ThongTinGiaHan
            {
                MaCTHD = 1, MaHD = 1, MaPhongCode = "P0101", TrangThaiHD = "HieuLuc",
                SoThangThue = 12, NgayKetThuc = new DateTime(2026, 12, 31)
            };
            f?.Invoke(t);
            return t;
        }

        private static NhanVienInput NhanVienHopLe()
        {
            return new NhanVienInput
            {
                HoTen = "Nguyen Thi Lan", CCCD = "079099007007", MaKH = 1, MaPhong = 1,
                ChucVu = "Nhan vien", DienThoai = "0909000111", NgayBaoCao = new DateTime(2026, 10, 30)
            };
        }

        private static NhanVienInput NV(Action<NhanVienInput> f) { var x = NhanVienHopLe(); f(x); return x; }

        private static TheXeInput TheXe(string loai, string bienSo)
        {
            return new TheXeInput { MaNV = 4, LoaiXe = loai, BienSo = bienSo, NgayCap = new DateTime(2026, 10, 30) };
        }

        private static KiemTraInput KiemTraHopLe()
        {
            return new KiemTraInput
            {
                MaPhong = 1, NgayKiemTra = new DateTime(2026, 10, 30), TinhTrang = "Tot",
                NguoiKiemTra = "Ban quan ly"
            };
        }

        private static KiemTraInput KT(Action<KiemTraInput> f) { var x = KiemTraHopLe(); f(x); return x; }
    }
}
