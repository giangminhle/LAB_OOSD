/* =====================================================================
   LAB8 - QUAN LY CHO THUE VAN PHONG O CAO OC
   Script tao CSDL QuanLyCaoOc: 14 bang, PK identity, FK, CHECK, UNIQUE
   ===================================================================== */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
IF DB_ID(N'QuanLyCaoOc') IS NULL
    CREATE DATABASE QuanLyCaoOc;
GO
USE QuanLyCaoOc;
GO

/* Xoa bang cu theo thu tu phu thuoc */
IF OBJECT_ID(N'dbo.PhieuPhat', N'U') IS NOT NULL DROP TABLE dbo.PhieuPhat;
IF OBJECT_ID(N'dbo.KiemTraPhong', N'U') IS NOT NULL DROP TABLE dbo.KiemTraPhong;
IF OBJECT_ID(N'dbo.TheGiuXe', N'U') IS NOT NULL DROP TABLE dbo.TheGiuXe;
IF OBJECT_ID(N'dbo.NhanVienVanPhong', N'U') IS NOT NULL DROP TABLE dbo.NhanVienVanPhong;
IF OBJECT_ID(N'dbo.ChiTietHoaDonDichVu', N'U') IS NOT NULL DROP TABLE dbo.ChiTietHoaDonDichVu;
IF OBJECT_ID(N'dbo.HoaDonDichVu', N'U') IS NOT NULL DROP TABLE dbo.HoaDonDichVu;
IF OBJECT_ID(N'dbo.DichVu', N'U') IS NOT NULL DROP TABLE dbo.DichVu;
IF OBJECT_ID(N'dbo.HoaDonTienThue', N'U') IS NOT NULL DROP TABLE dbo.HoaDonTienThue;
IF OBJECT_ID(N'dbo.PhieuGiaHan', N'U') IS NOT NULL DROP TABLE dbo.PhieuGiaHan;
IF OBJECT_ID(N'dbo.ChiTietHopDong', N'U') IS NOT NULL DROP TABLE dbo.ChiTietHopDong;
IF OBJECT_ID(N'dbo.HopDongThue', N'U') IS NOT NULL DROP TABLE dbo.HopDongThue;
IF OBJECT_ID(N'dbo.KhachHang', N'U') IS NOT NULL DROP TABLE dbo.KhachHang;
IF OBJECT_ID(N'dbo.Phong', N'U') IS NOT NULL DROP TABLE dbo.Phong;
IF OBJECT_ID(N'dbo.Tang', N'U') IS NOT NULL DROP TABLE dbo.Tang;
GO

CREATE TABLE dbo.Tang (
    MaTang   INT IDENTITY(1,1) PRIMARY KEY,
    SoTang   INT NOT NULL,
    HeSoGia  DECIMAL(4,2) NOT NULL,             -- tang cang cao he so cang lon
    MoTa     NVARCHAR(200) NULL,
    CONSTRAINT UQ_Tang_SoTang UNIQUE (SoTang),
    CONSTRAINT CK_Tang_SoTang CHECK (SoTang BETWEEN 1 AND 50),
    CONSTRAINT CK_Tang_HeSo CHECK (HeSoGia > 0)
);

CREATE TABLE dbo.Phong (
    MaPhong     INT IDENTITY(1,1) PRIMARY KEY,
    MaPhongCode VARCHAR(10) NOT NULL,           -- VD P0501 = tang 5 phong 01
    MaTang      INT NOT NULL,
    DienTich    DECIMAL(8,2) NOT NULL,          -- m2
    Huong       NVARCHAR(20) NOT NULL,
    GiaThue     DECIMAL(18,0) NOT NULL,         -- VND / thang, chua gom dich vu
    TrangThai   NVARCHAR(20) NOT NULL DEFAULT N'Trong',
    CONSTRAINT UQ_Phong_Code UNIQUE (MaPhongCode),
    CONSTRAINT FK_Phong_Tang FOREIGN KEY (MaTang) REFERENCES dbo.Tang(MaTang),
    CONSTRAINT CK_Phong_DienTich CHECK (DienTich > 0),
    CONSTRAINT CK_Phong_Gia CHECK (GiaThue > 0),
    CONSTRAINT CK_Phong_Huong CHECK (Huong IN (N'Dong', N'Tay', N'Nam', N'Bac',
        N'DongNam', N'DongBac', N'TayNam', N'TayBac')),
    CONSTRAINT CK_Phong_TrangThai CHECK (TrangThai IN (N'Trong', N'DangThue', N'SuaChua'))
);

CREATE TABLE dbo.KhachHang (
    MaKH         INT IDENTITY(1,1) PRIMARY KEY,
    TenCongTy    NVARCHAR(150) NOT NULL,
    MaSoThue     VARCHAR(14) NOT NULL,
    NguoiDaiDien NVARCHAR(100) NOT NULL,
    DienThoai    VARCHAR(15) NOT NULL,
    Email        VARCHAR(100) NULL,
    DiaChi       NVARCHAR(200) NULL,
    CONSTRAINT UQ_KhachHang_MST UNIQUE (MaSoThue)
);

CREATE TABLE dbo.HopDongThue (
    MaHD        INT IDENTITY(1,1) PRIMARY KEY,
    SoHD        VARCHAR(20) NOT NULL,
    MaKH        INT NOT NULL,
    NgayKy      DATE NOT NULL,
    NgayBatDau  DATE NOT NULL,
    SoThang     INT NOT NULL,                   -- thoi gian thue ban dau
    NgayKetThuc DATE NOT NULL,                  -- cap nhat khi gia han
    NoiDung     NVARCHAR(500) NULL,
    TrangThai   NVARCHAR(20) NOT NULL DEFAULT N'HieuLuc',
    CONSTRAINT UQ_HopDong_SoHD UNIQUE (SoHD),
    CONSTRAINT FK_HopDong_KhachHang FOREIGN KEY (MaKH) REFERENCES dbo.KhachHang(MaKH),
    CONSTRAINT CK_HopDong_SoThang CHECK (SoThang >= 6),
    CONSTRAINT CK_HopDong_Ngay CHECK (NgayKetThuc > NgayBatDau AND NgayKy <= NgayBatDau),
    CONSTRAINT CK_HopDong_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'KetThuc', N'Huy'))
);

/* Mot hop dong thue nhieu van phong */
CREATE TABLE dbo.ChiTietHopDong (
    MaCTHD      INT IDENTITY(1,1) PRIMARY KEY,
    MaHD        INT NOT NULL,
    MaPhong     INT NOT NULL,
    GiaThue     DECIMAL(18,0) NOT NULL,         -- gia chot luc ky
    SoThangThue INT NOT NULL,
    NgayKetThuc DATE NOT NULL,
    CONSTRAINT UQ_CTHD_HD_Phong UNIQUE (MaHD, MaPhong),
    CONSTRAINT FK_CTHD_HopDong FOREIGN KEY (MaHD) REFERENCES dbo.HopDongThue(MaHD),
    CONSTRAINT FK_CTHD_Phong FOREIGN KEY (MaPhong) REFERENCES dbo.Phong(MaPhong),
    CONSTRAINT CK_CTHD_Gia CHECK (GiaThue > 0),
    CONSTRAINT CK_CTHD_SoThang CHECK (SoThangThue >= 6)
);

CREATE TABLE dbo.PhieuGiaHan (
    MaPGH          INT IDENTITY(1,1) PRIMARY KEY,
    SoPhieu        VARCHAR(20) NOT NULL,
    MaCTHD         INT NOT NULL,
    NgayLap        DATE NOT NULL,
    SoThangGiaHan  INT NOT NULL,
    NgayKetThucCu  DATE NOT NULL,
    NgayKetThucMoi DATE NOT NULL,
    NguoiLap       NVARCHAR(100) NULL,
    CONSTRAINT UQ_PhieuGiaHan_So UNIQUE (SoPhieu),
    CONSTRAINT FK_PGH_CTHD FOREIGN KEY (MaCTHD) REFERENCES dbo.ChiTietHopDong(MaCTHD),
    CONSTRAINT CK_PGH_SoThang CHECK (SoThangGiaHan >= 1),
    CONSTRAINT CK_PGH_Ngay CHECK (NgayKetThucMoi > NgayKetThucCu)
);

/* Tien thue: 1 hoa don tra truoc 6 thang + hoa don hang thang tu thang thu 7 */
CREATE TABLE dbo.HoaDonTienThue (
    MaHDTT        INT IDENTITY(1,1) PRIMARY KEY,
    SoHoaDon      VARCHAR(20) NOT NULL,
    MaHD          INT NOT NULL,
    LoaiHoaDon    NVARCHAR(20) NOT NULL,        -- TraTruoc6Thang / HangThang
    KyTu          DATE NOT NULL,
    KyDen         DATE NOT NULL,
    SoTien        DECIMAL(18,0) NOT NULL,
    HanThanhToan  DATE NOT NULL,
    DaThanhToan   BIT NOT NULL DEFAULT 0,
    NgayThanhToan DATE NULL,
    CONSTRAINT UQ_HDTT_So UNIQUE (SoHoaDon),
    CONSTRAINT UQ_HDTT_HD_Ky UNIQUE (MaHD, KyTu),
    CONSTRAINT FK_HDTT_HopDong FOREIGN KEY (MaHD) REFERENCES dbo.HopDongThue(MaHD),
    CONSTRAINT CK_HDTT_Loai CHECK (LoaiHoaDon IN (N'TraTruoc6Thang', N'HangThang')),
    CONSTRAINT CK_HDTT_SoTien CHECK (SoTien > 0),
    CONSTRAINT CK_HDTT_Ky CHECK (KyDen > KyTu),
    CONSTRAINT CK_HDTT_ThanhToan CHECK ((DaThanhToan = 0 AND NgayThanhToan IS NULL)
        OR (DaThanhToan = 1 AND NgayThanhToan IS NOT NULL))
);

CREATE TABLE dbo.DichVu (
    MaDV    INT IDENTITY(1,1) PRIMARY KEY,
    TenDV   NVARCHAR(50) NOT NULL,
    DonVi   NVARCHAR(20) NOT NULL,
    DonGia  DECIMAL(18,0) NOT NULL,
    CONSTRAINT UQ_DichVu_Ten UNIQUE (TenDV),
    CONSTRAINT CK_DichVu_Gia CHECK (DonGia > 0)
);

/* Hoa don dich vu thanh toan cuoi thang */
CREATE TABLE dbo.HoaDonDichVu (
    MaHDDV        INT IDENTITY(1,1) PRIMARY KEY,
    SoHoaDon      VARCHAR(20) NOT NULL,
    MaHD          INT NOT NULL,
    Thang         INT NOT NULL,
    Nam           INT NOT NULL,
    TongTien      DECIMAL(18,0) NOT NULL DEFAULT 0,
    HanThanhToan  DATE NOT NULL,
    DaThanhToan   BIT NOT NULL DEFAULT 0,
    NgayThanhToan DATE NULL,
    CONSTRAINT UQ_HDDV_So UNIQUE (SoHoaDon),
    CONSTRAINT UQ_HDDV_HD_Thang UNIQUE (MaHD, Thang, Nam),
    CONSTRAINT FK_HDDV_HopDong FOREIGN KEY (MaHD) REFERENCES dbo.HopDongThue(MaHD),
    CONSTRAINT CK_HDDV_Thang CHECK (Thang BETWEEN 1 AND 12),
    CONSTRAINT CK_HDDV_ThanhToan CHECK ((DaThanhToan = 0 AND NgayThanhToan IS NULL)
        OR (DaThanhToan = 1 AND NgayThanhToan IS NOT NULL))
);

CREATE TABLE dbo.ChiTietHoaDonDichVu (
    MaCTDV    INT IDENTITY(1,1) PRIMARY KEY,
    MaHDDV    INT NOT NULL,
    MaPhong   INT NOT NULL,
    MaDV      INT NOT NULL,
    SoLuong   DECIMAL(12,2) NOT NULL,
    DonGia    DECIMAL(18,0) NOT NULL,
    ThanhTien DECIMAL(18,0) NOT NULL,
    CONSTRAINT UQ_CTDV UNIQUE (MaHDDV, MaPhong, MaDV),
    CONSTRAINT FK_CTDV_HoaDon FOREIGN KEY (MaHDDV) REFERENCES dbo.HoaDonDichVu(MaHDDV),
    CONSTRAINT FK_CTDV_Phong FOREIGN KEY (MaPhong) REFERENCES dbo.Phong(MaPhong),
    CONSTRAINT FK_CTDV_DichVu FOREIGN KEY (MaDV) REFERENCES dbo.DichVu(MaDV),
    CONSTRAINT CK_CTDV_SoLuong CHECK (SoLuong > 0)
);

/* Nhan vien cua cac cong ty lam viec trong toa nha */
CREATE TABLE dbo.NhanVienVanPhong (
    MaNV       INT IDENTITY(1,1) PRIMARY KEY,
    MaNVCode   VARCHAR(10) NOT NULL,
    HoTen      NVARCHAR(100) NOT NULL,
    CCCD       VARCHAR(12) NOT NULL,
    HinhAnh    NVARCHAR(260) NULL,
    MaKH       INT NOT NULL,                    -- cong ty
    MaPhong    INT NOT NULL,                    -- phong, tang lam viec
    ChucVu     NVARCHAR(50) NOT NULL,
    DienThoai  VARCHAR(15) NULL,
    NgayBaoCao DATE NOT NULL,
    TrangThai  NVARCHAR(20) NOT NULL DEFAULT N'DangLam',
    CONSTRAINT UQ_NVVP_Code UNIQUE (MaNVCode),
    CONSTRAINT UQ_NVVP_CCCD UNIQUE (CCCD),
    CONSTRAINT FK_NVVP_KhachHang FOREIGN KEY (MaKH) REFERENCES dbo.KhachHang(MaKH),
    CONSTRAINT FK_NVVP_Phong FOREIGN KEY (MaPhong) REFERENCES dbo.Phong(MaPhong),
    CONSTRAINT CK_NVVP_TrangThai CHECK (TrangThai IN (N'DangLam', N'NghiViec'))
);

CREATE TABLE dbo.TheGiuXe (
    MaThe     INT IDENTITY(1,1) PRIMARY KEY,
    MaTheCode VARCHAR(10) NOT NULL,
    MaNV      INT NOT NULL,
    LoaiXe    NVARCHAR(20) NOT NULL,
    BienSo    VARCHAR(15) NOT NULL,
    MauXe     NVARCHAR(30) NULL,
    NgayCap   DATE NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'HieuLuc',
    CONSTRAINT UQ_The_Code UNIQUE (MaTheCode),
    CONSTRAINT UQ_The_BienSo UNIQUE (BienSo),
    CONSTRAINT FK_The_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVienVanPhong(MaNV),
    CONSTRAINT CK_The_LoaiXe CHECK (LoaiXe IN (N'XeMay', N'OTo')),
    CONSTRAINT CK_The_TrangThai CHECK (TrangThai IN (N'HieuLuc', N'Huy'))
);
/* moi nhan vien chi co 1 the dang hieu luc */
CREATE UNIQUE INDEX UX_The_NV_HieuLuc ON dbo.TheGiuXe(MaNV) WHERE TrangThai = N'HieuLuc';

/* Kiem tra tinh trang phong dinh ky 1 nam / lan */
CREATE TABLE dbo.KiemTraPhong (
    MaKT            INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong         INT NOT NULL,
    NgayKiemTra     DATE NOT NULL,
    Nam             AS YEAR(NgayKiemTra) PERSISTED,
    TinhTrang       NVARCHAR(20) NOT NULL,      -- Tot / CanSua / HuHong
    CoThayDoiKetCau BIT NOT NULL DEFAULT 0,
    MoTa            NVARCHAR(300) NULL,
    KeHoachTuSua    NVARCHAR(300) NULL,
    NguoiKiemTra    NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_KT_Phong_Nam UNIQUE (MaPhong, Nam),
    CONSTRAINT FK_KT_Phong FOREIGN KEY (MaPhong) REFERENCES dbo.Phong(MaPhong),
    CONSTRAINT CK_KT_TinhTrang CHECK (TinhTrang IN (N'Tot', N'CanSua', N'HuHong'))
);

/* Phat khi tu y thay doi ket cau van phong */
CREATE TABLE dbo.PhieuPhat (
    MaPhat        INT IDENTITY(1,1) PRIMARY KEY,
    SoPhieu       VARCHAR(20) NOT NULL,
    MaKT          INT NOT NULL,
    MaHD          INT NOT NULL,
    LyDo          NVARCHAR(300) NOT NULL,
    SoTien        DECIMAL(18,0) NOT NULL,
    NgayLap       DATE NOT NULL,
    DaThanhToan   BIT NOT NULL DEFAULT 0,
    NgayThanhToan DATE NULL,
    CONSTRAINT UQ_Phat_So UNIQUE (SoPhieu),
    CONSTRAINT UQ_Phat_KT UNIQUE (MaKT),
    CONSTRAINT FK_Phat_KiemTra FOREIGN KEY (MaKT) REFERENCES dbo.KiemTraPhong(MaKT),
    CONSTRAINT FK_Phat_HopDong FOREIGN KEY (MaHD) REFERENCES dbo.HopDongThue(MaHD),
    CONSTRAINT CK_Phat_SoTien CHECK (SoTien > 0)
);
GO

/* Trigger: khong cho 1 phong nam trong 2 hop dong dang hieu luc cung luc */
CREATE TRIGGER dbo.trg_CTHD_KhongTrungPhong ON dbo.ChiTietHopDong
AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.ChiTietHopDong c ON c.MaPhong = i.MaPhong AND c.MaCTHD <> i.MaCTHD
        JOIN dbo.HopDongThue h ON h.MaHD = c.MaHD AND h.TrangThai = N'HieuLuc'
        JOIN dbo.HopDongThue hi ON hi.MaHD = i.MaHD
        -- 2 khoang thoi gian thue giao nhau
        WHERE hi.NgayBatDau <= c.NgayKetThuc AND h.NgayBatDau <= i.NgayKetThuc
    )
    BEGIN
        RAISERROR (N'Phong dang duoc thue trong mot hop dong khac.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO
