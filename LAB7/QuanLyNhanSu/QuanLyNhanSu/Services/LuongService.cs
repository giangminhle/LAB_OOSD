using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models;

namespace QuanLyNhanSu.Services
{
    /// <summary>
    /// Tinh bang luong thang tu: quyet dinh luong, quyet dinh chuc vu,
    /// bang cham cong, phieu phu troi, khen thuong/ky luat, bao hiem.
    /// Cong thuc:
    ///   Luong ngay cong = MucLuongCoBan / 26 * min(26, so ngay cong)
    ///   Phu troi        = MucLuongCoBan / 26 / 8 * 1.5 * so gio
    ///   Bao hiem        = MucDong * TyLeNhanVien / 100 (chi hop dong dai han)
    ///   Thuc linh       = Luong + Phu troi + Phu cap + Thuong - Phat - Bao hiem
    /// </summary>
    public class LuongService
    {
        public const int NgayCongChuan = 26;
        public const int GioChuanNgay = 8;

        public static decimal TinhLuongTheoNgay(decimal mucLuongCoBan, int soNgayCong)
        {
            if (mucLuongCoBan <= 0 || soNgayCong <= 0) return 0m;
            int ngay = Math.Min(NgayCongChuan, soNgayCong);
            return Math.Round(mucLuongCoBan / NgayCongChuan * ngay, 0, MidpointRounding.AwayFromZero);
        }

        public static decimal TinhPhuTroi(decimal mucLuongCoBan, decimal soGio)
        {
            return PhuTroiService.TinhTienPhuTroi(mucLuongCoBan, soGio);
        }

        public static decimal TinhKhauTruBaoHiem(decimal mucDong, decimal tyLePhanTram)
        {
            if (mucDong <= 0 || tyLePhanTram <= 0) return 0m;
            return Math.Round(mucDong * tyLePhanTram / 100m, 0, MidpointRounding.AwayFromZero);
        }

        public static decimal TinhThucLinh(decimal luongNgayCong, decimal phuTroi,
            decimal phuCapChucVu, decimal phuCapLuong, decimal phuCapDocHai,
            decimal thuong, decimal phat, decimal bhxh, decimal bhyt, decimal bhtn)
        {
            decimal tong = luongNgayCong + phuTroi + phuCapChucVu + phuCapLuong + phuCapDocHai
                         + thuong - phat - bhxh - bhyt - bhtn;
            return Math.Max(0m, Math.Round(tong, 0, MidpointRounding.AwayFromZero));
        }

        public ServiceResult TaoBangLuong(int thang, int nam, string nguoiLap)
        {
            if (thang < 1 || thang > 12) return ServiceResult.Fail("Thang phai trong khoang 1-12.");
            if (nam < 2000 || nam > 2100) return ServiceResult.Fail("Nam khong hop le.");
            if (string.IsNullOrWhiteSpace(nguoiLap)) return ServiceResult.Fail("Vui long nhap nguoi lap bang luong.");

            DateTime dauThang = new DateTime(nam, thang, 1);
            DateTime cuoiThang = dauThang.AddMonths(1).AddDays(-1);

            // Nhan vien co hop dong hieu luc trong thang.
            DataTable nv = Db.Query(@"
SELECT nv.MaNV, nv.MaNVCode, nv.HoTen, nv.MaPB, pb.TenPB, hd.LoaiHD, hd.MucLuong
FROM dbo.NhanVien nv
JOIN dbo.PhongBan pb ON pb.MaPB=nv.MaPB
JOIN dbo.HopDongLaoDong hd ON hd.MaNV=nv.MaNV
WHERE hd.TrangThai=N'HieuLuc'
  AND hd.NgayHieuLuc <= @cuoi
  AND (hd.NgayHetHan IS NULL OR hd.NgayHetHan >= @dau)
  AND nv.TrangThai=N'DangLam'
ORDER BY nv.MaNVCode", new SqlParameter("@dau", dauThang), new SqlParameter("@cuoi", cuoiThang));
            if (nv.Rows.Count == 0) return ServiceResult.Fail("Khong co nhan vien nao co hop dong hieu luc trong thang.");

            Dictionary<int, DataRow> luong = LayQuyetDinhLuong(nv, cuoiThang);
            Dictionary<int, DataRow> chucVu = LayQuyetDinhChucVu(nv, cuoiThang);
            Dictionary<int, int> ngayCong = new Dictionary<int, int>();
            Dictionary<int, decimal> gioPhuTroi = new Dictionary<int, decimal>();
            Dictionary<string, decimal> baoHiem = new Dictionary<string, decimal>();
            Dictionary<int, decimal> thuong = new Dictionary<int, decimal>();
            Dictionary<int, decimal> phat = new Dictionary<int, decimal>();

            DataTable dtNgay = Db.Query(@"
SELECT MaNV, COUNT(*) AS SoNgay FROM dbo.ChamCong
WHERE Ngay BETWEEN @dau AND @cuoi AND TrangThai IN (N'DiLam', N'DiTre')
GROUP BY MaNV", new SqlParameter("@dau", dauThang), new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in dtNgay.Rows) ngayCong[Convert.ToInt32(r["MaNV"])] = Convert.ToInt32(r["SoNgay"]);

            DataTable dtGio = Db.Query(@"
SELECT ct.MaNV, SUM(ct.SoGio) AS TongGio
FROM dbo.ChiTietPhuTroi ct
JOIN dbo.PhieuPhuTroi p ON p.MaPhieuPT=ct.MaPhieuPT
WHERE p.TrangThai=N'DaDuyet' AND p.NgayPhieu BETWEEN @dau AND @cuoi
GROUP BY ct.MaNV", new SqlParameter("@dau", dauThang), new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in dtGio.Rows) gioPhuTroi[Convert.ToInt32(r["MaNV"])] = Convert.ToDecimal(r["TongGio"]);

            DataTable dtBH = Db.Query(@"
SELECT bh.MaNV, bh.LoaiBH, SUM(bh.MucDong * bh.TyLeNhanVien / 100.0) AS Tien
FROM dbo.BaoHiem bh
JOIN dbo.HopDongLaoDong hd ON hd.MaHD=bh.MaHD
WHERE bh.TrangThai=N'HieuLuc' AND hd.LoaiHD=N'DaiHan'
  AND bh.NgayCap <= @cuoi AND (bh.NgayHetHan IS NULL OR bh.NgayHetHan >= @dau)
GROUP BY bh.MaNV, bh.LoaiBH",
                new SqlParameter("@dau", dauThang), new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in dtBH.Rows)
            {
                string key = r["MaNV"] + "|" + r["LoaiBH"];
                baoHiem[key] = Convert.ToDecimal(r["Tien"]);
            }

            // Khen thuong ca nhan / tap the dua vao luong.
            DataTable kt = Db.Query(@"
SELECT DoiTuong, MaNV, MaPB, SoTien FROM dbo.QuyetDinhKhenThuong
WHERE TrangThai=N'HieuLuc' AND NguonChiTra=N'Luong'
  AND NgayBanHanh BETWEEN @dau AND @cuoi",
                new SqlParameter("@dau", dauThang), new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in kt.Rows)
            {
                decimal tien = Convert.ToDecimal(r["SoTien"]);
                if (Convert.ToString(r["DoiTuong"]) == "CaNhan")
                {
                    int ma = Convert.ToInt32(r["MaNV"]);
                    thuong[ma] = (thuong.ContainsKey(ma) ? thuong[ma] : 0m) + tien;
                }
                else
                {
                    int maPB = Convert.ToInt32(r["MaPB"]);
                    int soNV = 0;
                    foreach (DataRow e in nv.Rows) if (Convert.ToInt32(e["MaPB"]) == maPB) soNV++;
                    decimal moiNguoi = KhenThuongService.ChiaDeu(tien, soNV);
                    if (moiNguoi > 0)
                        foreach (DataRow e in nv.Rows)
                            if (Convert.ToInt32(e["MaPB"]) == maPB)
                            {
                                int ma = Convert.ToInt32(e["MaNV"]);
                                thuong[ma] = (thuong.ContainsKey(ma) ? thuong[ma] : 0m) + moiNguoi;
                            }
                }
            }

            DataTable kl = Db.Query(@"
SELECT DoiTuong, MaNV, MaPB, SoTienPhat FROM dbo.QuyetDinhKyLuat
WHERE TrangThai=N'HieuLuc' AND NguonThu=N'TruLuong'
  AND NgayBanHanh BETWEEN @dau AND @cuoi",
                new SqlParameter("@dau", dauThang), new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in kl.Rows)
            {
                decimal tien = Convert.ToDecimal(r["SoTienPhat"]);
                if (Convert.ToString(r["DoiTuong"]) == "CaNhan")
                {
                    int ma = Convert.ToInt32(r["MaNV"]);
                    phat[ma] = (phat.ContainsKey(ma) ? phat[ma] : 0m) + tien;
                }
                else
                {
                    int maPB = Convert.ToInt32(r["MaPB"]);
                    int soNV = 0;
                    foreach (DataRow e in nv.Rows) if (Convert.ToInt32(e["MaPB"]) == maPB) soNV++;
                    decimal moiNguoi = KhenThuongService.ChiaDeu(tien, soNV);
                    if (moiNguoi > 0)
                        foreach (DataRow e in nv.Rows)
                            if (Convert.ToInt32(e["MaPB"]) == maPB)
                            {
                                int ma = Convert.ToInt32(e["MaNV"]);
                                phat[ma] = (phat.ContainsKey(ma) ? phat[ma] : 0m) + moiNguoi;
                            }
                }
            }

            var rows = new List<LuongRow>();
            foreach (DataRow e in nv.Rows)
            {
                int maNV = Convert.ToInt32(e["MaNV"]);
                decimal mucLuong = Convert.ToDecimal(e["MucLuong"]);
                decimal pcLuong = 0m, pcDocHai = 0m, pcChucVu = 0m;
                if (luong.ContainsKey(maNV))
                {
                    mucLuong = Convert.ToDecimal(luong[maNV]["MucLuongCoBan"]);
                    pcLuong = Convert.ToDecimal(luong[maNV]["PhuCapLuong"]);
                    pcDocHai = Convert.ToDecimal(luong[maNV]["PhuCapDocHai"]);
                }
                if (chucVu.ContainsKey(maNV))
                    pcChucVu = Convert.ToDecimal(chucVu[maNV]["PhuCapChucVu"]);

                int ngay = ngayCong.ContainsKey(maNV) ? ngayCong[maNV] : 0;
                decimal gio = gioPhuTroi.ContainsKey(maNV) ? gioPhuTroi[maNV] : 0m;
                decimal luongNgay = TinhLuongTheoNgay(mucLuong, ngay);
                decimal tienPhuTroi = TinhPhuTroi(mucLuong, gio);
                decimal bhxh = LayBaoHiem(baoHiem, maNV, "BHXH");
                decimal bhyt = LayBaoHiem(baoHiem, maNV, "BHYT");
                decimal bhtn = LayBaoHiem(baoHiem, maNV, "BHTN");
                decimal tienThuong = thuong.ContainsKey(maNV) ? thuong[maNV] : 0m;
                decimal tienPhat = phat.ContainsKey(maNV) ? phat[maNV] : 0m;
                decimal thucLinh = TinhThucLinh(luongNgay, tienPhuTroi, pcChucVu, pcLuong,
                    pcDocHai, tienThuong, tienPhat, bhxh, bhyt, bhtn);

                rows.Add(new LuongRow
                {
                    MaNV = maNV,
                    MaNVCode = Convert.ToString(e["MaNVCode"]),
                    HoTen = Convert.ToString(e["HoTen"]),
                    MaPB = Convert.ToInt32(e["MaPB"]),
                    TenPB = Convert.ToString(e["TenPB"]),
                    LoaiHD = Convert.ToString(e["LoaiHD"]),
                    MucLuongCoBan = mucLuong,
                    SoNgayCong = ngay,
                    GioPhuTroi = gio,
                    TienPhuTroi = tienPhuTroi,
                    PhuCapChucVu = pcChucVu,
                    PhuCapLuong = pcLuong,
                    PhuCapDocHai = pcDocHai,
                    TienKhenThuong = tienThuong,
                    TienKyLuat = tienPhat,
                    BHXH = bhxh,
                    BHYT = bhyt,
                    BHTN = bhtn,
                    ThucLinh = thucLinh
                });
            }

            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maBL = 0;
                        string trangThai = null;
                        using (var cmd = new SqlCommand("SELECT MaBangLuong, TrangThai FROM dbo.BangLuong WHERE Thang=@t AND Nam=@n", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@t", thang);
                            cmd.Parameters.AddWithValue("@n", nam);
                            using (SqlDataReader rd = cmd.ExecuteReader())
                                if (rd.Read()) { maBL = rd.GetInt32(0); trangThai = rd.GetString(1); }
                        }
                        if (trangThai == "DaChot")
                            return ServiceResult.Fail("Bang luong thang " + thang + "/" + nam + " da chot, khong the tinh lai.");

                        if (maBL == 0)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.BangLuong(Thang, Nam, NgayLap, NguoiLap, TrangThai, TongChiPhi)
VALUES (@t, @n, @ngay, @nguoi, N'Nhap', 0);
SELECT SCOPE_IDENTITY();", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@t", thang);
                                cmd.Parameters.AddWithValue("@n", nam);
                                cmd.Parameters.AddWithValue("@ngay", DateTime.Today);
                                cmd.Parameters.AddWithValue("@nguoi", nguoiLap.Trim());
                                maBL = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else
                        {
                            using (var cmd = new SqlCommand(@"UPDATE dbo.BangLuong
                                SET NgayLap=@ngay, NguoiLap=@nguoi, TrangThai=N'Nhap', TongChiPhi=0
                                WHERE MaBangLuong=@ma", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@ngay", DateTime.Today);
                                cmd.Parameters.AddWithValue("@nguoi", nguoiLap.Trim());
                                cmd.Parameters.AddWithValue("@ma", maBL);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("DELETE dbo.ChiTietBangLuong WHERE MaBangLuong=@ma", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@ma", maBL);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        decimal tong = 0m;
                        foreach (LuongRow r in rows)
                        {
                            using (var cmd = new SqlCommand(@"
INSERT dbo.ChiTietBangLuong(MaBangLuong, MaNV, MucLuongCoBan, SoNgayCong, GioPhuTroi,
    TienPhuTroi, PhuCapChucVu, PhuCapLuong, PhuCapDocHai, TienKhenThuong, TienKyLuat,
    BHXH, BHYT, BHTN, ThucLinh)
VALUES (@bl, @nv, @luong, @ngay, @gio, @pt, @pccv, @pcl, @pcdh, @kt, @kl, @bhxh, @bhyt, @bhtn, @tl)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@bl", maBL);
                                cmd.Parameters.AddWithValue("@nv", r.MaNV);
                                cmd.Parameters.AddWithValue("@luong", r.MucLuongCoBan);
                                cmd.Parameters.AddWithValue("@ngay", r.SoNgayCong);
                                cmd.Parameters.AddWithValue("@gio", r.GioPhuTroi);
                                cmd.Parameters.AddWithValue("@pt", r.TienPhuTroi);
                                cmd.Parameters.AddWithValue("@pccv", r.PhuCapChucVu);
                                cmd.Parameters.AddWithValue("@pcl", r.PhuCapLuong);
                                cmd.Parameters.AddWithValue("@pcdh", r.PhuCapDocHai);
                                cmd.Parameters.AddWithValue("@kt", r.TienKhenThuong);
                                cmd.Parameters.AddWithValue("@kl", r.TienKyLuat);
                                cmd.Parameters.AddWithValue("@bhxh", r.BHXH);
                                cmd.Parameters.AddWithValue("@bhyt", r.BHYT);
                                cmd.Parameters.AddWithValue("@bhtn", r.BHTN);
                                cmd.Parameters.AddWithValue("@tl", r.ThucLinh);
                                cmd.ExecuteNonQuery();
                            }
                            tong += r.ThucLinh;
                        }
                        using (var cmd = new SqlCommand("UPDATE dbo.BangLuong SET TongChiPhi=@tong WHERE MaBangLuong=@ma", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@tong", tong);
                            cmd.Parameters.AddWithValue("@ma", maBL);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Da tinh bang luong " + thang + "/" + nam
                            + " cho " + rows.Count + " nhan vien. Tong thuc linh: "
                            + tong.ToString("N0") + " d.", maBL);
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        private static Dictionary<int, DataRow> LayQuyetDinhLuong(DataTable nv, DateTime cuoiThang)
        {
            var d = new Dictionary<int, DataRow>();
            DataTable dt = Db.Query(@"
SELECT MaNV, MucLuongCoBan, PhuCapLuong, PhuCapDocHai
FROM (SELECT q.*, ROW_NUMBER() OVER(PARTITION BY MaNV ORDER BY NgayHieuLuc DESC, MaQDLuong DESC) rn
      FROM dbo.QuyetDinhLuong q WHERE NgayHieuLuc <= @cuoi) t
WHERE rn=1", new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in dt.Rows) d[Convert.ToInt32(r["MaNV"])] = r;
            return d;
        }

        private static Dictionary<int, DataRow> LayQuyetDinhChucVu(DataTable nv, DateTime cuoiThang)
        {
            var d = new Dictionary<int, DataRow>();
            DataTable dt = Db.Query(@"
SELECT MaNV, ChucVu, PhuCapChucVu
FROM (SELECT q.*, ROW_NUMBER() OVER(PARTITION BY MaNV ORDER BY NgayHieuLuc DESC, MaQDChucVu DESC) rn
      FROM dbo.QuyetDinhChucVu q WHERE NgayHieuLuc <= @cuoi) t
WHERE rn=1", new SqlParameter("@cuoi", cuoiThang));
            foreach (DataRow r in dt.Rows) d[Convert.ToInt32(r["MaNV"])] = r;
            return d;
        }

        private static decimal LayBaoHiem(Dictionary<string, decimal> d, int maNV, string loai)
        {
            string key = maNV + "|" + loai;
            return d.ContainsKey(key) ? d[key] : 0m;
        }

        public LuongKetQua LayBangLuong(int thang, int nam)
        {
            DataTable header = Db.Query("SELECT * FROM dbo.BangLuong WHERE Thang=@t AND Nam=@n",
                new SqlParameter("@t", thang), new SqlParameter("@n", nam));
            if (header.Rows.Count == 0) return null;
            DataRow h = header.Rows[0];
            var kq = new LuongKetQua
            {
                Thang = thang,
                Nam = nam,
                MaBangLuong = Convert.ToInt32(h["MaBangLuong"]),
                TrangThai = Convert.ToString(h["TrangThai"]),
                TongChiPhi = Convert.ToDecimal(h["TongChiPhi"])
            };
            DataTable dt = Db.Query(@"
SELECT ct.*, nv.MaNVCode, nv.HoTen, nv.MaPB, pb.TenPB, hd.LoaiHD
FROM dbo.ChiTietBangLuong ct
JOIN dbo.NhanVien nv ON nv.MaNV=ct.MaNV
JOIN dbo.PhongBan pb ON pb.MaPB=nv.MaPB
LEFT JOIN dbo.HopDongLaoDong hd ON hd.MaNV=ct.MaNV AND hd.TrangThai=N'HieuLuc'
WHERE ct.MaBangLuong=@ma ORDER BY nv.MaNVCode", new SqlParameter("@ma", kq.MaBangLuong));
            foreach (DataRow r in dt.Rows)
            {
                kq.Rows.Add(new LuongRow
                {
                    MaNV = Convert.ToInt32(r["MaNV"]),
                    MaNVCode = Convert.ToString(r["MaNVCode"]),
                    HoTen = Convert.ToString(r["HoTen"]),
                    MaPB = Convert.ToInt32(r["MaPB"]),
                    TenPB = Convert.ToString(r["TenPB"]),
                    LoaiHD = r["LoaiHD"] == DBNull.Value ? "" : Convert.ToString(r["LoaiHD"]),
                    MucLuongCoBan = Convert.ToDecimal(r["MucLuongCoBan"]),
                    SoNgayCong = Convert.ToInt32(r["SoNgayCong"]),
                    GioPhuTroi = Convert.ToDecimal(r["GioPhuTroi"]),
                    TienPhuTroi = Convert.ToDecimal(r["TienPhuTroi"]),
                    PhuCapChucVu = Convert.ToDecimal(r["PhuCapChucVu"]),
                    PhuCapLuong = Convert.ToDecimal(r["PhuCapLuong"]),
                    PhuCapDocHai = Convert.ToDecimal(r["PhuCapDocHai"]),
                    TienKhenThuong = Convert.ToDecimal(r["TienKhenThuong"]),
                    TienKyLuat = Convert.ToDecimal(r["TienKyLuat"]),
                    BHXH = Convert.ToDecimal(r["BHXH"]),
                    BHYT = Convert.ToDecimal(r["BHYT"]),
                    BHTN = Convert.ToDecimal(r["BHTN"]),
                    ThucLinh = Convert.ToDecimal(r["ThucLinh"])
                });
            }
            return kq;
        }

        public ServiceResult Chot(int maBangLuong)
        {
            try
            {
                using (SqlConnection conn = Db.OpenConnection())
                using (var cmd = new SqlCommand("dbo.sp_ChotBangLuong", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaBangLuong", maBangLuong);
                    cmd.ExecuteNonQuery();
                }
                return ServiceResult.Ok("Da chot bang luong. Bang luong khong the tinh lai.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail(ex.Number >= 50000 ? ex.Message : "Loi CSDL: " + ex.Message);
            }
        }

        public DataTable DanhSachBangLuong()
        {
            return Db.Query(@"SELECT MaBangLuong, Thang, Nam, NgayLap, NguoiLap, TrangThai, TongChiPhi
                              FROM dbo.BangLuong ORDER BY Nam DESC, Thang DESC");
        }

        public DataTable DanhSachChiTiet(int maBangLuong)
        {
            return Db.Query(@"
SELECT nv.MaNVCode, nv.HoTen, ct.MucLuongCoBan, ct.SoNgayCong, ct.GioPhuTroi, ct.TienPhuTroi,
       ct.PhuCapChucVu, ct.PhuCapLuong, ct.PhuCapDocHai, ct.TienKhenThuong, ct.TienKyLuat,
       ct.BHXH, ct.BHYT, ct.BHTN, ct.ThucLinh
FROM dbo.ChiTietBangLuong ct JOIN dbo.NhanVien nv ON nv.MaNV=ct.MaNV
WHERE ct.MaBangLuong=@ma ORDER BY nv.MaNVCode", new SqlParameter("@ma", maBangLuong));
        }
    }
}
