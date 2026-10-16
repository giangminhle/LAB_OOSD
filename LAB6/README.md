# LAB6: Quan ly Ho so Benh an (Bai 5)

App WinForms cho benh vien: dang ky lan dau + le phi nam + cap ho so, kham - xet nghiem - toa thuoc, lich dinh ky theo quy, thanh toan vien phi roi moi hoan tat dot dieu tri.

## 1. Cong nghe

- C# WinForms `net9.0-windows` (`QuanLyBenhAn/QuanLyBenhAn/`, build bang `dotnet build`)
- SQL Server, DB `QuanLyBenhAn`, script `QuanLyBenhAn/QuanLyBenhAn/SQL/01_CreateTables.sql`
- ADO.NET co tham so. Connection string trong `Data/Db.cs` (va `App.config`)
- 3 lop: `UI` hien thi, `Services` nghiep vu + validate, `Models` + `Data` giu lieu

## 2. Nghiep vu chinh

**Dang ky (`BenhNhanService`, `FrmDangKy`, proc `sp_DangKyBenhNhan`):**

- Validate: ho ten, ngay sinh khong tuong lai, SDT 9-11 so, le phi nam > 0
- 1 TRANSACTION: `BenhNhan` + `PhieuDangKy` + `LePhiNam` + `HoSoBenhAn` (ma HS `HS000xxx`)
- Chan trung SDT (`UQ_BenhNhan_DienThoai`), moi BN 1 HS (`UQ_HoSo_MaBN`), moi BN 1 le phi/nam (`UQ_LePhi_BN_Nam`)

**Kham - XN - Toa - Thanh toan - Hoan tat (`KhamBenhService`, `FrmKhamBenh`):**

- Tra cuu HS theo MaHSCode/HoTen/SDT; xem lich su kham kem so XN/toa
- Tao phieu kham: than nhiet 34-42 do, huyet ap dang 120/80, loai DieuTri/DinhKyQuy
- XN va toa bat buoc lien ket `MaPhieuKham`; moi phieu toi da 1 toa (`UQ_Toa_PhieuKham`)
- Lap toa trong TRANSACTION: `ToaThuoc` + `ChiTietToa` + `UPDATE PhieuKham=DaKham`
- Lich dinh ky theo quy: 2 dot cach nhau it nhat 80 ngay, mien phi
- Hoan tat (`sp_HoanTatPhieuKham`) chi khi `HoaDonVienPhi.DaThanhToan = 1`

## 3. CSDL (12 bang)

`BenhNhan`, `PhieuDangKy`, `LePhiNam`, `HoSoBenhAn`, `LichHenKham`, `Khoa`, `BacSi`,
`PhieuKham`, `XetNghiem`, `ToaThuoc`, `ChiTietToa`, `Thuoc`, `HoaDonVienPhi`
(`HoaDon` tinh rieng: 12 bang + bang chi tiet = 13 table thuc te trong script).
PK identity + FK + UNIQUE + CHECK (xem muc 7 trong Word). 2 proc transaction:
`sp_DangKyBenhNhan`, `sp_HoanTatPhieuKham`.

## 4. Tai lieu phan tich

- `diagrams/1_ClassDiagram.drawio` - Class + ID + association/multiplicity (Cau 1-2)
- `diagrams/2_UseCase_TongQuat.drawio` - Actor + 12 UC (Cau 3)
- `diagrams/3_UseCase_PhanRa_DangKy_Toa.drawio` - phan ra dang ky -> XN -> toa (Cau 4)
- `diagrams/4_Activity_DangKy_Toa.drawio` - Activity (Cau 5)
- `diagrams/5_Sequence_LapToaThuoc.drawio` - Sequence Lap toa thuoc (Cau 6)
- `word/BaoCao_Lab6_QuanLyBenhAn.docx` - bao cao day du + dac ta UC08 + ket qua kiem thu

## 5. Kiem thu (da chay that)

- Offline `Services/RuleTests.cs`: **16 pass / 0 fail** (nut 3 tren `FrmMain`)
- DB that (sqlcmd + SQL Express): dang ky tao `HS000001` OK; chuoi XN->Toa->HD->HoanTat OK
  (`TrangThai=3`); trung SDT bi chan; trung toa vi pham `UQ_Toa_PhieuKham` (2627);
  hoan tat khi chua thanh toan bi chan. DB da reset ve seed sach.

## 6. Chay thu

1. Chay `QuanLyBenhAn/QuanLyBenhAn/SQL/01_CreateTables.sql` tren SQL Server
2. Sua connection string trong `Data/Db.cs`
3. `dotnet build QuanLyBenhAn/QuanLyBenhAn.sln` (hoac mo `.sln` bang Visual Studio), F5.
   `FrmMain` co 2 module + nut kiem thu quy tac.
