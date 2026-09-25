namespace QuanLyKhachSan.Services
{
    /// <summary>
    /// Kết quả trả về của mỗi thao tác nghiệp vụ: có thành công hay không
    /// và thông báo tương ứng để Form hiển thị cho người dùng.
    /// </summary>
    public class KetQuaXuLy
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; }

        public static KetQuaXuLy Ok(string thongBao)
        {
            return new KetQuaXuLy { ThanhCong = true, ThongBao = thongBao };
        }

        public static KetQuaXuLy Fail(string thongBao)
        {
            return new KetQuaXuLy { ThanhCong = false, ThongBao = thongBao };
        }
    }

    /// <summary>Một dòng phòng được chọn khi lập phiếu đặt phòng.</summary>
    public class PhongDatItem
    {
        public string SoPhong { get; set; }
        public int SoNguoi { get; set; }
        public decimal DonGiaNgay { get; set; }
    }

    /// <summary>Một dòng tiện nghi hư hỏng/mất khi lập phiếu đền bù.</summary>
    public class DenBuItem
    {
        public string MaTienNghi { get; set; }
        public string TenLoaiTN { get; set; }
        public string MucDoThietHai { get; set; }
        public decimal SoTien { get; set; }
    }
}
