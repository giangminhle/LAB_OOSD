using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmKetThucKhaoSat : Form
    {
        private readonly KetThucService svc = new KetThucService();
        DataGridView dgvDoan, dgvKS;
        TextBox txtSoTT, txtSoDK, txtGhiChu, txtMaKS, txtKSChon, txtGopY;
        DateTimePicker dtTT, dtGui, dtPH;
        NumericUpDown numTien, numDiem;
        ComboBox cboLoaiKS, cboDangKy;
        public FrmKetThucKhaoSat()
        {
            Text = "Kết thúc tour - thanh toán đoàn - khảo sát"; Width = 1200; Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var tab = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tab);
            tab.TabPages.Add(MakeTT());
            tab.TabPages.Add(MakeKS());
            var pnB = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var bDong = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            bDong.Click += (s, e) => Close();
            pnB.Controls.Add(bDong);
            pnB.Resize += (s, e) => bDong.Left = pnB.Width - 180;
            bDong.Left = 1020; bDong.Top = 8;
            Controls.Add(pnB);
            Load += (s, e) => { cboLoaiKS.Items.AddRange(new object[] { QuyDinh.Le, QuyDinh.Doan }); cboLoaiKS.SelectedIndex = 0; TaiThanhToan(); TaiKhaoSat(); };
        }
        private TabPage MakeTT()
        {
            var p = new TabPage("Thanh toán sau tour (đoàn)");
            dgvDoan = new DataGridView { Dock = DockStyle.Top, Height = 300, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvDoan.SelectionChanged += (s, e) => {
                txtSoDK.Text = FormHelper.O(dgvDoan, "SoDKDoan");
                string con = FormHelper.O(dgvDoan, "ConLai");
                if (con != "") { decimal v; if (decimal.TryParse(con, out v)) numTien.Value = Math.Max(0, Math.Min(numTien.Maximum, v)); }
            };
            var pn = new Panel { Dock = DockStyle.Fill };
            pn.Controls.Add(new Label { Text = "Số TT:", Left = 20, Top = 16, Width = 80 });
            txtSoTT = new TextBox { Left = 120, Top = 12, Width = 150 }; pn.Controls.Add(txtSoTT);
            pn.Controls.Add(new Label { Text = "Phiếu đoàn:", Left = 290, Top = 16, Width = 90 });
            txtSoDK = new TextBox { Left = 390, Top = 12, Width = 130, ReadOnly = true }; pn.Controls.Add(txtSoDK);
            pn.Controls.Add(new Label { Text = "Ngày TT:", Left = 540, Top = 16, Width = 80 });
            dtTT = new DateTimePicker { Left = 630, Top = 12, Width = 160, Format = DateTimePickerFormat.Short }; pn.Controls.Add(dtTT);
            pn.Controls.Add(new Label { Text = "Số tiền:", Left = 20, Top = 52, Width = 80 });
            numTien = new NumericUpDown { Left = 120, Top = 48, Width = 200, Maximum = 1000000000, Increment = 500000, ThousandsSeparator = true }; pn.Controls.Add(numTien);
            pn.Controls.Add(new Label { Text = "Ghi chú:", Left = 340, Top = 52, Width = 80 });
            txtGhiChu = new TextBox { Left = 430, Top = 48, Width = 420 }; pn.Controls.Add(txtGhiChu);
            var b = new Button { Text = "Ghi nhận thanh toán", Left = 880, Top = 46, Width = 220, Height = 38 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThanhToanDoan(txtSoTT.Text, txtSoDK.Text, dtTT.Value, numTien.Value, txtGhiChu.Text))) TaiThanhToan(); };
            pn.Controls.Add(b);
            p.Controls.Add(pn); p.Controls.Add(dgvDoan);
            return p;
        }
        private TabPage MakeKS()
        {
            var p = new TabPage("Khảo sát khách hàng");
            var top = new Panel { Dock = DockStyle.Top, Height = 100 };
            top.Controls.Add(new Label { Text = "Loại khách:", Left = 20, Top = 16, Width = 90 });
            cboLoaiKS = new ComboBox { Left = 130, Top = 12, Width = 130, DropDownStyle = ComboBoxStyle.DropDownList }; top.Controls.Add(cboLoaiKS);
            cboLoaiKS.SelectedIndexChanged += (s, e) => FormHelper.Nap(cboDangKy, svc.LayDangKyChoKhaoSat(cboLoaiKS.Text), "HienThi", "Ma");
            top.Controls.Add(new Label { Text = "Đăng ký đã kết thúc:", Left = 280, Top = 16, Width = 150 });
            cboDangKy = new ComboBox { Left = 440, Top = 12, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList }; top.Controls.Add(cboDangKy);
            top.Controls.Add(new Label { Text = "Mã KS:", Left = 820, Top = 16, Width = 70 });
            txtMaKS = new TextBox { Left = 900, Top = 12, Width = 140 }; top.Controls.Add(txtMaKS);
            top.Controls.Add(new Label { Text = "Ngày gửi:", Left = 20, Top = 52, Width = 90 });
            dtGui = new DateTimePicker { Left = 130, Top = 48, Width = 160, Format = DateTimePickerFormat.Short }; top.Controls.Add(dtGui);
            var b1 = new Button { Text = "Gửi phiếu khảo sát", Left = 860, Top = 46, Width = 220, Height = 38 };
            b1.Click += (s, e) => { if (FormHelper.Bao(svc.GuiKhaoSat(txtMaKS.Text, cboLoaiKS.Text, FormHelper.Gia(cboDangKy), dtGui.Value))) { TaiKhaoSat(); FormHelper.Nap(cboDangKy, svc.LayDangKyChoKhaoSat(cboLoaiKS.Text), "HienThi", "Ma"); } };
            top.Controls.Add(b1);
            dgvKS = new DataGridView { Dock = DockStyle.Top, Height = 260, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvKS.SelectionChanged += (s, e) => txtKSChon.Text = FormHelper.O(dgvKS, "MaKhaoSat");
            var bot = new Panel { Dock = DockStyle.Fill };
            bot.Controls.Add(new Label { Text = "Phiếu chọn:", Left = 20, Top = 14, Width = 90 });
            txtKSChon = new TextBox { Left = 130, Top = 10, Width = 140, ReadOnly = true }; bot.Controls.Add(txtKSChon);
            bot.Controls.Add(new Label { Text = "Ngày phản hồi:", Left = 290, Top = 14, Width = 110 });
            dtPH = new DateTimePicker { Left = 410, Top = 10, Width = 160, Format = DateTimePickerFormat.Short }; bot.Controls.Add(dtPH);
            bot.Controls.Add(new Label { Text = "Điểm (1-5):", Left = 590, Top = 14, Width = 90 });
            numDiem = new NumericUpDown { Left = 690, Top = 10, Width = 90, Minimum = 1, Maximum = 5 }; bot.Controls.Add(numDiem);
            bot.Controls.Add(new Label { Text = "Góp ý:", Left = 20, Top = 50, Width = 90 });
            txtGopY = new TextBox { Left = 130, Top = 46, Width = 700 }; bot.Controls.Add(txtGopY);
            var b2 = new Button { Text = "Ghi nhận góp ý", Left = 860, Top = 44, Width = 220, Height = 38 };
            b2.Click += (s, e) => { if (FormHelper.Bao(svc.GhiPhanHoi(txtKSChon.Text, dtPH.Value, (int)numDiem.Value, txtGopY.Text))) TaiKhaoSat(); };
            bot.Controls.Add(b2);
            p.Controls.Add(bot); p.Controls.Add(dgvKS); p.Controls.Add(top);
            return p;
        }
        private void TaiThanhToan() { dgvDoan.DataSource = svc.DoanCanThanhToan(); }
        private void TaiKhaoSat() { dgvKS.DataSource = svc.LayKhaoSat(); }
    }
}
