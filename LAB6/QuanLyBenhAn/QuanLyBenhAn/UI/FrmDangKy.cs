using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.UI
{
    /// <summary>Module 1: dang ky lan dau + dong le phi nam + cap ho so (1 transaction).</summary>
    public class FrmDangKy : Form
    {
        private TextBox txtHoTen, txtDiaChi, txtSDT;
        private PictureBox picAnh;
        private Label lblAnhPath;
        private string anhPath;   // duong dan file anh 3x4, luu vao cot Anh3x4
        private DateTimePicker dtNgaySinh;
        private NumericUpDown numNam, numTien;
        private TextBox txtNguoiThu;

        public FrmDangKy()
        {
            Text = "Dang ky lan dau + Le phi nam + Cap ho so";
            ClientSize = new Size(460, 460);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            int y = 15;
            Controls.Add(new Label { Text = "Ho ten:", Left = 20, Top = y, Width = 110 });
            txtHoTen = new TextBox { Left = 140, Top = y - 3, Width = 290 }; y += 35;
            Controls.Add(new Label { Text = "Ngay sinh:", Left = 20, Top = y, Width = 110 });
            dtNgaySinh = new DateTimePicker { Left = 140, Top = y - 3, Width = 290, Value = new DateTime(1990, 1, 1) }; y += 35;
            Controls.Add(new Label { Text = "Dia chi:", Left = 20, Top = y, Width = 110 });
            txtDiaChi = new TextBox { Left = 140, Top = y - 3, Width = 290 }; y += 35;
            Controls.Add(new Label { Text = "Dien thoai:", Left = 20, Top = y, Width = 110 });
            txtSDT = new TextBox { Left = 140, Top = y - 3, Width = 290 }; y += 35;
            Controls.Add(new Label { Text = "Anh 3x4:", Left = 20, Top = y, Width = 110 });
            picAnh = new PictureBox
            {
                Left = 140, Top = y - 3, Width = 90, Height = 120,
                BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            var btnChonAnh = new Button { Text = "Chon anh...", Left = 240, Top = y - 3, Width = 190 };
            btnChonAnh.Click += BtnChonAnh_Click;
            lblAnhPath = new Label
            {
                Text = "Chua chon anh.", Left = 240, Top = y + 30, Width = 190, Height = 60,
                ForeColor = Color.Gray
            };
            y += 130;
            Controls.Add(new Label { Text = "Nam le phi:", Left = 20, Top = y, Width = 110 });
            numNam = new NumericUpDown { Left = 140, Top = y - 3, Width = 130, Minimum = 2000, Maximum = 2100, Value = DateTime.Today.Year }; y += 35;
            Controls.Add(new Label { Text = "So tien:", Left = 20, Top = y, Width = 110 });
            numTien = new NumericUpDown { Left = 140, Top = y - 3, Width = 290, Minimum = 1, Maximum = 100000000, Value = BenhNhanService.LePhiNamMacDinh, ThousandsSeparator = true }; y += 35;
            Controls.Add(new Label { Text = "Nguoi thu:", Left = 20, Top = y, Width = 110 });
            txtNguoiThu = new TextBox { Left = 140, Top = y - 3, Width = 290 }; y += 45;

            Controls.AddRange(new Control[] { txtHoTen, dtNgaySinh, txtDiaChi, txtSDT, picAnh, btnChonAnh, lblAnhPath, numNam, numTien, txtNguoiThu });

            var btnLuu = new Button { Text = "Dang ky + Cap ho so", Left = 140, Top = y, Width = 290, Height = 40 };
            btnLuu.Click += BtnLuu_Click;
            Controls.Add(btnLuu);
        }

        private void BtnChonAnh_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Chon anh 3x4";
                dlg.Filter = "Anh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    byte[] data = File.ReadAllBytes(dlg.FileName);
                    using (var ms = new MemoryStream(data))
                    using (var img = Image.FromStream(ms))
                    {
                        if (picAnh.Image != null) picAnh.Image.Dispose();
                        picAnh.Image = new Bitmap(img);
                    }
                    anhPath = dlg.FileName;
                    lblAnhPath.Text = Path.GetFileName(anhPath);
                    lblAnhPath.ForeColor = Color.Black;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Khong doc duoc file anh: " + ex.Message, "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            var svc = new BenhNhanService();
            var r = svc.DangKyLanDau(txtHoTen.Text, dtNgaySinh.Value.Date, txtDiaChi.Text,
                txtSDT.Text, string.IsNullOrWhiteSpace(anhPath) ? null : anhPath,
                (int)numNam.Value, numTien.Value, txtNguoiThu.Text);
            MessageBox.Show(r.Message, r.Success ? "Thanh cong" : "Loi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) Close();
        }
    }
}
