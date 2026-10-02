# LAB OOSD - Phân tích thiết kế hướng đối tượng

Repo bài tập thực hành môn Phân tích thiết kế hệ thống hướng đối tượng.
Mỗi lab gồm code WinForms / Java + script SQL + tài liệu phân tích (Word + diagram).

Sinh viên: Lê Minh Giang - 1250080043 - 12CNPM1

## Nội dung các lab

| Lab | Đề bài | Code | Phân tích |
|-----|--------|------|-----------|
| LAB1 | OOP cơ bản + Hệ thống thư viện trực tuyến (Intranet) | Java: `CHinhVe` (abstract), `CDiem`, `CTamGiac`, `CTuGiac`, `CEllipse`, `Main` | Khảo sát yêu cầu, bảng thuật ngữ, Use Case tổng quát (13 UC), đặc tả UC01 Tìm kiếm tài liệu, Activity Diagram |
| LAB2 | Quản lý thư viện | C# WinForms + SQL Server (`QuanLyThuVien/QuanLyThuVien/`). 3 lớp: `Forms` -> `Services` -> `Data/Db.cs`. Chức năng: danh mục (NV, thể loại, NXB), đầu sách, độc giả, mượn / trả / mất / hư hỏng, thống kê lượt mượn + phí phạt | File `lab2.drawio.png` + Word báo cáo |
| LAB3 | Quản lý khách sạn | C# WinForms .NET Framework 4.7.2 + SQL Server (`QuanLyKhachSan_Solution/`). DB 17 bảng: Phòng, Phiếu đặt phòng, Dịch vụ, Phiếu đền bù, Hóa đơn, Thanh toán... Chức năng: đặt / nhận / no-show, ghi nhận dịch vụ theo phòng/ngày, đền bù, lập hóa đơn + thanh toán, thống kê doanh thu | Word `BaoCao_Lab3_QuanLyKhachSan.docx` |
| LAB4 | E-Shopping - Đặt hàng & thanh toán trực tuyến | C# WinForms (`EShopping/EShopping/`). Điểm chính: tính lại phí ship + phí thẻ ở `DatHangService`, validate thẻ bằng Luhn, check độ dài số thẻ / CSV theo loại thẻ (Visa/Master/Discover/Amex), tách cổng thanh toán qua interface `IPaymentGateway` (hiện dùng `MockPaymentGateway`) | 4 diagram trong `e-Shopping_diagram/`: UseCase tổng quát, UseCase Đặt hàng-Thanh toán, Activity Đặt hàng, Sequence Đặt hàng + Word báo cáo |

## Công nghệ dùng chung

- Ngôn ngữ: Java (Lab1), C# WinForms (Lab2-4)
- CSDL: SQL Server, truy cập qua ADO.NET (`SqlConnection` / `SqlDataAdapter`), query có tham số
- Mô hình code: UI chỉ hiển thị kết quả, nghiệp vụ và validate nằm ở lớp `Services`, kết nối DB gom ở `Data/Db.cs`
- Tài liệu: Word + draw.io

## Cấu trúc thư mục

```
LAB_OOSD/
├── LAB1/  # Java OOP + docx phân tích thư viện
├── LAB2/QuanLyThuVien/  # .slnx + Database/QuanLyThuVien.sql
├── LAB3/QuanLyKhachSan_Solution/  # .sln + Database/QuanLyKhachSan.sql
└── LAB4/  # EShopping/ (.slnx + SQL/01_CreateTables.sql) + e-Shopping_diagram/ + word/
```

## Cách chạy (Lab2-4)

1. Chạy file `.sql` trong thư mục `Database/` / `SQL/` để tạo DB + dữ liệu mẫu.
2. Sửa connection string trong `App.config` cho đúng SQL Server của máy.
3. Mở file `.sln` / `.slnx` bằng Visual Studio, Build rồi nhấn F5. Form chính là `FrmMain`.
