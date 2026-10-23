using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmHopDong : Form
    {
        private readonly TuyenDungService _td = new TuyenDungService();
        private readonly HopDongService _svc = new HopDongService();
        private ComboBox cboNV, cboLoaiHD;
        private TextBox txtSoHD, txtChucVu, txtNguoiKy;
        private DateTimePicker dtNgayKy, dtNgayHieuLuc, dtNgayHetHan;
        private CheckBox chkHetHan;
        private NumericUpDown numLuong, numPCLuong, numPCDocHai, numPCCV;
        private Label lblThongTin, lblBaoHiem, lblTrangThai;
        private DataGridView dgv;
        private DateTime? _probationEnd;

        public FrmHopDong()
        {
            Text = "Ký hợp đồng lao động chính thức + hồ sơ bảo hiểm";
            ClientSize = new Size(940, 760);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(UiHelper.Lbl("Nhân viên hết thử việc:", 20, 20, 150));
            cboNV = UiHelper.Cbo(180, 20, 430);
            lblThongTin = new Label { Left = 625, Top = 24, Width = 290, Height = 20, ForeColor = Color.DimGray };
            Controls.Add(cboNV); Controls.Add(lblThongTin);

            int y = 60;
            Controls.Add(UiHelper.Lbl("Số hợp đồng:", 20, y));
            txtSoHD = UiHelper.Txt(180, y, 220);
            txtSoHD.Text = "";
            Controls.Add(new Label { Text = "(để trống = tự sinh)", Left = 410, Top = y + 4, Width = 160, ForeColor = Color.Gray });
            Controls.Add(UiHelper.Lbl("Loại HĐ:", 590, y));
            cboLoaiHD = UiHelper.Cbo(690, y, 200); y += 32;

            Controls.Add(UiHelper.Lbl("Ngày ký:", 20, y));
            dtNgayKy = UiHelper.Date(180, y, 180);
            Controls.Add(UiHelper.Lbl("Ngày hiệu lực:", 390, y));
            dtNgayHieuLuc = UiHelper.Date(510, y, 160);
            Controls.Add(UiHelper.Lbl("Ngày hết hạn:", 690, y));
            dtNgayHetHan = UiHelper.Date(800, y, 100); y += 32;

            chkHetHan = new CheckBox { Text = "Hợp đồng ngắn hạn có ngày hết hạn", Left = 180, Top = y, Width = 300 };
            Controls.Add(chkHetHan); y += 28;

            Controls.Add(UiHelper.Lbl("Mức lương HĐ:", 20, y));
            numLuong = UiHelper.Num(180, y, 1000000000m, 180);
            Controls.Add(UiHelper.Lbl("Phụ cấp lương:", 390, y));
            numPCLuong = UiHelper.Num(510, y, 100000000m, 160);
            Controls.Add(UiHelper.Lbl("Phụ cấp độc hại:", 690, y));
            numPCDocHai = UiHelper.Num(810, y, 100000000m, 90); y += 32;

            Controls.Add(UiHelper.Lbl("Chức vụ:", 20, y));
            txtChucVu = UiHelper.Txt(180, y, 220);
            Controls.Add(UiHelper.Lbl("Phụ cấp chức vụ:", 420, y));
            numPCCV = UiHelper.Num(550, y, 100000000m, 160); y += 32;

            Controls.Add(UiHelper.Lbl("Người ký:", 20, y));
            txtNguoiKy = UiHelper.Txt(180, y, 300);
            txtNguoiKy.Text = "Giám đốc Trần Văn Hùng";
            var btnLuu = UiHelper.Btn("Ký HĐ + lập QĐ + bảo hiểm", 520, y - 2, 240);
            var btnMoi = UiHelper.Btn("Làm mới", 770, y - 2, 110); y += 46;

            lblBaoHiem = new Label
            {
                Text = "Hợp đồng dài hạn: tự động lập BHXH (NV 8% / CTY 17,5%) và BHYT (NV 1,5% / CTY 3%).",
                Left = 20, Top = y, Width = 900, Height = 22, ForeColor = Color.FromArgb(30, 80, 140)
            };
            y += 26;
            lblTrangThai = new Label { Left = 20, Top = y, Width = 900, Height = 22, ForeColor = Color.DarkGreen };
            y += 28;
            dgv = UiHelper.Grid(20, y, 900, 220);

            Controls.AddRange(new Control[] { dtNgayKy, dtNgayHieuLuc, dtNgayHetHan, chkHetHan,
                numLuong, numPCLuong, numPCDocHai, txtChucVu, numPCCV, txtNguoiKy,
                btnLuu, btnMoi, lblBaoHiem, lblTrangThai, dgv });

            cboLoaiHD.Items.AddRange(new object[] { "DaiHan", "NganHan" });
            cboLoaiHD.SelectedIndex = 0;
            cboLoaiHD.SelectedIndexChanged += (s, e) => CapNhatLoaiHD();
            cboNV.SelectedIndexChanged += (s, e) => ChonNhanVien();
            btnLuu.Click += (s, e) => Luu();
            btnMoi.Click += (s, e) => { LoadNhanVien(); LamMoi(); };
            Load += (s, e) => { LoadNhanVien(); CapNhatLoaiHD(); LoadGrid(); };
        }

        private void LoadNhanVien()
        {
            DataTable dt = _td.DanhSachThuViec();
            dt.Columns.Add("HienThi", typeof(string));
            foreach (DataRow r in dt.Rows)
                r["HienThi"] = r["MaNVCode"] + " - " + r["HoTen"] + " (KTTV: "
                    + Convert.ToDateTime(r["NgayKetThucThuViec"]).ToString("dd/MM/yyyy") + ")";
            UiHelper.RefreshCombo(cboNV, dt, "MaNV", "HienThi");
            if (dt.Rows.Count == 0)
            {
                lblThongTin.Text = "Không có nhân viên đang thử việc.";
                lblThongTin.ForeColor = Color.Firebrick;
            }
        }

        private void ChonNhanVien()
        {
            if (cboNV.SelectedItem == null) return;
            var row = ((DataRowView)cboNV.SelectedItem).Row;
            _probationEnd = Convert.ToDateTime(row["NgayKetThucThuViec"]);
            lblThongTin.Text = "KTTV: " + _probationEnd.Value.ToString("dd/MM/yyyy")
                + " | Lương thử việc: " + Convert.ToDecimal(row["MucLuongThuViec"]).ToString("N0") + " đ";
            lblThongTin.ForeColor = Color.DarkGreen;
            numLuong.Value = Convert.ToDecimal(row["MucLuongThuViec"]);
            dtNgayKy.Value = DateTime.Today;
            dtNgayHieuLuc.Value = _probationEnd.Value.AddDays(1);
            dtNgayHetHan.Value = dtNgayHieuLuc.Value.AddMonths(6);
            txtChucVu.Text = "Nhân viên";
            numPCLuong.Value = 0; numPCDocHai.Value = 0; numPCCV.Value = 0;
        }

        private void CapNhatLoaiHD()
        {
            bool daiHan = Convert.ToString(cboLoaiHD.SelectedItem) == HopDongService.DaiHan;
            dtNgayHetHan.Enabled = !daiHan;
            chkHetHan.Checked = !daiHan;
            chkHetHan.Enabled = false;
            if (daiHan) lblBaoHiem.ForeColor = Color.DarkGreen;
            else lblBaoHiem.ForeColor = Color.DarkOrange;
            lblBaoHiem.Text = daiHan
                ? "Hợp đồng dài hạn: tự động lập BHXH (NV 8% / CTY 17,5%) và BHYT (NV 1,5% / CTY 3%)."
                : "Hợp đồng ngắn hạn (tối đa 12 tháng): không lập hồ sơ bảo hiểm, không trừ BHXH/BHYT.";
        }

        private void LamMoi()
        {
            txtSoHD.Clear();
            cboLoaiHD.SelectedIndex = 0;
            txtChucVu.Clear();
            numLuong.Value = 0; numPCLuong.Value = 0; numPCDocHai.Value = 0; numPCCV.Value = 0;
            lblTrangThai.Text = "";
            if (cboNV.Items.Count > 0) cboNV.SelectedIndex = 0;
        }

        private void Luu()
        {
            if (cboNV.SelectedValue == null)
            {
                MessageBox.Show("Không có nhân viên thử việc để ký hợp đồng.", "Thông báo");
                return;
            }
            string loai = Convert.ToString(cboLoaiHD.SelectedItem);
            var x = new HopDongInput
            {
                MaNV = Convert.ToInt32(cboNV.SelectedValue),
                SoHD = txtSoHD.Text.Trim(),
                LoaiHD = loai,
                NgayKy = dtNgayKy.Value.Date,
                NgayHieuLuc = dtNgayHieuLuc.Value.Date,
                NgayHetHan = loai == HopDongService.NganHan ? (DateTime?)dtNgayHetHan.Value.Date : null,
                MucLuong = numLuong.Value,
                PhuCapLuong = numPCLuong.Value,
                PhuCapDocHai = numPCDocHai.Value,
                ChucVu = txtChucVu.Text.Trim(),
                PhuCapChucVu = numPCCV.Value,
                NguoiKy = txtNguoiKy.Text.Trim()
            };
            ServiceResult r = _svc.KyHopDong(x);
            lblTrangThai.Text = r.Message;
            lblTrangThai.ForeColor = r.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success)
            {
                LoadNhanVien();
                LoadGrid();
                LamMoi();
                lblTrangThai.Text = r.Message;
            }
        }

        private void LoadGrid()
        {
            dgv.DataSource = _svc.DanhSachHopDong();
            if (dgv.Columns["MucLuong"] != null) dgv.Columns["MucLuong"].DefaultCellStyle.Format = "N0";
            if (dgv.Columns["HoTen"] != null) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns["MaNVCode"] != null) dgv.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgv.Columns["TenPB"] != null) dgv.Columns["TenPB"].HeaderText = "Phòng ban";
            if (dgv.Columns["LoaiHD"] != null) dgv.Columns["LoaiHD"].HeaderText = "Loại HĐ";
        }
    }
}
