# LAB5 (Bài 6): Quản lý công ty du lịch Văn Hóa Việt

App WinForms quản lý tour, chuyến khách lẻ, phiếu đoàn, phân công HDV, thu tiền sau tour, khảo sát và lương.

## 1. Công nghệ

- C# WinForms, .NET Framework 4.7.2 (`QuanLyDuLich/QuanLyDuLich/`)
- SQL Server LocalDB, DB `QuanLyCongTyDuLich`, script `QuanLyDuLich/Database/QuanLyCongTyDuLich.sql`
- ADO.NET, query có tham số. Connection string trong `QuanLyDuLich/App.config`
- 3 lớp: `Forms` hiển thị, `Services` validate + nghiệp vụ, `Data/Db.cs` giữ kết nối

## 2. Nghiệp vụ chính

- Tour xuất phát từ TP.HCM. Ngày về chuyến lẻ = ngày đi + số ngày − 1
- Khách lẻ < 12 người, đăng ký theo chuyến mở, thu vé ngay. Đoàn > 12 người, chọn ngày bất kỳ, đặt cọc, trả sau tour, bỏ đi mất cọc. Đúng 12 người cả hai bên đều từ chối
- Đoàn mua bảo hiểm phải nhập đủ danh sách người đi
- 1 chuyến lẻ đúng 1 HDV, đoàn được nhiều HDV, HDV không trùng lịch
- Lương tháng = lương căn bản + thù lao các tour kết thúc trong tháng
- Khảo sát gửi sau tour, 1 đăng ký 1 phiếu, điểm 1–5

## 3. CSDL (16 bảng)

`Tour`, `TourDiemDung`, `TourPhuongTien`, `TourDiemThamQuan`, `ChuyenLe`, `DangKyLe`, `DoanKhach`, `DangKyDoan`, `ThanhVienDoan`, `PhanCongHDV`, `ThanhToanDoan`, `KhaoSat`, `HuongDanVien`, `PhuongTien`, `DiemBanVe`, `DiemThamQuan`.

Ràng buộc đáng chú ý: `CHECK` số người lẻ 1–11 / đoàn > 12, hạng sao 2–5, `UNIQUE` lọc 1 HDV/chuyến lẻ và 1 khảo sát/đăng ký. Trùng lịch HDV và tiền còn lại check ở Service vì phải so nhiều dòng.

## 4. Code

```
QuanLyDuLich/
  Database/QuanLyCongTyDuLich.sql
  QuanLyDuLich/
    Program.cs               # chạy FrmMain
    App.config               # QuanLyCongTyDuLichDB -> (localdb)\MSSQLLocalDB
    Data/Db.cs               # OpenConnection, Query, Execute, Scalar, P
    Services/Models.cs       # KetQuaXuLy, QuyDinh, ThanhVienDoanItem
    Services/                # DanhMuc, Tour, ChuyenLe, DangKyLe, DangKyDoan,
                             # PhanCong, KetThuc, ThongKe
    Forms/                   # FrmMain + 8 form nghiệp vụ, FormHelper
```

Form không viết SQL. Mọi thao tác trả về `KetQuaXuLy`, Form chỉ gọi `FormHelper.Bao()` rồi nạp lại lưới.

| Form | Service |
|---|---|
| FrmDanhMuc | DanhMucService |
| FrmTour | TourService |
| FrmChuyenLe | ChuyenLeService |
| FrmDangKyLe | DangKyLeService |
| FrmDangKyDoan | DangKyDoanService |
| FrmPhanCongHDV | PhanCongService |
| FrmKetThucKhaoSat | KetThucService |
| FrmLuongThongKe | ThongKeService |

## 5. Tài liệu

- `File drawio/`: `quanlydanhmuc_uc1`, `laplichkhachle_uc2`, `themtourmoi_sq1`, `taochuyenkhachle_sq2` (kèm `.png`)
- `form_img/`: 8 ảnh chụp form đang chạy
- `WordBaoCao/Word_Lab5_QuanLyDuLich.docx`: báo cáo lab

## 6. Chạy thử

1. Tạo DB (giữ `-f 65001` để không lỗi tiếng Việt, `-I` để tạo filtered index):
   ```
   sqlcmd -S "(localdb)\MSSQLLocalDB" -f 65001 -I -i QuanLyDuLich\Database\QuanLyCongTyDuLich.sql
   ```
2. Kiểm tra `App.config` trỏ đúng instance trên
3. Mở `QuanLyDuLich/QuanLyDuLich.slnx` bằng Visual Studio, Build, F5 từ `FrmMain`
