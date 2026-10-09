using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCongTyDuLich.Forms
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "Quản lý công ty du lịch Văn Hóa Việt";
            Width = 900; Height = 650;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            var lbl = new Label {
                Text = "CÔNG TY DU LỊCH VĂN HÓA VIỆT",
                Dock = DockStyle.Top, Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 70, 120)
            };
            Controls.Add(lbl);
            var pn = new Panel { Dock = DockStyle.Fill, Padding = new Padding(80, 10, 80, 20) };
            Controls.Add(pn);
            pn.BringToFront();
            string[,] items = {
                { "Danh mục", "Tour - hành trình" },
                { "Lịch chuyến khách lẻ", "Đăng ký khách lẻ" },
                { "Đăng ký theo đoàn", "Phân công hướng dẫn viên" },
                { "Kết thúc tour - khảo sát", "Lương - thống kê" }
            };
            int y = 10;
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 2; c++)
                {
                    var b = new Button { Text = items[r, c], Left = 20 + c * 350, Top = y, Width = 320, Height = 65 };
                    b.Click += Btn_Click;
                    pn.Controls.Add(b);
                }
            // y increment per row
            // place row by row
            foreach (Control ct in pn.Controls)
                ct.Top = 10 + (pn.Controls.IndexOf(ct) / 2) * 85;
            var bThoat = new Button { Text = "Thoát", Width = 320, Height = 60 };
            bThoat.Left = 20 + 175; bThoat.Top = 10 + 4 * 85 + 10;
            bThoat.Click += (s, e) => Close();
            pn.Controls.Add(bThoat);
        }
        private void Mo(Form f) { using (f) f.ShowDialog(this); }
        private void Btn_Click(object sender, EventArgs e)
        {
            var t = ((Button)sender).Text;
            if (t == "Danh mục") Mo(new FrmDanhMuc());
            else if (t == "Tour - hành trình") Mo(new FrmTour());
            else if (t == "Lịch chuyến khách lẻ") Mo(new FrmChuyenLe());
            else if (t == "Đăng ký khách lẻ") Mo(new FrmDangKyLe());
            else if (t == "Đăng ký theo đoàn") Mo(new FrmDangKyDoan());
            else if (t == "Phân công hướng dẫn viên") Mo(new FrmPhanCongHDV());
            else if (t == "Kết thúc tour - khảo sát") Mo(new FrmKetThucKhaoSat());
            else if (t == "Lương - thống kê") Mo(new FrmLuongThongKe());
        }
    }
}
