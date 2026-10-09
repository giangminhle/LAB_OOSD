using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmDanhMuc : Form
    {
        private readonly DanhMucService svc = new DanhMucService();
        DataGridView dgvPT, dgvDB, dgvHDV, dgvDTQ;
        TextBox txtPTMa, txtPTTen, txtPTGhiChu;
        TextBox txtDBMa, txtDBTen, txtDBDiaChi, txtDBDT;
        TextBox txtHDVMa, txtHDVTen, txtHDVDT;
        NumericUpDown numLuong;
        TextBox txtDTQMa, txtDTQTen, txtDTQDiaDiem, txtDTQNoiDung, txtDTQYNghia;

        public FrmDanhMuc()
        {
            Text = "Danh mục";
            Width = 1100; Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var btnDong = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnDong.Click += (s, e) => Close();
            var pnBottom = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            pnBottom.Controls.Add(btnDong);
            btnDong.Left = pnBottom.Width - 180; btnDong.Top = 8;
            btnDong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnBottom.Resize += (s, e) => btnDong.Left = pnBottom.Width - 180;
            Controls.Add(pnBottom);
            var tab = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tab);
            tab.BringToFront();
            tab.TabPages.Add(MakePT());
            tab.TabPages.Add(MakeDB());
            tab.TabPages.Add(MakeHDV());
            tab.TabPages.Add(MakeDTQ());
            Load += (s, e) => Tai();
        }
        private TabPage MakePT()
        {
            var p = new TabPage("Phương tiện");
            dgvPT = Grid(360);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvPT);
            var lb1 = new Label { Text = "Mã PT:", Left = 20, Top = 20, Width = 70 };
            txtPTMa = new TextBox { Left = 150, Top = 16, Width = 260 }; 
            var lb2 = new Label { Text = "Tên PT:", Left = 450, Top = 20, Width = 70 };
            txtPTTen = new TextBox { Left = 530, Top = 16, Width = 380 };
            var lb3 = new Label { Text = "Ghi chú:", Left = 20, Top = 58, Width = 70 };
            txtPTGhiChu = new TextBox { Left = 150, Top = 54, Width = 600 };
            var b = new Button { Text = "Thêm", Left = 790, Top = 52, Width = 180, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemPhuongTien(txtPTMa.Text, txtPTTen.Text, txtPTGhiChu.Text))) Tai(); };
            pn.Controls.AddRange(new Control[] { lb1, txtPTMa, lb2, txtPTTen, lb3, txtPTGhiChu, b });
            return p;
        }
        private TabPage MakeDB()
        {
            var p = new TabPage("Điểm bán vé");
            dgvDB = Grid(340);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvDB);
            pn.Controls.Add(new Label { Text = "Mã điểm bán:", Left = 20, Top = 16, Width = 100 });
            txtDBMa = new TextBox { Left = 150, Top = 12, Width = 200 }; pn.Controls.Add(txtDBMa);
            pn.Controls.Add(new Label { Text = "Tên điểm bán:", Left = 380, Top = 16, Width = 100 });
            txtDBTen = new TextBox { Left = 490, Top = 12, Width = 300 }; pn.Controls.Add(txtDBTen);
            pn.Controls.Add(new Label { Text = "Địa chỉ:", Left = 20, Top = 52, Width = 100 });
            txtDBDiaChi = new TextBox { Left = 150, Top = 48, Width = 400 }; pn.Controls.Add(txtDBDiaChi);
            pn.Controls.Add(new Label { Text = "Điện thoại:", Left = 580, Top = 52, Width = 90 });
            txtDBDT = new TextBox { Left = 680, Top = 48, Width = 180 }; pn.Controls.Add(txtDBDT);
            var b = new Button { Text = "Thêm", Left = 790, Top = 82, Width = 180, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemDiemBan(txtDBMa.Text, txtDBTen.Text, txtDBDiaChi.Text, txtDBDT.Text))) Tai(); };
            pn.Controls.Add(b);
            return p;
        }
        private TabPage MakeHDV()
        {
            var p = new TabPage("Hướng dẫn viên");
            dgvHDV = Grid(340);
            var pn = new Panel { Dock = DockStyle.Fill };
            p.Controls.Add(pn); p.Controls.Add(dgvHDV);
            pn.Controls.Add(new Label { Text = "Mã HDV:", Left = 20, Top = 16, Width = 100 });
            txtHDVMa = new TextBox { Left = 150, Top = 12, Width = 180 }; pn.Controls.Add(txtHDVMa);
            pn.Controls.Add(new Label { Text = "Họ tên:", Left = 360, Top = 16, Width = 70 });
            txtHDVTen = new TextBox { Left = 440, Top = 12, Width = 260 }; pn.Controls.Add(txtHDVTen);
            pn.Controls.Add(new Label { Text = "Điện thoại:", Left = 20, Top = 52, Width = 100 });
            txtHDVDT = new TextBox { Left = 150, Top = 48, Width = 180 }; pn.Controls.Add(txtHDVDT);
            pn.Controls.Add(new Label { Text = "Lương căn bản:", Left = 360, Top = 52, Width = 110 });
            numLuong = new NumericUpDown { Left = 480, Top = 48, Width = 220, Maximum = 100000000, Increment = 500000, ThousandsSeparator = true }; pn.Controls.Add(numLuong);
            var b = new Button { Text = "Thêm", Left = 790, Top = 46, Width = 180, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemHDV(txtHDVMa.Text, txtHDVTen.Text, txtHDVDT.Text, numLuong.Value))) Tai(); };
            pn.Controls.Add(b);
            return p;
        }
        private TabPage MakeDTQ()
        {
            var p = new TabPage("Điểm tham quan");
            dgvDTQ = Grid(300);
            var pn = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            p.Controls.Add(pn); p.Controls.Add(dgvDTQ);
            pn.Controls.Add(new Label { Text = "Mã điểm TQ:", Left = 20, Top = 14, Width = 100 });
            txtDTQMa = new TextBox { Left = 150, Top = 10, Width = 180 }; pn.Controls.Add(txtDTQMa);
            pn.Controls.Add(new Label { Text = "Tên điểm TQ:", Left = 360, Top = 14, Width = 100 });
            txtDTQTen = new TextBox { Left = 470, Top = 10, Width = 320 }; pn.Controls.Add(txtDTQTen);
            pn.Controls.Add(new Label { Text = "Địa điểm:", Left = 20, Top = 50, Width = 100 });
            txtDTQDiaDiem = new TextBox { Left = 150, Top = 46, Width = 640 }; pn.Controls.Add(txtDTQDiaDiem);
            pn.Controls.Add(new Label { Text = "Nội dung:", Left = 20, Top = 86, Width = 100 });
            txtDTQNoiDung = new TextBox { Left = 150, Top = 82, Width = 640 }; pn.Controls.Add(txtDTQNoiDung);
            pn.Controls.Add(new Label { Text = "Ý nghĩa:", Left = 20, Top = 122, Width = 100 });
            txtDTQYNghia = new TextBox { Left = 150, Top = 118, Width = 640 }; pn.Controls.Add(txtDTQYNghia);
            var b = new Button { Text = "Thêm", Left = 810, Top = 116, Width = 160, Height = 36 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.ThemDiemThamQuan(txtDTQMa.Text, txtDTQTen.Text, txtDTQDiaDiem.Text, txtDTQNoiDung.Text, txtDTQYNghia.Text))) Tai(); };
            pn.Controls.Add(b);
            return p;
        }
        private DataGridView Grid(int h)
        {
            return new DataGridView { Dock = DockStyle.Top, Height = h, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        }
        private void Tai()
        {
            dgvPT.DataSource = svc.LayPhuongTien();
            dgvDB.DataSource = svc.LayDiemBan();
            dgvHDV.DataSource = svc.LayHDV();
            dgvDTQ.DataSource = svc.LayDiemThamQuan();
        }
    }
}
