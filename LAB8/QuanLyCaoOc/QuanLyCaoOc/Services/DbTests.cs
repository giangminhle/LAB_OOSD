using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Models;

namespace QuanLyCaoOc.Services
{
    /// <summary>
    /// Kiem thu tich hop tren CSDL that (du lieu mau 02_SeedData.sql).
    /// Moi test co du lieu cu the; test nao ghi du lieu thi tu don lai sau khi chay.
    /// </summary>
    public static class DbTests
    {
        private static readonly DateTime Ngay = new DateTime(2026, 10, 30);

        public static string RunAll()
        {
            var sb = new StringBuilder();
            int pass = 0, fail = 0;

            TryTest(sb, ref pass, ref fail, "DB01_KetNoi_CSDL", () =>
            {
                Assert(Convert.ToString(Db.Scalar("SELECT DB_NAME()")) == "QuanLyCaoOc", "DB hien tai phai la QuanLyCaoOc");
            });

            TryTest(sb, ref pass, ref fail, "DB02_Du14Bang", () =>
            {
                int n = Dem("SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped=0");
                Assert(n == 14, "So bang = 14, thuc te " + n);
            });

            TryTest(sb, ref pass, ref fail, "DB03_DuLieuMau", () =>
            {
                Assert(Dem("SELECT COUNT(*) FROM dbo.Phong") == 10, "10 phong");
                Assert(Dem("SELECT COUNT(*) FROM dbo.HopDongThue") == 3, "3 hop dong");
                Assert(Dem("SELECT COUNT(*) FROM dbo.ChiTietHopDong") == 5, "5 phong dang thue");
                Assert(Dem("SELECT COUNT(*) FROM dbo.NhanVienVanPhong") == 6, "6 nhan vien");
            });

            TryTest(sb, ref pass, ref fail, "DB04_LapHopDong_2Phong_TraTruoc369Trieu", () =>
            {
                var r = new HopDongService().LapHopDong(new HopDongInput
                {
                    MaKH = 2, DanhSachPhong = new List<int> { 7, 9 }, NgayKy = new DateTime(2026, 10, 28),
                    NgayBatDau = new DateTime(2026, 11, 1), SoThang = 12, DaTraTruoc = true
                });
                try
                {
                    Assert(r.Success, r.Message);
                    decimal tien = Convert.ToDecimal(Db.Scalar("SELECT SoTien FROM dbo.HoaDonTienThue WHERE MaHD=@hd",
                        new SqlParameter("@hd", r.Id)));
                    Assert(tien == 369000000m, "Tra truoc (34.500.000 + 27.000.000) x 6 = 369.000.000, thuc te " + tien);
                    Assert(Dem("SELECT COUNT(*) FROM dbo.Phong WHERE MaPhong IN (7, 9) AND TrangThai=N'DangThue'") == 2,
                        "2 phong chuyen sang DangThue");
                }
                finally
                {
                    if (r.Success)
                        Exec(@"DELETE dbo.HoaDonTienThue WHERE MaHD=@id; DELETE dbo.ChiTietHopDong WHERE MaHD=@id;
DELETE dbo.HopDongThue WHERE MaHD=@id; UPDATE dbo.Phong SET TrangThai=N'Trong' WHERE MaPhong IN (7, 9);", r.Id);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB05_ChanThuePhongDangCoKhach", () =>
            {
                var r = new HopDongService().LapHopDong(new HopDongInput
                {
                    MaKH = 3, DanhSachPhong = new List<int> { 1 }, NgayKy = new DateTime(2026, 10, 28),
                    NgayBatDau = new DateTime(2026, 11, 1), SoThang = 6
                });
                Assert(!r.Success, "Phong P0101 dang thue phai bi chan");
            });

            TryTest(sb, ref pass, ref fail, "DB06_HoaDonThue_Thang11_HD1_35Trieu", () =>
            {
                var svc = new ThanhToanService();
                Assert(!svc.LapHoaDonTienThueThang(1, 7, 2026).Success, "Thang 7/2026 da co hoa don, phai bi chan");
                var r = svc.LapHoaDonTienThueThang(1, 11, 2026);
                try
                {
                    Assert(r.Success, r.Message);
                    decimal tien = Convert.ToDecimal(Db.Scalar("SELECT SoTien FROM dbo.HoaDonTienThue WHERE SoHoaDon=@so",
                        new SqlParameter("@so", r.Code)));
                    Assert(tien == 35000000m, "Tien thue thang = 35.000.000, thuc te " + tien);
                }
                finally
                {
                    if (r.Success) Exec("DELETE dbo.HoaDonTienThue WHERE SoHoaDon=@so", r.Code);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB07_ChanHoaDonThang_TrongKyTraTruoc", () =>
            {
                var r = new ThanhToanService().LapHoaDonTienThueThang(3, 10, 2026);
                Assert(!r.Success, "HD3 thang 10/2026 la thang thu 2, phai bi chan");
            });

            TryTest(sb, ref pass, ref fail, "DB08_HoaDonDichVu_T10_HD1_3800000", () =>
            {
                var r = new ThanhToanService().LapHoaDonDichVu(new HoaDonDichVuInput
                {
                    MaHD = 1, Thang = 10, Nam = 2026, NgayLap = new DateTime(2026, 10, 31),
                    ChiTiet = new List<ChiTietDichVuInput>
                    {
                        new ChiTietDichVuInput { MaPhong = 1, MaDV = 1, SoLuong = 1000 },
                        new ChiTietDichVuInput { MaPhong = 1, MaDV = 3, SoLuong = 1 }
                    }
                });
                try
                {
                    Assert(r.Success, r.Message);
                    decimal tong = Convert.ToDecimal(Db.Scalar("SELECT TongTien FROM dbo.HoaDonDichVu WHERE MaHDDV=@id",
                        new SqlParameter("@id", r.Id)));
                    Assert(tong == 3800000m, "1000 kWh x 3.500 + internet 300.000 = 3.800.000, thuc te " + tong);
                }
                finally
                {
                    if (r.Success) Exec("DELETE dbo.ChiTietHoaDonDichVu WHERE MaHDDV=@id; DELETE dbo.HoaDonDichVu WHERE MaHDDV=@id;", r.Id);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB09_GiaHan_HD1_P0101_6Thang", () =>
            {
                var r = new GiaHanService().LapPhieuGiaHan(new GiaHanInput
                { MaCTHD = 1, SoThangGiaHan = 6, NgayLap = Ngay, NguoiLap = "Kiem thu" });
                try
                {
                    Assert(r.Success, r.Message);
                    DateTime kt = Convert.ToDateTime(Db.Scalar("SELECT NgayKetThuc FROM dbo.ChiTietHopDong WHERE MaCTHD=1"));
                    Assert(kt == new DateTime(2027, 6, 30), "Ngay ket thuc moi 30/06/2027, thuc te " + kt.ToString("dd/MM/yyyy"));
                    DateTime ktHD = Convert.ToDateTime(Db.Scalar("SELECT NgayKetThuc FROM dbo.HopDongThue WHERE MaHD=1"));
                    Assert(ktHD == new DateTime(2027, 6, 30), "Hop dong cap nhat ngay ket thuc 30/06/2027");
                }
                finally
                {
                    if (r.Success)
                        Exec(@"DELETE dbo.PhieuGiaHan WHERE SoPhieu=@so;
UPDATE dbo.ChiTietHopDong SET NgayKetThuc='2026-12-31' WHERE MaCTHD=1;
UPDATE dbo.HopDongThue SET NgayKetThuc='2026-12-31' WHERE MaHD=1;", r.Code);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB10_ChanGiaHan_7Thang_QuaNua", () =>
            {
                var r = new GiaHanService().LapPhieuGiaHan(new GiaHanInput
                { MaCTHD = 1, SoThangGiaHan = 7, NgayLap = Ngay, NguoiLap = "Kiem thu" });
                Assert(!r.Success, "Thue 12 thang chi gia han toi da 6 thang");
            });

            TryTest(sb, ref pass, ref fail, "DB11_ChanGiaHan_HD2_NoTienThue", () =>
            {
                var r = new GiaHanService().LapPhieuGiaHan(new GiaHanInput
                { MaCTHD = 3, SoThangGiaHan = 2, NgayLap = Ngay, NguoiLap = "Kiem thu" });
                Assert(!r.Success && r.Message.Contains("26.250.000") || !r.Success && r.Message.Contains("26,250,000"),
                    "Phai bao no tien thue 26.250.000: " + r.Message);
            });

            TryTest(sb, ref pass, ref fail, "DB12_ChanGiaHan_HD3_NoDichVu", () =>
            {
                var r = new GiaHanService().LapPhieuGiaHan(new GiaHanInput
                { MaCTHD = 4, SoThangGiaHan = 3, NgayLap = Ngay, NguoiLap = "Kiem thu" });
                Assert(!r.Success && r.Message.Contains("dịch vụ"), "Phai bao chua tra dich vu: " + r.Message);
            });

            TryTest(sb, ref pass, ref fail, "DB13_BaoCaoNhanVienMoi", () =>
            {
                var svc = new NhanVienService();
                var sai = svc.BaoCaoNhanVienMoi(new NhanVienInput
                {
                    HoTen = "Nguyen Thi Lan", CCCD = "079099007007", MaKH = 1, MaPhong = 3,
                    ChucVu = "Nhan vien", NgayBaoCao = Ngay
                });
                Assert(!sai.Success, "KH1 khong thue P0201, phai bi chan");
                var r = svc.BaoCaoNhanVienMoi(new NhanVienInput
                {
                    HoTen = "Nguyen Thi Lan", CCCD = "079099007007", MaKH = 1, MaPhong = 1,
                    ChucVu = "Nhan vien", NgayBaoCao = Ngay
                });
                try
                {
                    Assert(r.Success, r.Message);
                    Assert(r.Code == "NV0007", "Ma nhan vien moi NV0007, thuc te " + r.Code);
                }
                finally
                {
                    if (r.Success) Exec("DELETE dbo.NhanVienVanPhong WHERE MaNV=@id", r.Id);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB14_CapTheGiuXe", () =>
            {
                var svc = new NhanVienService();
                var r = svc.CapTheGiuXe(new TheXeInput { MaNV = 4, LoaiXe = "XeMay", BienSo = "59C1-555.66", NgayCap = Ngay });
                try
                {
                    Assert(r.Success, r.Message);
                    Assert(r.Code == "TX0005", "Ma the moi TX0005, thuc te " + r.Code);
                    var trung = svc.CapTheGiuXe(new TheXeInput { MaNV = 1, LoaiXe = "XeMay", BienSo = "59C1-777.88", NgayCap = Ngay });
                    Assert(!trung.Success, "NV0001 da co the, phai bi chan");
                }
                finally
                {
                    if (r.Success) Exec("DELETE dbo.TheGiuXe WHERE MaTheCode=@so", r.Code);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB15_KiemTraPhong_ThayDoiKetCau_LapPhieuPhat", () =>
            {
                var svc = new KiemTraPhongService();
                var lai = svc.LapKiemTra(new KiemTraInput
                {
                    MaPhong = 3, NgayKiemTra = Ngay, TinhTrang = "Tot", NguoiKiemTra = "BQL"
                });
                Assert(!lai.Success, "P0201 da kiem tra nam 2026, phai bi chan");
                var r = svc.LapKiemTra(new KiemTraInput
                {
                    MaPhong = 1, NgayKiemTra = Ngay, TinhTrang = "CanSua", CoThayDoiKetCau = true,
                    KeHoachTuSua = "Khoi phuc cua so", NguoiKiemTra = "BQL", SoTienPhat = 5000000, LyDoPhat = "Tu y doi cua so"
                });
                try
                {
                    Assert(r.Success, r.Message);
                    int maHD = Dem("SELECT MaHD FROM dbo.PhieuPhat WHERE MaKT=" + r.Id);
                    Assert(maHD == 1, "Phieu phat gan voi HD1");
                }
                finally
                {
                    if (r.Success) Exec("DELETE dbo.PhieuPhat WHERE MaKT=@id; DELETE dbo.KiemTraPhong WHERE MaKT=@id;", r.Id);
                }
            });

            TryTest(sb, ref pass, ref fail, "DB16_Trigger_ChanPhongTrong2HopDong", () =>
            {
                bool loi = false;
                try
                {
                    Db.Execute("INSERT dbo.ChiTietHopDong(MaHD, MaPhong, GiaThue, SoThangThue, NgayKetThuc) VALUES (3, 1, 20000000, 6, '2027-02-28')");
                }
                catch (SqlException) { loi = true; }
                if (!loi) Exec("DELETE dbo.ChiTietHopDong WHERE MaHD=3 AND MaPhong=@id", 1);
                Assert(loi, "Trigger phai chan P0101 nam trong 2 hop dong");
            });

            sb.AppendLine("TONG: " + pass + " pass / " + fail + " fail tren " + (pass + fail) + " case.");
            return sb.ToString();
        }

        private static int Dem(string sql) { return Convert.ToInt32(Db.Scalar(sql)); }

        private static void Exec(string sql, object id)
        {
            string p = sql.Contains("@so") ? "@so" : "@id";
            Db.Execute(sql, new SqlParameter(p, id));
        }

        private static void Assert(bool ok, string msg)
        {
            if (!ok) throw new Exception(msg);
        }

        private static void TryTest(StringBuilder sb, ref int pass, ref int fail, string id, Action test)
        {
            try
            {
                test();
                pass++;
                sb.AppendLine("PASS " + id);
            }
            catch (Exception ex)
            {
                fail++;
                sb.AppendLine("FAIL " + id + " - " + ex.Message);
            }
        }
    }
}
