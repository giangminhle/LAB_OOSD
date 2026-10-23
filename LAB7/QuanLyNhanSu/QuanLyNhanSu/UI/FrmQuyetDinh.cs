using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmQuyetDinh : Form
    {
        private readonly TuyenDungService _td = new TuyenDungService();
        private readonly QuyetDinhService _svc = new QuyetDinhService();
        private ComboBox cboNV;
        private NumericUpDown numLuong, numPCLuong, numPCDocHai, numPCCV;
        private DateTimePicker dtLuong, dtChucVu;
        private TextBox txtLyDoLuong, txtChucVu, txtLyDoCV, txtKyLuong, txtKyCV;
        private DataGridView dgv;
        private Label lblThongTin, lblTrangThai;

        public FrmQuyetDinh()
        {
            Text = "Quyết định lương - chức vụ (tăng lương / bổ nhiệm)";
            ClientSize = new Size(940, 730);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(UiHelper.Lbl("Nhân viên:", 20, 20));
            cboNV = UiHelper.Cbo(110, 20, 420);
            lblThongTin = new Label { Left = 550, Top = 24, Width = 370, Height = 20, ForeColor = Color.DimGray };

            var tabs = new TabControl { Left = 20, Top = 55, Width = 900, Height = 300 };
            var tabL = new TabPage("Quyết định lương");
            var tabC = new TabPage("Quyết định chức vụ");

            int y = 20;
            tabL.Controls.Add(UiHelper.Lbl("Mức lương cơ bản:", 15, y));
            numLuong = UiHelper.Num(160, y, 1000000000m, 200);
            tabL.Controls.Add(UiHelper.Lbl("Phụ cấp lương:", 390, y));
            numPCLuong = UiHelper.Num(510, y, 100000000m, 160); y += 38;
            tabL.Controls.Add(UiHelper.Lbl("Phụ cấp độc hại:", 15, y));
            numPCDocHai = UiHelper.Num(160, y, 100000000m, 200);
            tabL.Controls.Add(UiHelper.Lbl("Ngày hiệu lực:", 390, y));
            dtLuong = UiHelper.Date(510, y, 160); y += 38;
            tabL.Controls.Add(UiHelper.Lbl("Lý do:", 15, y));
            txtLyDoLuong = UiHelper.Txt(160, y, 510);
            txtLyDoLuong.Text = "Tăng lương / điều chỉnh lương";
            tabL.Controls.Add(UiHelper.Lbl("Người ký:", 15, y + 38));
            txtKyLuong = UiHelper.Txt(160, y + 38, 300);
            txtKyLuong.Text = "Giám đốc Trần Văn Hùng";
            var btnL = UiHelper.Btn("Lập quyết định lương", 490, y + 36, 190);
            tabL.Controls.AddRange(new Control[] { numLuong, numPCLuong, numPCDocHai, dtLuong,
                txtLyDoLuong, txtKyLuong, btnL });

            y = 20;
            tabC.Controls.Add(UiHelper.Lbl("Chức vụ:", 15, y));
            txtChucVu = UiHelper.Txt(160, y, 220);
            tabC.Controls.Add(UiHelper.Lbl("Phụ cấp chức vụ:", 400, y));
            numPCCV = UiHelper.Num(530, y, 100000000m, 160); y += 38;
            tabC.Controls.Add(UiHelper.Lbl("Ngày hiệu lực:", 15, y));
            dtChucVu = UiHelper.Date(160, y, 160);
            tabC.Controls.Add(UiHelper.Lbl("Lý do:", 400, y));
            txtLyDoCV = UiHelper.Txt(480, y, 280);
            txtLyDoCV.Text = "Bổ nhiệm chức vụ"; y += 38;
            tabC.Controls.Add(UiHelper.Lbl("Người ký:", 15, y));
            txtKyCV = UiHelper.Txt(160, y, 300);
            txtKyCV.Text = "Giám đốc Trần Văn Hùng";
            var btnC = UiHelper.Btn("Lập quyết định chức vụ", 480, y - 2, 210);
            tabC.Controls.AddRange(new Control[] { txtChucVu, numPCCV, dtChucVu, txtLyDoCV, txtKyCV, btnC });

            tabs.TabPages.Add(tabL); tabs.TabPages.Add(tabC);

            lblTrangThai = new Label { Left = 20, Top = 365, Width = 900, Height = 24, ForeColor = Color.DarkGreen };
            dgv = UiHelper.Grid(20, 395, 900, 315);

            Controls.AddRange(new Control[] { cboNV, lblThongTin, tabs, lblTrangThai, dgv });

            cboNV.SelectedIndexChanged += (s, e) => ChonNhanVien();
            btnL.Click += (s, e) => LuuLuong();
            btnC.Click += (s, e) => LuuChucVu();
            Load += (s, e) => { LoadNhanVien(); LoadGrid(); };
        }

        private void LoadNhanVien()
        {
            DataTable dt = _td.DanhSachNhanVien("DangLam");
            dt.Columns.Add("HienThi", typeof(string));
            foreach (DataRow r in dt.Rows) r["HienThi"] = r["MaNVCode"] + " - " + r["HoTen"];
            UiHelper.RefreshCombo(cboNV, dt, "MaNV", "HienThi");
            if (dt.Rows.Count > 0) cboNV.SelectedIndex = 0;
        }

        private void ChonNhanVien()
        {
            if (cboNV.SelectedValue == null) return;
            int ma = Convert.ToInt32(cboNV.SelectedValue);
            DataTable l = _svc.LayQuyetDinhLuongGanNhat(ma);
            DataTable c = _svc.LayQuyetDinhChucVuGanNhat(ma);
            string s = "";
            if (l.Rows.Count > 0)
            {
                s += "Lương hiện tại: " + Convert.ToDecimal(l.Rows[0]["MucLuongCoBan"]).ToString("N0")
                    + " đ (từ " + Convert.ToDateTime(l.Rows[0]["NgayHieuLuc"]).ToString("dd/MM/yyyy") + ")";
                numLuong.Value = Math.Min(numLuong.Maximum, Convert.ToDecimal(l.Rows[0]["MucLuongCoBan"]));
                numPCLuong.Value = Convert.ToDecimal(l.Rows[0]["PhuCapLuong"]);
                numPCDocHai.Value = Convert.ToDecimal(l.Rows[0]["PhuCapDocHai"]);
            }
            if (c.Rows.Count > 0)
            {
                s += " | Chức vụ: " + c.Rows[0]["ChucVu"];
                txtChucVu.Text = Convert.ToString(c.Rows[0]["ChucVu"]);
                numPCCV.Value = Convert.ToDecimal(c.Rows[0]["PhuCapChucVu"]);
            }
            lblThongTin.Text = s;
            dtLuong.Value = DateTime.Today;
            dtChucVu.Value = DateTime.Today;
        }

        private void LuuLuong()
        {
            if (cboNV.SelectedValue == null) return;
            ServiceResult r = _svc.LapQuyetDinhLuong(Convert.ToInt32(cboNV.SelectedValue),
                numLuong.Value, numPCLuong.Value, numPCDocHai.Value, dtLuong.Value.Date,
                txtLyDoLuong.Text.Trim(), txtKyLuong.Text.Trim());
            HienThi(r);
            if (r.Success) { ChonNhanVien(); LoadGrid(); }
        }

        private void LuuChucVu()
        {
            if (cboNV.SelectedValue == null) return;
            ServiceResult r = _svc.LapQuyetDinhChucVu(Convert.ToInt32(cboNV.SelectedValue),
                txtChucVu.Text.Trim(), numPCCV.Value, dtChucVu.Value.Date,
                txtLyDoCV.Text.Trim(), txtKyCV.Text.Trim());
            HienThi(r);
            if (r.Success) { ChonNhanVien(); LoadGrid(); }
        }

        private void HienThi(ServiceResult r)
        {
            lblTrangThai.Text = r.Message;
            lblTrangThai.ForeColor = r.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void LoadGrid()
        {
            dgv.DataSource = _svc.DanhSachTatCaQuyetDinh();
            if (dgv.Columns["GiaTriChinh"] != null) dgv.Columns["GiaTriChinh"].DefaultCellStyle.Format = "N0";
            if (dgv.Columns["HoTen"] != null) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns["MaNVCode"] != null) dgv.Columns["MaNVCode"].HeaderText = "Mã NV";
            if (dgv.Columns["LoaiQD"] != null) dgv.Columns["LoaiQD"].HeaderText = "Loại QĐ";
            if (dgv.Columns["NgayHieuLuc"] != null) dgv.Columns["NgayHieuLuc"].HeaderText = "Hiệu lực";
        }
    }
}
