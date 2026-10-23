using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmKeHoach : Form
    {
        private readonly KeHoachService _svc = new KeHoachService();
        private readonly TuyenDungService _td = new TuyenDungService();
        private ComboBox cboPB, cboNV, cboCa;
        private NumericUpDown numTuan, numNam;
        private DateTimePicker dtNgay;
        private TextBox txtGhiChu, txtNguoiLap;
        private CheckBox chkPhuTroi;
        private DataGridView dgvChiTiet, dgvKeHoach;
        private Label lblTrangThai, lblKhoangTuan;
        private DataTable _chiTiet;
        private int _maKH;
        private bool _dangTai;
        private Button btnThem, btnXoa, btnLuu, btnDuyet;

        public FrmKeHoach()
        {
            Text = "Kế hoạch làm việc tuần - phân công ca và phụ trội";
            ClientSize = new Size(1000, 790);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            int y = 18;
            Controls.Add(UiHelper.Lbl("Phòng ban:", 20, y));
            cboPB = UiHelper.Cbo(100, y, 210);
            Controls.Add(UiHelper.Lbl("Tuần:", 330, y));
            numTuan = new NumericUpDown { Left = 380, Top = y, Width = 60, Minimum = 1, Maximum = 53, Value = 42 };
            Controls.Add(numTuan);
            Controls.Add(UiHelper.Lbl("Năm:", 455, y));
            numNam = new NumericUpDown { Left = 500, Top = y, Width = 80, Minimum = 2000, Maximum = 2100, Value = 2026 };
            Controls.Add(numNam);
            var btnTai = UiHelper.Btn("Tải kế hoạch", 595, y - 2, 120);
            lblKhoangTuan = new Label { Left = 730, Top = y + 4, Width = 250, Height = 20, ForeColor = Color.DimGray };
            Controls.AddRange(new Control[] { cboPB, numTuan, numNam, btnTai, lblKhoangTuan });

            dgvChiTiet = UiHelper.Grid(20, 58, 960, 300);
            Controls.Add(dgvChiTiet);

            var grpThem = UiHelper.Group("Thêm phân công", 20, 370, 960, 120);
            grpThem.Controls.Add(UiHelper.Lbl("Nhân viên:", 15, 28));
            cboNV = UiHelper.Cbo(90, 28, 260);
            grpThem.Controls.Add(UiHelper.Lbl("Ngày:", 370, 28));
            dtNgay = UiHelper.Date(420, 28, 140);
            grpThem.Controls.Add(UiHelper.Lbl("Ca:", 580, 28));
            cboCa = UiHelper.Cbo(620, 28, 100);
            cboCa.Items.AddRange(new object[] { "Sang", "Chieu", "Toi" });
            cboCa.SelectedIndex = 0;
            chkPhuTroi = new CheckBox { Text = "Phụ trội", Left = 735, Top = 30, Width = 90 };
            grpThem.Controls.Add(chkPhuTroi);
            grpThem.Controls.Add(UiHelper.Lbl("Ghi chú:", 15, 70));
            txtGhiChu = UiHelper.Txt(90, 70, 620);
            btnThem = UiHelper.Btn("Thêm dòng", 730, 66, 100);
            btnXoa = UiHelper.Btn("Xóa dòng", 840, 66, 100);
            grpThem.Controls.AddRange(new Control[] { cboNV, dtNgay, cboCa, chkPhuTroi, txtGhiChu, btnThem, btnXoa });
            Controls.Add(grpThem);

            Controls.Add(UiHelper.Lbl("Người lập:", 20, 505));
            txtNguoiLap = UiHelper.Txt(100, 505, 250);
            txtNguoiLap.Text = "Trưởng phòng";
            btnLuu = UiHelper.Btn("Lưu kế hoạch", 370, 501, 140);
            btnDuyet = UiHelper.Btn("Duyệt kế hoạch", 520, 501, 140);
            var btnLamMoi = UiHelper.Btn("Làm mới", 670, 501, 100);
            lblTrangThai = new Label { Left = 780, Top = 508, Width = 200, Height = 20, ForeColor = Color.DarkGreen };
            Controls.AddRange(new Control[] { txtNguoiLap, btnLuu, btnDuyet, btnLamMoi, lblTrangThai });

            Controls.Add(new Label
            {
                Text = "Danh sách kế hoạch (nhấp đúp để tải):", Left = 20, Top = 545,
                Width = 300, ForeColor = Color.DimGray
            });
            dgvKeHoach = UiHelper.Grid(20, 568, 960, 195);
            Controls.Add(dgvKeHoach);

            btnTai.Click += (s, e) => LoadKeHoach();
            btnThem.Click += (s, e) => ThemDong();
            btnXoa.Click += (s, e) => XoaDong();
            btnLuu.Click += (s, e) => Luu();
            btnDuyet.Click += (s, e) => Duyet();
            btnLamMoi.Click += (s, e) => { _maKH = 0; _chiTiet.Rows.Clear(); LoadKeHoach(); };
            cboPB.SelectedIndexChanged += (s, e) => { if (!_dangTai) LoadNhanVien(); };
            dgvKeHoach.CellDoubleClick += (s, e) => ChonKeHoach();
            dgvKeHoach.CellClick += (s, e) => ChonKeHoach();
            Load += (s, e) =>
            {
                _dangTai = true;
                UiHelper.RefreshCombo(cboPB, _td.DanhSachPhongBan(), "MaPB", "TenPB", 3);
                _dangTai = false;
                TaoBang();
                LoadNhanVien();
                LoadKeHoach();
            };
        }

        private void TaoBang()
        {
            _chiTiet = new DataTable();
            _chiTiet.Columns.Add("MaCTKH", typeof(int));
            _chiTiet.Columns.Add("MaNV", typeof(int));
            _chiTiet.Columns.Add("MaNVCode", typeof(string));
            _chiTiet.Columns.Add("HoTen", typeof(string));
            _chiTiet.Columns.Add("Ngay", typeof(DateTime));
            _chiTiet.Columns.Add("CaLam", typeof(string));
            _chiTiet.Columns.Add("LaPhuTroi", typeof(bool));
            _chiTiet.Columns.Add("GhiChu", typeof(string));
            dgvChiTiet.DataSource = _chiTiet;
            if (dgvChiTiet.Columns["Ngay"] != null) dgvChiTiet.Columns["Ngay"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (dgvChiTiet.Columns["MaCTKH"] != null) dgvChiTiet.Columns["MaCTKH"].Visible = false;
            if (dgvChiTiet.Columns["MaNV"] != null) dgvChiTiet.Columns["MaNV"].Visible = false;
            if (dgvChiTiet.Columns["MaNVCode"] != null) dgvChiTiet.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgvChiTiet.Columns["HoTen"] != null) dgvChiTiet.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgvChiTiet.Columns["CaLam"] != null) dgvChiTiet.Columns["CaLam"].HeaderText = "Ca";
            if (dgvChiTiet.Columns["LaPhuTroi"] != null) dgvChiTiet.Columns["LaPhuTroi"].HeaderText = "Phụ trội";
            if (dgvChiTiet.Columns["GhiChu"] != null) dgvChiTiet.Columns["GhiChu"].HeaderText = "Ghi chú";
        }

        private void LoadNhanVien()
        {
            if (cboPB.SelectedValue == null) return;
            DataTable dt = _svc.DanhSachNhanVienTheoPhong(Convert.ToInt32(cboPB.SelectedValue));
            dt.Columns.Add("HienThi", typeof(string));
            foreach (DataRow r in dt.Rows) r["HienThi"] = r["MaNVCode"] + " - " + r["HoTen"];
            UiHelper.RefreshCombo(cboNV, dt, "MaNV", "HienThi");
        }

        private void LoadKeHoach()
        {
            if (cboPB.SelectedValue == null) return;
            int tuan = (int)numTuan.Value, nam = (int)numNam.Value;
            DateTime dau = KeHoachService.NgayDauTuan(tuan, nam);
            DateTime cuoi = dau.AddDays(6);
            lblKhoangTuan.Text = "(" + dau.ToString("dd/MM/yyyy") + " - " + cuoi.ToString("dd/MM/yyyy") + ")";
            dtNgay.Value = dau;

            DataTable h = _svc.LayKeHoachTheoTuan(Convert.ToInt32(cboPB.SelectedValue), tuan, nam);
            _chiTiet.Rows.Clear();
            if (h.Rows.Count == 0)
            {
                _maKH = 0;
                lblTrangThai.Text = "Chưa có kế hoạch - nhập phân công rồi lưu.";
                lblTrangThai.ForeColor = Color.DarkOrange;
                BatTatSoan(true);
            }
            else
            {
                _maKH = Convert.ToInt32(h.Rows[0]["MaKH"]);
                string tt = Convert.ToString(h.Rows[0]["TrangThai"]);
                lblTrangThai.Text = "Kế hoạch #" + _maKH + " - " + (tt == "DaDuyet" ? "Đã duyệt" : "Bản nháp");
                lblTrangThai.ForeColor = tt == "DaDuyet" ? Color.DarkGreen : Color.DarkOrange;
                txtNguoiLap.Text = Convert.ToString(h.Rows[0]["NguoiLap"]);
                DataTable ct = _svc.LayChiTiet(_maKH);
                foreach (DataRow r in ct.Rows)
                    _chiTiet.Rows.Add(r["MaCTKH"], r["MaNV"], r["MaNVCode"], r["HoTen"],
                        Convert.ToDateTime(r["Ngay"]), Convert.ToString(r["CaLam"]),
                        Convert.ToBoolean(r["LaPhuTroi"]), r["GhiChu"]);
                BatTatSoan(tt != "DaDuyet");
            }
            LoadDanhSach();
        }

        private void BatTatSoan(bool choPhep)
        {
            cboNV.Enabled = choPhep; dtNgay.Enabled = choPhep; cboCa.Enabled = choPhep;
            chkPhuTroi.Enabled = choPhep; txtGhiChu.Enabled = choPhep;
            btnThem.Enabled = choPhep; btnXoa.Enabled = choPhep;
            btnLuu.Enabled = choPhep;
            btnDuyet.Enabled = choPhep && _maKH > 0;
        }

        private void ThemDong()
        {
            if (cboNV.SelectedValue == null) return;
            DateTime ngay = dtNgay.Value.Date;
            DateTime dau = KeHoachService.NgayDauTuan((int)numTuan.Value, (int)numNam.Value);
            if (ngay < dau.Date || ngay > dau.AddDays(6).Date)
            {
                MessageBox.Show("Ngày phải nằm trong tuần " + numTuan.Value + "/" + numNam.Value
                    + " (" + dau.ToString("dd/MM/yyyy") + " - " + dau.AddDays(6).ToString("dd/MM/yyyy") + ").",
                    "Sai ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int maNV = Convert.ToInt32(cboNV.SelectedValue);
            foreach (DataRow r in _chiTiet.Rows)
                if (Convert.ToInt32(r["MaNV"]) == maNV && Convert.ToDateTime(r["Ngay"]).Date == ngay)
                {
                    MessageBox.Show("Nhân viên đã được phân công trong ngày này.", "Trùng phân công");
                    return;
                }
            var rowNV = ((DataRowView)cboNV.SelectedItem).Row;
            _chiTiet.Rows.Add(0, maNV, rowNV["MaNVCode"], rowNV["HoTen"], ngay,
                Convert.ToString(cboCa.SelectedItem), chkPhuTroi.Checked, txtGhiChu.Text.Trim());
            txtGhiChu.Clear();
        }

        private void XoaDong()
        {
            if (dgvChiTiet.CurrentRow != null && dgvChiTiet.CurrentRow.Index >= 0)
                _chiTiet.Rows.RemoveAt(dgvChiTiet.CurrentRow.Index);
        }

        private void Luu()
        {
            if (cboPB.SelectedValue == null) return;
            var x = new KeHoachInput
            {
                MaPB = Convert.ToInt32(cboPB.SelectedValue),
                TuanSo = (int)numTuan.Value,
                Nam = (int)numNam.Value,
                NguoiLap = txtNguoiLap.Text.Trim()
            };
            foreach (DataRow r in _chiTiet.Rows)
                x.ChiTiet.Add(new ChiTietKeHoachInput
                {
                    MaNV = Convert.ToInt32(r["MaNV"]),
                    Ngay = Convert.ToDateTime(r["Ngay"]),
                    CaLam = Convert.ToString(r["CaLam"]),
                    LaPhuTroi = Convert.ToBoolean(r["LaPhuTroi"]),
                    GhiChu = Convert.ToString(r["GhiChu"])
                });
            ServiceResult kq = _svc.Luu(x);
            lblTrangThai.Text = kq.Message;
            lblTrangThai.ForeColor = kq.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(kq.Message, kq.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, kq.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (kq.Success) { _maKH = kq.Id; LoadKeHoach(); }
        }

        private void Duyet()
        {
            if (_maKH == 0) { MessageBox.Show("Hãy lưu kế hoạch trước khi duyệt."); return; }
            ServiceResult r = _svc.Duyet(_maKH);
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) LoadKeHoach();
        }

        private void LoadDanhSach()
        {
            dgvKeHoach.DataSource = _svc.DanhSachKeHoach();
            if (dgvKeHoach.Columns["MaKH"] != null) dgvKeHoach.Columns["MaKH"].HeaderText = "Mã KH";
            if (dgvKeHoach.Columns["TenPB"] != null) dgvKeHoach.Columns["TenPB"].HeaderText = "Phòng ban";
            if (dgvKeHoach.Columns["SoPhanCong"] != null) dgvKeHoach.Columns["SoPhanCong"].HeaderText = "Số phân công";
        }

        private void ChonKeHoach()
        {
            if (dgvKeHoach.CurrentRow == null) return;
            DataGridViewRow r = dgvKeHoach.CurrentRow;
            if (r.Cells["MaKH"].Value == null) return;
            _dangTai = true;
            try
            {
                cboPB.SelectedValue = Convert.ToInt32(r.Cells["MaPB"].Value);
                numTuan.Value = Convert.ToDecimal(r.Cells["TuanSo"].Value);
                numNam.Value = Convert.ToDecimal(r.Cells["Nam"].Value);
                LoadNhanVien();
                LoadKeHoach();
            }
            finally { _dangTai = false; }
        }
    }
}
