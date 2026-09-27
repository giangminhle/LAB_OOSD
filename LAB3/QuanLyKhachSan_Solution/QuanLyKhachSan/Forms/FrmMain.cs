using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKhachSan.Forms; 

namespace QuanLyKhachSan.UI
{
    public partial class FrmMain : Form
    {
        private Label lblTitle;
        private Button btnDanhMuc;
        private Button btnPhongTienNghi;
        private Button btnDatNhanPhong;
        private Button btnSuDungDichVu;
        private Button btnTraPhong;
        private Button btnThongKe;
        private Button btnThoat;

        public FrmMain()
        {
            InitializeComponent();
            VeGiaoDien();
        }

        private void VeGiaoDien()
        {
            this.Text = "Quản lý khách sạn";
            this.Size = new Size(760, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(235, 240, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Controls.Clear();

            // Tiêu đề
            lblTitle = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ KHÁCH SẠN",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 50, 110),
                AutoSize = true,
                Location = new Point(190, 25)
            };
            this.Controls.Add(lblTitle);

            Size btnSize = new Size(210, 65);
            // Tăng size font lên 1 chút để Emoji nhìn to rõ hơn
            Font btnFont = new Font("Segoe UI", 11F, FontStyle.Regular); 

            int col1 = 50;
            int col2 = 270;
            int col3 = 490;

            int row1 = 80;
            int row2 = 160;
            int row3 = 240;

            // Hàng 1 - Nhét thẳng Emoji vào chuỗi Text
            btnDanhMuc = TaoButton("📋 Danh mục", new Point(col1, row1), btnSize, btnFont);
            btnDanhMuc.Click += (s, e) => MoForm(new FrmDanhMuc());

            btnPhongTienNghi = TaoButton("🛏️ Phòng - Tiện nghi", new Point(col2, row1), btnSize, btnFont);
            btnPhongTienNghi.Click += (s, e) => MoForm(new FrmPhongTienNghi());

            btnDatNhanPhong = TaoButton("🔑 Đặt / Nhận phòng", new Point(col3, row1), btnSize, btnFont);
            btnDatNhanPhong.Click += (s, e) => MoForm(new FrmDatPhong());

            // Hàng 2
            btnSuDungDichVu = TaoButton("⚙️ Sử dụng dịch vụ", new Point(col1, row2), btnSize, btnFont);
            btnSuDungDichVu.Click += (s, e) => MoForm(new FrmDichVu());

            btnTraPhong = TaoButton("💰 Trả phòng - TT", new Point(col2, row2), btnSize, btnFont);
            btnTraPhong.Click += (s, e) => MoForm(new FrmTraPhong());

            btnThongKe = TaoButton("📊 Thống kê", new Point(col3, row2), btnSize, btnFont);
            btnThongKe.Click += (s, e) => MoForm(new FrmThongKe());

            // Hàng 3 (Ở giữa)
            btnThoat = TaoButton("🚪 Thoát", new Point(col2, row3), btnSize, btnFont);
            btnThoat.Click += (s, e) => Application.Exit();

            // Thêm các nút vào Form
            this.Controls.Add(btnDanhMuc);
            this.Controls.Add(btnPhongTienNghi);
            this.Controls.Add(btnDatNhanPhong);
            this.Controls.Add(btnSuDungDichVu);
            this.Controls.Add(btnTraPhong);
            this.Controls.Add(btnThongKe);
            this.Controls.Add(btnThoat);
        }

        // Hàm tạo nút đã được dọn dẹp sạch sẽ, không cần xử lý Image nữa
        private Button TaoButton(string text, Point location, Size size, Font font)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = size,
                Font = font,
                BackColor = Color.FromArgb(245, 245, 245),
                FlatStyle = FlatStyle.Standard,
                Cursor = Cursors.Hand
            };
        }

        // Hàm mở form con
        private void MoForm(Form f)
        {
            this.Hide();
            f.ShowDialog(this);
            this.Show();
        }
    }
}