using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    /// <summary>Khach tham khao vi tri, dien tich, gia thue cac phong.</summary>
    public class FrmPhong : Form
    {
        private readonly PhongService _svc = new PhongService();
        private readonly ComboBox cboLoc;
        private readonly DataGridView grid;
        private readonly NumericUpDown numDienTich, numTang;
        private readonly Label lblGia;

        public FrmPhong()
        {
            Text = "Tầng, phòng và giá thuê";
            ClientSize = new Size(900, 520);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            Controls.Add(UiHelper.Lbl("Lọc trạng thái:", 20, 15));
            cboLoc = UiHelper.Cbo(130, 15, 180);
            cboLoc.Items.AddRange(new object[] { "Tất cả", "Trong", "DangThue", "SuaChua" });
            cboLoc.SelectedIndex = 0;
            cboLoc.SelectedIndexChanged += (s, e) => TaiLai();
            Controls.Add(cboLoc);

            grid = UiHelper.Grid(20, 50, 860, 340);
            Controls.Add(grid);

            var grp = UiHelper.Group("Tính giá thuê tham khảo (giá = diện tích x 250.000 x hệ số tầng)", 20, 400, 860, 100);
            grp.Controls.Add(UiHelper.Lbl("Diện tích (m2):", 15, 30));
            numDienTich = UiHelper.Num(130, 30, 1000, 120);
            numDienTich.Value = 60;
            grp.Controls.Add(numDienTich);
            grp.Controls.Add(UiHelper.Lbl("Tầng:", 280, 30, 50));
            numTang = UiHelper.Num(335, 30, 50, 80);
            numTang.Minimum = 1;
            numTang.Value = 3;
            grp.Controls.Add(numTang);
            var btn = UiHelper.Btn("Tính giá", 440, 26, 120);
            grp.Controls.Add(btn);
            lblGia = new Label { Left = 580, Top = 32, Width = 270, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            grp.Controls.Add(lblGia);
            btn.Click += (s, e) =>
            {
                decimal heSo = PhongService.HeSoTheoTang((int)numTang.Value);
                decimal gia = PhongService.TinhGiaThue(numDienTich.Value, heSo);
                lblGia.Text = "Hệ số " + heSo.ToString("0.00") + " → " + gia.ToString("N0") + " đ/tháng";
            };
            Controls.Add(grp);

            Load += (s, e) => TaiLai();
        }

        private void TaiLai()
        {
            string loc = cboLoc.SelectedIndex <= 0 ? null : cboLoc.Text;
            grid.DataSource = _svc.DanhSachPhong(loc);
            UiHelper.FormatMoneyGrid(grid);
        }
    }
}
