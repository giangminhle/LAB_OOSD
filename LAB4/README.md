# LAB4: E-Shopping - Đăng ký, Đặt hàng và Thanh toán

Bài thực hành xây app WinForms cho web bán hàng: khách đăng ký tài khoản, sau đó đặt hàng và thanh toán bằng thẻ tín dụng.

## 1. Công nghệ

- C# WinForms, .NET Framework 4.7.2 (`EShopping/EShopping/`)
- SQL Server, DB `eShoppingDb`, script `EShopping/SQL/01_CreateTables.sql`
- Truy cập DB bằng ADO.NET, câu lệnh có tham số. Connection string nằm trong `Data/Db.cs`
- Chia 3 phần rõ ràng: `UI` hiển thị, `Services` xử lý nghiệp vụ, `Models` + `Data` giữ liệu

## 2. Chức năng

**Đăng ký (`KhachHangService`, form `FrmDangKy`):**
- Kiểm tra tuổi >= 16, số CMND 8-12 ký tự, SĐT theo đầu 0 / +84, tên đăng nhập 4-30 ký tự, mật khẩu >= 6 ký tự
- Chặn trùng tên đăng nhập và số CMND
- Mật khẩu băm PBKDF2 + salt rồi mới lưu vào cột `MatKhauHash`, không lưu text gốc

**Đặt hàng + thanh toán (`DatHangService`, form `FrmThanhToan`):**
- Phí giao hàng: Thường 20.000đ, Nhanh 40.000đ (miễn phí nếu tiền hàng >= 1.000.000đ), Nhanh trong ngày 70.000đ (miễn phí nếu >= 5.000.000đ)
- Phí thẻ tính theo % trên (tiền hàng + ship): Visa / MasterCard 1%, Discover 1.5%, Amex 2%
- Kiểm tra thẻ: đúng độ dài (Amex 15 số + CSV 4 số, còn lại 16 số + CSV 3 số), đúng checksum Luhn, đúng tên chủ thẻ, chưa hết hạn
- Mọi con số tiền đều tính lại ở Service, không lấy số UI gửi lên
- Gọi qua interface `IPaymentGateway` để duyệt thanh toán (bản demo dùng `MockPaymentGateway` luôn đồng ý, khi có cổng thật chỉ cần viết class mới)
- Lưu DB chỉ giữ `SoTheMask` dạng `**** **** **** 1234`, không lưu số thẻ đầy đủ và CSV

## 3. CSDL (2 bảng)

- `KhachHang`: thông tin + tài khoản, unique theo `TenDangNhap` và `SoCMND`
- `DonHang`: MaKH, loại giao hàng, người nhận, tiền hàng / phí ship / phí thẻ / tổng thanh toán, loại thẻ + thẻ đã che + hạn thẻ

## 4. Tài liệu phân tích

Nằm trong `e-Shopping_diagram/`, vẽ bằng draw.io:

- `1_UseCase_TongQuat.drawio`, `1.2_UseCase_DatHang_ThanhToan.drawio`
- `2_Activity_DatHang.drawio`, `3_Sequence_DatHang.drawio`
- Báo cáo Word trong `word/Word_BaoCao_LAB4.docx`

## 5. Chạy thử

1. Chạy `SQL/01_CreateTables.sql` trên SQL Server để tạo DB `eShoppingDb`
2. Kiểm tra connection string trong `Data/Db.cs` cho đúng server của máy
3. Mở `EShopping.slnx` bằng Visual Studio, Build rồi F5. `FrmMain` cho nhập MaKH + tổng tiền hàng rồi mở 2 form đăng ký / thanh toán
