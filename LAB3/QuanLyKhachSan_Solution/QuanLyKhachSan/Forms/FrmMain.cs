using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Quản lý khách sạn";
            Width = 800; // Mở rộng form ra để chứa 3 cột
            Height = 450;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;

            // --- TIÊU ĐỀ ---
            var title = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ KHÁCH SẠN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Navy, // Chữ màu xanh đậm chuẩn mẫu
                AutoSize = true
            };
            title.Location = new Point(190, 30); // Căn ra giữa
            Controls.Add(title);

            // --- THÔNG SỐ NÚT BẤM ---
            int btnWidth = 200;
            int btnHeight = 60;
            int startX = 70;      // Tọa độ X bắt đầu (Cột 1)
            int startY = 110;     // Tọa độ Y bắt đầu (Hàng 1)
            int gapX = 220;       // Khoảng cách giữa các cột
            int gapY = 90;        // Khoảng cách giữa các hàng

            // --- KHỞI TẠO NÚT BẤM (Gắn sẵn tọa độ chia 3 cột) ---
            var btnDanhMuc = MakeBtn("btnDanhMuc", "Danh mục", startX, startY, btnWidth, btnHeight);
            var btnPhong = MakeBtn("btnPhong", "Phòng - Tiện nghi", startX + gapX, startY, btnWidth, btnHeight);
            var btnDatPhong = MakeBtn("btnDatPhong", "Đặt / Nhận phòng", startX + gapX * 2, startY, btnWidth, btnHeight);

            var btnDichVu = MakeBtn("btnDichVu", "Sử dụng dịch vụ", startX, startY + gapY, btnWidth, btnHeight);
            var btnTraPhong = MakeBtn("btnTraPhong", "Trả phòng - Thanh toán", startX + gapX, startY + gapY, btnWidth, btnHeight);
            var btnThongKe = MakeBtn("btnThongKe", "Thống kê", startX + gapX * 2, startY + gapY, btnWidth, btnHeight);

            var btnThoat = MakeBtn("btnThoat", "Thoát", startX + gapX, startY + gapY * 2, btnWidth, btnHeight); // Nút thoát nằm giữa hàng 3

            // --- GẮN SỰ KIỆN CLICK ---
            btnDanhMuc.Click += (s, e) => { using (var f = new FrmDanhMuc()) f.ShowDialog(this); };
            btnPhong.Click += (s, e) => { using (var f = new FrmPhongTienNghi()) f.ShowDialog(this); };
            btnDatPhong.Click += (s, e) => { using (var f = new FrmDatPhong()) f.ShowDialog(this); };
            btnDichVu.Click += (s, e) => { using (var f = new FrmDichVu()) f.ShowDialog(this); };
            btnTraPhong.Click += (s, e) => { using (var f = new FrmTraPhong()) f.ShowDialog(this); };
            btnThongKe.Click += (s, e) => { using (var f = new FrmThongKe()) f.ShowDialog(this); };
            btnThoat.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có thực sự muốn thoát?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };

            // --- THÊM VÀO FORM ---
            Controls.Add(btnDanhMuc);
            Controls.Add(btnPhong);
            Controls.Add(btnDatPhong);
            Controls.Add(btnDichVu);
            Controls.Add(btnTraPhong);
            Controls.Add(btnThongKe);
            Controls.Add(btnThoat);
        }

        // Hàm tạo nút đã được nâng cấp để chuẩn bị sẵn layout cho Icon
        private Button MakeBtn(string name, string text, int x, int y, int w, int h) => new Button
        {
            Name = name,
            Text = text,
            Location = new Point(x, y),
            Size = new Size(w, h),
            Font = new Font("Segoe UI", 10),

            // 3 dòng này cực kỳ quan trọng để Icon và Chữ không đè lên nhau
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(15, 0, 0, 0) // Thụt đầu dòng cho Icon đỡ dính viền
        };
    }
}