using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmLuongThongKe : Form
    {
        private readonly ThongKeService svc = new ThongKeService();
        NumericUpDown numThang, numNam;
        DataGridView dgvLuong, dgvTongHop;
        DateTimePicker dtTu, dtDen;
        public FrmLuongThongKe()
        {
            Text = "Lương - thống kê"; Width = 1100; Height = 640;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var tab = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tab);
            var p1 = new TabPage("Lương hướng dẫn viên");
            p1.Controls.Add(new Label { Text = "Tháng:", Left = 20, Top = 16, Width = 60 });
            numThang = new NumericUpDown { Left = 90, Top = 12, Width = 80, Minimum = 1, Maximum = 12 }; p1.Controls.Add(numThang);
            p1.Controls.Add(new Label { Text = "Năm:", Left = 190, Top = 16, Width = 50 });
            numNam = new NumericUpDown { Left = 250, Top = 12, Width = 100, Minimum = 2020, Maximum = 2035 }; p1.Controls.Add(numNam);
            var b1 = new Button { Text = "Tính lương", Left = 380, Top = 10, Width = 180, Height = 36 };
            b1.Click += (s, e) => dgvLuong.DataSource = svc.LuongHDV((int)numThang.Value, (int)numNam.Value);
            p1.Controls.Add(b1);
            p1.Controls.Add(new Label { Text = "Lương tháng = lương căn bản + tổng thù lao các tour kết thúc trong tháng.", Left = 590, Top = 16, Width = 450, ForeColor = Color.Gray });
            dgvLuong = new DataGridView { Left = 20, Top = 60, Width = 1030, Height = 460, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            p1.Controls.Add(dgvLuong);
            var p2 = new TabPage("Thống kê tổng hợp");
            p2.Controls.Add(new Label { Text = "Từ ngày:", Left = 20, Top = 16, Width = 70 });
            dtTu = new DateTimePicker { Left = 100, Top = 12, Width = 160, Format = DateTimePickerFormat.Short }; p2.Controls.Add(dtTu);
            p2.Controls.Add(new Label { Text = "Đến ngày:", Left = 280, Top = 16, Width = 80 });
            dtDen = new DateTimePicker { Left = 370, Top = 12, Width = 160, Format = DateTimePickerFormat.Short }; p2.Controls.Add(dtDen);
            var b2 = new Button { Text = "Thống kê", Left = 560, Top = 10, Width = 180, Height = 36 };
            b2.Click += (s, e) => {
                if (dtDen.Value.Date < dtTu.Value.Date) { MessageBox.Show("Đến ngày không được trước từ ngày."); return; }
                dgvTongHop.DataSource = svc.TongHop(dtTu.Value, dtDen.Value);
            };
            p2.Controls.Add(b2);
            dgvTongHop = new DataGridView { Left = 20, Top = 60, Width = 1030, Height = 460, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            p2.Controls.Add(dgvTongHop);
            tab.TabPages.Add(p1); tab.TabPages.Add(p2);
            var pnB = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var bDong = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            bDong.Click += (s, e) => Close();
            pnB.Controls.Add(bDong);
            pnB.Resize += (s, e) => bDong.Left = pnB.Width - 180;
            bDong.Left = 920; bDong.Top = 8;
            Controls.Add(pnB);
            Load += (s, e) => { numThang.Value = DateTime.Today.Month; numNam.Value = DateTime.Today.Year; dtTu.Value = new DateTime(DateTime.Today.Year, 1, 1); dtDen.Value = DateTime.Today; };
        }
    }
}
