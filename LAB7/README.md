# LAB7 - Quản lý nhân sự - Công ty Sơn Đông Á

Hệ thống quản lý nhân sự và tiền lương viết bằng C# WinForms (.NET Framework 4.7.2), kiến trúc 3 lớp **UI → Service → Data**, CSDL SQL Server `QuanLyNhanSu`.

## 1. Chức năng chính

- Tiếp nhận ứng viên, quản lý thời gian thử việc (1-60 ngày).
- Lập hợp đồng lao động ngắn hạn/dài hạn; hợp đồng dài hạn lập hồ sơ BHXH 8% và BHYT 1.5%.
- Quyết định chức vụ, phụ cấp; quyết định lương, phụ cấp lương, phụ cấp độc hại.
- Lập kế hoạch tuần, phân công công việc.
- Chấm công hằng ngày, ghi nhận giờ tăng ca (hệ số 1.5).
- Khen thưởng/kỷ luật cá nhân hoặc tập thể; tự sinh phiếu chi/phiếu thu.
- Tính bảng lương tháng: lương cơ bản, tăng ca, phụ cấp, khen thưởng, kỷ luật, bảo hiểm.

## 2. Công nghệ và cấu trúc

```text
LAB7/
├── QuanLyNhanSu/
│   ├── QuanLyNhanSu.slnx
│   ├── QuanLyNhanSu/
│   │   ├── UI/          # Các form WinForms
│   │   ├── Services/    # Nghiệp vụ, kiểm tra dữ liệu, transaction
│   │   ├── Models/      # DTO
│   │   ├── Data/        # Db.cs - lớp truy cập CSDL
│   │   ├── SQL/         # 01_CreateTables.sql, 02_SeedData.sql
│   │   └── App.config   # Chuỗi kết nối
│   └── QuanLyNhanSu.Tests/  # Console test runner
├── diagrams/            # 6 sơ đồ nguồn .drawio + PNG
└── word/                # Báo cáo và ảnh chụp màn hình
```

- **UI:** `FrmMain`, `FrmTiepNhan`, `FrmHopDong`, `FrmKeHoach`, `FrmChamCong`, `FrmPhuTroi`, `FrmKhenThuong`, `FrmLuong`, `FrmQuyetDinh`, `FrmDanhSach`, `FrmKetNoi`.
- **Service:** `TuyenDungService`, `HopDongService`, `KeHoachService`, `ChamCongService`, `PhuTroiService`, `KhenThuongService`, `LuongService`, `QuyetDinhService`.
- **Data:** `Db.cs` dùng tham số `@parameter`, không nối chuỗi SQL trực tiếp.

## 3. CSDL

- Database: `QuanLyNhanSu` (18 bảng, 3 stored procedure, trigger).
- Tạo CSDL:

```powershell
sqlcmd -S ".\SQLEXPRESS" -U sa -P 25022005 -b -I -i "LAB7\QuanLyNhanSu\QuanLyNhanSu\SQL\01_CreateTables.sql"
sqlcmd -S ".\SQLEXPRESS" -U sa -P 25022005 -b -I -i "LAB7\QuanLyNhanSu\QuanLyNhanSu\SQL\02_SeedData.sql"
```

> `02_SeedData.sql` không idempotent. Khi chạy lại, chạy `01_CreateTables.sql` trước để tạo lại CSDL sạch.

Dữ liệu mẫu: 7 nhân viên, 6 hợp đồng đang hiệu lực, 8 dòng bảo hiểm, 1 nhân viên thử việc, 2 kế hoạch tuần, 156 dòng chấm công, 3 khen thưởng, 2 kỷ luật. Bảng lương tháng 10/2026 có tổng thực lĩnh kỳ vọng **88.035.577 VND**.

Chuỗi kết nối nằm trong `QuanLyNhanSu/App.config` và `Data/Db.cs`. Có thể đổi trực tiếp hoặc dùng form `FrmKetNoi`.

## 4. Build và chạy

```powershell
cd LAB7\QuanLyNhanSu
dotnet build QuanLyNhanSu.slnx -c Debug
& "QuanLyNhanSu\bin\Debug\net472\QuanLyNhanSu.exe"
```

Hoặc mở `QuanLyNhanSu.slnx` bằng Visual Studio và nhấn F5.

## 5. Kiểm thử

```powershell
cd LAB7\QuanLyNhanSu
dotnet build QuanLyNhanSu.slnx -c Debug
& "QuanLyNhanSu.Tests\bin\Debug\net472\QuanLyNhanSu.Tests.exe" all
```

- `offline`: chỉ chạy 49 test quy tắc nghiệp vụ.
- `db`: chỉ chạy 16 test tích hợp CSDL.
- `all`: chạy cả hai (mặc định).

Kết quả mong đợi: **49/49 test offline PASS**, **16/16 test CSDL PASS**.

## 6. Sơ đồ

| Câu hỏi | File nguồn Draw.io | Nội dung |
| :--- | :--- | :--- |
| Câu 1, 2 | `diagrams/1_ClassDiagram.drawio` | 18 lớp, thuộc tính, ID/PK, FK, phương thức, quan hệ và bội số |
| Câu 3 | `diagrams/2_UseCase_TongQuat.drawio` | 6 actor, 12 use case tổng quát |
| Câu 4 | `diagrams/3_UseCase_PhanRa_ThuViec_HopDong.drawio` | Phân rã từ thử việc đến hợp đồng chính thức, `<<extend>>` lập hồ sơ BHXH-BHYT |
| Câu 5 | `diagrams/4_Activity_ThuViec_HopDong.drawio` | Activity diagram quy trình thử việc → hợp đồng |
| Câu 6 | `diagrams/5_Sequence_LapQuyetDinhKhenThuong.drawio` | Sequence diagram lập quyết định khen thưởng cá nhân/tập thể |
| Kiến trúc | `diagrams/6_KienTruc_3Lop.drawio` | Kiến trúc UI → Service → Data → SQL Server |

Có thể mở file `.drawio` bằng [app.diagrams.net](https://app.diagrams.net) hoặc extension Draw.io trong VS Code.

## 7. Báo cáo

- Báo cáo chính: `word/BaoCao_LAB7_QuanLyNhanSu.docx`.
- Bản PDF để xem nhanh: `word/BaoCao_LAB7_QuanLyNhanSu.pdf`.
- Script tạo báo cáo: `word/make_report.py` (cần `python-docx`).
- Script tạo sơ đồ: `diagrams/make_diagrams.py`.
- File `Bai_7_Quan_ly_nhan_su.docx` là đề bài mẫu, chỉ đọc, không chỉnh sửa.
