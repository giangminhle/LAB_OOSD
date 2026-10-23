using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmKhenThuong : Form
    {
        private readonly KhenThuongService _svc = new KhenThuongService();
        private readonly TuyenDungService _td = new TuyenDungService();
        private ComboBox cboLoai, cboDoiTuong, cboNV, cboPB, cboNguon;
        private DateTimePicker dtNgay;
        private NumericUpDown numTien;
        private TextBox txtHinhThuc, txtLyDo, txtNguoiKy;
        private DataGridView dgv;
        private Label lblMucTieu, lblNguon, lblTrangThai;

        public FrmKhenThuong()
        {
            Text = "Quyết định khen thưởng / kỷ luật - cá nhân hoặc tập thể";
            ClientSize = new Size(1000, 780);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(UiHelper.Lbl("Loại quyết định:", 20, 20));
            cboLoai = UiHelper.Cbo(130, 20, 140);
            cboLoai.Items.AddRange(new object[] { "KhenThuong", "KyLuat" });
            cboLoai.SelectedIndex = 0;
            Controls.Add(UiHelper.Lbl("Đối tượng:", 300, 20));
            cboDoiTuong = UiHelper.Cbo(385, 20, 120);
            cboDoiTuong.Items.AddRange(new object[] { "CaNhan", "TapThe" });
            cboDoiTuong.SelectedIndex = 0;
            Controls.Add(UiHelper.Lbl("Ngày ban hành:", 535, 20));
            dtNgay = UiHelper.Date(645, 20, 140);

            int y = 60;
            lblMucTieu = new Label { Left = 20, Top = y + 4, Width = 100, Height = 20, Text = "Nhân viên:" };
            Controls.Add(lblMucTieu);
            cboNV = UiHelper.Cbo(130, y, 300);
            Controls.Add(cboNV);

            Controls.Add(UiHelper.Lbl("Phòng ban:", 450, y));
            cboPB = UiHelper.Cbo(540, y, 260);
            Controls.Add(cboPB); y += 36;

            Controls.Add(UiHelper.Lbl("Số tiền:", 20, y));
            numTien = UiHelper.Num(130, y, 1000000000m, 200);
            numTien.Value = 1000000m;
            Controls.Add(UiHelper.Lbl("Hình thức:", 360, y));
            txtHinhThuc = UiHelper.Txt(440, y, 260);
            txtHinhThuc.Text = "Thưởng hiệu quả công việc"; y += 36;

            Controls.Add(UiHelper.Lbl("Lý do:", 20, y));
            txtLyDo = new TextBox { Left = 130, Top = y, Width = 570, Height = 48, Multiline = true };
            Controls.Add(txtLyDo); y += 56;

            lblNguon = new Label { Left = 20, Top = y + 4, Width = 100, Height = 20, Text = "Nguồn chi:" };
            Controls.Add(lblNguon);
            cboNguon = UiHelper.Cbo(130, y, 180);
            Controls.Add(cboNguon);
            Controls.Add(UiHelper.Lbl("Người ký:", 340, y));
            txtNguoiKy = UiHelper.Txt(420, y, 280);
            txtNguoiKy.Text = "Giám đốc Trần Văn Hùng";
            var btnLuu = UiHelper.Btn("Lập quyết định", 730, y - 2, 160); y += 44;

            lblTrangThai = new Label { Left = 20, Top = y, Width = 960, Height = 24, ForeColor = Color.DarkGreen }; y += 28;
            Controls.AddRange(new Control[] { cboLoai, cboDoiTuong, dtNgay, cboNV, cboPB, numTien,
                txtHinhThuc, txtLyDo, cboNguon, txtNguoiKy, btnLuu, lblTrangThai });

            Controls.Add(new Label
            {
                Text = "Lịch sử quyết định (khen thưởng hoặc kỷ luật theo loại đang chọn):",
                Left = 20, Top = y, Width = 500, ForeColor = Color.DimGray
            });
            dgv = UiHelper.Grid(20, y + 22, 960, 330);
            Controls.Add(dgv);

            cboLoai.SelectedIndexChanged += (s, e) => { CapNhatNguon(); LoadData(); };
            cboDoiTuong.SelectedIndexChanged += (s, e) => CapNhatMucTieu();
            btnLuu.Click += (s, e) => Luu();
            Load += (s, e) =>
            {
                LoadNhanVien();
                LoadPhongBan();
                CapNhatNguon();
                CapNhatMucTieu();
                LoadData();
            };
        }

        private void LoadNhanVien()
        {
            DataTable dt = _td.DanhSachNhanVien();
            dt.Columns.Add("HienThi", typeof(string));
            foreach (DataRow r in dt.Rows)
                r["HienThi"] = r["MaNVCode"] + " - " + r["HoTen"] + " (" + r["TrangThai"] + ")";
            UiHelper.RefreshCombo(cboNV, dt, "MaNV", "HienThi");
        }

        private void LoadPhongBan()
        {
            UiHelper.RefreshCombo(cboPB, _td.DanhSachPhongBan(), "MaPB", "TenPB");
        }

        private void CapNhatMucTieu()
        {
            bool caNhan = Convert.ToString(cboDoiTuong.SelectedItem) == "CaNhan";
            cboNV.Enabled = caNhan;
            cboPB.Enabled = !caNhan;
            lblMucTieu.Text = caNhan ? "Nhân viên:" : "Phòng ban:";
        }

        private void CapNhatNguon()
        {
            bool khen = Convert.ToString(cboLoai.SelectedItem) == "KhenThuong";
            cboNguon.Items.Clear();
            if (khen)
            {
                cboNguon.Items.AddRange(new object[] { "PhieuChi", "Luong" });
                lblNguon.Text = "Nguồn chi:";
                txtHinhThuc.Text = "Thưởng hiệu quả công việc";
                lblTrangThai.Text = "PhieuChi: tạo phiếu chi ngay. Luong: cộng vào bảng lương tháng.";
            }
            else
            {
                cboNguon.Items.AddRange(new object[] { "PhieuThu", "TruLuong" });
                lblNguon.Text = "Nguồn thu:";
                txtHinhThuc.Text = "Phạt tiền";
                lblTrangThai.Text = "PhieuThu: tạo phiếu thu ngay. TruLuong: trừ vào bảng lương tháng.";
            }
            cboNguon.SelectedIndex = 0;
            lblTrangThai.ForeColor = Color.DimGray;
            numTien.Value = khen ? 1000000m : 500000m;
        }

        private void Luu()
        {
            bool khen = Convert.ToString(cboLoai.SelectedItem) == "KhenThuong";
            bool caNhan = Convert.ToString(cboDoiTuong.SelectedItem) == "CaNhan";
            int? maNV = caNhan && cboNV.SelectedValue != null ? (int?)Convert.ToInt32(cboNV.SelectedValue) : null;
            int? maPB = !caNhan && cboPB.SelectedValue != null ? (int?)Convert.ToInt32(cboPB.SelectedValue) : null;
            ServiceResult r;

            if (khen)
            {
                r = _svc.LapKhenThuong(new KhenThuongInput
                {
                    NgayBanHanh = dtNgay.Value.Date,
                    DoiTuong = Convert.ToString(cboDoiTuong.SelectedItem),
                    MaNV = maNV, MaPB = maPB,
                    LyDo = txtLyDo.Text.Trim(),
                    HinhThuc = txtHinhThuc.Text.Trim(),
                    SoTien = numTien.Value,
                    NguonChiTra = Convert.ToString(cboNguon.SelectedItem),
                    NguoiKy = txtNguoiKy.Text.Trim()
                });
            }
            else
            {
                r = _svc.LapKyLuat(new KyLuatInput
                {
                    NgayBanHanh = dtNgay.Value.Date,
                    DoiTuong = Convert.ToString(cboDoiTuong.SelectedItem),
                    MaNV = maNV, MaPB = maPB,
                    LyDo = txtLyDo.Text.Trim(),
                    HinhThuc = txtHinhThuc.Text.Trim(),
                    SoTienPhat = numTien.Value,
                    NguonThu = Convert.ToString(cboNguon.SelectedItem),
                    NguoiKy = txtNguoiKy.Text.Trim()
                });
            }
            lblTrangThai.Text = r.Message;
            lblTrangThai.ForeColor = r.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) LoadData();
        }

        private void LoadData()
        {
            bool khen = Convert.ToString(cboLoai.SelectedItem) == "KhenThuong";
            dgv.DataSource = khen ? _svc.DanhSachKhenThuong() : _svc.DanhSachKyLuat();
            if (dgv.Columns["SoTien"] != null) dgv.Columns["SoTien"].DefaultCellStyle.Format = "N0";
            if (dgv.Columns["SoTienPhat"] != null) dgv.Columns["SoTienPhat"].DefaultCellStyle.Format = "N0";
            if (dgv.Columns["SoQD"] != null) dgv.Columns["SoQD"].HeaderText = "Số QĐ";
            if (dgv.Columns["NgayBanHanh"] != null) dgv.Columns["NgayBanHanh"].HeaderText = "Ngày";
            if (dgv.Columns["DoiTuong"] != null) dgv.Columns["DoiTuong"].HeaderText = "Đối tượng";
            if (dgv.Columns["MucTieu"] != null) dgv.Columns["MucTieu"].HeaderText = "Cá nhân/Tập thể";
            if (dgv.Columns["LyDo"] != null) dgv.Columns["LyDo"].HeaderText = "Lý do";
            if (dgv.Columns["NguoiKy"] != null) dgv.Columns["NguoiKy"].HeaderText = "Người ký";
        }
    }
}
