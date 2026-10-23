using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmTiepNhan : Form
    {
        private readonly TuyenDungService _svc = new TuyenDungService();
        private TextBox txtHoTen, txtCCCD, txtDienThoai, txtEmail, txtDiaChi, txtNoiDung, txtNguoiKy;
        private ComboBox cboGioiTinh, cboPhongBan;
        private DateTimePicker dtNgaySinh, dtNgayBanHanh, dtNgayBatDau, dtNgayKetThuc;
        private NumericUpDown numLuong;
        private DataGridView dgv;
        private Label lblTrangThai;

        public FrmTiepNhan()
        {
            Text = "Tiếp nhận nhân viên thử việc - Lập quyết định tuyển dụng";
            ClientSize = new Size(920, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            int y = 20;
            Controls.Add(UiHelper.Lbl("Họ và tên:", 20, y));
            txtHoTen = UiHelper.Txt(135, y, 270); y += 32;
            Controls.Add(UiHelper.Lbl("Giới tính:", 430, y - 32));
            cboGioiTinh = UiHelper.Cbo(535, y - 32, 100);
            cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nu", "Khac" });
            cboGioiTinh.SelectedIndex = 0;
            Controls.Add(UiHelper.Lbl("Ngày sinh:", 660, y - 32));
            dtNgaySinh = UiHelper.Date(750, y - 32, 140);
            dtNgaySinh.Value = new DateTime(1995, 1, 1);

            Controls.Add(UiHelper.Lbl("CCCD (12 số):", 20, y));
            txtCCCD = UiHelper.Txt(135, y, 150);
            Controls.Add(UiHelper.Lbl("Điện thoại:", 310, y));
            txtDienThoai = UiHelper.Txt(420, y, 150);
            Controls.Add(UiHelper.Lbl("Phòng ban:", 600, y));
            cboPhongBan = UiHelper.Cbo(700, y, 190); y += 32;

            Controls.Add(UiHelper.Lbl("Email:", 20, y));
            txtEmail = UiHelper.Txt(135, y, 250);
            Controls.Add(UiHelper.Lbl("Địa chỉ:", 410, y));
            txtDiaChi = UiHelper.Txt(500, y, 390); y += 32;

            Controls.Add(UiHelper.Lbl("Ngày ban hành:", 20, y));
            dtNgayBanHanh = UiHelper.Date(135, y, 140);
            Controls.Add(UiHelper.Lbl("Bắt đầu thử việc:", 300, y));
            dtNgayBatDau = UiHelper.Date(430, y, 140);
            Controls.Add(UiHelper.Lbl("Kết thúc (≤60 ngày):", 590, y));
            dtNgayKetThuc = UiHelper.Date(760, y, 130); y += 32;

            Controls.Add(UiHelper.Lbl("Lương thử việc:", 20, y));
            numLuong = UiHelper.Num(135, y, 1000000000m, 200);
            numLuong.Value = 7000000m;
            Controls.Add(UiHelper.Lbl("Số QĐ:", 360, y));
            Controls.Add(new Label { Text = "Tự sinh khi lưu", Left = 430, Top = y + 4, Width = 200, ForeColor = Color.Gray }); y += 32;

            Controls.Add(UiHelper.Lbl("Nội dung QĐ:", 20, y));
            txtNoiDung = new TextBox { Left = 135, Top = y, Width = 755, Height = 50, Multiline = true };
            txtNoiDung.Text = "Thử việc tại phòng ban theo nhu cầu tuyển dụng; mức lương thử việc theo quyết định.";
            y += 60;

            Controls.Add(UiHelper.Lbl("Người ký:", 20, y));
            txtNguoiKy = UiHelper.Txt(135, y, 300);
            txtNguoiKy.Text = "Giám đốc Trần Văn Hùng";
            var btnLuu = UiHelper.Btn("Tiếp nhận + lập QĐ", 450, y - 2, 180);
            var btnMoi = UiHelper.Btn("Làm mới", 640, y - 2, 110); y += 48;

            lblTrangThai = new Label { Left = 20, Top = y, Width = 870, Height = 22, ForeColor = Color.DarkGreen }; y += 26;
            dgv = UiHelper.Grid(20, y, 880, 205);
            Controls.AddRange(new Control[] { txtHoTen, cboGioiTinh, dtNgaySinh, txtCCCD, txtDienThoai,
                cboPhongBan, txtEmail, txtDiaChi, dtNgayBanHanh, dtNgayBatDau, dtNgayKetThuc, numLuong,
                txtNoiDung, txtNguoiKy, btnLuu, btnMoi, lblTrangThai, dgv });

            btnLuu.Click += (s, e) => Luu();
            btnMoi.Click += (s, e) => LamMoi();
            dtNgayBatDau.ValueChanged += (s, e) =>
            {
                if (dtNgayKetThuc.Value <= dtNgayBatDau.Value)
                    dtNgayKetThuc.Value = dtNgayBatDau.Value.AddDays(60);
            };
            Load += (s, e) => { LoadPhongBan(); LamMoi(); LoadGrid(); };
        }

        private void LoadPhongBan()
        {
            UiHelper.RefreshCombo(cboPhongBan, _svc.DanhSachPhongBan(), "MaPB", "TenPB");
        }

        private void LamMoi()
        {
            txtHoTen.Clear(); txtCCCD.Clear(); txtDienThoai.Clear(); txtEmail.Clear(); txtDiaChi.Clear();
            dtNgaySinh.Value = new DateTime(1995, 1, 1);
            dtNgayBanHanh.Value = DateTime.Today;
            dtNgayBatDau.Value = DateTime.Today;
            dtNgayKetThuc.Value = DateTime.Today.AddDays(60);
            numLuong.Value = 7000000m;
            lblTrangThai.Text = "";
            txtHoTen.Focus();
        }

        private void Luu()
        {
            var x = new TuyenDungInput
            {
                HoTen = txtHoTen.Text.Trim(),
                GioiTinh = Convert.ToString(cboGioiTinh.SelectedItem),
                NgaySinh = dtNgaySinh.Value.Date,
                CCCD = txtCCCD.Text.Trim(),
                DienThoai = txtDienThoai.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                MaPB = cboPhongBan.SelectedValue == null ? 0 : Convert.ToInt32(cboPhongBan.SelectedValue),
                NgayBanHanh = dtNgayBanHanh.Value.Date,
                NgayBatDau = dtNgayBatDau.Value.Date,
                NgayKetThuc = dtNgayKetThuc.Value.Date,
                MucLuong = numLuong.Value,
                NoiDung = txtNoiDung.Text.Trim(),
                NguoiKy = txtNguoiKy.Text.Trim()
            };
            ServiceResult r = _svc.TiepNhan(x);
            lblTrangThai.Text = r.Message;
            lblTrangThai.ForeColor = r.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success)
            {
                LoadGrid();
                LamMoi();
                lblTrangThai.Text = r.Message;
            }
        }

        private void LoadGrid()
        {
            dgv.DataSource = _svc.DanhSachQuyetDinhTuyenDung();
            if (dgv.Columns["MucLuongThuViec"] != null)
                dgv.Columns["MucLuongThuViec"].DefaultCellStyle.Format = "N0";
            if (dgv.Columns["HoTen"] != null) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns["MaNVCode"] != null) dgv.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgv.Columns["TenPB"] != null) dgv.Columns["TenPB"].HeaderText = "Phòng ban";
            if (dgv.Columns["SoQD"] != null) dgv.Columns["SoQD"].HeaderText = "Số QĐ";
        }
    }
}
