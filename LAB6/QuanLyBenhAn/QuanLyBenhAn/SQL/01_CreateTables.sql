-- =============================================================
-- LAB6 - QUAN LY HO SO BENH AN
-- DB: QuanLyBenhAn | SQL Server
-- PK + FK + UNIQUE + CHECK + TRANSACTION (stored procedure)
-- =============================================================
IF DB_ID('QuanLyBenhAn') IS NULL CREATE DATABASE QuanLyBenhAn;
GO
USE QuanLyBenhAn;
GO

-- Xoa theo thu tu nguoc FK de chay lai duoc nhieu lan
IF OBJECT_ID('dbo.HoaDonVienPhi','U') IS NOT NULL DROP TABLE dbo.HoaDonVienPhi;
IF OBJECT_ID('dbo.ChiTietToa','U') IS NOT NULL DROP TABLE dbo.ChiTietToa;
IF OBJECT_ID('dbo.ToaThuoc','U') IS NOT NULL DROP TABLE dbo.ToaThuoc;
IF OBJECT_ID('dbo.XetNghiem','U') IS NOT NULL DROP TABLE dbo.XetNghiem;
IF OBJECT_ID('dbo.PhieuKham','U') IS NOT NULL DROP TABLE dbo.PhieuKham;
IF OBJECT_ID('dbo.LichHenKham','U') IS NOT NULL DROP TABLE dbo.LichHenKham;
IF OBJECT_ID('dbo.HoSoBenhAn','U') IS NOT NULL DROP TABLE dbo.HoSoBenhAn;
IF OBJECT_ID('dbo.LePhiNam','U') IS NOT NULL DROP TABLE dbo.LePhiNam;
IF OBJECT_ID('dbo.PhieuDangKy','U') IS NOT NULL DROP TABLE dbo.PhieuDangKy;
IF OBJECT_ID('dbo.Thuoc','U') IS NOT NULL DROP TABLE dbo.Thuoc;
IF OBJECT_ID('dbo.BacSi','U') IS NOT NULL DROP TABLE dbo.BacSi;
IF OBJECT_ID('dbo.Khoa','U') IS NOT NULL DROP TABLE dbo.Khoa;
IF OBJECT_ID('dbo.BenhNhan','U') IS NOT NULL DROP TABLE dbo.BenhNhan;
GO

-- ---------- 1. BENH NHAN ----------
CREATE TABLE dbo.BenhNhan (
    MaBN      INT IDENTITY(1,1) PRIMARY KEY,
    HoTen     NVARCHAR(100) NOT NULL,
    NgaySinh  DATE          NOT NULL,
    DiaChi    NVARCHAR(200) NOT NULL,
    DienThoai VARCHAR(15)   NOT NULL,
    Anh3x4    NVARCHAR(260) NULL,               -- duong dan file anh
    NgayTao   DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_BenhNhan_DienThoai UNIQUE (DienThoai),
    CONSTRAINT CK_BenhNhan_NgaySinh CHECK (NgaySinh <= GETDATE())
);
GO

-- ---------- 2. KHOA ----------
CREATE TABLE dbo.Khoa (
    MaKhoa  INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Khoa_Ten UNIQUE (TenKhoa)
);
GO

-- ---------- 3. BAC SI ----------
CREATE TABLE dbo.BacSi (
    MaBS      INT IDENTITY(1,1) PRIMARY KEY,
    HoTen     NVARCHAR(100) NOT NULL,
    MaKhoa    INT NOT NULL,
    DienThoai VARCHAR(15) NULL,
    CONSTRAINT FK_BacSi_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa)
);
GO

-- ---------- 4. THUOC (danh muc) ----------
CREATE TABLE dbo.Thuoc (
    MaThuoc  INT IDENTITY(1,1) PRIMARY KEY,
    TenThuoc NVARCHAR(100) NOT NULL,
    DonVi    NVARCHAR(20)  NOT NULL,
    DonGia   DECIMAL(18,0) NOT NULL DEFAULT 0,
    CONSTRAINT UQ_Thuoc_Ten UNIQUE (TenThuoc),
    CONSTRAINT CK_Thuoc_Gia CHECK (DonGia >= 0)
);
GO

-- ---------- 5. PHIEU DANG KY (lan dau) ----------
CREATE TABLE dbo.PhieuDangKy (
    MaPhieuDK INT IDENTITY(1,1) PRIMARY KEY,
    MaBN      INT NOT NULL,
    NgayDK    DATETIME NOT NULL DEFAULT GETDATE(),
    HoTen     NVARCHAR(100) NOT NULL,          -- snapshot tu phieu giay
    DiaChi    NVARCHAR(200) NOT NULL,
    NgaySinh  DATE NOT NULL,
    DienThoai VARCHAR(15) NOT NULL,
    TrangThai TINYINT NOT NULL DEFAULT 0,       -- 0=Moi, 1=DaCapHoSo
    CONSTRAINT FK_PhieuDK_BenhNhan FOREIGN KEY (MaBN) REFERENCES dbo.BenhNhan(MaBN),
    CONSTRAINT CK_PhieuDK_TrangThai CHECK (TrangThai IN (0,1))
);
GO

-- ---------- 6. LE PHI NAM ----------
CREATE TABLE dbo.LePhiNam (
    MaLePhi   INT IDENTITY(1,1) PRIMARY KEY,
    MaBN      INT NOT NULL,
    Nam       INT NOT NULL,                    -- VD 2026
    SoTien    DECIMAL(18,0) NOT NULL,
    NgayDong  DATETIME NOT NULL DEFAULT GETDATE(),
    NguoiThu  NVARCHAR(100) NULL,
    CONSTRAINT FK_LePhi_BenhNhan FOREIGN KEY (MaBN) REFERENCES dbo.BenhNhan(MaBN),
    CONSTRAINT UQ_LePhi_BN_Nam UNIQUE (MaBN, Nam),   -- moi BN 1 lan/nam
    CONSTRAINT CK_LePhi_Nam CHECK (Nam BETWEEN 2000 AND 2100),
    CONSTRAINT CK_LePhi_Tien CHECK (SoTien > 0)
);
GO

-- ---------- 7. HO SO BENH AN ----------
CREATE TABLE dbo.HoSoBenhAn (
    MaHoSo    INT IDENTITY(1,1) PRIMARY KEY,
    MaBN      INT NOT NULL,
    MaHSCode  VARCHAR(20) NOT NULL,            -- Ma ho so in tren bia (VD HS000123)
    NgayCap   DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai TINYINT NOT NULL DEFAULT 1,      -- 1=HoatDong, 0=Khoa
    CONSTRAINT FK_HoSo_BenhNhan FOREIGN KEY (MaBN) REFERENCES dbo.BenhNhan(MaBN),
    CONSTRAINT UQ_HoSo_MaBN UNIQUE (MaBN),     -- moi BN chi 1 ho so
    CONSTRAINT UQ_HoSo_Code UNIQUE (MaHSCode),
    CONSTRAINT CK_HoSo_TrangThai CHECK (TrangThai IN (0,1))
);
GO

-- ---------- 8. LICH HEN KHAM DINH KY (theo quy, mien phi) ----------
CREATE TABLE dbo.LichHenKham (
    MaLichHen  INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo     INT NOT NULL,
    NgayGioHen DATETIME NOT NULL,
    LoaiHen    TINYINT NOT NULL DEFAULT 2,     -- 1=KhamBenh, 2=DinhKyQuy
    TrangThai  TINYINT NOT NULL DEFAULT 0,     -- 0=DaHen, 1=DaKham, 2=Huy
    GhiChu     NVARCHAR(200) NULL,
    CONSTRAINT FK_LichHen_HoSo FOREIGN KEY (MaHoSo) REFERENCES dbo.HoSoBenhAn(MaHoSo),
    CONSTRAINT CK_LichHen_Loai CHECK (LoaiHen IN (1,2)),
    CONSTRAINT CK_LichHen_TrangThai CHECK (TrangThai IN (0,1,2))
);
GO

-- ---------- 9. PHIEU KHAM (moi dot dieu tri / dot dinh ky) ----------
CREATE TABLE dbo.PhieuKham (
    MaPhieuKham INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo      INT NOT NULL,
    MaBS        INT NOT NULL,
    MaKhoa      INT NOT NULL,
    NgayGioKham DATETIME NOT NULL DEFAULT GETDATE(),
    TrieuChung  NVARCHAR(500) NULL,
    ThanNhiet   DECIMAL(4,1) NULL,             -- do C
    HuyetAp     VARCHAR(15) NULL,              -- VD 120/80
    ChanDoan    NVARCHAR(500) NULL,
    LoaiKham    TINYINT NOT NULL DEFAULT 1,    -- 1=DieuTri, 2=DinhKyQuy (khong benh van ghi nhan)
    TrangThai   TINYINT NOT NULL DEFAULT 0,    -- 0=ChoKham, 1=DaKham, 2=DaThanhToan, 3=HoanTat
    CONSTRAINT FK_PhieuKham_HoSo FOREIGN KEY (MaHoSo) REFERENCES dbo.HoSoBenhAn(MaHoSo),
    CONSTRAINT FK_PhieuKham_BacSi FOREIGN KEY (MaBS) REFERENCES dbo.BacSi(MaBS),
    CONSTRAINT FK_PhieuKham_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa),
    CONSTRAINT CK_PhieuKham_Loai CHECK (LoaiKham IN (1,2)),
    CONSTRAINT CK_PhieuKham_TrangThai CHECK (TrangThai IN (0,1,2,3)),
    CONSTRAINT CK_PhieuKham_NhietDo CHECK (ThanNhiet IS NULL OR (ThanNhiet BETWEEN 34.0 AND 42.0))
);
GO

-- ---------- 10. XET NGHIEM (lien ket phieu kham) ----------
CREATE TABLE dbo.XetNghiem (
    MaXN          INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKham   INT NOT NULL,
    LoaiXN        NVARCHAR(100) NOT NULL,      -- VD Mau, X-Quang, Sieu am...
    NgayChiDinh   DATETIME NOT NULL DEFAULT GETDATE(),
    KetQua        NVARCHAR(500) NULL,
    NguoiThucHien NVARCHAR(100) NULL,
    CONSTRAINT FK_XN_PhieuKham FOREIGN KEY (MaPhieuKham) REFERENCES dbo.PhieuKham(MaPhieuKham)
);
GO

-- ---------- 11. TOA THUOC (lien ket phieu kham, moi phieu toi da 1 toa) ----------
CREATE TABLE dbo.ToaThuoc (
    MaToa       INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKham INT NOT NULL,
    MaBS        INT NOT NULL,
    NgayKe      DATETIME NOT NULL DEFAULT GETDATE(),
    GhiChu      NVARCHAR(300) NULL,
    CONSTRAINT FK_Toa_PhieuKham FOREIGN KEY (MaPhieuKham) REFERENCES dbo.PhieuKham(MaPhieuKham),
    CONSTRAINT FK_Toa_BacSi FOREIGN KEY (MaBS) REFERENCES dbo.BacSi(MaBS),
    CONSTRAINT UQ_Toa_PhieuKham UNIQUE (MaPhieuKham)
);
GO

CREATE TABLE dbo.ChiTietToa (
    MaToa     INT NOT NULL,
    MaThuoc   INT NOT NULL,
    LieuLuong NVARCHAR(100) NOT NULL,
    SoLuong   INT NOT NULL,
    CachDung  NVARCHAR(200) NULL,
    CONSTRAINT PK_ChiTietToa PRIMARY KEY (MaToa, MaThuoc),
    CONSTRAINT FK_CTToa_Toa FOREIGN KEY (MaToa) REFERENCES dbo.ToaThuoc(MaToa),
    CONSTRAINT FK_CTToa_Thuoc FOREIGN KEY (MaThuoc) REFERENCES dbo.Thuoc(MaThuoc),
    CONSTRAINT CK_CTToa_SoLuong CHECK (SoLuong > 0)
);
GO

-- ---------- 12. HOA DON VIEN PHI ----------
CREATE TABLE dbo.HoaDonVienPhi (
    MaHD        INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKham INT NOT NULL,
    NgayLap     DATETIME NOT NULL DEFAULT GETDATE(),
    TongTien    DECIMAL(18,0) NOT NULL DEFAULT 0,
    DaThanhToan BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_HD_PhieuKham FOREIGN KEY (MaPhieuKham) REFERENCES dbo.PhieuKham(MaPhieuKham),
    CONSTRAINT UQ_HD_PhieuKham UNIQUE (MaPhieuKham),  -- moi phieu 1 hoa don
    CONSTRAINT CK_HD_Tien CHECK (TongTien >= 0)
);
GO

-- =============================================================
-- STORED PROCEDURE 1: Dang ky lan dau + le phi nam + cap ho so
-- (TRANSACTION nhieu bang - dung tu FrmDangKy)
-- =============================================================
IF OBJECT_ID('dbo.sp_DangKyBenhNhan','P') IS NOT NULL DROP PROCEDURE dbo.sp_DangKyBenhNhan;
GO
CREATE PROCEDURE dbo.sp_DangKyBenhNhan
    @HoTen NVARCHAR(100), @NgaySinh DATE, @DiaChi NVARCHAR(200),
    @DienThoai VARCHAR(15), @Anh3x4 NVARCHAR(260) = NULL,
    @Nam INT, @SoTien DECIMAL(18,0), @NguoiThu NVARCHAR(100) = NULL,
    @MaBN INT OUTPUT, @MaHoSo INT OUTPUT, @MaHSCode VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        -- Chan trung SDT (UNIQUE cung chan, nhung bao loi tieng Viet som)
        IF EXISTS (SELECT 1 FROM dbo.BenhNhan WHERE DienThoai = @DienThoai)
        BEGIN
            RAISERROR(N'Số điện thoại đã đăng ký hồ sơ trước đó.', 16, 1);
            ROLLBACK TRANSACTION; RETURN -1;
        END
        INSERT INTO dbo.BenhNhan (HoTen, NgaySinh, DiaChi, DienThoai, Anh3x4)
        VALUES (@HoTen, @NgaySinh, @DiaChi, @DienThoai, @Anh3x4);
        SET @MaBN = SCOPE_IDENTITY();

        INSERT INTO dbo.PhieuDangKy (MaBN, HoTen, DiaChi, NgaySinh, DienThoai, TrangThai)
        VALUES (@MaBN, @HoTen, @DiaChi, @NgaySinh, @DienThoai, 1);

        INSERT INTO dbo.LePhiNam (MaBN, Nam, SoTien, NguoiThu)
        VALUES (@MaBN, @Nam, @SoTien, @NguoiThu);

        -- Ma ho so: HS + zero-pad 6 so theo MaBN
        SET @MaHSCode = 'HS' + RIGHT('000000' + CAST(@MaBN AS VARCHAR(10)), 6);
        INSERT INTO dbo.HoSoBenhAn (MaBN, MaHSCode) VALUES (@MaBN, @MaHSCode);
        SET @MaHoSo = SCOPE_IDENTITY();

        COMMIT TRANSACTION; RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================================
-- STORED PROCEDURE 2: Hoan tat dot dieu tri SAU thanh toan vien phi
-- Quy tac: chi HoanTat khi HoaDon.DaThanhToan = 1
-- =============================================================
IF OBJECT_ID('dbo.sp_HoanTatPhieuKham','P') IS NOT NULL DROP PROCEDURE dbo.sp_HoanTatPhieuKham;
GO
CREATE PROCEDURE dbo.sp_HoanTatPhieuKham @MaPhieuKham INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @DaTT BIT;
        SELECT @DaTT = DaThanhToan FROM dbo.HoaDonVienPhi WHERE MaPhieuKham = @MaPhieuKham;
        IF @DaTT IS NULL
        BEGIN
            RAISERROR(N'Phiếu khám chưa có hóa đơn viện phí.', 16, 1);
            ROLLBACK TRANSACTION; RETURN -1;
        END
        IF @DaTT = 0
        BEGIN
            RAISERROR(N'Chưa thanh toán viện phí nên không thể hoàn tất đợt điều trị.', 16, 1);
            ROLLBACK TRANSACTION; RETURN -2;
        END
        UPDATE dbo.PhieuKham SET TrangThai = 3 WHERE MaPhieuKham = @MaPhieuKham; -- HoanTat
        COMMIT TRANSACTION; RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ---------- Seed ----------
INSERT INTO dbo.Khoa (TenKhoa) VALUES (N'Nội tổng quát'), (N'Ngoại'), (N'Nhi'), (N'Xét nghiệm'), (N'Dược');
INSERT INTO dbo.BacSi (HoTen, MaKhoa, DienThoai) VALUES
    (N'BS. Trần An', 1, '0901000001'), (N'BS. Lê Bình', 4, '0901000002'), (N'BS. Phạm Chi', 2, '0901000003');
INSERT INTO dbo.Thuoc (TenThuoc, DonVi, DonGia) VALUES
    (N'Paracetamol 500mg', N'viên', 1500), (N'Amoxicillin 500mg', N'viên', 2500),
    (N'Vitamin C 500mg', N'viên', 1200), (N'Oresol', N'gói', 3000);
GO
