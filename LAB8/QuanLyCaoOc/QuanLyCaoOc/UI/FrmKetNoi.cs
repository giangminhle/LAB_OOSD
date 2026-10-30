using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCaoOc.Data;

namespace QuanLyCaoOc.UI
{
    public class FrmKetNoi : Form
    {
        private ComboBox cboServer;
        private TextBox txtDatabase;
        private ComboBox cboAuth;
        private TextBox txtUser;
        private TextBox txtPassword;
        private TextBox txtChuoi;
        private Label lblKetQua;
        private Button btnTest;
        private Button btnLuu;
        private Button btnKhoiTaoDb;
        private Button btnDong;

        public FrmKetNoi()
        {
            Text = "Cấu hình kết nối CSDL SQL Server";
            ClientSize = new Size(680, 480);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            var grp = new GroupBox
            {
                Text = "Thông tin máy chủ và cơ sở dữ liệu",
                Left = 20, Top = 15, Width = 640, Height = 220
            };

            int y = 28;
            grp.Controls.Add(new Label { Text = "Server Name:", Left = 20, Top = y + 3, Width = 130 });
            cboServer = new ComboBox { Left = 160, Top = y, Width = 450, DropDownStyle = ComboBoxStyle.DropDown };
            cboServer.Items.AddRange(new object[] { ".", @".\SQLEXPRESS", "(local)", "localhost", @"ADMIN-PC\SQLEXPRESS", @"DESKTOP\SQLEXPRESS" });
            cboServer.Text = ".";
            grp.Controls.Add(cboServer);
            y += 35;

            grp.Controls.Add(new Label { Text = "Database Name:", Left = 20, Top = y + 3, Width = 130 });
            txtDatabase = new TextBox { Left = 160, Top = y, Width = 450, Text = "QuanLyCaoOc" };
            grp.Controls.Add(txtDatabase);
            y += 35;

            grp.Controls.Add(new Label { Text = "Authentication:", Left = 20, Top = y + 3, Width = 130 });
            cboAuth = new ComboBox { Left = 160, Top = y, Width = 450, DropDownStyle = ComboBoxStyle.DropDownList };
            cboAuth.Items.AddRange(new object[] { "SQL Server Authentication", "Windows Authentication" });
            cboAuth.SelectedIndex = 0;
            grp.Controls.Add(cboAuth);
            y += 35;

            grp.Controls.Add(new Label { Text = "User Name:", Left = 20, Top = y + 3, Width = 130 });
            txtUser = new TextBox { Left = 160, Top = y, Width = 450, Text = "sa" };
            grp.Controls.Add(txtUser);
            y += 35;

            grp.Controls.Add(new Label { Text = "Password:", Left = 20, Top = y + 3, Width = 130 });
            txtPassword = new TextBox { Left = 160, Top = y, Width = 450, UseSystemPasswordChar = true, Text = "25022005" };
            grp.Controls.Add(txtPassword);

            Controls.Add(grp);

            Controls.Add(new Label
            {
                Text = "Connection String kết nối thực tế:",
                Left = 20, Top = 245, Width = 300, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            txtChuoi = new TextBox
            {
                Left = 20, Top = 268, Width = 640, Height = 60,
                Multiline = true, ScrollBars = ScrollBars.Vertical,
                Text = Db.ConnectionString
            };
            Controls.Add(txtChuoi);

            btnTest = UiHelper.Btn("Kiểm tra kết nối", 20, 340, 140);
            btnLuu = UiHelper.Btn("Lưu & Áp dụng", 170, 340, 130);
            btnKhoiTaoDb = UiHelper.Btn("Khởi tạo CSDL mẫu", 310, 340, 170);
            btnDong = UiHelper.Btn("Đóng", 490, 340, 100);

            Controls.AddRange(new Control[] { btnTest, btnLuu, btnKhoiTaoDb, btnDong });

            lblKetQua = new Label
            {
                Left = 20, Top = 390, Width = 640, Height = 75,
                ForeColor = Color.DarkSlateGray,
                BorderStyle = BorderStyle.Fixed3D,
                Text = "Sẵn sàng kết nối. Nhấn 'Kiểm tra kết nối' để xác thực."
            };
            Controls.Add(lblKetQua);

            // Su kien
            cboAuth.SelectedIndexChanged += (s, e) =>
            {
                bool isSqlAuth = cboAuth.SelectedIndex == 0;
                txtUser.Enabled = isSqlAuth;
                txtPassword.Enabled = isSqlAuth;
                UpdateConnectionString();
            };

            cboServer.TextChanged += (s, e) => UpdateConnectionString();
            txtDatabase.TextChanged += (s, e) => UpdateConnectionString();
            txtUser.TextChanged += (s, e) => UpdateConnectionString();
            txtPassword.TextChanged += (s, e) => UpdateConnectionString();

            btnTest.Click += BtnTest_Click;
            btnLuu.Click += BtnLuu_Click;
            btnKhoiTaoDb.Click += BtnKhoiTaoDb_Click;
            btnDong.Click += (s, e) => Close();
        }

        private void UpdateConnectionString()
        {
            string srv = string.IsNullOrWhiteSpace(cboServer.Text) ? "." : cboServer.Text.Trim();
            string db = string.IsNullOrWhiteSpace(txtDatabase.Text) ? "QuanLyCaoOc" : txtDatabase.Text.Trim();

            if (cboAuth.SelectedIndex == 1) // Windows Auth
            {
                txtChuoi.Text = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;MultipleActiveResultSets=True;", srv, db);
            }
            else // SQL Server Auth
            {
                string u = txtUser.Text.Trim();
                string p = txtPassword.Text;
                txtChuoi.Text = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=True;", srv, db, u, p);
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            string old = Db.ConnectionString;
            Db.ConnectionString = txtChuoi.Text.Trim();
            string msg;
            bool ok = Db.TestConnection(out msg);
            if (!ok) Db.ConnectionString = old; // khoi phuc neu loi

            lblKetQua.Text = msg;
            lblKetQua.ForeColor = ok ? Color.DarkGreen : Color.Firebrick;
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            Db.ConnectionString = txtChuoi.Text.Trim();
            string msg;
            bool ok = Db.TestConnection(out msg);
            lblKetQua.Text = (ok ? "Đã lưu chuỗi kết nối và áp dụng thành công!\n" : "Đã lưu nhưng chưa kết nối được:\n") + msg;
            lblKetQua.ForeColor = ok ? Color.DarkGreen : Color.Firebrick;
        }

        private void BtnKhoiTaoDb_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Thao tác này sẽ chạy script tạo bảng (01_CreateTables.sql) và nạp dữ liệu mẫu (02_SeedData.sql) vào database.\nBạn có muốn tiếp tục?",
                "Xác nhận khởi tạo CSDL",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (dr != DialogResult.Yes) return;

            Db.ConnectionString = txtChuoi.Text.Trim();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string f1 = Path.Combine(baseDir, "SQL", "01_CreateTables.sql");
            string f2 = Path.Combine(baseDir, "SQL", "02_SeedData.sql");

            if (!File.Exists(f1))
            {
                // Thu tim o thu muc goc du an
                f1 = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\QuanLyCaoOc\SQL\01_CreateTables.sql"));
                f2 = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\QuanLyCaoOc\SQL\02_SeedData.sql"));
            }

            string m1, m2;
            bool ok1 = Db.ExecuteScriptFile(f1, out m1);
            if (!ok1)
            {
                lblKetQua.Text = "Lỗi tạo bảng: " + m1;
                lblKetQua.ForeColor = Color.Firebrick;
                return;
            }

            bool ok2 = Db.ExecuteScriptFile(f2, out m2);
            if (!ok2)
            {
                lblKetQua.Text = "Đã tạo bảng thành công nhưng lỗi nạp dữ liệu: " + m2;
                lblKetQua.ForeColor = Color.OrangeRed;
                return;
            }

            lblKetQua.Text = "Khởi tạo CSDL hoàn tất thành công 100%! Đã tạo 14 bảng và nạp toàn bộ dữ liệu mẫu.";
            lblKetQua.ForeColor = Color.DarkGreen;
            MessageBox.Show("Khởi tạo cơ sở dữ liệu mẫu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
