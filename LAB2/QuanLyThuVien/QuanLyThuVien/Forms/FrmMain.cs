using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyThuVien.Forms
{
    public partial class FrmMain : Form
    {
        // Khai báo các control giao diện
        private Label lblTitle;
        private Button btnDanhMuc, btnSach, btnDocGia, btnMuonTra, btnThongKe, btnThoat;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Thiết lập Form
            this.Text = "Quản lý thư viện";
            this.ClientSize = new Size(600, 360);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Khởi tạo các thành phần
            lblTitle = new Label() { Text = "HỆ THỐNG QUẢN LÝ THƯ VIỆN", Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 60 };
            btnDanhMuc = new Button() { Text = "Danh mục / Nhân viên", Size = new Size(240, 60), Location = new Point(40, 80) };
            btnSach = new Button() { Text = "Quản lý đầu sách", Size = new Size(240, 60), Location = new Point(310, 80) };
            btnDocGia = new Button() { Text = "Độc giả và thẻ", Size = new Size(240, 60), Location = new Point(40, 160) };
            btnMuonTra = new Button() { Text = "Mượn - Trả sách", Size = new Size(240, 60), Location = new Point(310, 160) };
            btnThongKe = new Button() { Text = "Thống kê", Size = new Size(240, 60), Location = new Point(40, 240) };
            btnThoat = new Button() { Text = "Thoát", Size = new Size(240, 60), Location = new Point(310, 240) };

            // Gắn sự kiện (Event)
            btnDanhMuc.Click += btnDanhMuc_Click;
            btnSach.Click += btnSach_Click;
            btnDocGia.Click += btnDocGia_Click;
            btnMuonTra.Click += btnMuonTra_Click;
            btnThongKe.Click += btnThongKe_Click;
            btnThoat.Click += btnThoat_Click;

            // Thêm vào Form
            this.Controls.AddRange(new Control[] { lblTitle, btnDanhMuc, btnSach, btnDocGia, btnMuonTra, btnThongKe, btnThoat });
        }

        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            // Tạo form mới và hiển thị nó lên
            Forms.FrmDanhMuc frm = new Forms.FrmDanhMuc();
            frm.ShowDialog();
        }

        private void btnSach_Click(object sender, EventArgs e)
        {
            // using (FrmSach f = new FrmSach()) f.ShowDialog(this); 
            MessageBox.Show("Chức năng Sách");
        }

        private void btnDocGia_Click(object sender, EventArgs e)
        {
            // using (FrmDocGia f = new FrmDocGia()) f.ShowDialog(this); 
            MessageBox.Show("Chức năng Độc giả");
        }

        private void btnMuonTra_Click(object sender, EventArgs e)
        {
            // using (FrmMuonTra f = new FrmMuonTra()) f.ShowDialog(this); 
            MessageBox.Show("Chức năng Mượn Trả");
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            // using (FrmThongKe f = new FrmThongKe()) f.ShowDialog(this); 
            MessageBox.Show("Chức năng Thống kê");
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có thực sự muốn thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
        }
    }
}