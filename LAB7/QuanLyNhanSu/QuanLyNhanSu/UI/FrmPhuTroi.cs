using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmPhuTroi : Form
    {
        private readonly PhuTroiService _svc = new PhuTroiService();
        private readonly TuyenDungService _td = new TuyenDungService();
        private ComboBox cboNV, cboHinhThuc;
        private DateTimePicker dtNgay;
        private TextBox txtNguoiLap, txtGhiChu, txtGhiChuCT;
        private NumericUpDown numGio;
        private DataGridView dgvCT, dgvPhieu;
        private DataTable _chiTiet;
        private Label lblTrangThai, lblTongGio;
        private int _maPhieu;

        public FrmPhuTroi()
        {
            Text = "Phiếu ghi nhận giờ phụ trội (hệ số 1.5)";
            ClientSize = new Size(1000, 780);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var grpHeader = UiHelper.Group("Thông tin phiếu", 20, 15, 960, 105);
            grpHeader.Controls.Add(UiHelper.Lbl("Ngày phiếu:", 15, 25));
            dtNgay = UiHelper.Date(100, 25, 140);
            grpHeader.Controls.Add(UiHelper.Lbl("Hình thức:", 260, 25));
            cboHinhThuc = UiHelper.Cbo(335, 25, 130);
            cboHinhThuc.Items.AddRange(new object[] { "TangCa", "NgayLe", "NgayNghi" });
            cboHinhThuc.SelectedIndex = 0;
            grpHeader.Controls.Add(UiHelper.Lbl("Người lập:", 490, 25));
            txtNguoiLap = UiHelper.Txt(570, 25, 220);
            txtNguoiLap.Text = "Trưởng phòng";
            grpHeader.Controls.Add(UiHelper.Lbl("Ghi chú:", 15, 65));
            txtGhiChu = UiHelper.Txt(100, 65, 690);
            grpHeader.Controls.AddRange(new Control[] { dtNgay, cboHinhThuc, txtNguoiLap, txtGhiChu });
            Controls.Add(grpHeader);

            var grpAdd = UiHelper.Group("Thêm nhân viên vào phiếu", 20, 130, 960, 85);
            grpAdd.Controls.Add(UiHelper.Lbl("Nhân viên:", 15, 25));
            cboNV = UiHelper.Cbo(100, 25, 330);
            grpAdd.Controls.Add(UiHelper.Lbl("Số giờ:", 450, 25));
            numGio = new NumericUpDown
            {
                Left = 510, Top = 25, Width = 90, Minimum = 0.5m, Maximum = 12m,
                DecimalPlaces = 2, Increment = 0.5m, Value = 2m
            };
            grpAdd.Controls.Add(UiHelper.Lbl("Ghi chú:", 620, 25));
            txtGhiChuCT = UiHelper.Txt(680, 25, 140);
            var btnThem = UiHelper.Btn("Thêm", 830, 21, 70);
            var btnXoa = UiHelper.Btn("Xóa", 905, 21, 40);
            grpAdd.Controls.AddRange(new Control[] { cboNV, numGio, txtGhiChuCT, btnThem, btnXoa });
            Controls.Add(grpAdd);

            dgvCT = UiHelper.Grid(20, 225, 960, 230);
            Controls.Add(dgvCT);

            lblTongGio = new Label { Left = 20, Top = 465, Width = 300, Height = 20, ForeColor = Color.DimGray };
            var btnLuu = UiHelper.Btn("Lưu phiếu", 600, 460, 120);
            var btnDuyet = UiHelper.Btn("Duyệt phiếu đang chọn", 730, 460, 190);
            lblTrangThai = new Label { Left = 20, Top = 492, Width = 960, Height = 22, ForeColor = Color.DarkGreen };
            Controls.AddRange(new Control[] { lblTongGio, btnLuu, btnDuyet, lblTrangThai });

            Controls.Add(new Label
            {
                Text = "Danh sách phiếu phụ trội:", Left = 20, Top = 522, Width = 300, ForeColor = Color.DimGray
            });
            dgvPhieu = UiHelper.Grid(20, 545, 960, 215);
            Controls.Add(dgvPhieu);

            TaoBang();
            btnThem.Click += (s, e) => ThemDong();
            btnXoa.Click += (s, e) => XoaDong();
            btnLuu.Click += (s, e) => Luu();
            btnDuyet.Click += (s, e) => Duyet();
            Load += (s, e) =>
            {
                LoadNhanVien();
                dtNgay.Value = new DateTime(2026, 10, 15);
                LoadPhieu();
            };
        }

        private void TaoBang()
        {
            _chiTiet = new DataTable();
            _chiTiet.Columns.Add("MaNV", typeof(int));
            _chiTiet.Columns.Add("MaNVCode", typeof(string));
            _chiTiet.Columns.Add("HoTen", typeof(string));
            _chiTiet.Columns.Add("SoGio", typeof(decimal));
            _chiTiet.Columns.Add("HeSo", typeof(decimal));
            _chiTiet.Columns.Add("GhiChu", typeof(string));
            dgvCT.DataSource = _chiTiet;
            if (dgvCT.Columns["MaNV"] != null) dgvCT.Columns["MaNV"].Visible = false;
            if (dgvCT.Columns["MaNVCode"] != null) dgvCT.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgvCT.Columns["HoTen"] != null) dgvCT.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgvCT.Columns["SoGio"] != null) dgvCT.Columns["SoGio"].HeaderText = "Số giờ";
            if (dgvCT.Columns["HeSo"] != null) dgvCT.Columns["HeSo"].HeaderText = "Hệ số";
            if (dgvCT.Columns["GhiChu"] != null) dgvCT.Columns["GhiChu"].HeaderText = "Ghi chú";
            _chiTiet.RowChanged += (s, e) => CapNhatTongGio();
        }

        private void LoadNhanVien()
        {
            DataTable dt = _td.DanhSachNhanVien("DangLam");
            dt.Columns.Add("HienThi", typeof(string));
            foreach (DataRow r in dt.Rows) r["HienThi"] = r["MaNVCode"] + " - " + r["HoTen"];
            UiHelper.RefreshCombo(cboNV, dt, "MaNV", "HienThi");
        }

        private void ThemDong()
        {
            if (cboNV.SelectedValue == null) return;
            int maNV = Convert.ToInt32(cboNV.SelectedValue);
            foreach (DataRow r in _chiTiet.Rows)
                if (Convert.ToInt32(r["MaNV"]) == maNV)
                {
                    MessageBox.Show("Nhân viên đã có trong phiếu.", "Trùng nhân viên");
                    return;
                }
            var row = ((DataRowView)cboNV.SelectedItem).Row;
            _chiTiet.Rows.Add(maNV, row["MaNVCode"], row["HoTen"], numGio.Value, 1.5m, txtGhiChuCT.Text.Trim());
            txtGhiChuCT.Clear();
            CapNhatTongGio();
        }

        private void XoaDong()
        {
            if (dgvCT.CurrentRow != null && dgvCT.CurrentRow.Index >= 0)
            {
                _chiTiet.Rows.RemoveAt(dgvCT.CurrentRow.Index);
                CapNhatTongGio();
            }
        }

        private void CapNhatTongGio()
        {
            decimal tong = 0;
            foreach (DataRow r in _chiTiet.Rows) tong += Convert.ToDecimal(r["SoGio"]);
            lblTongGio.Text = "Tổng giờ phụ trội: " + tong.ToString("N2") + " giờ | Hệ số bắt buộc: 1.5";
        }

        private void Luu()
        {
            var x = new PhuTroiInput
            {
                NgayPhieu = dtNgay.Value.Date,
                HinhThuc = Convert.ToString(cboHinhThuc.SelectedItem),
                NguoiLap = txtNguoiLap.Text.Trim(),
                GhiChu = txtGhiChu.Text.Trim()
            };
            foreach (DataRow r in _chiTiet.Rows)
                x.ChiTiet.Add(new ChiTietPhuTroiInput
                {
                    MaNV = Convert.ToInt32(r["MaNV"]),
                    SoGio = Convert.ToDecimal(r["SoGio"]),
                    HeSo = Convert.ToDecimal(r["HeSo"]),
                    GhiChu = Convert.ToString(r["GhiChu"])
                });
            ServiceResult kq = _svc.Luu(x);
            lblTrangThai.Text = kq.Message;
            lblTrangThai.ForeColor = kq.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(kq.Message, kq.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, kq.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (kq.Success) { _maPhieu = kq.Id; LoadPhieu(); }
        }

        private void Duyet()
        {
            int ma = _maPhieu;
            if (dgvPhieu.CurrentRow != null && dgvPhieu.CurrentRow.Cells["MaPhieuPT"].Value != null)
                ma = Convert.ToInt32(dgvPhieu.CurrentRow.Cells["MaPhieuPT"].Value);
            if (ma <= 0) { MessageBox.Show("Hãy chọn một phiếu để duyệt."); return; }
            ServiceResult r = _svc.Duyet(ma);
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) LoadPhieu();
        }

        private void LoadPhieu()
        {
            dgvPhieu.DataSource = _svc.DanhSachPhieu();
            if (dgvPhieu.Columns["SoPhieu"] != null) dgvPhieu.Columns["SoPhieu"].HeaderText = "Số phiếu";
            if (dgvPhieu.Columns["NgayPhieu"] != null) dgvPhieu.Columns["NgayPhieu"].HeaderText = "Ngày";
            if (dgvPhieu.Columns["HinhThuc"] != null) dgvPhieu.Columns["HinhThuc"].HeaderText = "Hình thức";
            if (dgvPhieu.Columns["NguoiLap"] != null) dgvPhieu.Columns["NguoiLap"].HeaderText = "Người lập";
            if (dgvPhieu.Columns["TongGio"] != null) dgvPhieu.Columns["TongGio"].HeaderText = "Tổng giờ";
        }
    }
}
