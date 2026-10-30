# LAB8: QUẢN LÝ CHO THUÊ VĂN PHÒNG Ở CAO ỐC (Bài 4)

Hệ thống quản lý cho thuê văn phòng: tầng/phòng và giá thuê theo độ cao, diện tích; hợp đồng thuê nhiều văn phòng (ít nhất 6 tháng, trả trước 6 tháng đầu); tiền thuê hằng tháng từ tháng thứ 7 (trả đầu tháng); hóa đơn dịch vụ điện, nước, internet (thanh toán cuối tháng); phiếu gia hạn (≤ ½ thời gian thuê, chỉ khi không còn nợ); nhân viên văn phòng và thẻ giữ xe; kiểm tra phòng hằng năm và phạt khi tự ý thay đổi kết cấu.

---

## 1. Công nghệ

- **C# WinForms (.NET Framework 4.7.2):** Solution `QuanLyCaoOc/QuanLyCaoOc.slnx`, build bằng `dotnet build` hoặc mở bằng Visual Studio.
- **Cơ sở dữ liệu:** SQL Server, database `QuanLyCaoOc`.
  - Script tạo bảng: `QuanLyCaoOc/QuanLyCaoOc/SQL/01_CreateTables.sql` (14 bảng, PK identity, FK, CHECK, UNIQUE, 1 trigger chặn 1 phòng nằm trong 2 hợp đồng).
  - Dữ liệu mẫu: `QuanLyCaoOc/QuanLyCaoOc/SQL/02_SeedData.sql`.
- **Kiến trúc 3 lớp:** `UI` (Form) → `Services` (nghiệp vụ, validate, transaction) → `Data/Db.cs` (ADO.NET, tham số `@parameter`).

## 2. Sơ đồ draw.io trong `LAB8/diagrams/`

| Câu | File | Nội dung |
| :--- | :--- | :--- |
| 1, 2 | `1_ClassDiagram.drawio` | 14 class, ID `{PK}`/`{FK}`, phương thức, bản số liên kết |
| 3 | `2_UseCase_TongQuat.drawio` | 4 actor, 12 use case tổng quát |
| 4 | `3_UseCase_PhanRa_HopDong_ThanhToan.drawio` | Phân rã từ ký hợp đồng đến thanh toán tiền thuê (tháng 7+) và dịch vụ |
| 5 | `4_Activity_HopDong_ThanhToan.drawio` | Activity cho Câu 4 |
| 6 | `5_Sequence_LapPhieuGiaHan.drawio` | Sequence "Lập phiếu gia hạn" (kiểm tra công nợ trước khi gia hạn) |
| – | `6_KienTruc_3Lop.drawio` | Kiến trúc 3 lớp |

## 3. Chức năng (FrmMain)

1. Tầng, phòng và giá thuê (lọc phòng trống, tính giá tham khảo)
2. Lập hợp đồng thuê nhiều văn phòng + hóa đơn trả trước 6 tháng
3. Tiền thuê hằng tháng (từ tháng thứ 7) + hóa đơn dịch vụ cuối tháng + thanh toán
4. Lập phiếu gia hạn (kiểm tra số tháng ≤ ½ thời gian thuê, nợ tiền thuê, nợ dịch vụ)
5. Báo cáo nhân viên mới của công ty + cấp thẻ giữ xe
6. Kiểm tra phòng hằng năm + lập phiếu phạt khi thay đổi kết cấu
7. Kết nối CSDL (`FrmKetNoi`), 8–9. Kiểm thử quy tắc / kiểm thử CSDL

## 4. Kết quả kiểm thử (đã chạy thật)

- Kiểm thử quy tắc offline (`Services/RuleTests.cs`): **42 / 42 PASS**
- Kiểm thử tích hợp CSDL (`Services/DbTests.cs`): **16 / 16 PASS** (test ghi dữ liệu tự dọn lại sau khi chạy)

Dữ liệu mẫu dùng để kiểm thử gia hạn (ngày 30/10/2026):
- HD2026-0001 (Minh Phát, P0101 + P0102): không nợ → gia hạn được tối đa 6 tháng.
- HD2026-0002 (Sao Việt, P0201): nợ tiền thuê tháng 10/2026 (26.250.000 đ) → bị chặn.
- HD2026-0003 (An Khang, P0301 + P0302): chưa trả dịch vụ tháng 9/2026 (6.150.000 đ) → bị chặn.

## 5. Chạy chương trình

1. Chạy `01_CreateTables.sql` rồi `02_SeedData.sql` trên SQL Server.
2. Sửa chuỗi kết nối trong `App.config` (mặc định `Data Source=.;User ID=sa;Password=25022005`) hoặc dùng nút "Kết nối CSDL".
3. Build và chạy:
   ```powershell
   cd LAB8/QuanLyCaoOc
   dotnet build QuanLyCaoOc.slnx
   & "QuanLyCaoOc/bin/Debug/net472/QuanLyCaoOc.exe"
   ```
4. Chạy test bằng console: `QuanLyCaoOc.Tests/bin/Debug/net472/QuanLyCaoOc.Tests.exe [offline|db|all]`.
