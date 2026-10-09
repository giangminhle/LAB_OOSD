using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmPhanCongHDV : Form
    {
        private readonly PhanCongService svc = new PhanCongService();
        TextBox txtMaPC;
        ComboBox cboHDV, cboLoai, cboDoiTuong;
        NumericUpDown numThuLao;
        DataGridView dgv;
        public FrmPhanCongHDV()
        {
            Text = "Phân công hướng dẫn viên"; Width = 1180; Height = 660;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var pnTop = new Panel { Dock = DockStyle.Top, Height = 130 };
            Controls.Add(pnTop);
            pnTop.Controls.Add(new Label { Text = "Mã phân công:", Left = 20, Top = 16, Width = 110 });
            txtMaPC = new TextBox { Left = 150, Top = 12, Width = 220 }; pnTop.Controls.Add(txtMaPC);
            pnTop.Controls.Add(new Label { Text = "Hướng dẫn viên:", Left = 400, Top = 16, Width = 130 });
            cboHDV = new ComboBox { Left = 540, Top = 12, Width = 420, DropDownStyle = ComboBoxStyle.DropDownList }; pnTop.Controls.Add(cboHDV);
            pnTop.Controls.Add(new Label { Text = "Loại:", Left = 20, Top = 52, Width = 110 });
            cboLoai = new ComboBox { Left = 150, Top = 48, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList }; pnTop.Controls.Add(cboLoai);
            cboLoai.SelectedIndexChanged += (s, e) => FormHelper.Nap(cboDoiTuong, svc.LayDoiTuong(cboLoai.Text), "HienThi", "Ma");
            pnTop.Controls.Add(new Label { Text = "Chuyến / đoàn:", Left = 400, Top = 52, Width = 130 });
            cboDoiTuong = new ComboBox { Left = 540, Top = 48, Width = 420, DropDownStyle = ComboBoxStyle.DropDownList }; pnTop.Controls.Add(cboDoiTuong);
            pnTop.Controls.Add(new Label { Text = "Thù lao tour:", Left = 20, Top = 88, Width = 110 });
            numThuLao = new NumericUpDown { Left = 150, Top = 84, Width = 220, Maximum = 100000000, Increment = 100000, ThousandsSeparator = true }; pnTop.Controls.Add(numThuLao);
            pnTop.Controls.Add(new Label { Text = "Ngày bắt đầu / kết thúc lấy theo chuyến hoặc phiếu đoàn.", Left = 400, Top = 88, Width = 420, ForeColor = Color.Gray });
            var b = new Button { Text = "Phân công", Left = 980, Top = 82, Width = 160, Height = 40 };
            b.Click += (s, e) => {
                if (FormHelper.Bao(svc.PhanCong(txtMaPC.Text, FormHelper.Gia(cboHDV), cboLoai.Text, FormHelper.Gia(cboDoiTuong), numThuLao.Value))) {
                    Tai(); FormHelper.Nap(cboDoiTuong, svc.LayDoiTuong(cboLoai.Text), "HienThi", "Ma");
                }
            };
            pnTop.Controls.Add(b);
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv); dgv.BringToFront();
            var pnB = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var bDong = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            bDong.Click += (s, e) => Close();
            pnB.Controls.Add(bDong);
            pnB.Resize += (s, e) => bDong.Left = pnB.Width - 180;
            bDong.Left = 980; bDong.Top = 8;
            Controls.Add(pnB);
            Load += (s, e) => { FormHelper.Nap(cboHDV, svc.LayHDVDangLam(), "HienThi", "MaHDV"); cboLoai.Items.AddRange(new object[] { QuyDinh.Le, QuyDinh.Doan }); cboLoai.SelectedIndex = 0; Tai(); };
        }
        private void Tai() { dgv.DataSource = svc.LayDanhSach(); }
    }
}
