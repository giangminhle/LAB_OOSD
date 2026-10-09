using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmDangKyLe : Form
    {
        private readonly DangKyLeService svc = new DangKyLeService();
        TextBox txtSo, txtTen, txtDT;
        ComboBox cboChuyen, cboDiemBan;
        NumericUpDown numNguoi;
        Label lblThanhTien;
        DataGridView dgv;
        public FrmDangKyLe()
        {
            Text = "Đăng ký khách lẻ theo chuyến"; Width = 1180; Height = 680;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var pnTop = new Panel { Dock = DockStyle.Top, Height = 130 };
            Controls.Add(pnTop);
            pnTop.Controls.Add(new Label { Text = "Số đăng ký:", Left = 20, Top = 16, Width = 90 });
            txtSo = new TextBox { Left = 130, Top = 12, Width = 220 }; pnTop.Controls.Add(txtSo);
            pnTop.Controls.Add(new Label { Text = "Chuyến:", Left = 380, Top = 16, Width = 70 });
            cboChuyen = new ComboBox { Left = 460, Top = 12, Width = 620, DropDownStyle = ComboBoxStyle.DropDownList }; pnTop.Controls.Add(cboChuyen);
            cboChuyen.SelectedIndexChanged += (s, e) => TinhTien();
            pnTop.Controls.Add(new Label { Text = "Điểm bán vé:", Left = 20, Top = 52, Width = 90 });
            cboDiemBan = new ComboBox { Left = 130, Top = 48, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList }; pnTop.Controls.Add(cboDiemBan);
            pnTop.Controls.Add(new Label { Text = "Người đăng ký:", Left = 430, Top = 52, Width = 110 });
            txtTen = new TextBox { Left = 550, Top = 48, Width = 280 }; pnTop.Controls.Add(txtTen);
            pnTop.Controls.Add(new Label { Text = "Điện thoại:", Left = 850, Top = 52, Width = 80 });
            txtDT = new TextBox { Left = 940, Top = 48, Width = 190 }; pnTop.Controls.Add(txtDT);
            pnTop.Controls.Add(new Label { Text = "Số người:", Left = 20, Top = 88, Width = 90 });
            numNguoi = new NumericUpDown { Left = 130, Top = 84, Width = 120, Minimum = 1, Maximum = 11, Value = 2 }; pnTop.Controls.Add(numNguoi);
            numNguoi.ValueChanged += (s, e) => TinhTien();
            pnTop.Controls.Add(new Label { Text = "Thành tiền:", Left = 280, Top = 88, Width = 90, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            lblThanhTien = new Label { Left = 380, Top = 86, Width = 200, Text = "0 đ", ForeColor = Color.DarkRed, Font = new Font("Segoe UI", 11, FontStyle.Bold) }; pnTop.Controls.Add(lblThanhTien);
            var b = new Button { Text = "Đăng ký và thanh toán vé", Left = 830, Top = 82, Width = 300, Height = 40 };
            b.Click += (s, e) => { if (FormHelper.Bao(svc.DangKy(txtSo.Text, FormHelper.Gia(cboChuyen), FormHelper.Gia(cboDiemBan), txtTen.Text, txtDT.Text, (int)numNguoi.Value))) Tai(); };
            pnTop.Controls.Add(b);
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv); dgv.BringToFront();
            var pnB = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var bDong = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            bDong.Click += (s, e) => Close();
            pnB.Controls.Add(bDong);
            pnB.Resize += (s, e) => bDong.Left = pnB.Width - 180;
            bDong.Left = 990; bDong.Top = 8;
            Controls.Add(pnB);
            Load += (s, e) => { FormHelper.Nap(cboChuyen, new ChuyenLeService().LayChuyenMo(), "HienThi", "MaChuyen"); FormHelper.Nap(cboDiemBan, new DanhMucService().LayDiemBan(), "TenDiemBan", "MaDiemBan"); Tai(); TinhTien(); };
        }
        private void Tai() { dgv.DataSource = svc.LayDanhSach(); }
        private void TinhTien()
        {
            var r = cboChuyen.SelectedItem as DataRowView;
            lblThanhTien.Text = r == null ? "0 đ" : (Convert.ToDecimal(r["DonGiaKhach"]) * numNguoi.Value).ToString("N0") + " đ";
        }
    }
}
