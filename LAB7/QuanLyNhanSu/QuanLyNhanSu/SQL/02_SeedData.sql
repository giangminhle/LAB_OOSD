-- =============================================================
-- LAB7 - DU LIEU MAU
-- Chay sau 01_CreateTables.sql. Du lieu duoc thiet ke de demo
-- va de kiem thu bang luong thang 10/2026.
-- Toan bo kich ban nam trong mot batch de dung bien @.
-- =============================================================
USE QuanLyNhanSu;
SET XACT_ABORT ON;
SET NOCOUNT ON;

DECLARE @PB_TC INT, @PB_KT INT, @PB_SX INT, @PB_NS INT;
DECLARE @NV1 INT, @NV2 INT, @NV3 INT, @NV4 INT, @NV5 INT, @NV6 INT, @NV7 INT;
DECLARE @Q1 INT, @Q2 INT, @Q3 INT, @Q4 INT, @Q5 INT, @Q6 INT;
DECLARE @HD1 INT, @HD2 INT, @HD4 INT, @HD6 INT;
DECLARE @KH INT, @KH2 INT, @PT INT;

-- ---------- Phong ban ----------
INSERT dbo.PhongBan(TenPB, DienThoai, GhiChu) VALUES
(N'Phòng Tài chính - Kế toán', '02838401201', N'Quản lý tài chính, kế toán, bảng lương'),
(N'Phòng Kỹ thuật', '02838401202', N'Thiết kế và kỹ thuật sản phẩm sơn'),
(N'Phòng Sản xuất', '02838401203', N'Vận hành dây chuyền sản xuất'),
(N'Phòng Nhân sự', '02838401204', N'Tuyển dụng, hợp đồng, bảo hiểm');
SET @PB_TC = (SELECT MaPB FROM dbo.PhongBan WHERE TenPB = N'Phòng Tài chính - Kế toán');
SET @PB_KT = (SELECT MaPB FROM dbo.PhongBan WHERE TenPB = N'Phòng Kỹ thuật');
SET @PB_SX = (SELECT MaPB FROM dbo.PhongBan WHERE TenPB = N'Phòng Sản xuất');
SET @PB_NS = (SELECT MaPB FROM dbo.PhongBan WHERE TenPB = N'Phòng Nhân sự');

-- ---------- Nhan vien ----------
INSERT dbo.NhanVien(HoTen, GioiTinh, NgaySinh, CCCD, DiaChi, DienThoai, Email, NgayVaoLam, MaPB, TrangThai) VALUES
(N'Nguyễn Văn An',  N'Nam', '1990-05-12', '079090005001', N'12 Lý Thường Kiệt, Q.10, TP.HCM', '0901000001', 'an.nguyen@sonDongA.vn', '2018-03-01', @PB_TC, N'DangLam'),
(N'Trần Thị Bình',  N'Nu',  '1992-08-20', '079092005002', N'45 Nguyễn Trãi, Q.5, TP.HCM',    '0901000002', 'binh.tran@sonDongA.vn', '2019-06-15', @PB_KT, N'DangLam'),
(N'Lê Hoàng Cường', N'Nam', '1988-02-10', '079088005003', N'88 Tạ Quang Bửu, Q.8, TP.HCM',   '0901000003', 'cuong.le@sonDongA.vn',  '2017-01-05', @PB_SX, N'DangLam'),
(N'Phạm Thu Dung',  N'Nu',  '1995-11-30', '079095005004', N'21 Điện Biên Phủ, Q.3, TP.HCM',  '0901000004', 'dung.pham@sonDongA.vn', '2020-09-01', @PB_NS, N'DangLam'),
(N'Võ Minh Đức',    N'Nam', '1998-04-18', '079098005005', N'105 Lê Văn Việt, Q.9, TP.HCM',   '0901000005', 'duc.vo@sonDongA.vn',    '2025-02-01', @PB_SX, N'DangLam'),
(N'Đặng Thị Hoa',   N'Nu',  '1993-07-07', '079093005006', N'33 Cách Mạng Tháng 8, Q.10',     '0901000006', 'hoa.dang@sonDongA.vn',  '2021-05-10', @PB_TC, N'DangLam'),
(N'Bùi Thanh Khoa', N'Nam', '2000-12-12', '079000005007', N'67 Phan Xích Long, Phú Nhuận',   '0901000007', 'khoa.bui@sonDongA.vn',  '2026-10-01', @PB_SX, N'ThuViec');
SET @NV1 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079090005001');
SET @NV2 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079092005002');
SET @NV3 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079088005003');
SET @NV4 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079095005004');
SET @NV5 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079098005005');
SET @NV6 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079093005006');
SET @NV7 = (SELECT MaNV FROM dbo.NhanVien WHERE CCCD = '079000005007');

-- ---------- Quyet dinh tuyen dung / thu viec (lich su) ----------
INSERT dbo.QuyetDinhTuyenDung(SoQD, MaNV, MaPB, NgayBanHanh, NgayBatDauThuViec, NgayKetThucThuViec,
                              MucLuongThuViec, NoiDung, NguoiKy, TrangThai) VALUES
('QDTD-2018-001', @NV1, @PB_TC, '2018-02-15', '2018-03-01', '2018-04-30', 10000000, N'Thử việc kế toán tổng hợp', N'Giám đốc Trần Văn Hùng', N'HoanThanh'),
('QDTD-2019-002', @NV2, @PB_KT, '2019-05-20', '2019-06-15', '2019-07-31', 8000000,  N'Thử việc kỹ thuật viên', N'Giám đốc Trần Văn Hùng', N'HoanThanh'),
('QDTD-2016-003', @NV3, @PB_SX, '2016-12-05', '2017-01-05', '2017-02-28', 7000000,  N'Thử việc tổ trưởng sản xuất', N'Giám đốc Trần Văn Hùng', N'HoanThanh'),
('QDTD-2020-004', @NV4, @PB_NS, '2020-08-01', '2020-09-01', '2020-10-15', 12000000, N'Thử việc chuyên viên nhân sự', N'Giám đốc Trần Văn Hùng', N'HoanThanh'),
('QDTD-2025-005', @NV5, @PB_SX, '2025-01-10', '2025-02-01', '2025-03-31', 6500000,  N'Thử việc công nhân vận hành', N'Giám đốc Trần Văn Hùng', N'HoanThanh'),
('QDTD-2021-006', @NV6, @PB_TC, '2021-04-15', '2021-05-10', '2021-06-30', 7500000,  N'Thử việc kế toán viên', N'Giám đốc Trần Văn Hùng', N'HoanThanh'),
('QDTD-2026-007', @NV7, @PB_SX, '2026-09-25', '2026-10-01', '2026-11-30', 7000000,  N'Thử việc công nhân sản xuất - đang theo dõi', N'Giám đốc Trần Văn Hùng', N'HieuLuc');
SET @Q1 = (SELECT MaQDTD FROM dbo.QuyetDinhTuyenDung WHERE SoQD = 'QDTD-2018-001');
SET @Q2 = (SELECT MaQDTD FROM dbo.QuyetDinhTuyenDung WHERE SoQD = 'QDTD-2019-002');
SET @Q3 = (SELECT MaQDTD FROM dbo.QuyetDinhTuyenDung WHERE SoQD = 'QDTD-2016-003');
SET @Q4 = (SELECT MaQDTD FROM dbo.QuyetDinhTuyenDung WHERE SoQD = 'QDTD-2020-004');
SET @Q5 = (SELECT MaQDTD FROM dbo.QuyetDinhTuyenDung WHERE SoQD = 'QDTD-2025-005');
SET @Q6 = (SELECT MaQDTD FROM dbo.QuyetDinhTuyenDung WHERE SoQD = 'QDTD-2021-006');

-- ---------- Hop dong lao dong dang hieu luc ----------
INSERT dbo.HopDongLaoDong(SoHD, MaNV, MaQDTD, LoaiHD, NgayKy, NgayHieuLuc, NgayHetHan, MucLuong, TrangThai) VALUES
('HD-2018-001', @NV1, @Q1, N'DaiHan',  '2018-04-25', '2018-05-01', NULL,         15000000, N'HieuLuc'),
('HD-2019-002', @NV2, @Q2, N'DaiHan',  '2019-07-25', '2019-08-01', NULL,         12000000, N'HieuLuc'),
('HD-2026-003', @NV3, @Q3, N'NganHan', '2025-12-20', '2026-01-01', '2026-12-31',  9000000, N'HieuLuc'),
('HD-2020-004', @NV4, @Q4, N'DaiHan',  '2020-10-10', '2020-10-15', NULL,         18000000, N'HieuLuc'),
('HD-2026-005', @NV5, @Q5, N'NganHan', '2025-12-20', '2026-01-01', '2026-12-31',  8000000, N'HieuLuc'),
('HD-2021-006', @NV6, @Q6, N'DaiHan',  '2021-06-20', '2021-07-01', NULL,         10000000, N'HieuLuc');
SET @HD1 = (SELECT MaHD FROM dbo.HopDongLaoDong WHERE SoHD = 'HD-2018-001');
SET @HD2 = (SELECT MaHD FROM dbo.HopDongLaoDong WHERE SoHD = 'HD-2019-002');
SET @HD4 = (SELECT MaHD FROM dbo.HopDongLaoDong WHERE SoHD = 'HD-2020-004');
SET @HD6 = (SELECT MaHD FROM dbo.HopDongLaoDong WHERE SoHD = 'HD-2021-006');

-- ---------- Bao hiem: chi 4 nhan vien co hop dong dai han ----------
INSERT dbo.BaoHiem(MaNV, MaHD, LoaiBH, SoSo, NgayCap, NgayHetHan, MucDong, TyLeNhanVien, TyLeCongTy, TrangThai) VALUES
(@NV1, @HD1, N'BHXH', 'BHXH-NV1', '2018-05-01', NULL, 15000000, 8.00, 17.50, N'HieuLuc'),
(@NV1, @HD1, N'BHYT', 'BHYT-NV1', '2018-05-01', NULL, 15000000, 1.50, 3.00,  N'HieuLuc'),
(@NV2, @HD2, N'BHXH', 'BHXH-NV2', '2019-08-01', NULL, 12000000, 8.00, 17.50, N'HieuLuc'),
(@NV2, @HD2, N'BHYT', 'BHYT-NV2', '2019-08-01', NULL, 12000000, 1.50, 3.00,  N'HieuLuc'),
(@NV4, @HD4, N'BHXH', 'BHXH-NV4', '2020-10-15', NULL, 18000000, 8.00, 17.50, N'HieuLuc'),
(@NV4, @HD4, N'BHYT', 'BHYT-NV4', '2020-10-15', NULL, 18000000, 1.50, 3.00,  N'HieuLuc'),
(@NV6, @HD6, N'BHXH', 'BHXH-NV6', '2021-07-01', NULL, 10000000, 8.00, 17.50, N'HieuLuc'),
(@NV6, @HD6, N'BHYT', 'BHYT-NV6', '2021-07-01', NULL, 10000000, 1.50, 3.00,  N'HieuLuc');

-- ---------- Quyet dinh luong ----------
INSERT dbo.QuyetDinhLuong(SoQD, MaNV, MucLuongCoBan, PhuCapLuong, PhuCapDocHai,
                          NgayBanHanh, NgayHieuLuc, LyDo, NguoiKy) VALUES
('QDL-2018-001', @NV1, 15000000, 1000000,  500000, '2018-04-25', '2018-05-01', N'Lương chính thức khi ký HĐLĐ', N'Giám đốc Trần Văn Hùng'),
('QDL-2019-002', @NV2, 12000000,  500000,       0, '2019-07-25', '2019-08-01', N'Lương chính thức khi ký HĐLĐ', N'Giám đốc Trần Văn Hùng'),
('QDL-2026-003', @NV3,  9000000,       0, 1000000, '2025-12-20', '2026-01-01', N'Lương hợp đồng ngắn hạn', N'Giám đốc Trần Văn Hùng'),
('QDL-2020-004', @NV4, 18000000, 1500000,       0, '2020-10-10', '2020-10-15', N'Lương chính thức khi ký HĐLĐ', N'Giám đốc Trần Văn Hùng'),
('QDL-2026-005', @NV5,  8000000,       0,  700000, '2025-12-20', '2026-01-01', N'Lương hợp đồng ngắn hạn', N'Giám đốc Trần Văn Hùng'),
('QDL-2021-006', @NV6, 10000000,  500000,       0, '2021-06-20', '2021-07-01', N'Lương chính thức khi ký HĐLĐ', N'Giám đốc Trần Văn Hùng');

-- ---------- Quyet dinh chuc vu ----------
INSERT dbo.QuyetDinhChucVu(SoQD, MaNV, MaPB, ChucVu, PhuCapChucVu, NgayBanHanh, NgayHieuLuc, LyDo, NguoiKy) VALUES
('QDCV-2018-001', @NV1, @PB_TC, N'Kế toán trưởng',        1500000, '2018-04-25', '2018-05-01', N'Bổ nhiệm kế toán trưởng', N'Giám đốc Trần Văn Hùng'),
('QDCV-2019-002', @NV2, @PB_KT, N'Kỹ thuật viên',         1000000, '2019-07-25', '2019-08-01', N'Bổ nhiệm kỹ thuật viên', N'Giám đốc Trần Văn Hùng'),
('QDCV-2026-003', @NV3, @PB_SX, N'Tổ trưởng sản xuất',    1200000, '2025-12-20', '2026-01-01', N'Bổ nhiệm tổ trưởng', N'Giám đốc Trần Văn Hùng'),
('QDCV-2020-004', @NV4, @PB_NS, N'Trưởng phòng nhân sự',  2000000, '2020-10-10', '2020-10-15', N'Bổ nhiệm trưởng phòng', N'Giám đốc Trần Văn Hùng'),
('QDCV-2026-005', @NV5, @PB_SX, N'Công nhân vận hành',          0, '2025-12-20', '2026-01-01', N'Phân công vị trí', N'Giám đốc Trần Văn Hùng'),
('QDCV-2021-006', @NV6, @PB_TC, N'Kế toán viên',            500000, '2021-06-20', '2021-07-01', N'Bổ nhiệm kế toán viên', N'Giám đốc Trần Văn Hùng');

-- ---------- Ke hoach tuan 42/2026 (12/10/2026 - 18/10/2026) ----------
INSERT dbo.KeHoachTuan(MaPB, TuanSo, Nam, NgayLap, NguoiLap, TrangThai)
VALUES (@PB_SX, 42, 2026, '2026-10-09', N'Lê Hoàng Cường', N'DaDuyet');
SET @KH = SCOPE_IDENTITY();
;WITH NgayLam AS (
    SELECT CONVERT(DATE, '2026-10-12') AS Ngay UNION ALL SELECT '2026-10-13' UNION ALL
    SELECT '2026-10-14' UNION ALL SELECT '2026-10-15' UNION ALL SELECT '2026-10-16'
)
INSERT dbo.ChiTietKeHoach(MaKH, MaNV, Ngay, CaLam, LaPhuTroi, GhiChu)
SELECT @KH, nv.MaNV, n.Ngay,
       CASE WHEN nv.MaNV % 2 = 0 THEN N'Sang' ELSE N'Chieu' END,
       CASE WHEN n.Ngay = '2026-10-15' THEN 1 ELSE 0 END,
       CASE WHEN n.Ngay = '2026-10-15' THEN N'Chuẩn bị lô sơn giao gấp' ELSE NULL END
FROM dbo.NhanVien nv
JOIN dbo.PhongBan pb ON pb.MaPB = nv.MaPB
CROSS JOIN NgayLam n
WHERE pb.TenPB = N'Phòng Sản xuất' AND nv.TrangThai IN (N'ThuViec', N'DangLam');

-- Ke hoach mau cho phong ke toan: tuan 43/2026
INSERT dbo.KeHoachTuan(MaPB, TuanSo, Nam, NgayLap, NguoiLap, TrangThai)
VALUES (@PB_TC, 43, 2026, '2026-10-16', N'Nguyễn Văn An', N'Nhap');
SET @KH2 = SCOPE_IDENTITY();
INSERT dbo.ChiTietKeHoach(MaKH, MaNV, Ngay, CaLam, LaPhuTroi) VALUES
(@KH2, @NV1, '2026-10-19', N'Sang', 0),
(@KH2, @NV6, '2026-10-19', N'Sang', 0),
(@KH2, @NV1, '2026-10-20', N'Sang', 1);

-- ---------- Cham cong thang 10/2026: 26 ngay cong cho 6 nhan vien chinh thuc ----------
;WITH SoNgay AS (
    SELECT TOP (26) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS d
    FROM sys.all_objects
)
INSERT dbo.ChamCong(MaNV, Ngay, CaLam, GioVao, GioRa, SoGioLam, TrangThai, GhiChu)
SELECT nv.MaNV, DATEADD(DAY, d - 1, CONVERT(DATE, '2026-10-01')), N'Sang',
       CONVERT(TIME(0), '08:00'), CONVERT(TIME(0), '17:00'), 8.00, N'DiLam', N'Đủ công'
FROM dbo.NhanVien nv
CROSS JOIN SoNgay
WHERE nv.TrangThai = N'DangLam';

-- ---------- Phieu phu troi thang 10/2026 ----------
INSERT dbo.PhieuPhuTroi(SoPhieu, NgayPhieu, HinhThuc, NguoiLap, TrangThai, GhiChu)
VALUES ('PT-2026-10-001', '2026-10-15', N'TangCa', N'Lê Hoàng Cường', N'DaDuyet', N'Tăng ca sản xuất lô sơn giao gấp');
SET @PT = SCOPE_IDENTITY();
INSERT dbo.ChiTietPhuTroi(MaPhieuPT, MaNV, SoGio, HeSo, GhiChu) VALUES
(@PT, @NV1, 8.00,  1.5, N'Đối soát bảng lương'),
(@PT, @NV3, 10.00, 1.5, N'Trực dây chuyền'),
(@PT, @NV5, 6.00,  1.5, N'Vệ sinh máy trộn');

-- ---------- Khen thuong / ky luat thang 10/2026 ----------
INSERT dbo.QuyetDinhKhenThuong(SoQD, NgayBanHanh, DoiTuong, MaNV, MaPB, LyDo,
        HinhThucKhenThuong, SoTien, NguonChiTra, NguoiKy, TrangThai) VALUES
('QDKT-2026-10-001', '2026-10-20', N'CaNhan', @NV1, NULL, N'Hoàn thành xuất sắc nhiệm vụ quý III',
 N'Thưởng hiệu quả công việc', 2000000, N'Luong', N'Giám đốc Trần Văn Hùng', N'HieuLuc'),
('QDKT-2026-10-002', '2026-10-21', N'TapThe', NULL, @PB_SX, N'Tập thể phòng sản xuất đạt chỉ tiêu tháng 9',
 N'Thưởng tập thể', 6000000, N'Luong', N'Giám đốc Trần Văn Hùng', N'HieuLuc'),
('QDKT-2026-10-003', '2026-10-22', N'CaNhan', @NV2, NULL, N'Sáng kiến cải tiến quy trình kiểm tra màu sơn',
 N'Thưởng sáng kiến', 1000000, N'PhieuChi', N'Giám đốc Trần Văn Hùng', N'HieuLuc');

INSERT dbo.QuyetDinhKyLuat(SoQD, NgayBanHanh, DoiTuong, MaNV, MaPB, LyDo,
        HinhThucKyLuat, SoTienPhat, NguonThu, NguoiKy, TrangThai) VALUES
('QDKL-2026-10-001', '2026-10-22', N'CaNhan', @NV5, NULL, N'Không đeo đồ bảo hộ khi vận hành máy trộn',
 N'Phạt tiền', 500000, N'TruLuong', N'Giám đốc Trần Văn Hùng', N'HieuLuc'),
('QDKL-2026-10-002', '2026-10-22', N'CaNhan', @NV6, NULL, N'Đi trễ 3 lần trong tháng 10/2026',
 N'Phạt tiền', 200000, N'PhieuThu', N'Giám đốc Trần Văn Hùng', N'HieuLuc');

-- Phieu chi / phieu thu tuong ung voi quyet dinh nguon PhieuChi / PhieuThu
INSERT dbo.PhieuChiKhenThuong(SoPhieu, MaQDKhenThuong, NgayChi, SoTien, NguoiChi, GhiChu)
SELECT 'PC-2026-10-001', MaQDKhenThuong, '2026-10-23', SoTien, N'Nguyễn Văn An', N'Chi thưởng sáng kiến'
FROM dbo.QuyetDinhKhenThuong WHERE SoQD = 'QDKT-2026-10-003';

INSERT dbo.PhieuThuNopPhat(SoPhieu, MaQDKyLuat, NgayThu, SoTien, NguoiThu, GhiChu)
SELECT 'PTNP-2026-10-001', MaQDKyLuat, '2026-10-23', SoTienPhat, N'Nguyễn Văn An', N'Thu phạt đi trễ'
FROM dbo.QuyetDinhKyLuat WHERE SoQD = 'QDKL-2026-10-002';

PRINT N'Da nap du lieu mau LAB7 (7 nhan vien, 6 hop dong, 8 bao hiem, luong 10/2026).';
