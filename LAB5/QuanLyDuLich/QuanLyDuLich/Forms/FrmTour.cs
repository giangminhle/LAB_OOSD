using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmTour : Form
    {
        private readonly TourService svc = new TourService();
        private readonly DanhMucService dm = new DanhMucService();
        ComboBox cboTour, cboPT, cboDTQ;
        DataGridView dgvTour, dgvDiemDung, dgvChang, dgvTQ;
        TextBox txtMa, txtTen, txtMoTa, txtDiemDung, txtGhiChuDD, txtGhiChuPT;
        NumericUpDown numNgay, numDem, numGia, numThuTu, numSao, numChang, numThuTuTQ;
        CheckBox chkDoiPT, chkAn, chkKS;

        public FrmTour()
        {
            Text = "Tour - hành trình"; Width = 1180; Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var pnTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            pnTop.Controls.Add(new Label { Text = "Tour đang chọn (cho các tab hành trình):", Left = 20, Top = 15, Width = 280 });
            cboTour = new ComboBox { Left = 310, Top = 11, Width = 560, DropDownStyle = ComboBoxStyle.DropDownList };
            pnTop.Controls.Add(cboTour);
            cboTour.SelectedIndexChanged += (s, e) => { if (cboTour.ValueMember == "MaTour") TaiChiTiet(); };
            Controls.Add(pnTop);
            AddCloseButton();
            var tab = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tab); tab.BringToFront();
            tab.TabPages.Add(MakeTour());
            tab.TabPages.Add(MakeDD());
            tab.TabPages.Add(MakeChang());
            tab.TabPages.Add(MakeTQ());
            Load += (s, e) => {
                FormHelper.Nap(cboPT, dm.LayPhuongTien(), "TenPT", "MaPT");
                FormHelper.Nap(cboDTQ, dm.LayDiemThamQuan(), "TenDiemTQ", "MaDiemTQ");
                TaiTour();
            };
        }
        private void AddCloseButton()
        {
            var pn = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var b = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            b.Click += (s, e) => Close();
            pn.Controls.Add(b);
            pn.Resize += (s, e) => b.Left = pn.Width - 180;
            b.Left = 990; b.Top = 8;
            Controls.Add(pn);
        }
        private DataGridView Grid(int h) { return new DataGridView { Dock = DockStyle.Top, Height = h, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill }; }
        private TabPage MakeTour()
        {
            var p = new TabPage("Tour");
            dgvTour = Grid(330);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvTour);
            pn.Controls.Add(new Label { Text = "Mã tour:", Left = 20, Top = 16, Width = 70 });
            txtMa = new TextBox { Left = 130, Top = 12, Width = 180 }; pn.Controls.Add(txtMa);
            pn.Controls.Add(new Label { Text = "Tên tour:", Left = 340, Top = 16, Width = 70 });
            txtTen = new TextBox { Left = 420, Top = 12, Width = 480 }; pn.Controls.Add(txtTen);
            pn.Controls.Add(new Label { Text = "Số ngày:", Left = 20, Top = 52, Width = 70 });
            numNgay = new NumericUpDown { Left = 130, Top = 48, Width = 120, Minimum = 1, Maximum = 60, Value = 3 }; pn.Controls.Add(numNgay);
            pn.Controls.Add(new Label { Text = "Số đêm:", Left = 280, Top = 52, Width = 70 });
            numDem = new NumericUpDown { Left = 360, Top = 48, Width = 120, Maximum = 60, Value = 2 }; pn.Controls.Add(numDem);
            pn.Controls.Add(new Label { Text = "Đơn giá / khách:", Left = 510, Top = 52, Width = 110 });
            numGia = new NumericUpDown { Left = 630, Top = 48, Width = 200, Maximum = 1000000000, Increment = 100000, ThousandsSeparator = true }; pn.Controls.Add(numGia);
            pn.Controls.Add(new Label { Text = "Mô tả:", Left = 20, Top = 88, Width = 70 });
            txtMoTa = new TextBox { Left = 130, Top = 84, Width = 700 }; pn.Controls.Add(txtMoTa);
            var b = new Button { Text = "Thêm tour", Left = 860, Top = 82, Width = 200, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemTour(txtMa.Text, txtTen.Text, (int)numNgay.Value, (int)numDem.Value, numGia.Value, txtMoTa.Text))) TaiTour(); };
            pn.Controls.Add(b);
            return p;
        }
        private TabPage MakeDD()
        {
            var p = new TabPage("Điểm dừng");
            dgvDiemDung = Grid(330);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvDiemDung);
            pn.Controls.Add(new Label { Text = "Thứ tự:", Left = 20, Top = 16, Width = 70 });
            numThuTu = new NumericUpDown { Left = 130, Top = 12, Width = 100, Minimum = 1, Maximum = 50 }; pn.Controls.Add(numThuTu);
            pn.Controls.Add(new Label { Text = "Tên điểm dừng:", Left = 260, Top = 16, Width = 110 });
            txtDiemDung = new TextBox { Left = 380, Top = 12, Width = 400 }; pn.Controls.Add(txtDiemDung);
            chkDoiPT = new CheckBox { Text = "Đổi phương tiện", Left = 20, Top = 48 }; pn.Controls.Add(chkDoiPT);
            chkAn = new CheckBox { Text = "Có nơi ăn", Left = 200, Top = 48 }; pn.Controls.Add(chkAn);
            chkKS = new CheckBox { Text = "Có khách sạn", Left = 360, Top = 48 }; pn.Controls.Add(chkKS);
            chkKS.CheckedChanged += (s, e) => numSao.Enabled = chkKS.Checked;
            pn.Controls.Add(new Label { Text = "Hạng sao:", Left = 540, Top = 50, Width = 80 });
            numSao = new NumericUpDown { Left = 630, Top = 46, Width = 100, Minimum = 2, Maximum = 5, Enabled = false }; pn.Controls.Add(numSao);
            pn.Controls.Add(new Label { Text = "Ghi chú:", Left = 20, Top = 86, Width = 70 });
            txtGhiChuDD = new TextBox { Left = 130, Top = 82, Width = 700 }; pn.Controls.Add(txtGhiChuDD);
            var b = new Button { Text = "Thêm điểm dừng", Left = 860, Top = 80, Width = 200, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemDiemDung(FormHelper.Gia(cboTour), (int)numThuTu.Value, txtDiemDung.Text, chkDoiPT.Checked, chkAn.Checked, chkKS.Checked, (int)numSao.Value, txtGhiChuDD.Text))) TaiChiTiet(); };
            pn.Controls.Add(b);
            return p;
        }
        private TabPage MakeChang()
        {
            var p = new TabPage("Phương tiện theo chặng");
            dgvChang = Grid(360);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvChang);
            pn.Controls.Add(new Label { Text = "Chặng thứ:", Left = 20, Top = 16, Width = 90 });
            numChang = new NumericUpDown { Left = 130, Top = 12, Width = 100, Minimum = 1, Maximum = 50 }; pn.Controls.Add(numChang);
            pn.Controls.Add(new Label { Text = "Phương tiện:", Left = 260, Top = 16, Width = 100 });
            cboPT = new ComboBox { Left = 370, Top = 12, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }; pn.Controls.Add(cboPT);
            pn.Controls.Add(new Label { Text = "Ghi chú:", Left = 20, Top = 52, Width = 90 });
            txtGhiChuPT = new TextBox { Left = 130, Top = 48, Width = 700 }; pn.Controls.Add(txtGhiChuPT);
            var b = new Button { Text = "Gắn phương tiện", Left = 860, Top = 46, Width = 200, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemChang(FormHelper.Gia(cboTour), (int)numChang.Value, FormHelper.Gia(cboPT), txtGhiChuPT.Text))) TaiChiTiet(); };
            pn.Controls.Add(b);
            pn.Controls.Add(new Label { Text = "Chặng k là đoạn đi tới điểm dừng thứ k; một chặng có thể dùng nhiều phương tiện.", Left = 20, Top = 84, Width = 700, ForeColor = System.Drawing.Color.Gray });
            return p;
        }
        private TabPage MakeTQ()
        {
            var p = new TabPage("Điểm tham quan");
            dgvTQ = Grid(380);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvTQ);
            pn.Controls.Add(new Label { Text = "Điểm tham quan:", Left = 20, Top = 18, Width = 120 });
            cboDTQ = new ComboBox { Left = 150, Top = 14, Width = 420, DropDownStyle = ComboBoxStyle.DropDownList }; pn.Controls.Add(cboDTQ);
            pn.Controls.Add(new Label { Text = "Thứ tự:", Left = 600, Top = 18, Width = 70 });
            numThuTuTQ = new NumericUpDown { Left = 680, Top = 14, Width = 100, Minimum = 1, Maximum = 50 }; pn.Controls.Add(numThuTuTQ);
            var b = new Button { Text = "Gắn điểm TQ", Left = 860, Top = 12, Width = 200, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemDiemTQTour(FormHelper.Gia(cboTour), FormHelper.Gia(cboDTQ), (int)numThuTuTQ.Value))) TaiChiTiet(); };
            pn.Controls.Add(b);
            return p;
        }
        private void TaiTour() { dgvTour.DataSource = svc.LayTour(); FormHelper.Nap(cboTour, svc.LayTour(), "TenTour", "MaTour"); TaiChiTiet(); }
        private void TaiChiTiet() { string ma = FormHelper.Gia(cboTour); dgvDiemDung.DataSource = svc.LayDiemDung(ma); dgvChang.DataSource = svc.LayChang(ma); dgvTQ.DataSource = svc.LayDiemTQTour(ma); }
    }
}
