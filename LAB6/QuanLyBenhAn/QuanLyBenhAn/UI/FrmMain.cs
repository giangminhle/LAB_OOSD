using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.UI
{
    /// <summary>Form menu de mo cac chuc nang (dung cho thuc hanh/demo).</summary>
    public class FrmMain : Form
    {
        private TextBox txtKetQua;

        public FrmMain()
        {
            Text = "LAB6 - Quan ly Ho so Benh an";
            ClientSize = new Size(520, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var btnDK = new Button { Text = "1. Dang ky lan dau + Le phi nam + Cap ho so", Left = 30, Top = 20, Width = 460, Height = 42 };
            btnDK.Click += (s, e) => { using (var f = new FrmDangKy()) f.ShowDialog(this); };

            var btnKham = new Button { Text = "2. Kham benh - Xet nghiem - Toa thuoc - Thanh toan - Hoan tat", Left = 30, Top = 72, Width = 460, Height = 42 };
            btnKham.Click += (s, e) => { using (var f = new FrmKhamBenh()) f.ShowDialog(this); };

            var btnTest = new Button { Text = "3. Kiem thu quy tac nghiep vu", Left = 30, Top = 124, Width = 460, Height = 42 };
            btnTest.Click += (s, e) => { txtKetQua.Text = RuleTests.RunAll(); };

            txtKetQua = new TextBox { Left = 30, Top = 176, Width = 460, Height = 210, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };

            Controls.AddRange(new Control[] { btnDK, btnKham, btnTest, txtKetQua });
        }
    }
}
