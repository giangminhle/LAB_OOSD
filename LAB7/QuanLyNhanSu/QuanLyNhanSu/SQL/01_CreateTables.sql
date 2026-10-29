-- =============================================================
-- LAB7 - QUAN LY NHAN SU - CONG TY SON DONG A
-- DB: QuanLyNhanSu | SQL Server 2016+
-- Thiet ke CSDL nhat quan voi Class Diagram (18 bang).
-- Chay lai duoc nhieu lan: DROP theo thu tu nguoc FK roi tao lai.
-- =============================================================
IF DB_ID(N'QuanLyNhanSu') IS NULL CREATE DATABASE QuanLyNhanSu;
GO
USE QuanLyNhanSu;
GO
-- Cac SET option bat buoc khi tao computed column / filtered index.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
SET NOCOUNT ON;
GO

-- ---------- Xoa theo thu tu nguoc FK ----------
IF OBJECT_ID(N'dbo.ChiTietBangLuong', N'U') IS NOT NULL DROP TABLE dbo.ChiTietBangLuong;
IF OBJECT_ID(N'dbo.BangLuong', N'U') IS NOT NULL DROP TABLE dbo.BangLuong;
IF OBJECT_ID(N'dbo.PhieuThuNopPhat', N'U') IS NOT NULL DROP TABLE dbo.PhieuThuNopPhat;
IF OBJECT_ID(N'dbo.QuyetDinhKyLuat', N'U') IS NOT NULL DROP TABLE dbo.QuyetDinhKyLuat;
IF OBJECT_ID(N'dbo.PhieuChiKhenThuong', N'U') IS NOT NULL DROP TABLE dbo.PhieuChiKhenThuong;
IF OBJECT_ID(N'dbo.QuyetDinhKhenThuong', N'U') IS NOT NULL DROP TABLE dbo.QuyetDinhKhenThuong;
IF OBJECT_ID(N'dbo.ChiTietPhuTroi', N'U') IS NOT NULL DROP TABLE dbo.ChiTietPhuTroi;
IF OBJECT_ID(N'dbo.PhieuPhuTroi', N'U') IS NOT NULL DROP TABLE dbo.PhieuPhuTroi;
IF OBJECT_ID(N'dbo.ChamCong', N'U') IS NOT NULL DROP TABLE dbo.ChamCong;
IF OBJECT_ID(N'dbo.ChiTietKeHoach', N'U') IS NOT NULL DROP TABLE dbo.ChiTietKeHoach;
IF OBJECT_ID(N'dbo.KeHoachTuan', N'U') IS NOT NULL DROP TABLE dbo.KeHoachTuan;
IF OBJECT_ID(N'dbo.BaoHiem', N'U') IS NOT NULL DROP TABLE dbo.BaoHiem;
IF OBJECT_ID(N'dbo.QuyetDinhLuong', N'U') IS NOT NULL DROP TABLE dbo.QuyetDinhLuong;
IF OBJECT_ID(N'dbo.QuyetDinhChucVu', N'U') IS NOT NULL DROP TABLE dbo.QuyetDinhChucVu;
IF OBJECT_ID(N'dbo.HopDongLaoDong', N'U') IS NOT NULL DROP TABLE dbo.HopDongLaoDong;
IF OBJECT_ID(N'dbo.QuyetDinhTuyenDung', N'U') IS NOT NULL DROP TABLE dbo.QuyetDinhTuyenDung;
IF OBJECT_ID(N'dbo.NhanVien', N'U') IS NOT NULL DROP TABLE dbo.NhanVien;
IF OBJECT_ID(N'dbo.PhongBan', N'U') IS NOT NULL DROP TABLE dbo.PhongBan;
GO

-- =============================================================
-- 1. PHONG BAN
-- =============================================================
CREATE TABLE dbo.PhongBan (
    MaPB          INT IDENTITY(1,1) NOT NULL,
    TenPB         NVARCHAR(100) NOT NULL,
    DienThoai     VARCHAR(15) NULL,
    GhiChu        NVARCHAR(200) NULL,
    TrangThai     BIT NOT NULL CONSTRAINT DF_PhongBan_TrangThai DEFAULT (1),
    CONSTRAINT PK_PhongBan PRIMARY KEY (MaPB),
    CONSTRAINT UQ_PhongBan_Ten UNIQUE (TenPB)
);
GO

-- =============================================================
-- 2. NHAN VIEN
-- MaNV la ID ky thuat; MaNVCode la ma nghiep vu in tren ho so.
-- =============================================================
CREATE TABLE dbo.NhanVien (
    MaNV          INT IDENTITY(1,1) NOT NULL,
    MaNVCode      AS ('NV' + RIGHT('00000' + CONVERT(VARCHAR(10), MaNV), 5)) PERSISTED,
    HoTen         NVARCHAR(100) NOT NULL,
    GioiTinh      NVARCHAR(10) NOT NULL,
    NgaySinh      DATE NOT NULL,
    CCCD          VARCHAR(12) NOT NULL,
    DiaChi        NVARCHAR(200) NOT NULL,
    DienThoai     VARCHAR(15) NOT NULL,
    Email         VARCHAR(100) NULL,
    NgayVaoLam    DATE NOT NULL,
    MaPB          INT NOT NULL,
    TrangThai     NVARCHAR(20) NOT NULL,
    NgayTao       DATETIME NOT NULL CONSTRAINT DF_NhanVien_NgayTao DEFAULT (GETDATE()),
    CONSTRAINT PK_NhanVien PRIMARY KEY (MaNV),
    CONSTRAINT UQ_NhanVien_MaNVCode UNIQUE (MaNVCode),
    CONSTRAINT UQ_NhanVien_CCCD UNIQUE (CCCD),
    CONSTRAINT UQ_NhanVien_DienThoai UNIQUE (DienThoai),
    CONSTRAINT FK_NhanVien_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB),
    CONSTRAINT CK_NhanVien_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nu', N'Khac')),
    CONSTRAINT CK_NhanVien_TrangThai CHECK (TrangThai IN (N'ThuViec', N'DangLam', N'NghiViec')),
    CONSTRAINT CK_NhanVien_CCCD CHECK (LEN(CCCD) = 12 AND CCCD NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_NhanVien_NgaySinh CHECK (NgaySinh <= CONVERT(DATE, GETDATE()))
);
GO
CREATE INDEX IX_NhanVien_MaPB ON dbo.NhanVien(MaPB);
GO

-- =============================================================
-- 3. QUYET DINH TUYEN DUNG / THU VIEC
-- =============================================================
CREATE TABLE dbo.QuyetDinhTuyenDung (
    MaQDTD               INT IDENTITY(1,1) NOT NULL,
    SoQD                 VARCHAR(30) NOT NULL,
    MaNV                 INT NOT NULL,
    MaPB                 INT NOT NULL,
    NgayBanHanh          DATE NOT NULL,
    NgayBatDauThuViec    DATE NOT NULL,
    NgayKetThucThuViec   DATE NOT NULL,
    MucLuongThuViec      DECIMAL(18,0) NOT NULL,
    NoiDung              NVARCHAR(500) NOT NULL,
    NguoiKy              NVARCHAR(100) NOT NULL,
    TrangThai            NVARCHAR(20) NOT NULL CONSTRAINT DF_QDTD_TrangThai DEFAULT (N'HieuLuc'),
    CONSTRAINT PK_QuyetDinhTuyenDung PRIMARY KEY (MaQDTD),
    CONSTRAINT UQ_QDTD_SoQD UNIQUE (SoQD),
    CONSTRAINT FK_QDTD_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_QDTD_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB),
    CONSTRAINT CK_QDTD_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'HoanThanh', N'Huy')),
    CONSTRAINT CK_QDTD_Ngay CHECK (NgayBatDauThuViec >= NgayBanHanh),
    CONSTRAINT CK_QDTD_ThoiGian CHECK (NgayKetThucThuViec > NgayBatDauThuViec
        AND DATEDIFF(DAY, NgayBatDauThuViec, NgayKetThucThuViec) <= 60),
    CONSTRAINT CK_QDTD_Luong CHECK (MucLuongThuViec > 0)
);
GO
-- Moi nhan vien chi co 1 quyet dinh thu viec dang hieu luc.
CREATE UNIQUE INDEX UX_QDTD_DangHieuLuc ON dbo.QuyetDinhTuyenDung(MaNV)
    WHERE TrangThai = N'HieuLuc';
GO

-- =============================================================
-- 4. HOP DONG LAO DONG
-- DaiHan  : khong xac dinh thoi han (NgayHetHan = NULL), bat buoc BHXH/BHYT.
-- NganHan : co NgayHetHan, toi da 12 thang, khong bat buoc bao hiem.
-- =============================================================
CREATE TABLE dbo.HopDongLaoDong (
    MaHD          INT IDENTITY(1,1) NOT NULL,
    SoHD          VARCHAR(30) NOT NULL,
    MaNV          INT NOT NULL,
    MaQDTD        INT NULL,
    LoaiHD        NVARCHAR(20) NOT NULL,
    NgayKy        DATE NOT NULL,
    NgayHieuLuc   DATE NOT NULL,
    NgayHetHan    DATE NULL,
    MucLuong      DECIMAL(18,0) NOT NULL,
    TrangThai     NVARCHAR(20) NOT NULL CONSTRAINT DF_HD_TrangThai DEFAULT (N'HieuLuc'),
    CONSTRAINT PK_HopDongLaoDong PRIMARY KEY (MaHD),
    CONSTRAINT UQ_HopDong_SoHD UNIQUE (SoHD),
    CONSTRAINT FK_HopDong_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_HopDong_QDTD FOREIGN KEY (MaQDTD) REFERENCES dbo.QuyetDinhTuyenDung(MaQDTD),
    CONSTRAINT CK_HopDong_Loai CHECK (LoaiHD IN (N'DaiHan', N'NganHan')),
    CONSTRAINT CK_HopDong_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'HetHan', N'ChamDut')),
    CONSTRAINT CK_HopDong_Ngay CHECK (NgayHetHan IS NULL OR NgayHetHan > NgayHieuLuc),
    CONSTRAINT CK_HopDong_ThoiHan CHECK (
        (LoaiHD = N'DaiHan' AND NgayHetHan IS NULL)
        OR (LoaiHD = N'NganHan' AND NgayHetHan IS NOT NULL
            AND NgayHetHan <= DATEADD(MONTH, 12, NgayHieuLuc))),
    CONSTRAINT CK_HopDong_Luong CHECK (MucLuong > 0)
);
GO
CREATE UNIQUE INDEX UX_HopDong_DangHieuLuc ON dbo.HopDongLaoDong(MaNV)
    WHERE TrangThai = N'HieuLuc';
GO

-- =============================================================
-- 5. BAO HIEM (chi hop dong dai han)
-- =============================================================
CREATE TABLE dbo.BaoHiem (
    MaBH           INT IDENTITY(1,1) NOT NULL,
    MaNV           INT NOT NULL,
    MaHD           INT NOT NULL,
    LoaiBH         NVARCHAR(10) NOT NULL,
    SoSo           VARCHAR(30) NOT NULL,
    NgayCap        DATE NOT NULL,
    NgayHetHan     DATE NULL,
    MucDong        DECIMAL(18,0) NOT NULL,
    TyLeNhanVien   DECIMAL(5,2) NOT NULL,
    TyLeCongTy     DECIMAL(5,2) NOT NULL,
    TrangThai      NVARCHAR(20) NOT NULL CONSTRAINT DF_BaoHiem_TrangThai DEFAULT (N'HieuLuc'),
    CONSTRAINT PK_BaoHiem PRIMARY KEY (MaBH),
    CONSTRAINT UQ_BaoHiem_SoSo UNIQUE (LoaiBH, SoSo),
    CONSTRAINT FK_BaoHiem_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_BaoHiem_HopDong FOREIGN KEY (MaHD) REFERENCES dbo.HopDongLaoDong(MaHD),
    CONSTRAINT CK_BaoHiem_Loai CHECK (LoaiBH IN (N'BHXH', N'BHYT', N'BHTN')),
    CONSTRAINT CK_BaoHiem_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'DaDong')),
    CONSTRAINT CK_BaoHiem_MucDong CHECK (MucDong > 0),
    CONSTRAINT CK_BaoHiem_TyLe CHECK (TyLeNhanVien BETWEEN 0 AND 100 AND TyLeCongTy BETWEEN 0 AND 100),
    CONSTRAINT CK_BaoHiem_Ngay CHECK (NgayHetHan IS NULL OR NgayHetHan > NgayCap)
);
GO
CREATE UNIQUE INDEX UX_BaoHiem_DangHieuLuc ON dbo.BaoHiem(MaNV, LoaiBH)
    WHERE TrangThai = N'HieuLuc';
GO
-- Rang buoc nghiep vu: khong the lap bao hiem cho hop dong ngan han.
CREATE TRIGGER TR_BaoHiem_ChiDaiHan
ON dbo.BaoHiem
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.HopDongLaoDong hd ON hd.MaHD = i.MaHD
        WHERE hd.LoaiHD <> N'DaiHan')
    BEGIN
        ROLLBACK TRANSACTION;
        ;THROW 50011, N'Chi duoc lap bao hiem cho hop dong dai han.', 1;
    END
END
GO

-- =============================================================
-- 6. QUYET DINH CHUC VU / PHU CAP CHUC VU
-- =============================================================
CREATE TABLE dbo.QuyetDinhChucVu (
    MaQDChucVu     INT IDENTITY(1,1) NOT NULL,
    SoQD           VARCHAR(30) NOT NULL,
    MaNV           INT NOT NULL,
    MaPB           INT NOT NULL,
    ChucVu         NVARCHAR(50) NOT NULL,
    PhuCapChucVu   DECIMAL(18,0) NOT NULL CONSTRAINT DF_QDCV_PhuCap DEFAULT (0),
    NgayBanHanh    DATE NOT NULL,
    NgayHieuLuc    DATE NOT NULL,
    LyDo           NVARCHAR(200) NULL,
    NguoiKy        NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_QuyetDinhChucVu PRIMARY KEY (MaQDChucVu),
    CONSTRAINT UQ_QDCV_SoQD UNIQUE (SoQD),
    CONSTRAINT FK_QDCV_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_QDCV_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB),
    CONSTRAINT CK_QDCV_PhuCap CHECK (PhuCapChucVu >= 0),
    CONSTRAINT CK_QDCV_Ngay CHECK (NgayHieuLuc >= NgayBanHanh)
);
GO
CREATE INDEX IX_QDCV_NhanVien_Ngay ON dbo.QuyetDinhChucVu(MaNV, NgayHieuLuc DESC);
GO

-- =============================================================
-- 7. QUYET DINH LUONG
-- =============================================================
CREATE TABLE dbo.QuyetDinhLuong (
    MaQDLuong       INT IDENTITY(1,1) NOT NULL,
    SoQD            VARCHAR(30) NOT NULL,
    MaNV            INT NOT NULL,
    MucLuongCoBan   DECIMAL(18,0) NOT NULL,
    PhuCapLuong     DECIMAL(18,0) NOT NULL CONSTRAINT DF_QDL_PhuCapLuong DEFAULT (0),
    PhuCapDocHai    DECIMAL(18,0) NOT NULL CONSTRAINT DF_QDL_PhuCapDocHai DEFAULT (0),
    NgayBanHanh     DATE NOT NULL,
    NgayHieuLuc     DATE NOT NULL,
    LyDo            NVARCHAR(200) NULL,
    NguoiKy         NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_QuyetDinhLuong PRIMARY KEY (MaQDLuong),
    CONSTRAINT UQ_QDL_SoQD UNIQUE (SoQD),
    CONSTRAINT FK_QDL_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT CK_QDL_Luong CHECK (MucLuongCoBan > 0),
    CONSTRAINT CK_QDL_PhuCap CHECK (PhuCapLuong >= 0 AND PhuCapDocHai >= 0),
    CONSTRAINT CK_QDL_Ngay CHECK (NgayHieuLuc >= NgayBanHanh)
);
GO
CREATE INDEX IX_QDL_NhanVien_Ngay ON dbo.QuyetDinhLuong(MaNV, NgayHieuLuc DESC);
GO

-- =============================================================
-- 8. KE HOACH LAM VIEC TUAN
-- =============================================================
CREATE TABLE dbo.KeHoachTuan (
    MaKH        INT IDENTITY(1,1) NOT NULL,
    MaPB        INT NOT NULL,
    TuanSo      INT NOT NULL,
    Nam         INT NOT NULL,
    NgayLap     DATE NOT NULL,
    NguoiLap    NVARCHAR(100) NOT NULL,
    TrangThai   NVARCHAR(20) NOT NULL CONSTRAINT DF_KHT_TrangThai DEFAULT (N'Nhap'),
    CONSTRAINT PK_KeHoachTuan PRIMARY KEY (MaKH),
    CONSTRAINT FK_KHT_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB),
    CONSTRAINT UQ_KHT_PhongBan_Tuan UNIQUE (MaPB, TuanSo, Nam),
    CONSTRAINT CK_KHT_Tuan CHECK (TuanSo BETWEEN 1 AND 53),
    CONSTRAINT CK_KHT_Nam CHECK (Nam BETWEEN 2000 AND 2100),
    CONSTRAINT CK_KHT_TrangThai CHECK (TrangThai IN (N'Nhap', N'DaDuyet'))
);
GO

CREATE TABLE dbo.ChiTietKeHoach (
    MaCTKH      INT IDENTITY(1,1) NOT NULL,
    MaKH        INT NOT NULL,
    MaNV        INT NOT NULL,
    Ngay        DATE NOT NULL,
    CaLam       NVARCHAR(10) NOT NULL,
    LaPhuTroi   BIT NOT NULL CONSTRAINT DF_CTKH_PhuTroi DEFAULT (0),
    GhiChu      NVARCHAR(200) NULL,
    CONSTRAINT PK_ChiTietKeHoach PRIMARY KEY (MaCTKH),
    CONSTRAINT FK_CTKH_KeHoach FOREIGN KEY (MaKH) REFERENCES dbo.KeHoachTuan(MaKH) ON DELETE CASCADE,
    CONSTRAINT FK_CTKH_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT UQ_CTKH_NV_Ngay UNIQUE (MaKH, MaNV, Ngay),
    CONSTRAINT CK_CTKH_Ca CHECK (CaLam IN (N'Sang', N'Chieu', N'Toi'))
);
GO
CREATE INDEX IX_CTKH_Ngay ON dbo.ChiTietKeHoach(Ngay, MaNV);
GO

-- =============================================================
-- 9. CHAM CONG
-- =============================================================
CREATE TABLE dbo.ChamCong (
    MaChamCong   INT IDENTITY(1,1) NOT NULL,
    MaNV         INT NOT NULL,
    Ngay         DATE NOT NULL,
    MaCTKH       INT NULL,
    CaLam        NVARCHAR(10) NOT NULL,
    GioVao       TIME(0) NULL,
    GioRa        TIME(0) NULL,
    SoGioLam     DECIMAL(5,2) NOT NULL CONSTRAINT DF_ChamCong_SoGio DEFAULT (0),
    TrangThai    NVARCHAR(20) NOT NULL,
    GhiChu       NVARCHAR(200) NULL,
    CONSTRAINT PK_ChamCong PRIMARY KEY (MaChamCong),
    CONSTRAINT FK_ChamCong_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_ChamCong_CTKH FOREIGN KEY (MaCTKH) REFERENCES dbo.ChiTietKeHoach(MaCTKH) ON DELETE SET NULL,
    CONSTRAINT UQ_ChamCong_NV_Ngay UNIQUE (MaNV, Ngay),
    CONSTRAINT CK_ChamCong_TrangThai CHECK (TrangThai IN (N'DiLam', N'DiTre', N'Vang', N'CoPhep')),
    CONSTRAINT CK_ChamCong_Ca CHECK (CaLam IN (N'Sang', N'Chieu', N'Toi')),
    CONSTRAINT CK_ChamCong_Gio CHECK (
        (TrangThai IN (N'DiLam', N'DiTre') AND GioVao IS NOT NULL AND GioRa IS NOT NULL AND GioRa > GioVao)
        OR (TrangThai IN (N'Vang', N'CoPhep') AND GioVao IS NULL AND GioRa IS NULL)),
    CONSTRAINT CK_ChamCong_SoGio CHECK (SoGioLam BETWEEN 0 AND 24)
);
GO
CREATE INDEX IX_ChamCong_Thang ON dbo.ChamCong(Ngay, MaNV);
GO

-- =============================================================
-- 10. PHIEU GHI NHAN PHU TROI
-- =============================================================
CREATE TABLE dbo.PhieuPhuTroi (
    MaPhieuPT   INT IDENTITY(1,1) NOT NULL,
    SoPhieu     VARCHAR(30) NOT NULL,
    NgayPhieu   DATE NOT NULL,
    HinhThuc    NVARCHAR(20) NOT NULL,
    NguoiLap    NVARCHAR(100) NOT NULL,
    TrangThai   NVARCHAR(20) NOT NULL CONSTRAINT DF_PhieuPT_TrangThai DEFAULT (N'Nhap'),
    GhiChu      NVARCHAR(200) NULL,
    CONSTRAINT PK_PhieuPhuTroi PRIMARY KEY (MaPhieuPT),
    CONSTRAINT UQ_PhieuPT_SoPhieu UNIQUE (SoPhieu),
    CONSTRAINT CK_PhieuPT_HinhThuc CHECK (HinhThuc IN (N'TangCa', N'NgayLe', N'NgayNghi')),
    CONSTRAINT CK_PhieuPT_TrangThai CHECK (TrangThai IN (N'Nhap', N'DaDuyet'))
);
GO

CREATE TABLE dbo.ChiTietPhuTroi (
    MaCTPT      INT IDENTITY(1,1) NOT NULL,
    MaPhieuPT   INT NOT NULL,
    MaNV        INT NOT NULL,
    SoGio       DECIMAL(5,2) NOT NULL,
    HeSo        DECIMAL(4,2) NOT NULL CONSTRAINT DF_CTPT_HeSo DEFAULT (1.5),
    GhiChu      NVARCHAR(200) NULL,
    CONSTRAINT PK_ChiTietPhuTroi PRIMARY KEY (MaCTPT),
    CONSTRAINT FK_CTPT_Phieu FOREIGN KEY (MaPhieuPT) REFERENCES dbo.PhieuPhuTroi(MaPhieuPT) ON DELETE CASCADE,
    CONSTRAINT FK_CTPT_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT UQ_CTPT_Phieu_NV UNIQUE (MaPhieuPT, MaNV),
    CONSTRAINT CK_CTPT_SoGio CHECK (SoGio > 0 AND SoGio <= 12),
    CONSTRAINT CK_CTPT_HeSo CHECK (HeSo >= 1.5 AND HeSo <= 3.0)
);
GO

-- =============================================================
-- 11. KHEN THUONG
-- =============================================================
CREATE TABLE dbo.QuyetDinhKhenThuong (
    MaQDKhenThuong    INT IDENTITY(1,1) NOT NULL,
    SoQD              VARCHAR(30) NOT NULL,
    NgayBanHanh       DATE NOT NULL,
    DoiTuong          NVARCHAR(20) NOT NULL,
    MaNV              INT NULL,
    MaPB              INT NULL,
    LyDo              NVARCHAR(300) NOT NULL,
    HinhThucKhenThuong NVARCHAR(100) NOT NULL,
    SoTien            DECIMAL(18,0) NOT NULL,
    NguonChiTra       NVARCHAR(20) NOT NULL,
    NguoiKy           NVARCHAR(100) NOT NULL,
    TrangThai         NVARCHAR(20) NOT NULL CONSTRAINT DF_QDKT_TrangThai DEFAULT (N'HieuLuc'),
    CONSTRAINT PK_QuyetDinhKhenThuong PRIMARY KEY (MaQDKhenThuong),
    CONSTRAINT UQ_QDKT_SoQD UNIQUE (SoQD),
    CONSTRAINT FK_QDKT_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_QDKT_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB),
    CONSTRAINT CK_QDKT_DoiTuong CHECK (DoiTuong IN (N'CaNhan', N'TapThe')),
    CONSTRAINT CK_QDKT_Nguon CHECK (NguonChiTra IN (N'PhieuChi', N'Luong')),
    CONSTRAINT CK_QDKT_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'DaHuy')),
    CONSTRAINT CK_QDKT_SoTien CHECK (SoTien > 0),
    CONSTRAINT CK_QDKT_MucTieu CHECK (
        (DoiTuong = N'CaNhan' AND MaNV IS NOT NULL AND MaPB IS NULL)
        OR (DoiTuong = N'TapThe' AND MaNV IS NULL AND MaPB IS NOT NULL))
);
GO

CREATE TABLE dbo.PhieuChiKhenThuong (
    MaPhieuChi      INT IDENTITY(1,1) NOT NULL,
    SoPhieu         VARCHAR(30) NOT NULL,
    MaQDKhenThuong  INT NOT NULL,
    NgayChi         DATE NOT NULL,
    SoTien          DECIMAL(18,0) NOT NULL,
    NguoiChi        NVARCHAR(100) NOT NULL,
    GhiChu          NVARCHAR(200) NULL,
    CONSTRAINT PK_PhieuChiKhenThuong PRIMARY KEY (MaPhieuChi),
    CONSTRAINT UQ_PhieuChi_SoPhieu UNIQUE (SoPhieu),
    CONSTRAINT UQ_PhieuChi_QDKT UNIQUE (MaQDKhenThuong),
    CONSTRAINT FK_PhieuChi_QDKT FOREIGN KEY (MaQDKhenThuong) REFERENCES dbo.QuyetDinhKhenThuong(MaQDKhenThuong),
    CONSTRAINT CK_PhieuChi_SoTien CHECK (SoTien > 0)
);
GO

-- =============================================================
-- 12. KY LUAT
-- =============================================================
CREATE TABLE dbo.QuyetDinhKyLuat (
    MaQDKyLuat      INT IDENTITY(1,1) NOT NULL,
    SoQD            VARCHAR(30) NOT NULL,
    NgayBanHanh     DATE NOT NULL,
    DoiTuong        NVARCHAR(20) NOT NULL,
    MaNV            INT NULL,
    MaPB            INT NULL,
    LyDo            NVARCHAR(300) NOT NULL,
    HinhThucKyLuat  NVARCHAR(50) NOT NULL,
    SoTienPhat      DECIMAL(18,0) NOT NULL CONSTRAINT DF_QDKL_SoTien DEFAULT (0),
    NguonThu        NVARCHAR(20) NOT NULL,
    NguoiKy         NVARCHAR(100) NOT NULL,
    TrangThai       NVARCHAR(20) NOT NULL CONSTRAINT DF_QDKL_TrangThai DEFAULT (N'HieuLuc'),
    CONSTRAINT PK_QuyetDinhKyLuat PRIMARY KEY (MaQDKyLuat),
    CONSTRAINT UQ_QDKL_SoQD UNIQUE (SoQD),
    CONSTRAINT FK_QDKL_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT FK_QDKL_PhongBan FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB),
    CONSTRAINT CK_QDKL_DoiTuong CHECK (DoiTuong IN (N'CaNhan', N'TapThe')),
    CONSTRAINT CK_QDKL_Nguon CHECK (NguonThu IN (N'PhieuThu', N'TruLuong')),
    CONSTRAINT CK_QDKL_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'DaHuy')),
    CONSTRAINT CK_QDKL_SoTien CHECK (SoTienPhat >= 0),
    CONSTRAINT CK_QDKL_MucTieu CHECK (
        (DoiTuong = N'CaNhan' AND MaNV IS NOT NULL AND MaPB IS NULL)
        OR (DoiTuong = N'TapThe' AND MaNV IS NULL AND MaPB IS NOT NULL))
);
GO

CREATE TABLE dbo.PhieuThuNopPhat (
    MaPhieuThu      INT IDENTITY(1,1) NOT NULL,
    SoPhieu         VARCHAR(30) NOT NULL,
    MaQDKyLuat      INT NOT NULL,
    NgayThu         DATE NOT NULL,
    SoTien          DECIMAL(18,0) NOT NULL,
    NguoiThu        NVARCHAR(100) NOT NULL,
    GhiChu          NVARCHAR(200) NULL,
    CONSTRAINT PK_PhieuThuNopPhat PRIMARY KEY (MaPhieuThu),
    CONSTRAINT UQ_PhieuThu_SoPhieu UNIQUE (SoPhieu),
    CONSTRAINT UQ_PhieuThu_QDKL UNIQUE (MaQDKyLuat),
    CONSTRAINT FK_PhieuThu_QDKL FOREIGN KEY (MaQDKyLuat) REFERENCES dbo.QuyetDinhKyLuat(MaQDKyLuat),
    CONSTRAINT CK_PhieuThu_SoTien CHECK (SoTien > 0)
);
GO

-- =============================================================
-- 13. BANG LUONG THANG
-- =============================================================
CREATE TABLE dbo.BangLuong (
    MaBangLuong   INT IDENTITY(1,1) NOT NULL,
    Thang         TINYINT NOT NULL,
    Nam           INT NOT NULL,
    NgayLap       DATE NOT NULL,
    NguoiLap      NVARCHAR(100) NOT NULL,
    TrangThai     NVARCHAR(20) NOT NULL CONSTRAINT DF_BangLuong_TrangThai DEFAULT (N'Nhap'),
    TongChiPhi    DECIMAL(18,0) NOT NULL CONSTRAINT DF_BangLuong_Tong DEFAULT (0),
    CONSTRAINT PK_BangLuong PRIMARY KEY (MaBangLuong),
    CONSTRAINT UQ_BangLuong_Thang_Nam UNIQUE (Thang, Nam),
    CONSTRAINT CK_BangLuong_Thang CHECK (Thang BETWEEN 1 AND 12),
    CONSTRAINT CK_BangLuong_Nam CHECK (Nam BETWEEN 2000 AND 2100),
    CONSTRAINT CK_BangLuong_TrangThai CHECK (TrangThai IN (N'Nhap', N'DaChot')),
    CONSTRAINT CK_BangLuong_Tong CHECK (TongChiPhi >= 0)
);
GO

CREATE TABLE dbo.ChiTietBangLuong (
    MaCTBL            INT IDENTITY(1,1) NOT NULL,
    MaBangLuong       INT NOT NULL,
    MaNV              INT NOT NULL,
    MucLuongCoBan     DECIMAL(18,0) NOT NULL,
    SoNgayCong        INT NOT NULL,
    GioPhuTroi        DECIMAL(5,2) NOT NULL CONSTRAINT DF_CTBL_Gio DEFAULT (0),
    TienPhuTroi       DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_PhuTroi DEFAULT (0),
    PhuCapChucVu      DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_PCCV DEFAULT (0),
    PhuCapLuong       DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_PCL DEFAULT (0),
    PhuCapDocHai      DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_PCDH DEFAULT (0),
    TienKhenThuong    DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_KT DEFAULT (0),
    TienKyLuat        DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_KL DEFAULT (0),
    BHXH              DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_BHXH DEFAULT (0),
    BHYT              DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_BHYT DEFAULT (0),
    BHTN              DECIMAL(18,0) NOT NULL CONSTRAINT DF_CTBL_BHTN DEFAULT (0),
    ThucLinh          DECIMAL(18,0) NOT NULL,
    CONSTRAINT PK_ChiTietBangLuong PRIMARY KEY (MaCTBL),
    CONSTRAINT FK_CTBL_BangLuong FOREIGN KEY (MaBangLuong) REFERENCES dbo.BangLuong(MaBangLuong) ON DELETE CASCADE,
    CONSTRAINT FK_CTBL_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
    CONSTRAINT UQ_CTBL_BangLuong_NV UNIQUE (MaBangLuong, MaNV),
    CONSTRAINT CK_CTBL_NgayCong CHECK (SoNgayCong BETWEEN 0 AND 31),
    CONSTRAINT CK_CTBL_Gio CHECK (GioPhuTroi >= 0),
    CONSTRAINT CK_CTBL_Tien CHECK (TienPhuTroi >= 0 AND PhuCapChucVu >= 0 AND PhuCapLuong >= 0
        AND PhuCapDocHai >= 0 AND TienKhenThuong >= 0 AND TienKyLuat >= 0
        AND BHXH >= 0 AND BHYT >= 0 AND BHTN >= 0 AND ThucLinh >= 0)
);
GO
CREATE INDEX IX_CTBL_NhanVien ON dbo.ChiTietBangLuong(MaNV);
GO
-- Khong cho sua chi tiet cua bang luong da chot.
CREATE TRIGGER TR_ChiTietBangLuong_Khoa
ON dbo.ChiTietBangLuong
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i JOIN dbo.BangLuong b ON b.MaBangLuong = i.MaBangLuong
               WHERE b.TrangThai = N'DaChot')
       OR EXISTS (SELECT 1 FROM deleted d JOIN dbo.BangLuong b ON b.MaBangLuong = d.MaBangLuong
               WHERE b.TrangThai = N'DaChot')
    BEGIN
        ROLLBACK TRANSACTION;
        ;THROW 50010, N'Bang luong da chot, khong duoc sua chi tiet.', 1;
    END
END
GO

-- =============================================================
-- STORED PROCEDURES
-- =============================================================

-- Tiep nhan thu viec: NhanVien + QuyetDinhTuyenDung trong 1 transaction.
CREATE OR ALTER PROCEDURE dbo.sp_TiepNhanThuViec
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @CCCD VARCHAR(12),
    @DienThoai VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @Email VARCHAR(100) = NULL,
    @MaPB INT,
    @SoQD VARCHAR(30),
    @NgayBanHanh DATE,
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @MucLuong DECIMAL(18,0),
    @NoiDung NVARCHAR(500),
    @NguoiKy NVARCHAR(100),
    @MaNV INT OUTPUT,
    @MaQDTD INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.PhongBan WHERE MaPB = @MaPB AND TrangThai = 1)
    BEGIN
        ;THROW 50020, N'Phong ban khong ton tai hoac da ngung hoat dong.', 1;
    END

    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT dbo.NhanVien(HoTen, GioiTinh, NgaySinh, CCCD, DiaChi, DienThoai, Email,
                            NgayVaoLam, MaPB, TrangThai)
        VALUES (@HoTen, @GioiTinh, @NgaySinh, @CCCD, @DiaChi, @DienThoai, @Email,
                @NgayBatDau, @MaPB, N'ThuViec');
        SET @MaNV = SCOPE_IDENTITY();

        INSERT dbo.QuyetDinhTuyenDung(SoQD, MaNV, MaPB, NgayBanHanh, NgayBatDauThuViec,
                                      NgayKetThucThuViec, MucLuongThuViec, NoiDung, NguoiKy, TrangThai)
        VALUES (@SoQD, @MaNV, @MaPB, @NgayBanHanh, @NgayBatDau, @NgayKetThuc, @MucLuong,
                @NoiDung, @NguoiKy, N'HieuLuc');
        SET @MaQDTD = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Ky hop dong chinh thuc + 2 quyet dinh + bao hiem (neu dai han): 1 transaction.
CREATE OR ALTER PROCEDURE dbo.sp_KyHopDongChinhThuc
    @MaNV INT,
    @SoHD VARCHAR(30),
    @LoaiHD NVARCHAR(20),
    @NgayKy DATE,
    @NgayHieuLuc DATE,
    @NgayHetHan DATE = NULL,
    @MucLuong DECIMAL(18,0),
    @SoQDLuong VARCHAR(30),
    @PhuCapLuong DECIMAL(18,0) = 0,
    @PhuCapDocHai DECIMAL(18,0) = 0,
    @SoQDChucVu VARCHAR(30),
    @ChucVu NVARCHAR(50),
    @PhuCapChucVu DECIMAL(18,0) = 0,
    @NguoiKy NVARCHAR(100),
    @MaHD INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MaPB INT, @MaQDTD INT, @NgayKetThucThuViec DATE, @TrangThaiNV NVARCHAR(20);
    SELECT TOP 1 @MaPB = q.MaPB, @MaQDTD = q.MaQDTD,
                 @NgayKetThucThuViec = q.NgayKetThucThuViec, @TrangThaiNV = nv.TrangThai
    FROM dbo.QuyetDinhTuyenDung q
    JOIN dbo.NhanVien nv ON nv.MaNV = q.MaNV
    WHERE q.MaNV = @MaNV AND q.TrangThai = N'HieuLuc'
    ORDER BY q.MaQDTD DESC;

    IF @MaQDTD IS NULL
    BEGIN
        ;THROW 50021, N'Nhan vien khong co quyet dinh thu viec dang hieu luc.', 1;
    END
    IF @TrangThaiNV <> N'ThuViec'
    BEGIN
        ;THROW 50022, N'Nhan vien khong o trang thai thu viec.', 1;
    END
    IF @NgayHieuLuc < @NgayKetThucThuViec
    BEGIN
        ;THROW 50023, N'Ngay hieu luc hop dong phai sau ngay ket thuc thu viec.', 1;
    END
    IF @LoaiHD = N'DaiHan' AND @NgayHetHan IS NOT NULL
    BEGIN
        ;THROW 50024, N'Hop dong dai han khong co ngay het han.', 1;
    END
    IF @LoaiHD = N'NganHan' AND @NgayHetHan IS NULL
    BEGIN
        ;THROW 50025, N'Hop dong ngan han phai co ngay het han.', 1;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT dbo.HopDongLaoDong(SoHD, MaNV, MaQDTD, LoaiHD, NgayKy, NgayHieuLuc, NgayHetHan,
                                  MucLuong, TrangThai)
        VALUES (@SoHD, @MaNV, @MaQDTD, @LoaiHD, @NgayKy, @NgayHieuLuc, @NgayHetHan,
                @MucLuong, N'HieuLuc');
        SET @MaHD = SCOPE_IDENTITY();

        UPDATE dbo.QuyetDinhTuyenDung SET TrangThai = N'HoanThanh' WHERE MaQDTD = @MaQDTD;
        UPDATE dbo.NhanVien SET TrangThai = N'DangLam' WHERE MaNV = @MaNV;

        INSERT dbo.QuyetDinhLuong(SoQD, MaNV, MucLuongCoBan, PhuCapLuong, PhuCapDocHai,
                                  NgayBanHanh, NgayHieuLuc, LyDo, NguoiKy)
        VALUES (@SoQDLuong, @MaNV, @MucLuong, @PhuCapLuong, @PhuCapDocHai,
                @NgayKy, @NgayHieuLuc, N'Quyet dinh luong khi ky hop dong chinh thuc', @NguoiKy);

        INSERT dbo.QuyetDinhChucVu(SoQD, MaNV, MaPB, ChucVu, PhuCapChucVu,
                                   NgayBanHanh, NgayHieuLuc, LyDo, NguoiKy)
        VALUES (@SoQDChucVu, @MaNV, @MaPB, @ChucVu, @PhuCapChucVu,
                @NgayKy, @NgayHieuLuc, N'Quyet dinh chuc vu khi ky hop dong chinh thuc', @NguoiKy);

        IF @LoaiHD = N'DaiHan'
        BEGIN
            INSERT dbo.BaoHiem(MaNV, MaHD, LoaiBH, SoSo, NgayCap, NgayHetHan,
                               MucDong, TyLeNhanVien, TyLeCongTy, TrangThai)
            VALUES
                (@MaNV, @MaHD, N'BHXH', CONCAT('BHXH-', @MaNV, '-', FORMAT(@NgayHieuLuc, 'yyyyMMdd')),
                 @NgayHieuLuc, NULL, @MucLuong, 8.00, 17.50, N'HieuLuc'),
                (@MaNV, @MaHD, N'BHYT', CONCAT('BHYT-', @MaNV, '-', FORMAT(@NgayHieuLuc, 'yyyyMMdd')),
                 @NgayHieuLuc, NULL, @MucLuong, 1.50, 3.00, N'HieuLuc');
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Chot bang luong: chi khi da co chi tiet.
CREATE OR ALTER PROCEDURE dbo.sp_ChotBangLuong
    @MaBangLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.BangLuong WHERE MaBangLuong = @MaBangLuong)
    BEGIN
        ;THROW 50030, N'Bang luong khong ton tai.', 1;
    END
    IF NOT EXISTS (SELECT 1 FROM dbo.ChiTietBangLuong WHERE MaBangLuong = @MaBangLuong)
    BEGIN
        ;THROW 50031, N'Bang luong chua co chi tiet, khong the chot.', 1;
    END
    IF EXISTS (SELECT 1 FROM dbo.BangLuong WHERE MaBangLuong = @MaBangLuong AND TrangThai = N'DaChot')
    BEGIN
        ;THROW 50032, N'Bang luong da duoc chot truoc do.', 1;
    END
    UPDATE dbo.BangLuong
    SET TrangThai = N'DaChot', NgayLap = CONVERT(DATE, GETDATE())
    WHERE MaBangLuong = @MaBangLuong;
END
GO

PRINT N'Da tao xong 18 bang + 3 stored procedure + trigger cho CSDL QuanLyNhanSu.';
GO
