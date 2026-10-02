using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using EShopping.Models;
using EShopping.Services;

namespace EShopping.UI
{
    /// <summary>
    /// Form đặt hàng & thanh toán.
    /// maKH: khách đã đăng nhập; tongTienHang: tổng tiền sản phẩm trong giỏ (do module giỏ hàng truyền sang).
    /// </summary>
    public class FrmThanhToan : Form
    {
        private readonly DatHangService _service = new DatHangService();
        private readonly int _maKH;
        private readonly decimal _tongTienHang;
        private static readonly CultureInfo VN = new CultureInfo("vi-VN");

        private Label lblTongHang, lblPhiShip, lblPhiThe, lblTongTT;
        private RadioButton rdoThuong, rdoNhanh, rdoTrongNgay;
        private TextBox txtTenNhan, txtDiaChiNhan, txtDtNhan;
        private ComboBox cboLoaiThe;
        private TextBox txtSoThe, txtHetHan, txtChuThe, txtCsv;
        private Button btnDatHang, btnHuy;
        private int _y = 12;

        public FrmThanhToan(int maKH, decimal tongTienHang)
        {
            _maKH = maKH;
            _tongTienHang = tongTienHang;
            InitializeUI();
            CapNhatGiaoDien();
        }

        // ---------------- Dựng giao diện ----------------
        private void AddHeader(string text)
        {
            Controls.Add(new Label
            {
                Text = text, Left = 15, Top = _y, Width = 470,
                Font = new Font(Font, FontStyle.Bold), ForeColor = Color.DarkBlue
            });
            _y += 26;
        }

        private T AddRow<T>(string label, T ctl, int width = 290) where T : Control
        {
            Controls.Add(new Label { Text = label, Left = 25, Top = _y + 4, Width = 150 });
            ctl.Left = 180; ctl.Top = _y; ctl.Width = width;
            Controls.Add(ctl);
            _y += 32;
            return ctl;
        }

        private void InitializeUI()
        {
            Text = "Đặt hàng & Thanh toán";
            ClientSize = new Size(500, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            AddHeader("1. Loại hình giao hàng");
            rdoThuong = new RadioButton { Left = 25, Top = _y, Width = 450, Checked = true };
            rdoNhanh = new RadioButton { Left = 25, Top = _y + 24, Width = 450 };
            rdoTrongNgay = new RadioButton { Left = 25, Top = _y + 48, Width = 450 };
            foreach (var r in new[] { rdoThuong, rdoNhanh, rdoTrongNgay })
            {
                r.CheckedChanged += (s, e) => CapNhatGiaoDien();   // đổi loại giao hàng -> tính lại phí
                Controls.Add(r);
            }
            _y += 78;

            AddHeader("2. Thông tin người nhận (có thể khác người mua)");
            txtTenNhan = AddRow("Họ tên người nhận (*)", new TextBox());
            txtDiaChiNhan = AddRow("Địa chỉ nhận (*)", new TextBox());
            txtDtNhan = AddRow("Điện thoại (*)", new TextBox());

            AddHeader("3. Thanh toán bằng thẻ tín dụng");
            cboLoaiThe = AddRow("Loại thẻ (*)", new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList });
            foreach (LoaiThe t in Enum.GetValues(typeof(LoaiThe))) cboLoaiThe.Items.Add(t);
            cboLoaiThe.SelectedIndex = 0;
            cboLoaiThe.SelectedIndexChanged += (s, e) => CapNhatGiaoDien();  // phí thẻ phụ thuộc loại thẻ

            txtSoThe = AddRow("Số thẻ (*)", new TextBox { MaxLength = 19 });
            txtHetHan = AddRow("Hết hạn (MM/yy) (*)", new TextBox { MaxLength = 5 }, 80);
            txtChuThe = AddRow("Tên chủ thẻ (*)", new TextBox());
            txtCsv = AddRow("Mã CSV (*)", new TextBox { MaxLength = 4, UseSystemPasswordChar = true }, 80);

            AddHeader("4. Tổng kết");
            lblTongHang = AddRow("Tiền hàng:", new Label { TextAlign = ContentAlignment.MiddleRight });
            lblPhiShip = AddRow("Phí giao hàng:", new Label { TextAlign = ContentAlignment.MiddleRight });
            lblPhiThe = AddRow("Lệ phí thẻ:", new Label { TextAlign = ContentAlignment.MiddleRight });
            lblTongTT = AddRow("TỔNG THANH TOÁN:", new Label
            {
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.Firebrick
            });

            btnDatHang = new Button { Text = "Đặt hàng", Left = 180, Top = _y + 8, Width = 140, Height = 34 };
            btnHuy = new Button { Text = "Hủy", Left = 330, Top = _y + 8, Width = 140, Height = 34 };
            btnDatHang.Click += btnDatHang_Click;
            btnHuy.Click += (s, e) => Close();
            Controls.Add(btnDatHang); Controls.Add(btnHuy);
        }

        // ---------------- Cập nhật giá hiển thị ----------------
        private LoaiGiaoHang LayLoaiGiaoHang()
        {
            if (rdoTrongNgay.Checked) return LoaiGiaoHang.NhanhTrongNgay;
            if (rdoNhanh.Checked) return LoaiGiaoHang.Nhanh;
            return LoaiGiaoHang.Thuong;
        }

        private static string Tien(decimal v) { return v.ToString("N0", VN) + " đ"; }

        private string NhanRadio(string ten, LoaiGiaoHang loai)
        {
            decimal phi = _service.TinhPhiGiaoHang(loai, _tongTienHang);
            return ten + " - " + (phi == 0 ? "MIỄN PHÍ" : Tien(phi));
        }

        private void CapNhatGiaoDien()
        {
            if (cboLoaiThe == null || lblTongTT == null) return;   // chưa dựng xong form

            rdoThuong.Text = NhanRadio("Giao hàng thường", LoaiGiaoHang.Thuong);
            rdoNhanh.Text = NhanRadio("Chuyển phát nhanh (miễn phí từ 1.000.000 đ)", LoaiGiaoHang.Nhanh);
            rdoTrongNgay.Text = NhanRadio("Chuyển phát nhanh trong ngày (miễn phí từ 5.000.000 đ)", LoaiGiaoHang.NhanhTrongNgay);

            var loaiThe = (LoaiThe)cboLoaiThe.SelectedItem;
            decimal phiShip = _service.TinhPhiGiaoHang(LayLoaiGiaoHang(), _tongTienHang);
            decimal phiThe = _service.TinhPhiThe(loaiThe, _tongTienHang, phiShip);

            lblTongHang.Text = Tien(_tongTienHang);
            lblPhiShip.Text = phiShip == 0 ? "Miễn phí" : Tien(phiShip);
            lblPhiThe.Text = Tien(phiThe);
            lblTongTT.Text = Tien(_service.TinhTongThanhToan(_tongTienHang, phiShip, phiThe));
        }

        // ---------------- Đặt hàng ----------------
        private void btnDatHang_Click(object sender, EventArgs e)
        {
            // Parse ngày hết hạn MM/yy -> ngày cuối tháng
            DateTime het;
            if (!DateTime.TryParseExact(txtHetHan.Text.Trim(), "MM/yy", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out het))
            {
                Canh("Ngày hết hạn thẻ phải theo định dạng MM/yy (ví dụ 08/28).", txtHetHan);
                return;
            }
            het = new DateTime(het.Year, het.Month, DateTime.DaysInMonth(het.Year, het.Month));

            var the = new ThongTinThe
            {
                LoaiThe = (LoaiThe)cboLoaiThe.SelectedItem,
                SoThe = txtSoThe.Text,
                NgayHetHan = het,
                TenChuThe = txtChuThe.Text,
                Csv = txtCsv.Text
            };

            // Validate thẻ cơ bản ngay tại form để báo lỗi sớm (Service sẽ kiểm tra lại)
            string loiThe = _service.ValidateThe(the);
            if (loiThe != null) { Canh(loiThe, txtSoThe); return; }

            var dh = new DonHang
            {
                MaKH = _maKH,
                LoaiGiaoHang = LayLoaiGiaoHang(),
                TenNguoiNhan = txtTenNhan.Text,
                DiaChiNhan = txtDiaChiNhan.Text,
                DienThoaiNhan = txtDtNhan.Text,
                TongTienHang = _tongTienHang
            };

            btnDatHang.Enabled = false;
            try
            {
                ServiceResult kq = _service.DatHang(dh, the);
                if (kq.Success)
                {
                    MessageBox.Show(string.Format("{0}\nMã đơn hàng: {1}\nTổng thanh toán: {2}",
                                    kq.Message, kq.Id, Tien(dh.TongThanhToan)),
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    Canh(kq.Message, null);
                }
            }
            finally { btnDatHang.Enabled = true; }
        }

        private static void Canh(string msg, Control focus)
        {
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (focus != null) focus.Focus();
        }
    }
}
