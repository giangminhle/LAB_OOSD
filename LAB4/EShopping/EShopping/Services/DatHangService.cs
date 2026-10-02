using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EShopping.Data;
using EShopping.Models;

namespace EShopping.Services
{
    public class DatHangService
    {
        // ---- Quy tắc nghiệp vụ (đưa về hằng số để dễ chỉnh) ----
        public const decimal NguongMienPhiNhanh = 1000000m;         // >= 1.000.000đ: chuyển phát nhanh miễn phí
        public const decimal NguongMienPhiNhanhTrongNgay = 5000000m;  // >= 5.000.000đ: nhanh trong ngày miễn phí

        // Phí cơ bản theo loại giao hàng (giả định; thực tế còn phụ thuộc khu vực giao hàng)
        private static readonly Dictionary<LoaiGiaoHang, decimal> PhiCoBan = new Dictionary<LoaiGiaoHang, decimal>
        {
            { LoaiGiaoHang.Thuong,         20000m },
            { LoaiGiaoHang.Nhanh,          40000m },
            { LoaiGiaoHang.NhanhTrongNgay, 70000m }
        };

        // Lệ phí thanh toán theo loại thẻ (tỷ lệ % trên tổng tiền hàng + ship) - giả định
        private static readonly Dictionary<LoaiThe, decimal> TyLePhiThe = new Dictionary<LoaiThe, decimal>
        {
            { LoaiThe.Visa,            0.010m },
            { LoaiThe.MasterCard,      0.010m },
            { LoaiThe.Discover,        0.015m },
            { LoaiThe.AmericanExpress, 0.020m }
        };

        private readonly IPaymentGateway _gateway;

        public DatHangService() : this(new MockPaymentGateway()) { }
        public DatHangService(IPaymentGateway gateway) { _gateway = gateway; }

        // =================== TÍNH TOÁN ===================
        public decimal TinhPhiGiaoHang(LoaiGiaoHang loai, decimal tongTienHang)
        {
            switch (loai)
            {
                case LoaiGiaoHang.Nhanh:
                    return tongTienHang >= NguongMienPhiNhanh ? 0m : PhiCoBan[loai];
                case LoaiGiaoHang.NhanhTrongNgay:
                    return tongTienHang >= NguongMienPhiNhanhTrongNgay ? 0m : PhiCoBan[loai];
                default:
                    return PhiCoBan[LoaiGiaoHang.Thuong];
            }
        }

        public decimal TinhPhiThe(LoaiThe loaiThe, decimal tongTienHang, decimal phiGiaoHang)
        {
            return Math.Round((tongTienHang + phiGiaoHang) * TyLePhiThe[loaiThe], 0);
        }

        public decimal TinhTongThanhToan(decimal tongTienHang, decimal phiGiaoHang, decimal phiThe)
        {
            return tongTienHang + phiGiaoHang + phiThe;
        }

        // =================== VALIDATE THẺ ===================
        /// <summary>Trả về null nếu hợp lệ, ngược lại trả về thông báo lỗi.</summary>
        public string ValidateThe(ThongTinThe the)
        {
            if (the == null) return "Vui lòng nhập thông tin thẻ.";

            string so = (the.SoThe ?? "").Replace(" ", "").Replace("-", "");
            if (so.Length == 0 || !so.All(char.IsDigit)) return "Số thẻ chỉ gồm chữ số.";

            bool laAmex = the.LoaiThe == LoaiThe.AmericanExpress;
            int doDaiSo = laAmex ? 15 : 16;
            int doDaiCsv = laAmex ? 4 : 3;

            if (so.Length != doDaiSo)
                return string.Format("Số thẻ {0} phải có {1} chữ số.", the.LoaiThe, doDaiSo);
            if (!KiemTraLuhn(so)) return "Số thẻ không hợp lệ (sai checksum).";

            if (string.IsNullOrWhiteSpace(the.TenChuThe)) return "Vui lòng nhập họ tên chủ thẻ.";

            string csv = the.Csv ?? "";
            if (csv.Length != doDaiCsv || !csv.All(char.IsDigit))
                return string.Format("Mã CSV của thẻ {0} phải có {1} chữ số.", the.LoaiThe, doDaiCsv);

            // Thẻ hết hạn vào ngày cuối của tháng ghi trên thẻ
            if (the.NgayHetHan.Date < DateTime.Today) return "Thẻ đã hết hạn.";
            return null;
        }

        private static bool KiemTraLuhn(string so)
        {
            int sum = 0; bool nhanDoi = false;
            for (int i = so.Length - 1; i >= 0; i--)
            {
                int d = so[i] - '0';
                if (nhanDoi) { d *= 2; if (d > 9) d -= 9; }
                sum += d; nhanDoi = !nhanDoi;
            }
            return sum % 10 == 0;
        }

        // =================== ĐẶT HÀNG ===================
        public ServiceResult DatHang(DonHang dh, ThongTinThe the)
        {
            if (dh == null) return ServiceResult.Fail("Dữ liệu đơn hàng không hợp lệ.");
            if (dh.MaKH <= 0) return ServiceResult.Fail("Vui lòng đăng nhập trước khi đặt hàng.");
            if (dh.TongTienHang <= 0) return ServiceResult.Fail("Giỏ hàng trống.");
            if (string.IsNullOrWhiteSpace(dh.TenNguoiNhan)) return ServiceResult.Fail("Vui lòng nhập họ tên người nhận.");
            if (string.IsNullOrWhiteSpace(dh.DiaChiNhan)) return ServiceResult.Fail("Vui lòng nhập địa chỉ người nhận.");
            if (string.IsNullOrWhiteSpace(dh.DienThoaiNhan)) return ServiceResult.Fail("Vui lòng nhập điện thoại người nhận.");

            string loiThe = ValidateThe(the);
            if (loiThe != null) return ServiceResult.Fail(loiThe);

            // Luôn TÍNH LẠI ở Service, không tin số liệu UI gửi xuống
            dh.LoaiThe = the.LoaiThe;
            dh.PhiGiaoHang = TinhPhiGiaoHang(dh.LoaiGiaoHang, dh.TongTienHang);
            dh.PhiThe = TinhPhiThe(the.LoaiThe, dh.TongTienHang, dh.PhiGiaoHang);
            dh.TongThanhToan = TinhTongThanhToan(dh.TongTienHang, dh.PhiGiaoHang, dh.PhiThe);

            // Kiểm tra thẻ với hệ thống thanh toán trực tuyến (Adapter)
            string tb;
            if (!_gateway.Authorize(the, dh.TongThanhToan, out tb))
                return ServiceResult.Fail("Thanh toán bị từ chối: " + tb);

            // Chỉ lưu số thẻ đã che; không lưu CSV
            string so = the.SoThe.Replace(" ", "").Replace("-", "");
            string mask = "**** **** **** " + so.Substring(so.Length - 4);

            const string sql = @"
INSERT INTO DonHang (MaKH, LoaiGiaoHang, TenNguoiNhan, DiaChiNhan, DienThoaiNhan,
                     TongTienHang, PhiGiaoHang, PhiThe, TongThanhToan,
                     LoaiThe, SoTheMask, TenChuThe, NgayHetHanThe)
OUTPUT INSERTED.MaDH
VALUES (@MaKH, @LoaiGH, @TenNhan, @DiaChiNhan, @DtNhan,
        @TongHang, @PhiGH, @PhiThe, @TongTT,
        @LoaiThe, @Mask, @ChuThe, @HetHan)";
            try
            {
                object id = Db.ExecuteScalar(sql,
                    new SqlParameter("@MaKH", dh.MaKH),
                    new SqlParameter("@LoaiGH", (byte)dh.LoaiGiaoHang),
                    new SqlParameter("@TenNhan", dh.TenNguoiNhan.Trim()),
                    new SqlParameter("@DiaChiNhan", dh.DiaChiNhan.Trim()),
                    new SqlParameter("@DtNhan", dh.DienThoaiNhan.Trim()),
                    new SqlParameter("@TongHang", dh.TongTienHang),
                    new SqlParameter("@PhiGH", dh.PhiGiaoHang),
                    new SqlParameter("@PhiThe", dh.PhiThe),
                    new SqlParameter("@TongTT", dh.TongThanhToan),
                    new SqlParameter("@LoaiThe", (byte)the.LoaiThe),
                    new SqlParameter("@Mask", mask),
                    new SqlParameter("@ChuThe", the.TenChuThe.Trim().ToUpper()),
                    new SqlParameter("@HetHan", the.NgayHetHan.Date));

                dh.MaDH = Convert.ToInt32(id);
                return ServiceResult.Ok("Đặt hàng thành công!", dh.MaDH);
            }
            catch (SqlException ex)
            {
                return ServiceResult.Fail("Lỗi cơ sở dữ liệu: " + ex.Message);
            }
        }
    }
}
