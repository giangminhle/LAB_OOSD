using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmDangKyDoan : Form
    {
        private readonly DangKyDoanService svc = new DangKyDoanService();
        private readonly BindingList<ThanhVienDoanItem> thanhVien = new BindingList<ThanhVienDoanItem>();
        TextBox txtMaDoan, txtTenCQ, txtDiaChi, txtDT, txtDaiDien, txtSo, txtDon;
        ComboBox cboTour;
        DateTimePicker dtDi;
        NumericUpDown numNguoi, numCoc;
        CheckBox chkBH;
        Label lblKetThuc, lblTong;
        DataGridView dgvThanhVien, dgv;
        public FrmDangKyDoan()
        {
            Text = "Phiếu đăng ký theo đoàn"; Width = 1240; Height = 760;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var pnTop = new Panel { Dock = DockStyle.Top, Height = 210 };
            Controls.Add(pnTop);
            var grpDoan = new GroupBox { Text = "Thông tin đoàn khách", Left = 12, Top = 6, Width = 580, Height = 198 };
            pnTop.Controls.Add(grpDoan);
            grpDoan.Controls.Add(new Label { Text = "Mã đoàn:", Left = 12, Top = 28, Width = 130 });
            txtMaDoan = new TextBox { Left = 160, Top = 24, Width = 200 }; grpDoan.Controls.Add(txtMaDoan);
            grpDoan.Controls.Add(new Label { Text = "Cơ quan / gia đình:", Left = 12, Top = 62, Width = 140 });
            txtTenCQ = new TextBox { Left = 160, Top = 58, Width = 400 }; grpDoan.Controls.Add(txtTenCQ);
            grpDoan.Controls.Add(new Label { Text = "Địa chỉ:", Left = 12, Top = 96, Width = 130 });
            txtDiaChi = new TextBox { Left = 160, Top = 92, Width = 400 }; grpDoan.Controls.Add(txtDiaChi);
            grpDoan.Controls.Add(new Label { Text = "Điện thoại:", Left = 12, Top = 130, Width = 130 });
            txtDT = new TextBox { Left = 160, Top = 126, Width = 200 }; grpDoan.Controls.Add(txtDT);
            grpDoan.Controls.Add(new Label { Text = "Người đại diện:", Left = 12, Top = 162, Width = 130 });
            txtDaiDien = new TextBox { Left = 160, Top = 158, Width = 400 }; grpDoan.Controls.Add(txtDaiDien);
            var grpTour = new GroupBox { Text = "Đăng ký tour", Left = 600, Top = 6, Width = 610, Height = 198 };
            pnTop.Controls.Add(grpTour);
            grpTour.Controls.Add(new Label { Text = "Số phiếu:", Left = 12, Top = 28, Width = 100 });
            txtSo = new TextBox { Left = 130, Top = 24, Width = 170 }; grpTour.Controls.Add(txtSo);
            grpTour.Controls.Add(new Label { Text = "Tour:", Left = 310, Top = 28, Width = 50 });
            cboTour = new ComboBox { Left = 360, Top = 24, Width = 235, DropDownStyle = ComboBoxStyle.DropDownList }; grpTour.Controls.Add(cboTour);
            cboTour.SelectedIndexChanged += (s, e) => TinhTong();
            grpTour.Controls.Add(new Label { Text = "Ngày đi:", Left = 12, Top = 62, Width = 100 });
            dtDi = new DateTimePicker { Left = 130, Top = 58, Width = 170, Format = DateTimePickerFormat.Short }; grpTour.Controls.Add(dtDi);
            dtDi.ValueChanged += (s, e) => TinhTong();
            grpTour.Controls.Add(new Label { Text = "Số người:", Left = 310, Top = 62, Width = 80 });
            numNguoi = new NumericUpDown { Left = 400, Top = 58, Width = 120, Minimum = 13, Maximum = 500, Value = 15 }; grpTour.Controls.Add(numNguoi);
            numNguoi.ValueChanged += (s, e) => TinhTong();
            grpTour.Controls.Add(new Label { Text = "Địa điểm đón:", Left = 12, Top = 96, Width = 100 });
            txtDon = new TextBox { Left = 130, Top = 92, Width = 465 }; grpTour.Controls.Add(txtDon);
            grpTour.Controls.Add(new Label { Text = "Tiền cọc:", Left = 12, Top = 130, Width = 100 });
            numCoc = new NumericUpDown { Left = 130, Top = 126, Width = 170, Maximum = 1000000000, Increment = 1000000, ThousandsSeparator = true }; grpTour.Controls.Add(numCoc);
            chkBH = new CheckBox { Text = "Mua bảo hiểm", Left = 320, Top = 128, Width = 140 }; grpTour.Controls.Add(chkBH);
            chkBH.CheckedChanged += (s, e) => dgvThanhVien.Enabled = chkBH.Checked;
            var lbK = new Label { Text = "Kết thúc DK:", Left = 12, Top = 164, Width = 100 };
            grpTour.Controls.Add(lbK);
            lblKetThuc = new Label { Left = 130, Top = 162, Width = 120, Text = "-", Font = new Font("Segoe UI", 9, FontStyle.Bold) }; grpTour.Controls.Add(lblKetThuc);
            grpTour.Controls.Add(new Label { Text = "Tổng dự kiến:", Left = 270, Top = 164, Width = 110, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            lblTong = new Label { Left = 390, Top = 162, Width = 205, Text = "0 đ", ForeColor = Color.DarkRed, Font = new Font("Segoe UI", 10, FontStyle.Bold) }; grpTour.Controls.Add(lblTong);
            var pnMid = new Panel { Dock = DockStyle.Top, Height = 200 };
            Controls.Add(pnMid);
            pnMid.Controls.Add(new Label { Text = "Danh sách người cùng đi (bắt buộc đủ số người khi mua bảo hiểm):", Left = 12, Top = 4, Width = 500 });
            dgvThanhVien = new DataGridView { Left = 12, Top = 26, Width = 1198, Height = 120, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pnMid.Controls.Add(dgvThanhVien);
            var b1 = new Button { Text = "Lập phiếu đăng ký", Left = 780, Top = 152, Width = 210, Height = 40 };
            b1.Click += BtnDangKy_Click; pnMid.Controls.Add(b1);
            var b2 = new Button { Text = "Hủy phiếu (mất cọc)", Left = 1000, Top = 152, Width = 210, Height = 40 };
            b2.Click += BtnHuy_Click; pnMid.Controls.Add(b2);
            var pnBot = new Panel { Dock = DockStyle.Fill };
            Controls.Add(pnBot); pnBot.BringToFront();
            // bottom close
            var pnB = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            var bDong = new Button { Text = "Đóng", Width = 150, Height = 36, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            bDong.Click += (s, e) => Close();
            pnB.Controls.Add(bDong);
            pnB.Resize += (s, e) => bDong.Left = pnB.Width - 170;
            bDong.Left = 1050; bDong.Top = 6;
            Controls.Add(pnB);
            pnBot.Controls.Add(new Label { Text = "Các phiếu đăng ký đoàn:", Left = 12, Top = 4, Width = 250 });
            dgv = new DataGridView { Left = 12, Top = 26, Width = 1198, Height = 200, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            pnBot.Controls.Add(dgv);
            Load += (s, e) => { FormHelper.Nap(cboTour, new TourService().LayTourMoBan(), "HienThi", "MaTour"); dtDi.Value = DateTime.Today.AddDays(14); dgvThanhVien.DataSource = thanhVien; Tai(); TinhTong(); dgvThanhVien.Enabled = chkBH.Checked; };
        }
        private void Tai() { dgv.DataSource = svc.LayDanhSach(); }
        private void TinhTong()
        {
            var r = cboTour.SelectedItem as DataRowView;
            if (r == null) { lblTong.Text = "0 đ"; lblKetThuc.Text = "-"; return; }
            lblTong.Text = (Convert.ToDecimal(r["DonGiaKhach"]) * numNguoi.Value).ToString("N0") + " đ";
            lblKetThuc.Text = dtDi.Value.Date.AddDays(Convert.ToInt32(r["SoNgay"]) - 1).ToString("dd/MM/yyyy");
        }
        private void BtnDangKy_Click(object sender, EventArgs e)
        {
            dgvThanhVien.EndEdit();
            var ds = thanhVien.Where(x => x != null && !string.IsNullOrWhiteSpace(x.HoTen)).ToList();
            var k = svc.DangKy(txtSo.Text, txtMaDoan.Text, txtTenCQ.Text, txtDiaChi.Text, txtDT.Text, txtDaiDien.Text,
                FormHelper.Gia(cboTour), dtDi.Value, (int)numNguoi.Value, txtDon.Text, chkBH.Checked, numCoc.Value, ds);
            if (FormHelper.Bao(k)) { thanhVien.Clear(); Tai(); }
        }
        private void BtnHuy_Click(object sender, EventArgs e)
        {
            string so = FormHelper.O(dgv, "SoDKDoan");
            if (so == "") { MessageBox.Show("Chọn phiếu đăng ký đoàn cần hủy."); return; }
            if (MessageBox.Show("Đoàn không đi sẽ mất tiền cọc. Hủy phiếu " + so + "?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            if (FormHelper.Bao(svc.HuyDangKy(so))) Tai();
        }
    }
}
