using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public class TraPhongService
    {
        public DataTable LayPhieuDangO() => Db.Query(
            "SELECT d.SoPhieuDat, k.HoTen, d.NgayNhanThucTe, d.NgayTraDuKien FROM PhieuDatPhong d " +
            "JOIN KhachHang k ON d.MaKhach = k.MaKhach WHERE d.TrangThai=N'Đang ở' ORDER BY d.SoPhieuDat");

        public DataTable LayPhongTheoPhieu(string soPhieuDat) => Db.Query(
            "SELECT c.SoPhong, p.DonGiaNgay FROM ChiTietDatPhong c JOIN Phong p ON c.SoPhong = p.SoPhong " +
            "WHERE c.SoPhieuDat=@s", new SqlParameter("@s", soPhieuDat));

        public DataTable LayTienNghiPhong(string soPhong) => Db.Query(
            "SELECT TOP 100 p.MaTienNghi, l.TenLoaiTN, t.TinhTrangHienTai FROM PhieuLapDat p " +
            "JOIN TienNghi t ON p.MaTienNghi = t.MaTienNghi JOIN LoaiTienNghi l ON t.MaLoaiTN = l.MaLoaiTN " +
            "WHERE p.SoPhong=@p ORDER BY p.NgayLap DESC", new SqlParameter("@p", soPhong));

        public DataTable LayQuyDinh() => Db.Query(
            "SELECT q.*, l.TenLoaiTN FROM QuyDinhDenBu q JOIN LoaiTienNghi l ON q.MaLoaiTN = l.MaLoaiTN " +
            "ORDER BY l.TenLoaiTN, q.MucDoThietHai");

        public DataTable LayHoaDon() => Db.Query(
            "SELECT h.*, k.HoTen FROM HoaDon h JOIN PhieuDatPhong d ON h.SoPhieuDat = d.SoPhieuDat " +
            "JOIN KhachHang k ON d.MaKhach = k.MaKhach ORDER BY h.NgayLap DESC");

        /// <summary>Lập phiếu đền bù theo từng tiện nghi và mức độ thiệt hại (BR08).</summary>
        public KetQuaXuLy LapPhieuDenBu(string soPhieuDenBu, string soPhieuDat, string soPhong, DateTime ngay,
                                         string maNV, List<DenBuItem> danhSach)
        {
            if (string.IsNullOrWhiteSpace(soPhieuDenBu) || string.IsNullOrWhiteSpace(soPhieuDat) ||
                string.IsNullOrWhiteSpace(soPhong) || string.IsNullOrWhiteSpace(maNV) ||
                danhSach == null || danhSach.Count == 0)
                return KetQuaXuLy.Fail("Phiếu đền bù chưa đủ thông tin.");

            using (var cn = Db.OpenConnection())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    decimal tong = 0;
                    foreach (var x in danhSach)
                    {
                        if (x.SoTien < 0) { tx.Rollback(); return KetQuaXuLy.Fail("Mức đền bù không hợp lệ."); }
                        tong += x.SoTien;
                    }

                    var h = new SqlCommand("INSERT INTO PhieuDenBu VALUES(@so,@d,@p,@n,@nv,@t)", cn, tx);
                    h.Parameters.AddWithValue("@so", soPhieuDenBu);
                    h.Parameters.AddWithValue("@d", soPhieuDat);
                    h.Parameters.AddWithValue("@p", soPhong);
                    h.Parameters.AddWithValue("@n", ngay);
                    h.Parameters.AddWithValue("@nv", maNV);
                    h.Parameters.AddWithValue("@t", tong);
                    h.ExecuteNonQuery();

                    foreach (var x in danhSach)
                    {
                        var c = new SqlCommand("INSERT INTO ChiTietPhieuDenBu VALUES(@so,@tn,@m,@t)", cn, tx);
                        c.Parameters.AddWithValue("@so", soPhieuDenBu);
                        c.Parameters.AddWithValue("@tn", x.MaTienNghi);
                        c.Parameters.AddWithValue("@m", x.MucDoThietHai);
                        c.Parameters.AddWithValue("@t", x.SoTien);
                        c.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return KetQuaXuLy.Ok("Đã lập phiếu đền bù.");
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return KetQuaXuLy.Fail(ex.Message);
                }
            }
        }

        /// <summary>
        /// Lập hóa đơn gồm tiền phòng (tổng đơn giá các phòng x số ngày tính tiền) và tiền dịch vụ (BR09).
        /// Số ngày tính tiền do nhân viên xác nhận khi lập hóa đơn.
        /// </summary>
        public KetQuaXuLy LapHoaDon(string soHoaDon, string soPhieuDat, DateTime ngay, string maNV, int soNgayTinhTien)
        {
            if (string.IsNullOrWhiteSpace(soHoaDon) || string.IsNullOrWhiteSpace(soPhieuDat) ||
                string.IsNullOrWhiteSpace(maNV) || soNgayTinhTien <= 0)
                return KetQuaXuLy.Fail("Thông tin hóa đơn chưa hợp lệ.");
            try
            {
                decimal tienPhong = Convert.ToDecimal(Db.Scalar(
                    "SELECT ISNULL(SUM(p.DonGiaNgay),0) FROM ChiTietDatPhong c " +
                    "JOIN Phong p ON c.SoPhong = p.SoPhong WHERE c.SoPhieuDat=@s",
                    new SqlParameter("@s", soPhieuDat))) * soNgayTinhTien;

                decimal tienDV = Convert.ToDecimal(Db.Scalar(
                    "SELECT ISNULL(SUM(c.ThanhTien),0) FROM PhieuSuDungDV h " +
                    "JOIN ChiTietPhieuSuDungDV c ON h.SoPhieuSDDV = c.SoPhieuSDDV WHERE h.SoPhieuDat=@s",
                    new SqlParameter("@s", soPhieuDat)));

                Db.Execute(
                    "INSERT INTO HoaDon(SoHoaDon,SoPhieuDat,NgayLap,MaNV,SoNgayTinhTien,TienPhong,TienDichVu,TrangThai) " +
                    "VALUES(@h,@s,@n,@nv,@ng,@p,@d,N'Chưa thanh toán')",
                    new SqlParameter("@h", soHoaDon), new SqlParameter("@s", soPhieuDat),
                    new SqlParameter("@n", ngay), new SqlParameter("@nv", maNV),
                    new SqlParameter("@ng", soNgayTinhTien), new SqlParameter("@p", tienPhong),
                    new SqlParameter("@d", tienDV));

                return KetQuaXuLy.Ok("Đã lập hóa đơn tiền phòng và dịch vụ.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        /// <summary>
        /// Ghi nhận một giao dịch thanh toán. Một hóa đơn có thể có nhiều giao dịch (nhiều phương thức),
        /// nhưng tổng các giao dịch không được vượt quá tổng tiền hóa đơn.
        /// Khi tổng thanh toán = tổng hóa đơn thì chuyển trạng thái hóa đơn thành Đã thanh toán.
        /// </summary>
        public KetQuaXuLy ThanhToan(string maThanhToan, string soHoaDon, DateTime ngay, string hinhThuc, decimal tien)
        {
            if (string.IsNullOrWhiteSpace(maThanhToan) || string.IsNullOrWhiteSpace(soHoaDon) ||
                string.IsNullOrWhiteSpace(hinhThuc) || tien <= 0)
                return KetQuaXuLy.Fail("Thông tin thanh toán không hợp lệ.");

            using (var cn = Db.OpenConnection())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    var q = new SqlCommand("SELECT TongTien FROM HoaDon WHERE SoHoaDon=@h", cn, tx);
                    q.Parameters.AddWithValue("@h", soHoaDon);
                    object o = q.ExecuteScalar();
                    if (o == null) { tx.Rollback(); return KetQuaXuLy.Fail("Không tìm thấy hóa đơn."); }
                    decimal tong = Convert.ToDecimal(o);

                    var paid = new SqlCommand("SELECT ISNULL(SUM(SoTien),0) FROM ThanhToan WHERE SoHoaDon=@h", cn, tx);
                    paid.Parameters.AddWithValue("@h", soHoaDon);
                    decimal daTra = Convert.ToDecimal(paid.ExecuteScalar());

                    if (daTra + tien > tong)
                    { tx.Rollback(); return KetQuaXuLy.Fail("Số tiền thanh toán vượt số tiền còn phải trả."); }

                    var i = new SqlCommand("INSERT INTO ThanhToan VALUES(@m,@h,@n,@ht,@t)", cn, tx);
                    i.Parameters.AddWithValue("@m", maThanhToan);
                    i.Parameters.AddWithValue("@h", soHoaDon);
                    i.Parameters.AddWithValue("@n", ngay);
                    i.Parameters.AddWithValue("@ht", hinhThuc);
                    i.Parameters.AddWithValue("@t", tien);
                    i.ExecuteNonQuery();

                    if (daTra + tien == tong)
                    {
                        var u = new SqlCommand("UPDATE HoaDon SET TrangThai=N'Đã thanh toán' WHERE SoHoaDon=@h", cn, tx);
                        u.Parameters.AddWithValue("@h", soHoaDon);
                        u.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return KetQuaXuLy.Ok("Đã ghi nhận thanh toán bằng " + hinhThuc + ".");
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return KetQuaXuLy.Fail(ex.Message);
                }
            }
        }

        /// <summary>Hoàn tất trả phòng: chỉ cho phép khi hóa đơn đã thanh toán đủ.</summary>
        public KetQuaXuLy TraPhong(string soPhieuDat, DateTime ngayTra)
        {
            using (var cn = Db.OpenConnection())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    var q = new SqlCommand("SELECT h.SoHoaDon, h.TrangThai FROM HoaDon h WHERE h.SoPhieuDat=@s", cn, tx);
                    q.Parameters.AddWithValue("@s", soPhieuDat);
                    string trangThai;
                    using (var rd = q.ExecuteReader())
                    {
                        if (!rd.Read())
                        { tx.Rollback(); return KetQuaXuLy.Fail("Chưa lập hóa đơn cho phiếu đặt phòng này."); }
                        trangThai = Convert.ToString(rd["TrangThai"]);
                    }
                    if (trangThai != "Đã thanh toán")
                    { tx.Rollback(); return KetQuaXuLy.Fail("Hóa đơn chưa thanh toán đủ, không thể trả phòng."); }

                    var u1 = new SqlCommand(
                        "UPDATE PhieuDatPhong SET TrangThai=N'Đã trả',NgayTraThucTe=@n WHERE SoPhieuDat=@s", cn, tx);
                    u1.Parameters.AddWithValue("@n", ngayTra);
                    u1.Parameters.AddWithValue("@s", soPhieuDat);
                    u1.ExecuteNonQuery();

                    var u2 = new SqlCommand(
                        "UPDATE Phong SET TrangThai=N'Trống' WHERE SoPhong IN " +
                        "(SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=@s)", cn, tx);
                    u2.Parameters.AddWithValue("@s", soPhieuDat);
                    u2.ExecuteNonQuery();

                    tx.Commit();
                    return KetQuaXuLy.Ok("Đã hoàn tất trả phòng, phòng chuyển về trạng thái Trống.");
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return KetQuaXuLy.Fail(ex.Message);
                }
            }
        }
    }
}
