using System;
using System.Collections.Generic;
using System.Text;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
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

            var td = new TuyenDungService();
            Check(sb, ref pass, ref fail, "TD01_TiepNhan_HopLe", td.Validate(TuyenDungHopLe()) == null);
            Check(sb, ref pass, ref fail, "TD02_ChanHoTen_Rong", td.Validate(With(t => t.HoTen = "A")) != null);
            Check(sb, ref pass, ref fail, "TD03_ChanDuoi18Tuoi", td.Validate(With(t =>
                t.NgaySinh = t.NgayBatDau.AddYears(-17))) != null);
            Check(sb, ref pass, ref fail, "TD04_ChanCCCD_11So", td.Validate(With(t => t.CCCD = "07909000500")) != null);
            Check(sb, ref pass, ref fail, "TD05_ChanThuViec_Qua60Ngay", td.Validate(With(t =>
                t.NgayKetThuc = t.NgayBatDau.AddDays(70))) != null);
            Check(sb, ref pass, ref fail, "TD06_ChanThuViec_0Ngay", td.Validate(With(t =>
                t.NgayKetThuc = t.NgayBatDau)) != null);
            Check(sb, ref pass, ref fail, "TD07_ChanNgayQD_SauNgayBatDau", td.Validate(With(t =>
                t.NgayBanHanh = t.NgayBatDau.AddDays(1))) != null);
            Check(sb, ref pass, ref fail, "TD08_ChanDienThoai_Sai", td.Validate(With(t => t.DienThoai = "abc")) != null);
            Check(sb, ref pass, ref fail, "TD09_ChinhXac18Tuoi_HopLe", td.Validate(With(t =>
                t.NgaySinh = t.NgayBatDau.AddYears(-18))) == null);

            var hd = new HopDongService();
            DateTime probationEnd = new DateTime(2026, 11, 30);
            Check(sb, ref pass, ref fail, "HD01_DaiHan_HopLe", hd.Validate(HopDongDaiHan(), probationEnd) == null);
            Check(sb, ref pass, ref fail, "HD02_ChanHieuLuc_TruocKetThucThuViec", hd.Validate(HopDongDaiHan(
                new DateTime(2026, 11, 25)), probationEnd) != null);
            Check(sb, ref pass, ref fail, "HD03_ChanDaiHan_CoNgayHetHan",
                hd.Validate(HopDongDaiHan(null, new DateTime(2029, 12, 1)), probationEnd) != null);
            Check(sb, ref pass, ref fail, "HD04_ChanNganHan_ThieuNgayHetHan",
                hd.Validate(HopDongNganHan(null), probationEnd) != null);
            Check(sb, ref pass, ref fail, "HD05_ChanNganHan_Qua12Thang",
                hd.Validate(HopDongNganHan(new DateTime(2028, 1, 1)), probationEnd) != null);
            Check(sb, ref pass, ref fail, "HD06_NganHan_6Thang_HopLe",
                hd.Validate(HopDongNganHan(new DateTime(2027, 5, 31)), probationEnd) == null);
            Check(sb, ref pass, ref fail, "HD07_NganHan_KhongLapBaoHiem",
                HopDongService.CanLapBaoHiem(HopDongService.NganHan) == false);
            Check(sb, ref pass, ref fail, "HD08_DaiHan_BatBuocBaoHiem",
                HopDongService.CanLapBaoHiem(HopDongService.DaiHan));

            var kh = new KeHoachService();
            Check(sb, ref pass, ref fail, "KH01_Tuan42_BatDau12_10_2026",
                KeHoachService.NgayDauTuan(42, 2026) == new DateTime(2026, 10, 12));
            Check(sb, ref pass, ref fail, "KH02_KeHoach_HopLe", kh.Validate(KeHoachHopLe()) == null);
            Check(sb, ref pass, ref fail, "KH03_ChanNgayNgoaiTuan", kh.Validate(KeHoachVoiNgay(
                new DateTime(2026, 10, 19))) != null);
            Check(sb, ref pass, ref fail, "KH04_ChanPhanCongTrung", kh.Validate(KeHoachTrung()) != null);
            Check(sb, ref pass, ref fail, "KH05_ChanCaLam_Sai", kh.Validate(KeHoachVoiCa("Dem")) != null);

            var cc = new ChamCongService();
            Check(sb, ref pass, ref fail, "CC01_ChamCong_HopLe", cc.Validate(ChamCongHopLe("DiLam")) == null);
            Check(sb, ref pass, ref fail, "CC02_ChanGioRa_TruocGioVao",
                cc.Validate(ChamCongVoiGio(new TimeSpan(17, 0, 0), new TimeSpan(8, 0, 0))) != null);
            Check(sb, ref pass, ref fail, "CC03_Vang_KhongGio_HopLe",
                cc.Validate(ChamCongVang("Vang", null, null)) == null);
            Check(sb, ref pass, ref fail, "CC04_ChanVang_CoGio",
                cc.Validate(ChamCongVang("Vang", new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0))) != null);
            Check(sb, ref pass, ref fail, "CC05_TinhGio_08Den17_La8",
                ChamCongService.TinhSoGioLam(new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)) == 8m);
            Check(sb, ref pass, ref fail, "CC06_TinhGio_08Den12_La4",
                ChamCongService.TinhSoGioLam(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0)) == 4m);

            var pt = new PhuTroiService();
            Check(sb, ref pass, ref fail, "PT01_PhieuPhuTroi_HopLe", pt.Validate(PhuTroiHopLe()) == null);
            Check(sb, ref pass, ref fail, "PT02_ChanSoGio_0", pt.Validate(PhuTroiVoiGio(0)) != null);
            Check(sb, ref pass, ref fail, "PT03_ChanSoGio_13", pt.Validate(PhuTroiVoiGio(13)) != null);
            Check(sb, ref pass, ref fail, "PT04_ChanHeSo_Khac1_5", pt.Validate(PhuTroiVoiHeSo(1.4m)) != null);
            Check(sb, ref pass, ref fail, "PT05_ChanTrungNhanVien", pt.Validate(PhuTroiTrung()) != null);
            Check(sb, ref pass, ref fail, "PT06_Tien8Gio_Luong15Tr_La865385",
                PhuTroiService.TinhTienPhuTroi(15000000m, 8m) == 865385m);

            var kt = new KhenThuongService();
            Check(sb, ref pass, ref fail, "KT01_KhenThuongCaNhan_HopLe",
                kt.ValidateKhenThuong(KhenThuongCaNhan()) == null);
            Check(sb, ref pass, ref fail, "KT02_KhenThuongTapThe_HopLe",
                kt.ValidateKhenThuong(KhenThuongTapThe()) == null);
            Check(sb, ref pass, ref fail, "KT03_ChanCaNhan_ChonPhongBan",
                kt.ValidateKhenThuong(KhenThuongSaiMucTieu()) != null);
            Check(sb, ref pass, ref fail, "KT04_ChanTienThuong_0",
                kt.ValidateKhenThuong(KhenThuongVoiTien(0)) != null);
            Check(sb, ref pass, ref fail, "KL01_KyLuatCaNhan_HopLe",
                kt.ValidateKyLuat(KyLuatCaNhan()) == null);
            Check(sb, ref pass, ref fail, "KL02_ChanKyLuat_SaiNguon",
                kt.ValidateKyLuat(KyLuatSaiNguon()) != null);
            Check(sb, ref pass, ref fail, "KT05_ChiaThuongTapThe_6Tr_2NV_La3Tr",
                KhenThuongService.ChiaDeu(6000000m, 2) == 3000000m);

            Check(sb, ref pass, ref fail, "L01_Luong26Ngay_15Tr", LuongService.TinhLuongTheoNgay(15000000m, 26) == 15000000m);
            Check(sb, ref pass, ref fail, "L02_Luong13Ngay_15Tr_La7_5Tr", LuongService.TinhLuongTheoNgay(15000000m, 13) == 7500000m);
            Check(sb, ref pass, ref fail, "L03_Luong30Ngay_ChiTinh26", LuongService.TinhLuongTheoNgay(15000000m, 30) == 15000000m);
            Check(sb, ref pass, ref fail, "L04_PhuTroi8Gio_15Tr_La865385", LuongService.TinhPhuTroi(15000000m, 8m) == 865385m);
            Check(sb, ref pass, ref fail, "L05_BHXH8Pct_15Tr_La1_2Tr", LuongService.TinhKhauTruBaoHiem(15000000m, 8m) == 1200000m);
            Check(sb, ref pass, ref fail, "L06_BHYT1_5Pct_15Tr_La225000", LuongService.TinhKhauTruBaoHiem(15000000m, 1.5m) == 225000m);
            Check(sb, ref pass, ref fail, "L07_ThucLinh_NV1_Thang10_La19_440_385",
                LuongService.TinhThucLinh(15000000m, 865385m, 1500000m, 1000000m, 500000m,
                    2000000m, 0m, 1200000m, 225000m, 0m) == 19440385m);
            Check(sb, ref pass, ref fail, "L08_ThucLinh_KhongAm",
                LuongService.TinhThucLinh(1000000m, 0m, 0m, 0m, 0m, 0m, 5000000m, 0m, 0m, 0m) == 0m);

            sb.AppendLine(string.Format("TONG: {0} pass / {1} fail tren {2} case.", pass, fail, pass + fail));
            return sb.ToString();
        }

        private static TuyenDungInput TuyenDungHopLe()
        {
            return new TuyenDungInput
            {
                HoTen = "Nguyen Van A", GioiTinh = "Nam", NgaySinh = new DateTime(1990, 5, 12),
                CCCD = "079090005001", DienThoai = "0901234567", DiaChi = "12 Ly Thuong Kiet",
                Email = "a@sonDongA.vn", MaPB = 1,
                NgayBanHanh = new DateTime(2026, 10, 1), NgayBatDau = new DateTime(2026, 10, 5),
                NgayKetThuc = new DateTime(2026, 12, 1), MucLuong = 8000000m,
                NoiDung = "Thu viec ke toan", NguoiKy = "Giam doc"
            };
        }

        private static TuyenDungInput With(Action<TuyenDungInput> change)
        {
            TuyenDungInput x = TuyenDungHopLe();
            change(x);
            return x;
        }

        private static HopDongInput HopDongDaiHan(DateTime? hieuLuc = null, DateTime? hetHan = null)
        {
            return new HopDongInput
            {
                MaNV = 1, SoHD = "HD-TEST-01", LoaiHD = HopDongService.DaiHan,
                NgayKy = new DateTime(2026, 11, 25), NgayHieuLuc = hieuLuc ?? new DateTime(2026, 12, 1),
                NgayHetHan = hetHan, MucLuong = 10000000m, PhuCapLuong = 500000m,
                PhuCapDocHai = 0m, ChucVu = "Nhan vien", PhuCapChucVu = 500000m, NguoiKy = "Giam doc"
            };
        }

        private static HopDongInput HopDongNganHan(DateTime? hetHan)
        {
            return new HopDongInput
            {
                MaNV = 1, SoHD = "HD-TEST-02", LoaiHD = HopDongService.NganHan,
                NgayKy = new DateTime(2026, 11, 25), NgayHieuLuc = new DateTime(2026, 12, 1),
                NgayHetHan = hetHan, MucLuong = 8000000m, ChucVu = "Nhan vien", NguoiKy = "Giam doc"
            };
        }

        private static KeHoachInput KeHoachHopLe()
        {
            var x = new KeHoachInput { MaPB = 3, TuanSo = 42, Nam = 2026, NguoiLap = "Truong phong" };
            x.ChiTiet.Add(new ChiTietKeHoachInput
            { MaNV = 3, Ngay = new DateTime(2026, 10, 12), CaLam = "Sang", LaPhuTroi = false });
            x.ChiTiet.Add(new ChiTietKeHoachInput
            { MaNV = 3, Ngay = new DateTime(2026, 10, 13), CaLam = "Chieu", LaPhuTroi = true });
            return x;
        }

        private static KeHoachInput KeHoachVoiNgay(DateTime ngay)
        {
            KeHoachInput x = KeHoachHopLe();
            x.ChiTiet[0].Ngay = ngay;
            return x;
        }

        private static KeHoachInput KeHoachTrung()
        {
            KeHoachInput x = KeHoachHopLe();
            x.ChiTiet[1].Ngay = x.ChiTiet[0].Ngay;
            return x;
        }

        private static KeHoachInput KeHoachVoiCa(string ca)
        {
            KeHoachInput x = KeHoachHopLe();
            x.ChiTiet[0].CaLam = ca;
            return x;
        }

        private static ChamCongInput ChamCongHopLe(string trangThai)
        {
            return new ChamCongInput
            {
                MaNV = 1, Ngay = new DateTime(2026, 10, 12), CaLam = "Sang",
                GioVao = new TimeSpan(8, 0, 0), GioRa = new TimeSpan(17, 0, 0),
                TrangThai = trangThai, GhiChu = "Test"
            };
        }

        private static ChamCongInput ChamCongVoiGio(TimeSpan vao, TimeSpan ra)
        {
            ChamCongInput x = ChamCongHopLe("DiLam");
            x.GioVao = vao; x.GioRa = ra;
            return x;
        }

        private static ChamCongInput ChamCongVang(string trangThai, TimeSpan? vao, TimeSpan? ra)
        {
            ChamCongInput x = ChamCongHopLe(trangThai);
            x.GioVao = vao; x.GioRa = ra;
            return x;
        }

        private static PhuTroiInput PhuTroiHopLe()
        {
            var x = new PhuTroiInput
            {
                NgayPhieu = new DateTime(2026, 10, 15), HinhThuc = "TangCa", NguoiLap = "Truong phong"
            };
            x.ChiTiet.Add(new ChiTietPhuTroiInput { MaNV = 1, SoGio = 2m, HeSo = 1.5m });
            x.ChiTiet.Add(new ChiTietPhuTroiInput { MaNV = 2, SoGio = 3m, HeSo = 1.5m });
            return x;
        }

        private static PhuTroiInput PhuTroiVoiGio(decimal gio)
        {
            PhuTroiInput x = PhuTroiHopLe();
            x.ChiTiet[0].SoGio = gio;
            return x;
        }

        private static PhuTroiInput PhuTroiVoiHeSo(decimal hs)
        {
            PhuTroiInput x = PhuTroiHopLe();
            x.ChiTiet[0].HeSo = hs;
            return x;
        }

        private static PhuTroiInput PhuTroiTrung()
        {
            PhuTroiInput x = PhuTroiHopLe();
            x.ChiTiet[1].MaNV = x.ChiTiet[0].MaNV;
            return x;
        }

        private static KhenThuongInput KhenThuongCaNhan()
        {
            return new KhenThuongInput
            {
                NgayBanHanh = new DateTime(2026, 10, 20), DoiTuong = KhenThuongService.CaNhan,
                MaNV = 1, LyDo = "Hoan thanh tot", HinhThuc = "Thuong nong",
                SoTien = 2000000m, NguonChiTra = KhenThuongService.NguonLuong, NguoiKy = "Giam doc"
            };
        }

        private static KhenThuongInput KhenThuongTapThe()
        {
            return new KhenThuongInput
            {
                NgayBanHanh = new DateTime(2026, 10, 20), DoiTuong = KhenThuongService.TapThe,
                MaPB = 3, LyDo = "Tap the dat chi tieu", HinhThuc = "Thuong tap the",
                SoTien = 6000000m, NguonChiTra = KhenThuongService.NguonLuong, NguoiKy = "Giam doc"
            };
        }

        private static KhenThuongInput KhenThuongSaiMucTieu()
        {
            KhenThuongInput x = KhenThuongCaNhan();
            x.MaNV = null; x.MaPB = 3;
            return x;
        }

        private static KhenThuongInput KhenThuongVoiTien(decimal tien)
        {
            KhenThuongInput x = KhenThuongCaNhan();
            x.SoTien = tien;
            return x;
        }

        private static KyLuatInput KyLuatCaNhan()
        {
            return new KyLuatInput
            {
                NgayBanHanh = new DateTime(2026, 10, 22), DoiTuong = KhenThuongService.CaNhan,
                MaNV = 5, LyDo = "Vi pham an toan", HinhThuc = "Phat tien",
                SoTienPhat = 500000m, NguonThu = KhenThuongService.NguonTruLuong, NguoiKy = "Giam doc"
            };
        }

        private static KyLuatInput KyLuatSaiNguon()
        {
            KyLuatInput x = KyLuatCaNhan();
            x.NguonThu = "TienMat";
            return x;
        }

        private static void Check(StringBuilder sb, ref int pass, ref int fail, string id, bool ok)
        {
            if (ok) { pass++; sb.AppendLine("PASS " + id); }
            else { fail++; sb.AppendLine("FAIL " + id); }
        }
    }
}
