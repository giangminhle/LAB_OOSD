using EShopping.Models;

namespace EShopping.Services
{
    /// <summary>
    /// Adapter tới "Hệ thống dịch vụ thanh toán trực tuyến" bên ngoài.
    /// Khi có hệ thống thật, chỉ cần viết lớp mới implement interface này.
    /// </summary>
    public interface IPaymentGateway
    {
        bool Authorize(ThongTinThe the, decimal soTien, out string thongBao);
    }

    /// <summary>Giả lập cổng thanh toán cho bài thực hành: luôn chấp nhận.</summary>
    public class MockPaymentGateway : IPaymentGateway
    {
        public bool Authorize(ThongTinThe the, decimal soTien, out string thongBao)
        {
            thongBao = "Giao dịch hợp lệ (giả lập).";
            return true;
        }
    }
}
