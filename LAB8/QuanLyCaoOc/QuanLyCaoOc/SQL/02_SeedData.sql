/* =====================================================================
   LAB8 - DU LIEU MAU (chay sau 01_CreateTables.sql)
   Gia thue = DienTich x 250.000 x HeSoGia (he so tang: +0,05 moi tang)
   ===================================================================== */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
USE QuanLyCaoOc;
GO

INSERT dbo.Tang (SoTang, HeSoGia, MoTa) VALUES
 (1, 1.00, N'Tầng trệt, sảnh'),
 (2, 1.05, NULL),
 (3, 1.10, NULL),
 (4, 1.15, NULL),
 (5, 1.20, N'Tầng cao, view đẹp');

/* MaPhong 1..10 */
INSERT dbo.Phong (MaPhongCode, MaTang, DienTich, Huong, GiaThue, TrangThai) VALUES
 ('P0101', 1,  80, N'Nam',     20000000, N'DangThue'),
 ('P0102', 1,  60, N'Dong',    15000000, N'DangThue'),
 ('P0201', 2, 100, N'Nam',     26250000, N'DangThue'),
 ('P0202', 2,  50, N'Tay',     13125000, N'Trong'),
 ('P0301', 3,  70, N'DongNam', 19250000, N'DangThue'),
 ('P0302', 3,  60, N'Bac',     16500000, N'DangThue'),
 ('P0401', 4, 120, N'Nam',     34500000, N'Trong'),
 ('P0402', 4,  40, N'TayBac',  11500000, N'SuaChua'),
 ('P0501', 5,  90, N'DongNam', 27000000, N'Trong'),
 ('P0502', 5,  55, N'Tay',     16500000, N'Trong');

INSERT dbo.KhachHang (TenCongTy, MaSoThue, NguoiDaiDien, DienThoai, Email, DiaChi) VALUES
 (N'Công ty TNHH Minh Phát', '0312345678', N'Nguyễn Văn An', '0901234567', 'lienhe@minhphat.vn', N'Q.1, TP.HCM'),
 (N'Công ty CP Sao Việt',    '0309876543', N'Lê Hoàng Cường', '0912345678', 'info@saoviet.vn', N'Q.3, TP.HCM'),
 (N'Công ty TNHH An Khang',  '0315556667', N'Võ Minh Em', '0987654321', 'ankhang@gmail.com', N'Q.7, TP.HCM');

/* HD1: KH1 thue P0101 + P0102 tu 01/01/2026, 12 thang -> khong no, gia han duoc
   HD2: KH2 thue P0201 tu 01/04/2026, 8 thang -> no tien thue thang 10/2026
   HD3: KH3 thue P0301 + P0302 tu 01/09/2026, 6 thang -> no hoa don dich vu thang 9/2026 */
INSERT dbo.HopDongThue (SoHD, MaKH, NgayKy, NgayBatDau, SoThang, NgayKetThuc, NoiDung) VALUES
 ('HD2026-0001', 1, '2025-12-20', '2026-01-01', 12, '2026-12-31',
  N'Trả trước 6 tháng đầu; từ tháng 7 trả đầu mỗi tháng; dịch vụ thanh toán cuối tháng.'),
 ('HD2026-0002', 2, '2026-03-25', '2026-04-01',  8, '2026-11-30',
  N'Trả trước 6 tháng đầu; từ tháng 7 trả đầu mỗi tháng; dịch vụ thanh toán cuối tháng.'),
 ('HD2026-0003', 3, '2026-08-25', '2026-09-01',  6, '2027-02-28',
  N'Trả trước 6 tháng đầu; dịch vụ thanh toán cuối tháng.');

INSERT dbo.ChiTietHopDong (MaHD, MaPhong, GiaThue, SoThangThue, NgayKetThuc) VALUES
 (1, 1, 20000000, 12, '2026-12-31'),
 (1, 2, 15000000, 12, '2026-12-31'),
 (2, 3, 26250000,  8, '2026-11-30'),
 (3, 5, 19250000,  6, '2027-02-28'),
 (3, 6, 16500000,  6, '2027-02-28');

INSERT dbo.HoaDonTienThue (SoHoaDon, MaHD, LoaiHoaDon, KyTu, KyDen, SoTien, HanThanhToan, DaThanhToan, NgayThanhToan) VALUES
 ('TT2026-0001', 1, N'TraTruoc6Thang', '2026-01-01', '2026-06-30', 210000000, '2026-01-01', 1, '2025-12-28'),
 ('TT2026-0002', 1, N'HangThang',      '2026-07-01', '2026-07-31',  35000000, '2026-07-05', 1, '2026-07-03'),
 ('TT2026-0003', 1, N'HangThang',      '2026-08-01', '2026-08-31',  35000000, '2026-08-05', 1, '2026-08-04'),
 ('TT2026-0004', 1, N'HangThang',      '2026-09-01', '2026-09-30',  35000000, '2026-09-05', 1, '2026-09-02'),
 ('TT2026-0005', 1, N'HangThang',      '2026-10-01', '2026-10-31',  35000000, '2026-10-05', 1, '2026-10-05'),
 ('TT2026-0006', 2, N'TraTruoc6Thang', '2026-04-01', '2026-09-30', 157500000, '2026-04-01', 1, '2026-03-30'),
 ('TT2026-0007', 2, N'HangThang',      '2026-10-01', '2026-10-31',  26250000, '2026-10-05', 0, NULL),
 ('TT2026-0008', 3, N'TraTruoc6Thang', '2026-09-01', '2027-02-28', 214500000, '2026-09-01', 1, '2026-08-28');

INSERT dbo.DichVu (TenDV, DonVi, DonGia) VALUES
 (N'Điện', N'kWh', 3500),
 (N'Nước', N'm3', 15000),
 (N'Internet', N'tháng', 300000);

INSERT dbo.HoaDonDichVu (SoHoaDon, MaHD, Thang, Nam, TongTien, HanThanhToan, DaThanhToan, NgayThanhToan) VALUES
 ('DV2026-0001', 1, 9, 2026, 8475000, '2026-10-05', 1, '2026-10-03'),
 ('DV2026-0002', 2, 9, 2026, 5925000, '2026-10-05', 1, '2026-10-04'),
 ('DV2026-0003', 3, 9, 2026, 6150000, '2026-10-05', 0, NULL);

INSERT dbo.ChiTietHoaDonDichVu (MaHDDV, MaPhong, MaDV, SoLuong, DonGia, ThanhTien) VALUES
 (1, 1, 1, 1200, 3500, 4200000), (1, 1, 2, 20, 15000, 300000), (1, 1, 3, 1, 300000, 300000),
 (1, 2, 1,  900, 3500, 3150000), (1, 2, 2, 15, 15000, 225000), (1, 2, 3, 1, 300000, 300000),
 (2, 3, 1, 1500, 3500, 5250000), (2, 3, 2, 25, 15000, 375000), (2, 3, 3, 1, 300000, 300000),
 (3, 5, 1,  800, 3500, 2800000), (3, 5, 2, 10, 15000, 150000), (3, 5, 3, 1, 300000, 300000),
 (3, 6, 1,  700, 3500, 2450000), (3, 6, 2, 10, 15000, 150000), (3, 6, 3, 1, 300000, 300000);

INSERT dbo.NhanVienVanPhong (MaNVCode, HoTen, CCCD, HinhAnh, MaKH, MaPhong, ChucVu, DienThoai, NgayBaoCao) VALUES
 ('NV0001', N'Nguyễn Văn An',   '079090001001', N'anh\NV0001.jpg', 1, 1, N'Giám đốc',      '0901234567', '2026-01-02'),
 ('NV0002', N'Trần Thị Bình',   '079095002002', N'anh\NV0002.jpg', 1, 2, N'Kế toán',       '0902345678', '2026-01-02'),
 ('NV0003', N'Lê Hoàng Cường',  '079088003003', N'anh\NV0003.jpg', 2, 3, N'Trưởng phòng',  '0912345678', '2026-04-01'),
 ('NV0004', N'Phạm Thu Dung',   '079099004004', NULL,              2, 3, N'Nhân viên',     '0913456789', '2026-05-10'),
 ('NV0005', N'Võ Minh Em',      '079085005005', N'anh\NV0005.jpg', 3, 5, N'Giám đốc',      '0987654321', '2026-09-01'),
 ('NV0006', N'Đặng Gia Huy',    '079001006006', NULL,              3, 6, N'Lập trình viên', '0976543210', '2026-09-15');

INSERT dbo.TheGiuXe (MaTheCode, MaNV, LoaiXe, BienSo, MauXe, NgayCap) VALUES
 ('TX0001', 1, N'OTo',   '51A-123.45',  N'Đen',  '2026-01-02'),
 ('TX0002', 2, N'XeMay', '59X1-234.56', N'Trắng', '2026-01-02'),
 ('TX0003', 3, N'XeMay', '59B2-111.22', N'Đỏ',   '2026-04-01'),
 ('TX0004', 5, N'OTo',   '51G-678.90',  N'Xám',  '2026-09-01');

INSERT dbo.KiemTraPhong (MaPhong, NgayKiemTra, TinhTrang, CoThayDoiKetCau, MoTa, KeHoachTuSua, NguoiKiemTra) VALUES
 (1, '2025-06-10', N'Tot',    0, N'Phòng sạch, thiết bị tốt', NULL, N'Ban quản lý cao ốc'),
 (3, '2025-06-10', N'CanSua', 0, N'Trần thấm nước nhẹ', N'Chống thấm trần trong quý 3/2025', N'Ban quản lý cao ốc'),
 (8, '2025-06-12', N'HuHong', 0, N'Hư hệ thống điện', N'Thay toàn bộ dây điện, tạm ngưng cho thuê', N'Ban quản lý cao ốc'),
 (3, '2026-06-15', N'CanSua', 1, N'Khách tự ý đập vách ngăn giữa 2 khu', N'Khôi phục vách ngăn theo thiết kế', N'Ban quản lý cao ốc');

INSERT dbo.PhieuPhat (SoPhieu, MaKT, MaHD, LyDo, SoTien, NgayLap) VALUES
 ('PP2026-0001', 4, 2, N'Tự ý thay đổi kết cấu văn phòng P0201 (đập vách ngăn)', 10000000, '2026-06-16');
GO
