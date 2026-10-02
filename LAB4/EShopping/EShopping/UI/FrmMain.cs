using System;
using System.Drawing;
using System.Windows.Forms;

namespace EShopping.UI
{
    /// <summary>Form menu để mở các chức năng (dùng cho thực hành/demo).</summary>
    public class FrmMain : Form
    {
        private NumericUpDown numMaKH, numTongTien;

        public FrmMain()
        {
            Text = "e-SHOPPING";
            ClientSize = new Size(380, 260);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var btnDangKy = new Button { Text = "1. Đăng ký tài khoản", Left = 30, Top = 20, Width = 320, Height = 40 };
            btnDangKy.Click += (s, e) => { using (var f = new FrmDangKy()) f.ShowDialog(this); };

            Controls.Add(new Label { Text = "Mã khách hàng (MaKH):", Left = 30, Top = 87, Width = 150 });
            numMaKH = new NumericUpDown { Left = 190, Top = 83, Width = 160, Minimum = 1, Maximum = 1000000, Value = 1 };

            Controls.Add(new Label { Text = "Tổng tiền hàng (đ):", Left = 30, Top = 122, Width = 150 });
            numTongTien = new NumericUpDown
            {
                Left = 190, Top = 118, Width = 160, Minimum = 1000, Maximum = 1000000000,
                Increment = 100000, Value = 1200000, ThousandsSeparator = true
            };

            var btnThanhToan = new Button { Text = "2. Đặt hàng && Thanh toán", Left = 30, Top = 165, Width = 320, Height = 40 };
            btnThanhToan.Click += (s, e) =>
            {
                using (var f = new FrmThanhToan((int)numMaKH.Value, numTongTien.Value)) f.ShowDialog(this);
            };

            Controls.AddRange(new Control[] { btnDangKy, numMaKH, numTongTien, btnThanhToan });
        }
    }
}
