using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmChamCong : Form
    {
        private readonly ChamCongService _svc = new ChamCongService();
        private readonly KeHoachService _kh = new KeHoachService();
        private DateTimePicker dtNgay;
        private DataGridView dgv;
        private TextBox txtVao, txtRa, txtGhiChu;
        private ComboBox cboTrangThai;
        private Label lblNhanVien, lblTrangThai;
        private DataTable _nguon;

        public FrmChamCong()
        {
            Text = "Chấm công theo kế hoạch tuần";
            ClientSize = new Size(1000, 750);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(UiHelper.Lbl("Ngày chấm công:", 20, 20));
            dtNgay = UiHelper.Date(130, 20, 150);
            var btnTai = UiHelper.Btn("Tải phân công", 300, 18, 130);
            var btnTatCa = UiHelper.Btn("Tất cả có mặt (08:00-17:00)", 440, 18, 240);
            lblTrangThai = new Label { Left = 700, Top = 24, Width = 280, Height = 20, ForeColor = Color.DimGray };
            Controls.AddRange(new Control[] { dtNgay, btnTai, btnTatCa, lblTrangThai });

            dgv = UiHelper.Grid(20, 58, 960, 390);
            Controls.Add(dgv);

            var grp = UiHelper.Group("Cập nhật dòng đang chọn", 20, 465, 960, 125);
            lblNhanVien = new Label { Left = 15, Top = 28, Width = 400, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            grp.Controls.Add(lblNhanVien);
            grp.Controls.Add(UiHelper.Lbl("Giờ vào:", 15, 60));
            txtVao = UiHelper.Txt(90, 60, 80);
            grp.Controls.Add(UiHelper.Lbl("Giờ ra:", 190, 60));
            txtRa = UiHelper.Txt(260, 60, 80);
            grp.Controls.Add(UiHelper.Lbl("Trạng thái:", 360, 60));
            cboTrangThai = UiHelper.Cbo(440, 60, 120);
            cboTrangThai.Items.AddRange(new object[] { "DiLam", "DiTre", "Vang", "CoPhep" });
            cboTrangThai.SelectedIndex = 0;
            grp.Controls.Add(UiHelper.Lbl("Ghi chú:", 580, 60));
            txtGhiChu = UiHelper.Txt(650, 60, 210);
            var btnLuu = UiHelper.Btn("Lưu dòng", 800, 22, 130);
            grp.Controls.AddRange(new Control[] { txtVao, txtRa, cboTrangThai, txtGhiChu, btnLuu });
            Controls.Add(grp);

            Controls.Add(new Label
            {
                Text = "Ghi chú: giờ làm = giờ ra - giờ vào - 1 giờ nghỉ trưa nếu ca > 6 giờ. Mỗi nhân viên 1 bản ghi/ngày.",
                Left = 20, Top = 605, Width = 960, ForeColor = Color.DimGray
            });

            btnTai.Click += (s, e) => LoadDuLieu();
            btnLuu.Click += (s, e) => LuuDong();
            btnTatCa.Click += (s, e) => TatCaCoMat();
            dgv.SelectionChanged += (s, e) => ChonDong();
            Load += (s, e) =>
            {
                DateTime? ngay = _kh.NgayKeHoachGanNhat();
                dtNgay.Value = ngay ?? DateTime.Today;
                LoadDuLieu();
            };
        }

        private void LoadDuLieu()
        {
            _nguon = _svc.LayTheoNgay(dtNgay.Value.Date);
            dgv.DataSource = _nguon;
            if (dgv.Columns["MaCTKH"] != null) dgv.Columns["MaCTKH"].Visible = false;
            if (dgv.Columns["MaNV"] != null) dgv.Columns["MaNV"].Visible = false;
            if (dgv.Columns["MaChamCong"] != null) dgv.Columns["MaChamCong"].Visible = false;
            if (dgv.Columns["MaNVCode"] != null) dgv.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgv.Columns["HoTen"] != null) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns["TenPB"] != null) dgv.Columns["TenPB"].HeaderText = "Phòng ban";
            if (dgv.Columns["CaKeHoach"] != null) dgv.Columns["CaKeHoach"].HeaderText = "Ca kế hoạch";
            if (dgv.Columns["LaPhuTroi"] != null) dgv.Columns["LaPhuTroi"].HeaderText = "Phụ trội";
            if (dgv.Columns["TrangThaiChamCong"] != null) dgv.Columns["TrangThaiChamCong"].HeaderText = "Trạng thái";
            lblTrangThai.Text = _nguon.Rows.Count == 0
                ? "Không có phân công đã duyệt cho ngày này."
                : _nguon.Rows.Count + " phân công trong ngày.";
            lblTrangThai.ForeColor = _nguon.Rows.Count == 0 ? Color.Firebrick : Color.DarkGreen;
            txtVao.Clear(); txtRa.Clear(); txtGhiChu.Clear(); lblNhanVien.Text = "";
        }

        private void ChonDong()
        {
            if (dgv.CurrentRow == null) return;
            DataGridViewRow r = dgv.CurrentRow;
            if (r.Cells["MaNV"].Value == null) return;
            lblNhanVien.Text = Convert.ToString(r.Cells["MaNVCode"].Value) + " - "
                + Convert.ToString(r.Cells["HoTen"].Value) + " | Ca: " + Convert.ToString(r.Cells["CaKeHoach"].Value);
            txtVao.Text = r.Cells["GioVao"].Value == null || r.Cells["GioVao"].Value == DBNull.Value
                ? "" : Convert.ToString(r.Cells["GioVao"].Value);
            txtRa.Text = r.Cells["GioRa"].Value == null || r.Cells["GioRa"].Value == DBNull.Value
                ? "" : Convert.ToString(r.Cells["GioRa"].Value);
            string tt = r.Cells["TrangThaiChamCong"].Value == null || r.Cells["TrangThaiChamCong"].Value == DBNull.Value
                ? "DiLam" : Convert.ToString(r.Cells["TrangThaiChamCong"].Value);
            if (cboTrangThai.Items.Contains(tt)) cboTrangThai.SelectedItem = tt;
            txtGhiChu.Text = r.Cells["GhiChu"].Value == null || r.Cells["GhiChu"].Value == DBNull.Value
                ? "" : Convert.ToString(r.Cells["GhiChu"].Value);
        }

        private void LuuDong()
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.Cells["MaNV"].Value == null)
            {
                MessageBox.Show("Hãy chọn một dòng phân công.", "Thông báo");
                return;
            }
            DataGridViewRow row = dgv.CurrentRow;
            var x = new ChamCongInput
            {
                MaNV = Convert.ToInt32(row.Cells["MaNV"].Value),
                Ngay = dtNgay.Value.Date,
                MaCTKH = row.Cells["MaCTKH"].Value == null || row.Cells["MaCTKH"].Value == DBNull.Value
                    ? (int?)null : Convert.ToInt32(row.Cells["MaCTKH"].Value),
                CaLam = Convert.ToString(row.Cells["CaKeHoach"].Value),
                GioVao = ParseTime(txtVao.Text),
                GioRa = ParseTime(txtRa.Text),
                TrangThai = Convert.ToString(cboTrangThai.SelectedItem),
                GhiChu = txtGhiChu.Text.Trim()
            };
            ServiceResult r = _svc.Luu(x);
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) LoadDuLieu();
        }

        private void TatCaCoMat()
        {
            if (_nguon == null || _nguon.Rows.Count == 0)
            {
                MessageBox.Show("Không có phân công để chấm công.", "Thông báo");
                return;
            }
            int ok = 0, loi = 0;
            foreach (DataRow r in _nguon.Rows)
            {
                var x = new ChamCongInput
                {
                    MaNV = Convert.ToInt32(r["MaNV"]),
                    Ngay = dtNgay.Value.Date,
                    MaCTKH = r["MaCTKH"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaCTKH"]),
                    CaLam = Convert.ToString(r["CaKeHoach"]),
                    GioVao = new TimeSpan(8, 0, 0),
                    GioRa = new TimeSpan(17, 0, 0),
                    TrangThai = "DiLam",
                    GhiChu = "Chấm công nhanh"
                };
                ServiceResult kq = _svc.Luu(x);
                if (kq.Success) ok++; else loi++;
            }
            MessageBox.Show("Đã chấm công " + ok + " nhân viên." + (loi > 0 ? " Lỗi: " + loi : ""),
                "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDuLieu();
        }

        private static TimeSpan? ParseTime(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            TimeSpan t;
            if (!TimeSpan.TryParse(s.Trim(), out t)) return null;
            if (t < TimeSpan.Zero || t >= TimeSpan.FromDays(1)) return null;
            return t;
        }
    }
}
