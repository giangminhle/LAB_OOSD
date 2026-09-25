using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public class PhongTienNghiService
    {
        public DataTable LayPhong() => Db.Query(
            "SELECT p.*, k.TenKhuVuc FROM Phong p JOIN KhuVuc k ON p.MaKhuVuc = k.MaKhuVuc ORDER BY p.SoPhong");

        public DataTable LayTienNghi() => Db.Query(
            "SELECT t.*, l.TenLoaiTN FROM TienNghi t JOIN LoaiTienNghi l ON t.MaLoaiTN = l.MaLoaiTN ORDER BY t.MaTienNghi");

        public DataTable LayLapDat() => Db.Query(
            "SELECT p.*, l.TenLoaiTN FROM PhieuLapDat p " +
            "JOIN TienNghi t ON p.MaTienNghi = t.MaTienNghi " +
            "JOIN LoaiTienNghi l ON t.MaLoaiTN = l.MaLoaiTN ORDER BY NgayLap DESC");

        public KetQuaXuLy ThemPhong(string so, string maKhu, int soNguoiToiDa, decimal donGia)
        {
            if (string.IsNullOrWhiteSpace(so) || string.IsNullOrWhiteSpace(maKhu) || soNguoiToiDa <= 0 || donGia < 0)
                return KetQuaXuLy.Fail("Thông tin phòng không hợp lệ (sức chứa phải > 0, đơn giá >= 0).");
            try
            {
                Db.Execute(
                    "INSERT INTO Phong(SoPhong,MaKhuVuc,SoNguoiToiDa,DonGiaNgay,TrangThai) VALUES(@s,@k,@m,@g,N'Trống')",
                    new SqlParameter("@s", so), new SqlParameter("@k", maKhu),
                    new SqlParameter("@m", soNguoiToiDa), new SqlParameter("@g", donGia));
                return KetQuaXuLy.Ok("Đã thêm phòng.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        public KetQuaXuLy ThemTienNghi(string ma, string maLoaiTN, int stt, string tinhTrang)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(maLoaiTN) || stt <= 0)
                return KetQuaXuLy.Fail("Thông tin tiện nghi không hợp lệ.");
            try
            {
                Db.Execute("INSERT INTO TienNghi VALUES(@m,@l,@s,@t)",
                    new SqlParameter("@m", ma), new SqlParameter("@l", maLoaiTN),
                    new SqlParameter("@s", stt),
                    new SqlParameter("@t", (object)tinhTrang ?? DBNull.Value));
                return KetQuaXuLy.Ok("Đã thêm tiện nghi.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return KetQuaXuLy.Fail("Số thứ tự này đã tồn tại trong cùng loại tiện nghi.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }

        /// <summary>
        /// Lập phiếu lắp đặt/luân chuyển thiết bị cho phòng.
        /// BR04: trong một ngày, một thiết bị chỉ được trang bị cho một phòng duy nhất
        /// (đảm bảo bằng UNIQUE(MaTienNghi, NgayLap) trong CSDL).
        /// </summary>
        public KetQuaXuLy LapDat(string soPhieu, string maTienNghi, string soPhong, DateTime ngay,
                                  string tinhTrang, string maNV, string ghiChu)
        {
            if (string.IsNullOrWhiteSpace(soPhieu) || string.IsNullOrWhiteSpace(maTienNghi) ||
                string.IsNullOrWhiteSpace(soPhong) || string.IsNullOrWhiteSpace(tinhTrang) ||
                string.IsNullOrWhiteSpace(maNV))
                return KetQuaXuLy.Fail("Phiếu lắp đặt chưa đủ thông tin.");

            try
            {
                Db.Execute("INSERT INTO PhieuLapDat VALUES(@p,@tn,@ph,@n,@tt,@nv,@g)",
                    new SqlParameter("@p", soPhieu), new SqlParameter("@tn", maTienNghi),
                    new SqlParameter("@ph", soPhong), new SqlParameter("@n", ngay.Date),
                    new SqlParameter("@tt", tinhTrang), new SqlParameter("@nv", maNV),
                    new SqlParameter("@g", (object)ghiChu ?? DBNull.Value));

                Db.Execute("UPDATE TienNghi SET TinhTrangHienTai=@tt WHERE MaTienNghi=@m",
                    new SqlParameter("@tt", tinhTrang), new SqlParameter("@m", maTienNghi));

                return KetQuaXuLy.Ok("Đã lập phiếu lắp đặt.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return KetQuaXuLy.Fail("Thiết bị này đã được lắp cho một phòng khác trong ngày đã chọn.");
            }
            catch (Exception ex) { return KetQuaXuLy.Fail(ex.Message); }
        }
    }
}
