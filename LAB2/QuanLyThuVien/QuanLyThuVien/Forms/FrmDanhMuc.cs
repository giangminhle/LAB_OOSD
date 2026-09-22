using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Services;
// using QuanLyThuVien.Models; // Nếu chữ NhanVien bên dưới bị đỏ thì bỏ // ở dòng này đi nhé

namespace QuanLyThuVien.Forms
{
    public class FrmDanhMuc : Form
    {
        private DanhMucService danhMucService = new DanhMucService();
        private TabControl tabControl;

        // Các control cho tab Nhân Viên theo chuẩn file Word
        private DataGridView dgvNhanVien;
        private TextBox txtMaNV, txtHoNV, txtTenNV, txtPhaiNV, txtChucVuNV, txtSDTNV;
        private DateTimePicker dtpNgaySinhNV;
        private Button btnThemNV, btnCapNhatNV, btnXoaNV, btnMoiNV;

        public FrmDanhMuc()
        {
            this.Text = "Danh mục và nhân viên";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl { Dock = DockStyle.Fill };

            // ========================================================
            // TẠO TAB NHÂN VIÊN (GIỐNG Y HỆT FILE WORD)
            // ========================================================
            TabPage tabNV = new TabPage("Nhân viên");
            Panel pnlNV = new Panel { Dock = DockStyle.Top, Height = 180 };
            dgvNhanVien = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            // --- CỘT 1 ---
            pnlNV.Controls.Add(new Label { Text = "Mã nhân viên:", Location = new Point(20, 23), AutoSize = true });
            txtMaNV = new TextBox { Location = new Point(110, 20), Width = 150 };

            pnlNV.Controls.Add(new Label { Text = "Họ:", Location = new Point(20, 63), AutoSize = true });
            txtHoNV = new TextBox { Location = new Point(110, 60), Width = 150 };

            pnlNV.Controls.Add(new Label { Text = "Tên:", Location = new Point(20, 103), AutoSize = true });
            txtTenNV = new TextBox { Location = new Point(110, 100), Width = 150 };

            // --- CỘT 2 ---
            pnlNV.Controls.Add(new Label { Text = "Phái:", Location = new Point(320, 23), AutoSize = true });
            txtPhaiNV = new TextBox { Location = new Point(400, 20), Width = 150 };

            pnlNV.Controls.Add(new Label { Text = "Ngày sinh:", Location = new Point(320, 63), AutoSize = true });
            dtpNgaySinhNV = new DateTimePicker { Location = new Point(400, 60), Width = 150, Format = DateTimePickerFormat.Short };

            pnlNV.Controls.Add(new Label { Text = "Chức vụ:", Location = new Point(320, 103), AutoSize = true });
            txtChucVuNV = new TextBox { Location = new Point(400, 100), Width = 150 };

            pnlNV.Controls.Add(new Label { Text = "Điện thoại:", Location = new Point(320, 143), AutoSize = true });
            txtSDTNV = new TextBox { Location = new Point(400, 140), Width = 150 };

            // --- CỘT 3 (4 Nút bấm xếp 2x2) ---
            btnThemNV = new Button { Text = "Thêm", Location = new Point(620, 20), Width = 100, Height = 30 };
            btnCapNhatNV = new Button { Text = "Cập nhật", Location = new Point(740, 20), Width = 100, Height = 30 };
            btnXoaNV = new Button { Text = "Xóa", Location = new Point(620, 60), Width = 100, Height = 30 };
            btnMoiNV = new Button { Text = "Làm mới", Location = new Point(740, 60), Width = 100, Height = 30 };

            // Gắn hết vào Panel Nhân viên
            pnlNV.Controls.AddRange(new Control[] { txtMaNV, txtHoNV, txtTenNV, txtPhaiNV, dtpNgaySinhNV, txtChucVuNV, txtSDTNV, btnThemNV, btnCapNhatNV, btnXoaNV, btnMoiNV });
            tabNV.Controls.Add(dgvNhanVien);
            tabNV.Controls.Add(pnlNV);

            // ========================================================
            // CÁC TAB KHÁC
            // ========================================================
            TabPage tabTL = new TabPage("Thể loại");
            TabPage tabNXB = new TabPage("Nhà xuất bản");
            // (Phần giao diện Thể Loại với NXB mày tự thêm code vào đây)

            tabControl.TabPages.Add(tabNV);
            tabControl.TabPages.Add(tabTL);
            tabControl.TabPages.Add(tabNXB);
            this.Controls.Add(tabControl);

            // Đổ dữ liệu khi mở form
            this.Load += (s, e) => {
                try { dgvNhanVien.DataSource = danhMucService.LayNhanVien(); }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            // Sự kiện click vào lưới
            dgvNhanVien.CellClick += DgvNhanVien_CellClick;

            // Xóa rỗng các ô
            btnMoiNV.Click += (s, e) => {
                txtMaNV.Clear(); txtHoNV.Clear(); txtTenNV.Clear(); txtPhaiNV.Clear();
                txtChucVuNV.Clear(); txtSDTNV.Clear(); txtMaNV.Enabled = true;
            };

            // Đăng ký sự kiện Thêm/Sửa/Xóa chuẩn chỉ
            btnThemNV.Click += BtnThemNV_Click;
            btnCapNhatNV.Click += BtnCapNhatNV_Click;
            btnXoaNV.Click += BtnXoaNV_Click;
        }

        private void DgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];
                txtMaNV.Text = row.Cells["MaNhanVien"].Value?.ToString();
                txtHoNV.Text = row.Cells["Ho"].Value?.ToString();
                txtTenNV.Text = row.Cells["Ten"].Value?.ToString();
                txtPhaiNV.Text = row.Cells["Phai"].Value?.ToString();
                txtChucVuNV.Text = row.Cells["ChucVu"].Value?.ToString();
                txtSDTNV.Text = row.Cells["SoDienThoai"].Value?.ToString();

                if (DateTime.TryParse(row.Cells["NgaySinh"].Value?.ToString(), out DateTime ns))
                    dtpNgaySinhNV.Value = ns;

                txtMaNV.Enabled = false; // Khóa trường Mã lại khi click
            }
        }

        // ========================================================
        // XỬ LÝ NÚT THÊM - CẬP NHẬT - XÓA CHO NHÂN VIÊN
        // ========================================================
        private NhanVien GetNhanVienFromUI()
        {
            return new NhanVien
            {
                MaNhanVien = txtMaNV.Text,
                Ho = txtHoNV.Text,
                Ten = txtTenNV.Text,
                Phai = txtPhaiNV.Text,
                NgaySinh = dtpNgaySinhNV.Value,
                ChucVu = txtChucVuNV.Text,
                SoDienThoai = txtSDTNV.Text
            };
        }

        private void BtnThemNV_Click(object sender, EventArgs e)
        {
            var nv = GetNhanVienFromUI();
            var kq = danhMucService.LuuNhanVien(nv, false);

            MessageBox.Show("Đã thực hiện xong lệnh Thêm!");
            dgvNhanVien.DataSource = danhMucService.LayNhanVien();
            btnMoiNV.PerformClick();
        }

        private void BtnCapNhatNV_Click(object sender, EventArgs e)
        {
            var nv = GetNhanVienFromUI();
            var kq = danhMucService.LuuNhanVien(nv, true);

            MessageBox.Show("Đã thực hiện xong lệnh Cập nhật!");
            dgvNhanVien.DataSource = danhMucService.LayNhanVien();
            btnMoiNV.PerformClick();
        }

        private void BtnXoaNV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Chọn 1 nhân viên dưới danh sách để xóa!");
                return;
            }

            if (MessageBox.Show("Chắc chắn muốn xóa nhân viên này chứ?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var kq = danhMucService.Xoa("NhanVien", "MaNhanVien", txtMaNV.Text);
                MessageBox.Show("Đã xử lý lệnh Xóa!");
                dgvNhanVien.DataSource = danhMucService.LayNhanVien();
                btnMoiNV.PerformClick();
            }
        }
    }
}