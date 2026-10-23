using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.UI
{
    public class FrmKetNoi : Form
    {
        private TextBox txtChuoi;
        private Label lblKetQua;

        public FrmKetNoi()
        {
            Text = "Kết nối CSDL SQL Server";
            ClientSize = new Size(720, 260);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(new Label
            {
                Text = "Connection string (sửa trong App.config để lưu lâu dài):",
                Left = 20, Top = 15, Width = 660
            });
            txtChuoi = new TextBox
            {
                Left = 20, Top = 40, Width = 680, Height = 80, Multiline = true,
                Text = Db.ConnectionString, ScrollBars = ScrollBars.Horizontal
            };
            Controls.Add(txtChuoi);

            var btnTest = UiHelper.Btn("Kiểm tra kết nối", 20, 135, 160);
            var btnLuu = UiHelper.Btn("Dùng chuỗi này", 190, 135, 150);
            var btnDong = UiHelper.Btn("Đóng", 350, 135, 100);
            lblKetQua = new Label
            {
                Left = 20, Top = 180, Width = 680, Height = 55,
                ForeColor = Color.DarkGreen, Text = "Chưa kiểm tra."
            };
            Controls.AddRange(new Control[] { btnTest, btnLuu, btnDong, lblKetQua });

            btnTest.Click += (s, e) =>
            {
                string msg;
                bool ok = Db.TestConnection(out msg);
                lblKetQua.Text = msg;
                lblKetQua.ForeColor = ok ? Color.DarkGreen : Color.Firebrick;
            };
            btnLuu.Click += (s, e) =>
            {
                Db.ConnectionString = txtChuoi.Text.Trim();
                string msg;
                bool ok = Db.TestConnection(out msg);
                lblKetQua.Text = (ok ? "Đã dùng chuỗi mới. " : "Chuỗi mới chưa kết nối được. ") + msg;
                lblKetQua.ForeColor = ok ? Color.DarkGreen : Color.Firebrick;
            };
            btnDong.Click += (s, e) => Close();
        }
    }
}
