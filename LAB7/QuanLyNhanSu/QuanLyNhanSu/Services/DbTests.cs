using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>
    /// Kiem thu tich hop tren CSDL that (du lieu mau LAB7).
    /// Moi test co du lieu cu the; cuoi moi test don du lieu tam.
    /// </summary>
    public static class DbTests
    {
        public static string RunAll()
        {
            var sb = new StringBuilder();
            int pass = 0, fail = 0;
            DonBangLuong();

            TryTest(sb, ref pass, ref fail, "DB01_KetNoi_CSDL", () =>
            {
                object v = Db.Scalar("SELECT DB_NAME()");
                Assert(Convert.ToString(v) == "QuanLyNhanSu", "DB hien tai phai la QuanLyNhanSu");
            });

            TryTest(sb, ref pass, ref fail, "DB02_Du18Bang", () =>
            {
                int n = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped=0"));
                Assert(n == 18, "So bang = 18, thuc te " + n);
            });

            TryTest(sb, ref pass, ref fail, "DB03_DuLieuMau", () =>
            {
                Assert(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.NhanVien")) == 7, "7 nhan vien");
                Assert(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.HopDongLaoDong WHERE TrangThai=N'HieuLuc'")) == 6, "6 hop dong hieu luc");
                Assert(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.BaoHiem")) == 8, "8 dong bao hiem");
                Assert(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.NhanVien WHERE TrangThai=N'ThuViec'")) == 1, "1 nhan vien thu viec");
            });

            int maBL = 0;
            TryTest(sb, ref pass, ref fail, "DB04_TinhLuong_Thang10_2026", () =>
            {
                ServiceResult r = new LuongService().TaoBangLuong(10, 2026, "Kiem thu");
                Assert(r.Success, r.Message);
                maBL = r.Id;
                LuongKetQua kq = new LuongService().LayBangLuong(10, 2026);
                Assert(kq != null && kq.Rows.Count == 6, "Bang luong phai co 6 nhan vien");
                Assert(kq.TongChiPhi == 88035577m, "Tong thuc linh phai la 88.035.577, thuc te " + kq.TongChiPhi);
            });

            TryTest(sb, ref pass, ref fail, "DB05_NV1_LuongVaBaoHiem", () =>
            {
                LuongKetQua kq = new LuongService().LayBangLuong(10, 2026);
                LuongRow r = Tim(kq, "NV00001");
                Assert(r != null, "Khong tim thay NV00001");
                Assert(r.MucLuongCoBan == 15000000m, "Luong CB 15.000.000");
                Assert(r.SoNgayCong == 26, "26 ngay cong");
                Assert(r.TienPhuTroi == 865385m, "Phu troi 8 gio = 865.385");
                Assert(r.PhuCapChucVu == 1500000m, "Phu cap chuc vu 1.500.000");
                Assert(r.TienKhenThuong == 2000000m, "Thuong ca nhan 2.000.000");
                Assert(r.BHXH == 1200000m, "BHXH 8% = 1.200.000");
                Assert(r.BHYT == 225000m, "BHYT 1,5% = 225.000");
                Assert(r.ThucLinh == 19440385m, "Thuc linh NV1 = 19.440.385, thuc te " + r.ThucLinh);
            });

            TryTest(sb, ref pass, ref fail, "DB06_NV5_ThuongTapThe_PhatTruLuong", () =>
            {
                LuongKetQua kq = new LuongService().LayBangLuong(10, 2026);
                LuongRow r = Tim(kq, "NV00005");
                Assert(r != null, "Khong tim thay NV00005");
                Assert(r.TienKhenThuong == 3000000m, "Thuong tap the chia deu 3.000.000");
                Assert(r.TienKyLuat == 500000m, "Phat tru luong 500.000");
                Assert(r.BHXH == 0m && r.BHYT == 0m, "Hop dong ngan han khong tru bao hiem");
                Assert(r.ThucLinh == 11546154m, "Thuc linh NV5 = 11.546.154, thuc te " + r.ThucLinh);
            });

            TryTest(sb, ref pass, ref fail, "DB07_NV3_ThuongTapThe", () =>
            {
                LuongKetQua kq = new LuongService().LayBangLuong(10, 2026);
                LuongRow r = Tim(kq, "NV00003");
                Assert(r != null && r.TienKhenThuong == 3000000m, "NV3 duoc 3.000.000 thuong tap the");
                Assert(r.TienPhuTroi == 649038m, "Phu troi 10 gio = 649.038");
                Assert(r.ThucLinh == 14849038m, "Thuc linh NV3 = 14.849.038, thuc te " + (r == null ? 0 : r.ThucLinh));
            });

            TryTest(sb, ref pass, ref fail, "DB08_HopDongNganHan_KhongCoBaoHiem", () =>
            {
                int n = Convert.ToInt32(Db.Scalar(@"
SELECT COUNT(*) FROM dbo.BaoHiem bh
JOIN dbo.HopDongLaoDong hd ON hd.MaHD=bh.MaHD
WHERE hd.LoaiHD=N'NganHan'"));
                Assert(n == 0, "Khong co bao hiem cho hop dong ngan han, thuc te " + n);
            });

            TryTest(sb, ref pass, ref fail, "DB09_TriggerChanBaoHiem_NganHan", () =>
            {
                int maNV = Convert.ToInt32(Db.Scalar("SELECT MaNV FROM dbo.NhanVien WHERE CCCD='079098005005'"));
                int maHD = Convert.ToInt32(Db.Scalar("SELECT MaHD FROM dbo.HopDongLaoDong WHERE MaNV=@m AND TrangThai=N'HieuLuc'",
                    new SqlParameter("@m", maNV)));
                bool biChan = false;
                try
                {
                    Db.Execute(@"INSERT dbo.BaoHiem(MaNV, MaHD, LoaiBH, SoSo, NgayCap, NgayHetHan,
                                 MucDong, TyLeNhanVien, TyLeCongTy, TrangThai)
                                 VALUES (@nv, @hd, N'BHTN', 'TEST-NGANHAN', '2026-01-01', NULL,
                                 8000000, 1.0, 1.0, N'HieuLuc')",
                        new SqlParameter("@nv", maNV), new SqlParameter("@hd", maHD));
                }
                catch (SqlException ex)
                {
                    biChan = ex.Number == 50011;
                }
                Assert(biChan, "Trigger phai chan bao hiem cua hop dong ngan han");
            });

            TryTest(sb, ref pass, ref fail, "DB10_ChanKyHopDong_TruocKetThucThuViec", () =>
            {
                int maNV = Convert.ToInt32(Db.Scalar("SELECT MaNV FROM dbo.NhanVien WHERE CCCD='079000005007'"));
                var x = new HopDongInput
                {
                    MaNV = maNV, SoHD = "HD-TEST-PROBATION", LoaiHD = HopDongService.NganHan,
                    NgayKy = new DateTime(2026, 10, 20), NgayHieuLuc = new DateTime(2026, 11, 15),
                    NgayHetHan = new DateTime(2027, 5, 15), MucLuong = 8000000m,
                    ChucVu = "Cong nhan", NguoiKy = "Giam doc"
                };
                ServiceResult r = new HopDongService().KyHopDong(x);
                Assert(!r.Success, "Phai chan hop dong hieu luc truoc ngay ket thuc thu viec");
                Assert(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.HopDongLaoDong WHERE SoHD='HD-TEST-PROBATION'")) == 0,
                    "Khong duoc tao hop dong khi validate that bai");
            });

            TryTest(sb, ref pass, ref fail, "DB11_ChanTrungCCCD", () =>
            {
                var x = new TuyenDungInput
                {
                    HoTen = "Nguoi Test Trung", GioiTinh = "Nam", NgaySinh = new DateTime(1995, 1, 1),
                    CCCD = "079090005001", DienThoai = "0909999999", DiaChi = "Test",
                    MaPB = Convert.ToInt32(Db.Scalar("SELECT TOP 1 MaPB FROM dbo.PhongBan ORDER BY MaPB")),
                    SoQD = "QDTD-TEST-TRUNG-" + DateTime.Now.ToString("HHmmssfff"),
                    NgayBanHanh = new DateTime(2026, 10, 20), NgayBatDau = new DateTime(2026, 10, 25),
                    NgayKetThuc = new DateTime(2026, 12, 20), MucLuong = 8000000m,
                    NoiDung = "Test trung CCCD", NguoiKy = "Giam doc"
                };
                ServiceResult r = new TuyenDungService().TiepNhan(x);
                Assert(!r.Success, "Phai chan CCCD trung");
                Assert(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.NhanVien WHERE CCCD='079090005001'")) == 1,
                    "Khong duoc tao them nhan vien trung CCCD");
            });

            int maKH = 0;
            TryTest(sb, ref pass, ref fail, "DB12_KeHoachTuan_LuuVaDuyet", () =>
            {
                Db.Execute("DELETE dbo.KeHoachTuan WHERE MaPB=3 AND TuanSo=44 AND Nam=2026");
                var x = new KeHoachInput
                {
                    MaPB = 3, TuanSo = 44, Nam = 2026, NguoiLap = "Kiem thu"
                };
                x.ChiTiet.Add(new ChiTietKeHoachInput { MaNV = 3, Ngay = new DateTime(2026, 10, 26), CaLam = "Sang" });
                x.ChiTiet.Add(new ChiTietKeHoachInput { MaNV = 5, Ngay = new DateTime(2026, 10, 26), CaLam = "Chieu" });
                ServiceResult r = new KeHoachService().Luu(x);
                Assert(r.Success, r.Message);
                maKH = r.Id;
                ServiceResult d = new KeHoachService().Duyet(maKH);
                Assert(d.Success, d.Message);
                string tt = Convert.ToString(Db.Scalar("SELECT TrangThai FROM dbo.KeHoachTuan WHERE MaKH=@m", new SqlParameter("@m", maKH)));
                Assert(tt == "DaDuyet", "Trang thai phai la DaDuyet");
            });

            TryTest(sb, ref pass, ref fail, "DB13_ChamCong_Upsert", () =>
            {
                Db.Execute("DELETE dbo.ChamCong WHERE MaNV=1 AND Ngay='2026-10-29'");
                var x = new ChamCongInput
                {
                    MaNV = 1, Ngay = new DateTime(2026, 10, 29), CaLam = "Sang",
                    GioVao = new TimeSpan(8, 0, 0), GioRa = new TimeSpan(17, 0, 0),
                    TrangThai = "DiLam", GhiChu = "Kiem thu upsert"
                };
                ServiceResult r = new ChamCongService().Luu(x);
                Assert(r.Success, r.Message);
                x.TrangThai = "DiTre"; x.GhiChu = "Kiem thu update";
                r = new ChamCongService().Luu(x);
                Assert(r.Success, r.Message);
                string tt = Convert.ToString(Db.Scalar("SELECT TrangThai FROM dbo.ChamCong WHERE MaNV=1 AND Ngay='2026-10-29'"));
                Assert(tt == "DiTre", "Ban ghi phai duoc cap nhat, khong tao trung");
            });

            TryTest(sb, ref pass, ref fail, "DB14_KhenThuong_PhieuChi_TuDong", () =>
            {
                int maNV = Convert.ToInt32(Db.Scalar("SELECT MaNV FROM dbo.NhanVien WHERE CCCD='079092005002'"));
                var x = new KhenThuongInput
                {
                    NgayBanHanh = new DateTime(2026, 10, 23), DoiTuong = KhenThuongService.CaNhan,
                    MaNV = maNV, LyDo = "Kiem thu phieu chi", HinhThuc = "Thuong nong",
                    SoTien = 1234000m, NguonChiTra = KhenThuongService.NguonPhieuChi, NguoiKy = "Giam doc"
                };
                ServiceResult r = new KhenThuongService().LapKhenThuong(x);
                Assert(r.Success, r.Message);
                int n = Convert.ToInt32(Db.Scalar(@"
SELECT COUNT(*) FROM dbo.PhieuChiKhenThuong pc
JOIN dbo.QuyetDinhKhenThuong q ON q.MaQDKhenThuong=pc.MaQDKhenThuong
WHERE q.LyDo=N'Kiem thu phieu chi' AND pc.SoTien=1234000"));
                Assert(n == 1, "Phai tu dong tao 1 phieu chi");
            });

            TryTest(sb, ref pass, ref fail, "DB15_KyLuat_PhieuThu_TuDong", () =>
            {
                int maNV = Convert.ToInt32(Db.Scalar("SELECT MaNV FROM dbo.NhanVien WHERE CCCD='079093005006'"));
                var x = new KyLuatInput
                {
                    NgayBanHanh = new DateTime(2026, 10, 23), DoiTuong = KhenThuongService.CaNhan,
                    MaNV = maNV, LyDo = "Kiem thu phieu thu", HinhThuc = "Phat tien",
                    SoTienPhat = 234000m, NguonThu = KhenThuongService.NguonPhieuThu, NguoiKy = "Giam doc"
                };
                ServiceResult r = new KhenThuongService().LapKyLuat(x);
                Assert(r.Success, r.Message);
                int n = Convert.ToInt32(Db.Scalar(@"
SELECT COUNT(*) FROM dbo.PhieuThuNopPhat pt
JOIN dbo.QuyetDinhKyLuat q ON q.MaQDKyLuat=pt.MaQDKyLuat
WHERE q.LyDo=N'Kiem thu phieu thu' AND pt.SoTien=234000"));
                Assert(n == 1, "Phai tu dong tao 1 phieu thu");
            });

            TryTest(sb, ref pass, ref fail, "DB16_ChotBangLuong_ChanSua", () =>
            {
                LuongKetQua kq = new LuongService().LayBangLuong(10, 2026);
                Assert(kq != null && kq.MaBangLuong > 0, "Phai co bang luong de chot");
                ServiceResult r = new LuongService().Chot(kq.MaBangLuong);
                Assert(r.Success, r.Message);
                ServiceResult r2 = new LuongService().TaoBangLuong(10, 2026, "Kiem thu");
                Assert(!r2.Success, "Khong duoc tinh lai bang luong da chot");
                bool biChan = false;
                try
                {
                    Db.Execute("UPDATE dbo.ChiTietBangLuong SET ThucLinh=1 WHERE MaBangLuong=@m",
                        new SqlParameter("@m", kq.MaBangLuong));
                }
                catch (SqlException ex) { biChan = ex.Number == 50010; }
                Assert(biChan, "Trigger phai chan sua chi tiet bang luong da chot");
            });

            // Don du lieu kiem thu, tra DB ve seed sach.
            try { Db.Execute("DELETE dbo.KeHoachTuan WHERE MaPB=3 AND TuanSo=44 AND Nam=2026"); } catch { }
            try { Db.Execute("DELETE dbo.ChamCong WHERE MaNV=1 AND Ngay='2026-10-29'"); } catch { }
            try
            {
                Db.Execute(@"DELETE pc FROM dbo.PhieuChiKhenThuong pc
                             JOIN dbo.QuyetDinhKhenThuong q ON q.MaQDKhenThuong=pc.MaQDKhenThuong
                             WHERE q.LyDo=N'Kiem thu phieu chi'");
                Db.Execute("DELETE dbo.QuyetDinhKhenThuong WHERE LyDo=N'Kiem thu phieu chi'");
            }
            catch { }
            try
            {
                Db.Execute(@"DELETE pt FROM dbo.PhieuThuNopPhat pt
                             JOIN dbo.QuyetDinhKyLuat q ON q.MaQDKyLuat=pt.MaQDKyLuat
                             WHERE q.LyDo=N'Kiem thu phieu thu'");
                Db.Execute("DELETE dbo.QuyetDinhKyLuat WHERE LyDo=N'Kiem thu phieu thu'");
            }
            catch { }
            DonBangLuong();

            sb.AppendLine(string.Format("TONG DB: {0} pass / {1} fail tren {2} case.", pass, fail, pass + fail));
            return sb.ToString();
        }

        private static void DonBangLuong()
        {
            try
            {
                Db.Execute("UPDATE dbo.BangLuong SET TrangThai=N'Nhap' WHERE Thang=10 AND Nam=2026");
                Db.Execute("DELETE dbo.ChiTietBangLuong WHERE MaBangLuong IN (SELECT MaBangLuong FROM dbo.BangLuong WHERE Thang=10 AND Nam=2026)");
                Db.Execute("DELETE dbo.BangLuong WHERE Thang=10 AND Nam=2026");
            }
            catch { }
        }

        private static LuongRow Tim(LuongKetQua kq, string maNVCode)
        {
            if (kq == null) return null;
            foreach (LuongRow r in kq.Rows) if (r.MaNVCode == maNVCode) return r;
            return null;
        }

        private static void Assert(bool dieuKien, string thongBao)
        {
            if (!dieuKien) throw new Exception(thongBao);
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
