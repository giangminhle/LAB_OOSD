using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public class DanhMucService
    {
        public DataTable LayKhuVuc() => Db.Query("SELECT * FROM KhuVuc ORDER BY MaKhuVuc");
        public DataTable LayNhanVien() => Db.Query("SELECT * FROM NhanVien ORDER BY MaNV");
        public DataTable LayLoaiTienNghi() => Db.Query("SELECT * FROM LoaiTienNghi ORDER BY MaLoaiTN");
        public DataTable LayDichVu() => Db.Query("SELECT * FROM DichVu ORDER BY MaDV");

        public DataTable LayQuyDinhDenBu() => Db.Query(
            "SELECT q.*, l.TenLoaiTN FROM QuyDinhDenBu q " +
            "JOIN LoaiTienNghi l ON q.MaLoaiTN = l.MaLoaiTN ORDER BY q.MaQuyDinh");

        public KetQuaXuLy ThemKhu(string ma, string ten)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
                return KetQuaXuLy.Fail("Mã khu vực và tên khu vực không được để trống.");
            try
            {
                Db.Execute("INSERT INTO KhuVuc VALUES(@m,@t)",
                    new SqlParameter("@m", ma), new SqlParameter("@t", ten));
                return KetQuaXuLy.Ok("Đã thêm khu vực.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        public KetQuaXuLy ThemNhanVien(string ma, string ten, string vaiTro, string sdt)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(vaiTro))
                return KetQuaXuLy.Fail("Thông tin nhân viên chưa đầy đủ.");
            try
            {
                Db.Execute("INSERT INTO NhanVien VALUES(@m,@t,@v,@s)",
                    new SqlParameter("@m", ma), new SqlParameter("@t", ten),
                    new SqlParameter("@v", vaiTro),
                    new SqlParameter("@s", (object)sdt ?? DBNull.Value));
                return KetQuaXuLy.Ok("Đã thêm nhân viên.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        public KetQuaXuLy ThemLoaiTN(string ma, string ten)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
                return KetQuaXuLy.Fail("Thông tin loại tiện nghi chưa đủ.");
            try
            {
                Db.Execute("INSERT INTO LoaiTienNghi VALUES(@m,@t)",
                    new SqlParameter("@m", ma), new SqlParameter("@t", ten));
                return KetQuaXuLy.Ok("Đã thêm loại tiện nghi.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        public KetQuaXuLy ThemDichVu(string ma, string ten, string dvt, decimal gia)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten) ||
                string.IsNullOrWhiteSpace(dvt) || gia < 0)
                return KetQuaXuLy.Fail("Thông tin dịch vụ không hợp lệ.");
            try
            {
                Db.Execute("INSERT INTO DichVu VALUES(@m,@t,@d,@g)",
                    new SqlParameter("@m", ma), new SqlParameter("@t", ten),
                    new SqlParameter("@d", dvt), new SqlParameter("@g", gia));
                return KetQuaXuLy.Ok("Đã thêm dịch vụ.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        public KetQuaXuLy ThemQuyDinh(string ma, string maLoaiTN, string mucDo, decimal tien)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(maLoaiTN) ||
                string.IsNullOrWhiteSpace(mucDo) || tien < 0)
                return KetQuaXuLy.Fail("Quy định đền bù không hợp lệ.");
            try
            {
                Db.Execute("INSERT INTO QuyDinhDenBu VALUES(@m,@l,@u,@t)",
                    new SqlParameter("@m", ma), new SqlParameter("@l", maLoaiTN),
                    new SqlParameter("@u", mucDo), new SqlParameter("@t", tien));
                return KetQuaXuLy.Ok("Đã thêm quy định đền bù.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return KetQuaXuLy.Fail("Loại tiện nghi này đã có quy định cho mức độ thiệt hại đã chọn.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }
    }
}
