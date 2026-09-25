# LAB 3: Hệ thống Quản lý Khách sạn

Ứng dụng desktop WinForms (.NET Framework 4.7.2) + SQL Server, triển khai quy trình nghiệp vụ khách sạn: danh mục → phòng/tiện nghi → đặt/nhận phòng → dịch vụ → trả phòng/thanh toán → thống kê.

## 1. Công nghệ

- Ngôn ngữ: C# WinForms, `TargetFramework: net472`
- CSDL: SQL Server, script `QuanLyKhachSan_Solution/Database/QuanLyKhachSan.sql`
- Truy cập dữ liệu: ADO.NET (`SqlConnection` / `SqlDataAdapter`), parameterized query
- Kiến trúc 3 lớp đơn giản: `Forms` (UI) → `Services` (nghiệp vụ + validate) → `Data/Db.cs` (kết nối)

## 2. Cấu trúc solution

```
QuanLyKhachSan_Solution/
├── QuanLyKhachSan.sln
├── Database/QuanLyKhachSan.sql   # CREATE DATABASE + 17 bảng + dữ liệu mẫu
└── QuanLyKhachSan/
    ├── Program.cs                # Entry point, chạy FrmMain
    ├── App.config                # Connection string QuanLyKhachSanDB
    ├── Data/Db.cs                # OpenConnection, Query, Execute, Scalar
    ├── Services/                 # Nghiệp vụ, trả về KetQuaXuLy
    │   ├── KetQuaXuLy.cs         # { ThanhCong, ThongBao } + DTO PhongDatItem, DenBuItem
    │   ├── DanhMucService.cs     # CRUD KhuVuc, NhanVien, LoaiTienNghi, DichVu, QuyDinhDenBu
    │   ├── PhongTienNghiService.cs # Thêm Phòng, Tiện nghi, Phiếu lắp đặt
    │   ├── DatPhongService.cs    # Khách hàng, Tạo đặt phòng, Nhận phòng, No-show
    │   ├── DichVuService.cs      # Ghi nhận sử dụng dịch vụ theo phòng/ngày
    │   ├── TraPhongService.cs    # Phiếu đền bù, Hóa đơn, Thanh toán, Trả phòng
    │   └── ThongKeService.cs     # Tổng hợp doanh thu + dịch vụ theo khoảng ngày
    └── Forms/                    # UI code-behind (không viết SQL trực tiếp)
        ├── FrmMain.cs            # Menu 6 chức năng
        ├── FrmDanhMuc.cs
        ├── FrmPhongTienNghi.cs
        ├── FrmDatPhong.cs
        ├── FrmDichVu.cs
        ├── FrmTraPhong.cs
        ├── FrmThongKe.cs
        └── UiHelper.cs
```

## 3. Mô hình CSDL (17 bảng)

- **Danh mục nền:** `NhanVien`, `KhuVuc`, `Phong` (Trống/Đã đặt/Đang ở/Bảo trì), `LoaiTienNghi`, `TienNghi`, `PhieuLapDat`
- **Đặt/nhận phòng:** `KhachHang`, `PhieuDatPhong` (Đã đặt/Đang ở/Đã trả/No-show/Hủy, kênh Điện thoại/Website/Trực tiếp), `ChiTietDatPhong`, `NguoiLuuTru`
- **Dịch vụ:** `DichVu`, `PhieuSuDungDV` (unique theo phiếu+phòng+ngày), `ChiTietPhieuSuDungDV`
- **Đền bù:** `QuyDinhDenBu`, `PhieuDenBu`, `ChiTietPhieuDenBu`
- **Hóa đơn:** `HoaDon` (`TongTien = TienPhong + TienDichVu` computed), `ThanhToan` (Tiền mặt/Chuyển khoản/Thẻ/Ví điện tử)

Ràng buộc đáng chú ý: `CHECK` giá/số người/số lượng > 0, `NgayTraDuKien >= NgayNhan`, `UQ_TienNghi_Loai_STT`, `UQ_PhieuLapDat_ThietBi_Ngay`, index trên `PhieuDatPhong(NgayNhan, NgayTraDuKien, TrangThai)`.

## 4. Chức năng chính

| Form | Nghiệp vụ |
|---|---|
| Danh mục | Thêm Khu vực, Nhân viên, Loại tiện nghi, Dịch vụ, Quy định đền bù |
| Phòng - Tiện nghi | Thêm Phòng (số người tối đa, đơn giá ngày), thêm Tiện nghi, lập Phiếu lắp đặt thiết bị vào phòng |
| Đặt / Nhận phòng | Thêm Khách hàng, tạo Phiếu đặt phòng nhiều phòng (check trùng phòng theo khoảng ngày), thêm Người lưu trú, Nhận phòng (chuyển Đã đặt → Đang ở), đánh dấu No-show |
| Sử dụng dịch vụ | Ghi nhận dịch vụ theo phòng/ngày cho phiếu đang ở, xem lịch sử |
| Trả phòng - Thanh toán | Lập Phiếu đền bù theo Quy định, lập Hóa đơn (tiền phòng × số ngày + tiền dịch vụ), Thanh toán, Trả phòng (chuyển → Đã trả, giải phóng phòng) |
| Thống kê | Tổng hợp doanh thu và sản lượng dịch vụ theo khoảng từ–đến |

Luật nghiệp vụ do `Services/*` kiểm soát, Form chỉ hiển thị `KetQuaXuLy.ThongBao`.

## 5. Cài đặt và chạy

1. Chạy script `Database/QuanLyKhachSan.sql` trên SQL Server (tạo DB `QuanLyKhachSan` + dữ liệu mẫu: 3 NV, 3 phòng, 3 tiện nghi, 3 dịch vụ, 4 quy định đền bù).
2. Sửa connection string trong `QuanLyKhachSan/App.config` cho đúng server của bạn:
   ```xml
   <add name="QuanLyKhachSanDB"
        connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyKhachSan;Integrated Security=True"
        providerName="System.Data.SqlClient" />
   ```
   (Mặc định trong repo đang để `ADMIN-PC\SQLEXPRESS` + user `sa`.)
3. Mở `QuanLyKhachSan.sln` bằng Visual Studio, Build và Run (F5). Form khởi động là `FrmMain`.

## 6. Ghi chú

- Form không truy vấn SQL trực tiếp, mọi thao tác DB qua `Data/Db.cs`.
- Muốn đổi môi trường: chỉ cần sửa `App.config`, không cần build lại logic.
