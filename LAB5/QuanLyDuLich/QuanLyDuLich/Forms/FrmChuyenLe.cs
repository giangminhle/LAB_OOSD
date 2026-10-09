using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Services;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmChuyenLe : Form
    {
        private readonly ChuyenLeService svc = new ChuyenLeService();
        private readonly TourService tour = new TourService();
        TextBox txtMa, txtDon;
        ComboBox cboTour;
        DateTimePicker dtDi;
        Label lblNgayVe;
        DataGridView dgv;
        public FrmChuyenLe()
        {
            Text = "Lịch chuyến khách lẻ"; Width = 1180; Height = 680;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            var pnTop = new Panel { Dock = DockStyle.Top, Height = 150 };
            Controls.Add(pnTop);
            pnTop.Controls.Add(new Label { Text = "Mã chuyến:", Left = 20, Top = 16, Width = 90 });
            txtMa = new TextBox { Left = 130, Top = 12, Width = 220 }; pnTop.Controls.Add(txtMa);
            pnTop.Controls.Add(new Label { Text = "Tour:", Left = 380, Top = 16, Width = 50 });
            cboTour = new ComboBox { Left = 440, Top = 12, Width = 500, DropDownStyle = ComboBoxStyle.DropDownList }; pnTop.Controls.Add(cboTour);
            cboTour.SelectedIndexChanged += (s, e) => TinhNgayVe();
            pnTop.Controls.Add(new Label { Text = "Ngày đi:", Left = 20, Top = 54, Width = 90 });
            dtDi = new DateTimePicker { Left = 130, Top = 50, Width = 220, Format = DateTimePickerFormat.Short }; pnTop.Controls.Add(dtDi);
            dtDi.ValueChanged += (s, e) => TinhNgayVe();
            pnTop.Controls.Add(new Label { Text = "Ngày về:", Left = 380, Top = 54, Width = 80 });
            lblNgayVe = new Label { Left = 470, Top = 52, Width = 150, Text = "-", Font = new Font("Segoe UI", 10, FontStyle.Bold) }; pnTop.Controls.Add(lblNgayVe);
            pnTop.Controls.Add(new Label { Text = "Địa điểm đón:", Left = 20, Top = 92, Width = 100 });
            txtDon = new TextBox { Left = 130, Top = 88, Width = 640 }; pnTop.Controls.Add(txtDon);
            var b1 = new Button { Text = "Tạo chuyến", Left = 800, Top = 86, Width = 160, Height = 40 };
            b1.Click += (s, e) => { if (FormHelper.Bao(svc.ThemChuyen(txtMa.Text, FormHelper.Gia(cboTour), dtDi.Value, txtDon.Text))) Tai(); };
            pnTop.Controls.Add(b1);
            var b2 = new Button { Text = "Đóng đăng ký", Left = 970, Top = 86, Width = 160, Height = 40 };
            b2.Click += (s, e) => { if (FormHelper.Bao(svc.DongDangKy(FormHelper.O(dgv, "MaChuyen")))) Tai(); };
            pnTop.Controls.Add(b2);
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv); dgv.BringToFront();
            var pnB = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var bDong = new Button { Text = "Đóng", Width = 160, Height = 38, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            bDong.Click += (s, e) => Close();
            pnB.Controls.Add(bDong);
            pnB.Resize += (s, e) => bDong.Left = pnB.Width - 180;
            bDong.Left = 990; bDong.Top = 8;
            Controls.Add(pnB);
            Load += (s, e) => { FormHelper.Nap(cboTour, tour.LayTourMoBan(), "HienThi", "MaTour"); dtDi.Value = DateTime.Today.AddDays(7); Tai(); TinhNgayVe(); };
        }
        private void Tai() { dgv.DataSource = svc.LayChuyen(); }
        private void TinhNgayVe()
        {
            var r = cboTour.SelectedItem as DataRowView;
            lblNgayVe.Text = r == null ? "-" : dtDi.Value.Date.AddDays(Convert.ToInt32(r["SoNgay"]) - 1).ToString("dd/MM/yyyy");
        }
    }
}
