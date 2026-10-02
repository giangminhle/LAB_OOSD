using System;
using System.Drawing;
using System.Windows.Forms;
using EShopping.Models;
using EShopping.Services;

namespace EShopping.UI
{
    /// <summary>UI chỉ gom dữ liệu + hiển thị kết quả. Mọi validate/nghiệp vụ nằm ở Service.</summary>
    public class FrmDangKy : Form
    {
        private readonly KhachHangService _service = new KhachHangService();

        private TextBox txtHoTen, txtCmnd, txtDiaChi, txtDienThoai, txtUser, txtPass, txtPass2, txtEmail;
        private DateTimePicker dtpNgaySinh;
        private Button btnDangKy, btnThoat;
        private int _y = 15;

        public FrmDangKy()
        {
            InitializeUI();
        }

        private T AddRow<T>(string label, T ctl) where T : Control
        {
            var lbl = new Label { Text = label, Left = 20, Top = _y + 4, Width = 140, AutoSize = false };
            ctl.Left = 170; ctl.Top = _y; ctl.Width = 280;
            Controls.Add(lbl); Controls.Add(ctl);
            _y += 34;
            return ctl;
        }

        private void InitializeUI()
        {
            Text = "Đăng ký tài khoản khách hàng";
            ClientSize = new Size(480, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            txtHoTen = AddRow("Họ tên (*)", new TextBox());
            dtpNgaySinh = AddRow("Ngày sinh (*)", new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today,
                Value = DateTime.Today.AddYears(-20)
            });
            txtCmnd = AddRow("CMND/Passport (*)", new TextBox());
            txtDiaChi = AddRow("Địa chỉ (*)", new TextBox());
            txtDienThoai = AddRow("Điện thoại (*)", new TextBox());
            txtUser = AddRow("Tên đăng nhập (*)", new TextBox());
            txtPass = AddRow("Mật khẩu (*)", new TextBox { UseSystemPasswordChar = true });
            txtPass2 = AddRow("Nhập lại mật khẩu (*)", new TextBox { UseSystemPasswordChar = true });
            txtEmail = AddRow("Email", new TextBox());

            btnDangKy = new Button { Text = "Đăng ký", Left = 170, Top = _y + 10, Width = 130, Height = 32 };
            btnThoat = new Button { Text = "Thoát", Left = 320, Top = _y + 10, Width = 130, Height = 32 };
            btnDangKy.Click += btnDangKy_Click;
            btnThoat.Click += (s, e) => Close();
            Controls.Add(btnDangKy); Controls.Add(btnThoat);
            AcceptButton = btnDangKy;
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            // Việc duy nhất UI tự kiểm tra: xác nhận mật khẩu (chỉ là tiện ích nhập liệu)
            if (txtPass.Text != txtPass2.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass2.Focus();
                return;
            }

            var kh = new KhachHang
            {
                HoTen = txtHoTen.Text,
                NgaySinh = dtpNgaySinh.Value,
                SoCMND = txtCmnd.Text,
                DiaChi = txtDiaChi.Text,
                DienThoai = txtDienThoai.Text,
                TenDangNhap = txtUser.Text,
                MatKhau = txtPass.Text,
                Email = txtEmail.Text
            };

            ServiceResult kq = _service.DangKy(kh);

            MessageBox.Show(kq.Message, kq.Success ? "Thành công" : "Lỗi",
                            MessageBoxButtons.OK,
                            kq.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (kq.Success) Close();
        }
    }
}
