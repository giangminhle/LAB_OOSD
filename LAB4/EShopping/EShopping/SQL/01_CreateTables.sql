-- =============================================
-- e-SHOPPING - Script tạo CSDL
-- =============================================
IF DB_ID('eShoppingDb') IS NULL CREATE DATABASE eShoppingDb;
GO
USE eShoppingDb;
GO

IF OBJECT_ID('dbo.DonHang') IS NOT NULL DROP TABLE dbo.DonHang;
IF OBJECT_ID('dbo.KhachHang') IS NOT NULL DROP TABLE dbo.KhachHang;
GO

CREATE TABLE dbo.KhachHang (
    MaKH         INT IDENTITY(1,1) PRIMARY KEY,
    HoTen        NVARCHAR(100) NOT NULL,
    NgaySinh     DATE          NOT NULL,
    SoCMND       VARCHAR(20)   NOT NULL,           -- CMND/CCCD/Passport
    DiaChi       NVARCHAR(200) NOT NULL,
    DienThoai    VARCHAR(15)   NOT NULL,
    TenDangNhap  VARCHAR(30)   NOT NULL,
    MatKhauHash  VARCHAR(128)  NOT NULL,           -- KHÔNG lưu mật khẩu thô
    Email        NVARCHAR(100) NULL,
    NgayTao      DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_KhachHang_TenDangNhap UNIQUE (TenDangNhap),
    CONSTRAINT UQ_KhachHang_SoCMND      UNIQUE (SoCMND)
);
GO

CREATE TABLE dbo.DonHang (
    MaDH            INT IDENTITY(1,1) PRIMARY KEY,
    MaKH            INT           NOT NULL,        -- người mua
    LoaiGiaoHang    TINYINT       NOT NULL,        -- 1=Thường, 2=Nhanh, 3=Nhanh trong ngày
    -- Người nhận (có thể khác người mua)
    TenNguoiNhan    NVARCHAR(100) NOT NULL,
    DiaChiNhan      NVARCHAR(200) NOT NULL,
    DienThoaiNhan   VARCHAR(15)   NOT NULL,
    -- Tiền
    TongTienHang    DECIMAL(18,0) NOT NULL,
    PhiGiaoHang     DECIMAL(18,0) NOT NULL DEFAULT 0,
    PhiThe          DECIMAL(18,0) NOT NULL DEFAULT 0,
    TongThanhToan   DECIMAL(18,0) NOT NULL,
    -- Thẻ tín dụng: chỉ lưu thông tin đã che, KHÔNG lưu số thẻ đầy đủ / CSV
    LoaiThe         TINYINT       NOT NULL,        -- 1=Visa, 2=Master, 3=Discover, 4=Amex
    SoTheMask       VARCHAR(25)   NOT NULL,        -- **** **** **** 1234
    TenChuThe       NVARCHAR(100) NOT NULL,
    NgayHetHanThe   DATE          NOT NULL,
    ThoiDiemDat     DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DonHang_KhachHang FOREIGN KEY (MaKH) REFERENCES dbo.KhachHang(MaKH),
    CONSTRAINT CK_DonHang_LoaiGH  CHECK (LoaiGiaoHang IN (1,2,3)),
    CONSTRAINT CK_DonHang_LoaiThe CHECK (LoaiThe IN (1,2,3,4)),
    CONSTRAINT CK_DonHang_Tien    CHECK (TongTienHang >= 0 AND PhiGiaoHang >= 0 AND PhiThe >= 0)
);
GO
