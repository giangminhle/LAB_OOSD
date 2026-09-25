using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public class DatPhongService
    {
        public DataTable LayKhach() => Db.Query("SELECT * FROM KhachHang ORDER BY HoTen");

        public DataTable LayPhong() => Db.Query(
            "SELECT p.*, k.TenKhuVuc FROM Phong p JOIN KhuVuc k ON p.MaKhuVuc = k.MaKhuVuc ORDER BY p.SoPhong");

        public DataTable LayPhieuDat() => Db.Query(
            "SELECT d.*, k.HoTen FROM PhieuDatPhong d JOIN KhachHang k ON d.MaKhach = k.MaKhach ORDER BY d.NgayLap DESC");

        public DataTable LayChiTiet(string soPhieuDat) => Db.Query(
            "SELECT c.*, p.SoNguoiToiDa, p.DonGiaNgay FROM ChiTietDatPhong c " +
            "JOIN Phong p ON c.SoPhong = p.SoPhong WHERE c.SoPhieuDat=@s",
            new SqlParameter("@s", soPhieuDat));

        public DataTable LayNguoiLuuTru(string soPhieuDat) => Db.Query(
            "SELECT * FROM NguoiLuuTru WHERE SoPhieuDat=@s ORDER BY SoPhong,MaNguoiLT",
            new SqlParameter("@s", soPhieuDat));

        public KetQuaXuLy ThemKhach(string ma, string ten, string cmnd, string quocTich, string sdt)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten) ||
                string.IsNullOrWhiteSpace(cmnd) || string.IsNullOrWhiteSpace(quocTich))
                return KetQuaXuLy.Fail("Thông tin khách chưa đầy đủ.");
            try
            {
                Db.Execute("INSERT INTO KhachHang VALUES(@m,@t,@c,@q,@s)",
                    new SqlParameter("@m", ma), new SqlParameter("@t", ten),
                    new SqlParameter("@c", cmnd), new SqlParameter("@q", quocTich),
                    new SqlParameter("@s", (object)sdt ?? DBNull.Value));
                return KetQuaXuLy.Ok("Đã lưu khách hàng.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        /// <summary>Kiểm tra phòng có bị trùng lịch với các phiếu Đã đặt/Đang ở hay không (khoảng ngày giao nhau).</summary>
        private bool PhongTrungLich(SqlConnection cn, SqlTransaction tx, string phong, DateTime nhan, DateTime tra)
        {
            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM ChiTietDatPhong c JOIN PhieuDatPhong d ON c.SoPhieuDat=d.SoPhieuDat " +
                "WHERE c.SoPhong=@p AND d.TrangThai IN (N'Đã đặt',N'Đang ở') " +
                "AND @nhan<=d.NgayTraDuKien AND @tra>=d.NgayNhan", cn, tx);
            cmd.Parameters.AddWithValue("@p", phong);
            cmd.Parameters.AddWithValue("@nhan", nhan.Date);
            cmd.Parameters.AddWithValue("@tra", tra.Date);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// Lập phiếu đặt phòng cho một hoặc nhiều phòng trong cùng một giao dịch.
        /// Kiểm tra sức chứa (BR02) và trùng lịch trước khi ghi dữ liệu.
        /// </summary>
        public KetQuaXuLy TaoDatPhong(string soPhieu, string maKhach, string maNVLeTan, DateTime ngayLap,
                                       DateTime nhan, DateTime tra, decimal coc, string kenh,
                                       List<PhongDatItem> danhSach)
        {
            if (string.IsNullOrWhiteSpace(soPhieu) || string.IsNullOrWhiteSpace(maKhach) ||
                string.IsNullOrWhiteSpace(maNVLeTan) || danhSach == null || danhSach.Count == 0)
                return KetQuaXuLy.Fail("Phiếu đặt phòng chưa đủ thông tin (thiếu phòng hoặc khách).");
            if (tra.Date < nhan.Date)
                return KetQuaXuLy.Fail("Ngày trả dự kiến không được trước ngày nhận.");

            using (var cn = Db.OpenConnection())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    foreach (var x in danhSach)
                    {
                        var q = new SqlCommand("SELECT SoNguoiToiDa FROM Phong WHERE SoPhong=@p", cn, tx);
                        q.Parameters.AddWithValue("@p", x.SoPhong);
                        var o = q.ExecuteScalar();
                        if (o == null) { tx.Rollback(); return KetQuaXuLy.Fail("Không tìm thấy phòng " + x.SoPhong); }
                        if (x.SoNguoi <= 0 || x.SoNguoi > Convert.ToInt32(o))
                        { tx.Rollback(); return KetQuaXuLy.Fail("Số người của phòng " + x.SoPhong + " vượt sức chứa."); }
                        if (PhongTrungLich(cn, tx, x.SoPhong, nhan, tra))
                        { tx.Rollback(); return KetQuaXuLy.Fail("Phòng " + x.SoPhong + " bị trùng lịch đặt."); }
                    }

                    var h = new SqlCommand(
                        "INSERT INTO PhieuDatPhong(SoPhieuDat,MaKhach,MaNVLeTan,NgayLap,NgayNhan,NgayTraDuKien,TienCoc,KenhDat,TrangThai) " +
                        "VALUES(@s,@k,@nv,@lap,@nhan,@tra,@c,@kenh,N'Đã đặt')", cn, tx);
                    h.Parameters.AddWithValue("@s", soPhieu);
                    h.Parameters.AddWithValue("@k", maKhach);
                    h.Parameters.AddWithValue("@nv", maNVLeTan);
                    h.Parameters.AddWithValue("@lap", ngayLap);
                    h.Parameters.AddWithValue("@nhan", nhan.Date);
                    h.Parameters.AddWithValue("@tra", tra.Date);
                    h.Parameters.AddWithValue("@c", coc);
                    h.Parameters.AddWithValue("@kenh", kenh);
                    h.ExecuteNonQuery();

                    foreach (var x in danhSach)
                    {
                        var c = new SqlCommand("INSERT INTO ChiTietDatPhong VALUES(@s,@p,@n)", cn, tx);
                        c.Parameters.AddWithValue("@s", soPhieu);
                        c.Parameters.AddWithValue("@p", x.SoPhong);
                        c.Parameters.AddWithValue("@n", x.SoNguoi);
                        c.ExecuteNonQuery();

                        var u = new SqlCommand("UPDATE Phong SET TrangThai=N'Đã đặt' WHERE SoPhong=@p", cn, tx);
                        u.Parameters.AddWithValue("@p", x.SoPhong);
                        u.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return KetQuaXuLy.Ok("Đã lập phiếu đặt phòng.");
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return KetQuaXuLy.Fail(ex.Message);
                }
            }
        }

        public KetQuaXuLy ThemNguoiLuuTru(string soPhieuDat, string soPhong, string ten, string cmnd, string quocTich)
        {
            if (string.IsNullOrWhiteSpace(soPhieuDat) || string.IsNullOrWhiteSpace(soPhong) ||
                string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(cmnd) || string.IsNullOrWhiteSpace(quocTich))
                return KetQuaXuLy.Fail("Thông tin người lưu trú chưa đầy đủ.");
            try
            {
                var maxObj = Db.Scalar("SELECT SoNguoi FROM ChiTietDatPhong WHERE SoPhieuDat=@s AND SoPhong=@p",
                    new SqlParameter("@s", soPhieuDat), new SqlParameter("@p", soPhong));
                if (maxObj == null) return KetQuaXuLy.Fail("Không tìm thấy phòng trong phiếu đặt này.");
                int max = Convert.ToInt32(maxObj);

                int dem = Convert.ToInt32(Db.Scalar(
                    "SELECT COUNT(*) FROM NguoiLuuTru WHERE SoPhieuDat=@s AND SoPhong=@p",
                    new SqlParameter("@s", soPhieuDat), new SqlParameter("@p", soPhong)));

                if (dem >= max) return KetQuaXuLy.Fail("Đã đủ số người đăng ký cho phòng này.");

                Db.Execute("INSERT INTO NguoiLuuTru(SoPhieuDat,SoPhong,HoTen,SoCMND,QuocTich) VALUES(@s,@p,@t,@c,@q)",
                    new SqlParameter("@s", soPhieuDat), new SqlParameter("@p", soPhong),
                    new SqlParameter("@t", ten), new SqlParameter("@c", cmnd), new SqlParameter("@q", quocTich));
                return KetQuaXuLy.Ok("Đã thêm người lưu trú.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        public KetQuaXuLy NhanPhong(string soPhieuDat, DateTime thucTe)
        {
            using (var cn = Db.OpenConnection())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    var c = new SqlCommand(
                        "UPDATE PhieuDatPhong SET TrangThai=N'Đang ở',NgayNhanThucTe=@n " +
                        "WHERE SoPhieuDat=@s AND TrangThai=N'Đã đặt'", cn, tx);
                    c.Parameters.AddWithValue("@n", thucTe);
                    c.Parameters.AddWithValue("@s", soPhieuDat);
                    if (c.ExecuteNonQuery() == 0)
                    { tx.Rollback(); return KetQuaXuLy.Fail("Phiếu không ở trạng thái có thể nhận phòng."); }

                    var u = new SqlCommand(
                        "UPDATE Phong SET TrangThai=N'Đang ở' WHERE SoPhong IN " +
                        "(SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=@s)", cn, tx);
                    u.Parameters.AddWithValue("@s", soPhieuDat);
                    u.ExecuteNonQuery();

                    tx.Commit();
                    return KetQuaXuLy.Ok("Đã nhận phòng.");
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return KetQuaXuLy.Fail(ex.Message);
                }
            }
        }

        public KetQuaXuLy DanhDauNoShow(string soPhieuDat)
        {
            try
            {
                Db.Execute("UPDATE PhieuDatPhong SET TrangThai=N'No-show' WHERE SoPhieuDat=@s AND TrangThai=N'Đã đặt'",
                    new SqlParameter("@s", soPhieuDat));
                Db.Execute(
                    "UPDATE Phong SET TrangThai=N'Trống' WHERE SoPhong IN " +
                    "(SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=@s)",
                    new SqlParameter("@s", soPhieuDat));
                return KetQuaXuLy.Ok("Đã đánh dấu không nhận phòng (No-show).");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }
    }
}
