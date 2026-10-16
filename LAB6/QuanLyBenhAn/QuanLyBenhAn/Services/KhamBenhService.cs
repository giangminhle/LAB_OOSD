using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using QuanLyBenhAn.Data;
using QuanLyBenhAn.Models;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiep vu kham: tao phieu, xet nghiem, toa thuoc, thanh toan, hoan tat.</summary>
    public class KhamBenhService
    {
        // ---- Validate thuan (kiem thu khong can DB) ----
        public string ValidatePhieuKham(PhieuKhamInput p)
        {
            if (p == null) return "Du lieu phieu kham khong hop le.";
            if (p.MaHoSo <= 0) return "Chua chon ho so benh an.";
            if (p.MaBS <= 0) return "Chua chon bac si.";
            if (p.MaKhoa <= 0) return "Chua chon khoa.";
            if (p.ThanNhiet.HasValue && (p.ThanNhiet < 34m || p.ThanNhiet > 42m))
                return "Than nhiet phai trong khoang 34-42 do C.";
            if (!string.IsNullOrWhiteSpace(p.HuyetAp) && !Regex.IsMatch(p.HuyetAp.Trim(), @"^\d{2,3}/\d{2,3}$"))
                return "Huyet ap nhap dang 120/80.";
            return null;
        }

        public string ValidateToa(int maPhieuKham, List<ChiTietToaInput> chiTiet)
        {
            if (maPhieuKham <= 0) return "Toa thuoc bat buoc lien ket voi 1 phieu kham.";
            if (chiTiet == null || chiTiet.Count == 0) return "Toa thuoc phai co it nhat 1 loai thuoc.";
            foreach (var c in chiTiet)
            {
                if (c.MaThuoc <= 0) return "Chua chon thuoc.";
                if (c.SoLuong <= 0) return "So luong thuoc phai > 0.";
                if (string.IsNullOrWhiteSpace(c.LieuLuong)) return "Vui long nhap lieu luong.";
            }
            return null;
        }

        public string ValidateXetNghiem(int maPhieuKham, string loaiXN)
        {
            if (maPhieuKham <= 0) return "Xet nghiem bat buoc lien ket voi 1 phieu kham.";
            if (string.IsNullOrWhiteSpace(loaiXN)) return "Vui long nhap loai xet nghiem.";
            return null;
        }

        /// <summary>Chi duoc HoanTat khi DaThanhToan = true (quy tac sau xuat vien).</summary>
        public string CanHoanTat(bool daThanhToan, bool daCoHoaDon)
        {
            if (!daCoHoaDon) return "Phieu kham chua co hoa don vien phi.";
            if (!daThanhToan) return "Chua thanh toan vien phi nen khong the hoan tat.";
            return null;
        }

        // ---- DB ----
        public ServiceResult TaoPhieuKham(PhieuKhamInput p)
        {
            string err = ValidatePhieuKham(p);
            if (err != null) return ServiceResult.Fail(err);
            const string sql = @"
INSERT INTO PhieuKham (MaHoSo, MaBS, MaKhoa, TrieuChung, ThanNhiet, HuyetAp, ChanDoan, LoaiKham, TrangThai)
OUTPUT INSERTED.MaPhieuKham
VALUES (@hs,@bs,@khoa,@tc,@nhiet,@ha,@cd,@loai,0)";
            try
            {
                object id = Db.ExecuteScalar(sql,
                    new SqlParameter("@hs", p.MaHoSo), new SqlParameter("@bs", p.MaBS),
                    new SqlParameter("@khoa", p.MaKhoa),
                    new SqlParameter("@tc", (object)p.TrieuChung ?? DBNull.Value),
                    new SqlParameter("@nhiet", (object)p.ThanNhiet ?? DBNull.Value),
                    new SqlParameter("@ha", (object)p.HuyetAp ?? DBNull.Value),
                    new SqlParameter("@cd", (object)p.ChanDoan ?? DBNull.Value),
                    new SqlParameter("@loai", (byte)p.LoaiKham));
                return ServiceResult.Ok("Tao phieu kham thanh cong.", Convert.ToInt32(id));
            }
            catch (SqlException ex) { return ServiceResult.Fail("Loi CSDL: " + ex.Message); }
        }

        public ServiceResult ChiDinhXetNghiem(int maPhieuKham, string loaiXN)
        {
            string err = ValidateXetNghiem(maPhieuKham, loaiXN);
            if (err != null) return ServiceResult.Fail(err);
            try
            {
                object id = Db.ExecuteScalar(
                    "INSERT INTO XetNghiem (MaPhieuKham, LoaiXN) OUTPUT INSERTED.MaXN VALUES (@pk,@loai)",
                    new SqlParameter("@pk", maPhieuKham), new SqlParameter("@loai", loaiXN.Trim()));
                return ServiceResult.Ok("Da chi dinh xet nghiem.", Convert.ToInt32(id));
            }
            catch (SqlException ex) { return ServiceResult.Fail("Loi CSDL: " + ex.Message); }
        }

        /// <summary>Lap toa thuoc: 1 phieu chi 1 toa (UNIQUE) + insert chi tiet trong TRANSACTION.</summary>
        public ServiceResult LapToaThuoc(int maPhieuKham, int maBS, List<ChiTietToaInput> chiTiet, string ghiChu)
        {
            string err = ValidateToa(maPhieuKham, chiTiet);
            if (err != null) return ServiceResult.Fail(err);
            try
            {
                using (var conn = new SqlConnection(Db.ConnectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        int maToa;
                        using (var cmd = new SqlCommand(
                            "INSERT INTO ToaThuoc (MaPhieuKham, MaBS, GhiChu) OUTPUT INSERTED.MaToa VALUES (@pk,@bs,@gc)",
                            conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@pk", maPhieuKham);
                            cmd.Parameters.AddWithValue("@bs", maBS);
                            cmd.Parameters.AddWithValue("@gc", (object)ghiChu ?? DBNull.Value);
                            maToa = (int)cmd.ExecuteScalar();
                        }
                        foreach (var c in chiTiet)
                        {
                            using (var cmd = new SqlCommand(
                                "INSERT INTO ChiTietToa (MaToa, MaThuoc, LieuLuong, SoLuong, CachDung) VALUES (@t,@th,@lieu,@sl,@cach)",
                                conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@t", maToa);
                                cmd.Parameters.AddWithValue("@th", c.MaThuoc);
                                cmd.Parameters.AddWithValue("@lieu", c.LieuLuong.Trim());
                                cmd.Parameters.AddWithValue("@sl", c.SoLuong);
                                cmd.Parameters.AddWithValue("@cach", (object)c.CachDung ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        using (var cmd = new SqlCommand(
                            "UPDATE PhieuKham SET TrangThai = 1 WHERE MaPhieuKham = @pk", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@pk", maPhieuKham);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return ServiceResult.Ok("Lap toa thuoc thanh cong.", maToa);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return ServiceResult.Fail("Phieu kham nay da co toa thuoc (moi phieu chi 1 toa).");
                return ServiceResult.Fail("Loi CSDL: " + ex.Message);
            }
        }

        public ServiceResult ThanhToanVienPhi(int maPhieuKham, decimal tongTien)
        {
            if (maPhieuKham <= 0) return ServiceResult.Fail("Chua chon phieu kham.");
            if (tongTien < 0) return ServiceResult.Fail("Tong tien khong hop le.");
            try
            {
                // Upsert hoa don (moi phieu 1 hoa don - UNIQUE)
                int n = Db.ExecuteNonQuery(
                    @"IF EXISTS (SELECT 1 FROM HoaDonVienPhi WHERE MaPhieuKham=@pk)
                          UPDATE HoaDonVienPhi SET TongTien=@t, DaThanhToan=1 WHERE MaPhieuKham=@pk
                      ELSE
                          INSERT INTO HoaDonVienPhi (MaPhieuKham, TongTien, DaThanhToan) VALUES (@pk,@t,1)",
                    new SqlParameter("@pk", maPhieuKham), new SqlParameter("@t", tongTien));
                Db.ExecuteNonQuery("UPDATE PhieuKham SET TrangThai = 2 WHERE MaPhieuKham = @pk",
                    new SqlParameter("@pk", maPhieuKham));
                return ServiceResult.Ok("Thanh toan vien phi thanh cong.");
            }
            catch (SqlException ex) { return ServiceResult.Fail("Loi CSDL: " + ex.Message); }
        }

        public ServiceResult HoanTatDieuTri(int maPhieuKham)
        {
            try
            {
                using (var conn = new SqlConnection(Db.ConnectionString))
                using (var cmd = new SqlCommand("dbo.sp_HoanTatPhieuKham", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaPhieuKham", maPhieuKham);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return ServiceResult.Ok("Da hoan tat dot dieu tri (ghi nhan sau xuat vien).");
                }
            }
            catch (SqlException ex) { return ServiceResult.Fail(ex.Message); }
        }
    }
}
