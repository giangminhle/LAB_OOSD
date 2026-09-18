# LAB 1: Thực hành OOP & Phân tích thiết kế hệ thống (OOAD)

Repository này chứa mã nguồn Java và tài liệu phân tích thiết kế cho bài LAB 1 môn Phân tích thiết kế phần mềm hướng đối tượng[cite: 2].

## 1. Triển khai mã nguồn (Java)
Phần code này minh họa các tính chất cơ bản của lập trình hướng đối tượng (Tính trừu tượng, Kế thừa, Đa hình) thông qua bài toán hình học[cite: 2].

**Cấu trúc class:**
- `CHinhVe` (Abstract class): Định nghĩa khung chuẩn với các phương thức `DienTich()`, `ChuVi()`, `Ve()`[cite: 2].
- `CDiem`: Định nghĩa tọa độ điểm (x, y) trên mặt phẳng[cite: 2].
- `CTamGiac`, `CTuGiac`, `CEllipse`: Các class kế thừa từ `CHinhVe`, tự implement lại các phương thức tính toán tùy theo đặc thù hình học[cite: 2].
- `Main`: File khởi chạy và test thử nghiệm[cite: 2].

## 2. Phân tích thiết kế: Hệ thống thư viện trực tuyến
Bản phân tích yêu cầu phần mềm cho dự án Thư viện trực tuyến chạy trên mạng nội bộ (Intranet) của trường học[cite: 2].

**Các tác nhân (Actors) chính:**
- **Độc giả** (Giảng viên, SV, Nhân viên): Tìm kiếm, đọc/tải tài liệu điện tử, đăng ký mượn sách in, yêu cầu đặt mua[cite: 2].
- **Thủ thư**: Kế thừa các quyền của Độc giả. Phụ trách quản lý mượn/trả, quản lý danh mục sách và duyệt yêu cầu mua tài liệu[cite: 2].
- **Hệ thống**: Chạy ngầm, tự động gửi email nhắc trả sách trước 3 ngày và xuất thống kê[cite: 2].

**Tài liệu đính kèm trong LAB:**
- Khảo sát yêu cầu chức năng và bảng thuật ngữ hệ thống[cite: 2].
- Sơ đồ Use Case tổng quát (gồm 13 Use cases cốt lõi)[cite: 2].
- Đặc tả chi tiết Use Case (VD: UC01 - Tìm kiếm tài liệu)[cite: 2].
- Activity Diagram biểu diễn luồng nghiệp vụ[cite: 2].