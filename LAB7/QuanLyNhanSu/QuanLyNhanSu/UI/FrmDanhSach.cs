using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    /// <summary>
    /// Xem nhanh danh sach nhan vien va hop dong hien hanh.
    /// Chi doc du lieu qua Service.
    /// </summary>
    public class FrmDanhSach : Form
    {
        private readonly TuyenDungService _td = new TuyenDungService();
        private readonly HopDongService _hd = new HopDongService();
        private ComboBox cboTrangThai;
        private DataGridView dgvNhanVien, dgvHopDong;
        private Label lblTong;
        private DataTable _hopDong;

        public FrmDanhSach()
        {
            Text = "Danh sách nhân viên";
            ClientSize = new Size(1080, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(UiHelper.Lbl("Trạng thái:", 20, 20, 80));
            cboTrangThai = UiHelper.Cbo(110, 20, 190);
            cboTrangThai.Items.AddRange(new object[] { "Tất cả", "ThuViec", "DangLam", "DaNghi" });
            cboTrangThai.SelectedIndex = 0;
            Controls.Add(cboTrangThai);

            var btnTaiLai = UiHelper.Btn("Tải lại", 320, 18, 100);
            Controls.Add(btnTaiLai);

            lblTong = new Label
            {
                Left = 440, Top = 24, Width = 620, Height = 22,
                ForeColor = Color.FromArgb(30, 80, 140)
            };
            Controls.Add(lblTong);

            dgvNhanVien = UiHelper.Grid(20, 60, 1040, 300);
            Controls.Add(dgvNhanVien);

            Controls.Add(new Label
            {
                Text = "Hợp đồng của nhân viên đang chọn:", Left = 20, Top = 368,
                Width = 400, Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });
            dgvHopDong = UiHelper.Grid(20, 390, 1040, 300);
            Controls.Add(dgvHopDong);

            cboTrangThai.SelectedIndexChanged += (s, e) => LoadNhanVien();
            btnTaiLai.Click += (s, e) => { LoadNhanVien(); LoadHopDong(); };
            dgvNhanVien.SelectionChanged += (s, e) => HienHopDong();
            Load += (s, e) => { LoadNhanVien(); LoadHopDong(); };
        }

        private void LoadNhanVien()
        {
            string tt = Convert.ToString(cboTrangThai.SelectedItem);
            DataTable dt = _td.DanhSachNhanVien(tt == "Tất cả" ? null : tt);
            dgvNhanVien.DataSource = dt;
            if (dgvNhanVien.Columns["MaNV"] != null) dgvNhanVien.Columns["MaNV"].Visible = false;
            if (dgvNhanVien.Columns["MaNVCode"] != null) dgvNhanVien.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgvNhanVien.Columns["HoTen"] != null) dgvNhanVien.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgvNhanVien.Columns["GioiTinh"] != null) dgvNhanVien.Columns["GioiTinh"].HeaderText = "Giới tính";
            if (dgvNhanVien.Columns["NgaySinh"] != null)
            {
                dgvNhanVien.Columns["NgaySinh"].HeaderText = "Ngày sinh";
                dgvNhanVien.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            if (dgvNhanVien.Columns["CCCD"] != null) dgvNhanVien.Columns["CCCD"].HeaderText = "CCCD";
            if (dgvNhanVien.Columns["DienThoai"] != null) dgvNhanVien.Columns["DienThoai"].HeaderText = "Điện thoại";
            if (dgvNhanVien.Columns["Email"] != null) dgvNhanVien.Columns["Email"].HeaderText = "Email";
            if (dgvNhanVien.Columns["NgayVaoLam"] != null)
            {
                dgvNhanVien.Columns["NgayVaoLam"].HeaderText = "Ngày vào làm";
                dgvNhanVien.Columns["NgayVaoLam"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            if (dgvNhanVien.Columns["TenPB"] != null) dgvNhanVien.Columns["TenPB"].HeaderText = "Phòng ban";
            if (dgvNhanVien.Columns["TrangThai"] != null) dgvNhanVien.Columns["TrangThai"].HeaderText = "Trạng thái";
            lblTong.Text = "Tổng số: " + dt.Rows.Count + " nhân viên";
            HienHopDong();
        }

        private void LoadHopDong()
        {
            _hopDong = _hd.DanhSachHopDong();
        }

        private void HienHopDong()
        {
            if (_hopDong == null) LoadHopDong();
            if (dgvNhanVien.CurrentRow == null || !(dgvNhanVien.CurrentRow.DataBoundItem is DataRowView drv))
            {
                dgvHopDong.DataSource = null;
                return;
            }
            string maNVCode = Convert.ToString(drv["MaNVCode"]);
            _hopDong.DefaultView.RowFilter = string.Format("MaNVCode = '{0}'", maNVCode.Replace("'", "''"));
            dgvHopDong.DataSource = _hopDong.DefaultView;
            if (dgvHopDong.Columns["MaHD"] != null) dgvHopDong.Columns["MaHD"].Visible = false;
            if (dgvHopDong.Columns["MucLuong"] != null)
                dgvHopDong.Columns["MucLuong"].DefaultCellStyle.Format = "N0";
            if (dgvHopDong.Columns["SoHD"] != null) dgvHopDong.Columns["SoHD"].HeaderText = "Số HĐ";
            if (dgvHopDong.Columns["LoaiHD"] != null) dgvHopDong.Columns["LoaiHD"].HeaderText = "Loại HĐ";
            if (dgvHopDong.Columns["NgayKy"] != null)
                dgvHopDong.Columns["NgayKy"].HeaderText = "Ngày ký";
            if (dgvHopDong.Columns["NgayHieuLuc"] != null)
                dgvHopDong.Columns["NgayHieuLuc"].HeaderText = "Hiệu lực";
            if (dgvHopDong.Columns["NgayHetHan"] != null)
                dgvHopDong.Columns["NgayHetHan"].HeaderText = "Hết hạn";
            if (dgvHopDong.Columns["MucLuong"] != null)
                dgvHopDong.Columns["MucLuong"].HeaderText = "Mức lương";
            if (dgvHopDong.Columns["TrangThai"] != null)
                dgvHopDong.Columns["TrangThai"].HeaderText = "Trạng thái";
        }
    }
}
