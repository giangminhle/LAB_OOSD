using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;
using static QuanLyKhachSan.Forms.UiHelper;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmPhongTienNghi : Form
    {
        private readonly PhongTienNghiService svc = new PhongTienNghiService();
        private readonly DanhMucService dm = new DanhMucService();

        // Phòng
        private TextBox txtPhong; private ComboBox cboKhu;
        private NumericUpDown numMax, numGia;
        private DataGridView dgvPhong;

        // Tiện nghi
        private TextBox txtMaTN, txtTinhTrang; private ComboBox cboLoai;
        private NumericUpDown numSTT;
        private DataGridView dgvTN;

        // Phiếu lắp đặt
        private TextBox txtSoLD, txtTTLD, txtGhiChu;
        private ComboBox cboTN, cboPhong, cboNV;
        private DateTimePicker dtNgay;
        private DataGridView dgvLD;

        public FrmPhongTienNghi()
        {
            InitializeComponent();
            Load += Frm_Load;
        }

        private void InitializeComponent()
        {
            Text = "Phòng - Tiện nghi";
            Width = 950;
            Height = 650;
            StartPosition = FormStartPosition.CenterParent;

            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabPhong());
            tab.TabPages.Add(TabTienNghi());
            tab.TabPages.Add(TabLapDat());

            var btnDong = Btn("btnDong", "Đóng");
            btnDong.Dock = DockStyle.Bottom;
            btnDong.Click += (s, e) => Close();

            Controls.Add(tab);
            Controls.Add(btnDong);
        }

        private TabPage TabPhong()
        {
            var tp = new TabPage("Phòng");
            var top = NewFormPanel(4, 2);
            txtPhong = Txt("txtPhong"); cboKhu = Cbo("cboKhu");
            numMax = Num("numMax", 20); numGia = Num("numGia", 100000000);
            var btn = Btn("btnThemPhong", "Thêm phòng");
            btn.Click += (s, e) => H(svc.ThemPhong(txtPhong.Text.Trim(),
                cboKhu.SelectedValue == null ? "" : cboKhu.SelectedValue.ToString(),
                (int)numMax.Value, numGia.Value));
            top.Controls.Add(Lbl("Số phòng:")); top.Controls.Add(txtPhong);
            top.Controls.Add(Lbl("Khu vực:")); top.Controls.Add(cboKhu);
            top.Controls.Add(Lbl("Sức chứa tối đa:")); top.Controls.Add(numMax);
            top.Controls.Add(Lbl("Đơn giá/ngày:")); top.Controls.Add(numGia);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvPhong = Grid("dgvPhong");
            tp.Controls.Add(dgvPhong);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabTienNghi()
        {
            var tp = new TabPage("Tiện nghi");
            var top = NewFormPanel(4, 2);
            txtMaTN = Txt("txtMaTN"); cboLoai = Cbo("cboLoai");
            numSTT = Num("numSTT", 999); txtTinhTrang = Txt("txtTinhTrang");
            var btn = Btn("btnThemTN", "Thêm tiện nghi");
            btn.Click += (s, e) => H(svc.ThemTienNghi(txtMaTN.Text.Trim(),
                cboLoai.SelectedValue == null ? "" : cboLoai.SelectedValue.ToString(),
                (int)numSTT.Value, txtTinhTrang.Text.Trim()));
            top.Controls.Add(Lbl("Mã tiện nghi:")); top.Controls.Add(txtMaTN);
            top.Controls.Add(Lbl("Loại tiện nghi:")); top.Controls.Add(cboLoai);
            top.Controls.Add(Lbl("Số thứ tự:")); top.Controls.Add(numSTT);
            top.Controls.Add(Lbl("Tình trạng:")); top.Controls.Add(txtTinhTrang);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvTN = Grid("dgvTN");
            tp.Controls.Add(dgvTN);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabLapDat()
        {
            var tp = new TabPage("Phiếu lắp đặt / luân chuyển");
            var top = NewFormPanel(4, 3);
            txtSoLD = Txt("txtSoLD"); cboTN = Cbo("cboTN");
            cboPhong = Cbo("cboPhong"); dtNgay = Date("dtNgay");
            txtTTLD = Txt("txtTTLD"); cboNV = Cbo("cboNV");
            txtGhiChu = Txt("txtGhiChu");
            var btn = Btn("btnLapDat", "Lập phiếu lắp đặt");
            btn.Click += (s, e) => H(svc.LapDat(txtSoLD.Text.Trim(), V(cboTN), V(cboPhong),
                dtNgay.Value, txtTTLD.Text.Trim(), V(cboNV), txtGhiChu.Text.Trim()));

            top.Controls.Add(Lbl("Số phiếu:")); top.Controls.Add(txtSoLD);
            top.Controls.Add(Lbl("Thiết bị:")); top.Controls.Add(cboTN);
            top.Controls.Add(Lbl("Phòng:")); top.Controls.Add(cboPhong);
            top.Controls.Add(Lbl("Ngày lắp:")); top.Controls.Add(dtNgay);
            top.Controls.Add(Lbl("Tình trạng:")); top.Controls.Add(txtTTLD);
            top.Controls.Add(Lbl("Nhân viên:")); top.Controls.Add(cboNV);
            top.Controls.Add(Lbl("Ghi chú:")); top.Controls.Add(txtGhiChu);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvLD = Grid("dgvLD");
            tp.Controls.Add(dgvLD);
            tp.Controls.Add(top);
            return tp;
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            cboKhu.DataSource = dm.LayKhuVuc();
            cboKhu.DisplayMember = "TenKhuVuc"; cboKhu.ValueMember = "MaKhuVuc";

            cboLoai.DataSource = dm.LayLoaiTienNghi();
            cboLoai.DisplayMember = "TenLoaiTN"; cboLoai.ValueMember = "MaLoaiTN";

            cboTN.DataSource = svc.LayTienNghi();
            cboTN.DisplayMember = "MaTienNghi"; cboTN.ValueMember = "MaTienNghi";

            cboPhong.DataSource = svc.LayPhong();
            cboPhong.DisplayMember = "SoPhong"; cboPhong.ValueMember = "SoPhong";

            cboNV.DataSource = dm.LayNhanVien();
            cboNV.DisplayMember = "HoTen"; cboNV.ValueMember = "MaNV";

            Tai();
        }

        private void Tai()
        {
            dgvPhong.DataSource = svc.LayPhong();
            dgvTN.DataSource = svc.LayTienNghi();
            dgvLD.DataSource = svc.LayLapDat();
        }

        private static string V(ComboBox c) => c.SelectedValue == null ? "" : c.SelectedValue.ToString();

        private void H(KetQuaXuLy k)
        {
            MessageBox.Show(k.ThongBao, k.ThanhCong ? "Thành công" : "Không thể thực hiện",
                MessageBoxButtons.OK, k.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (k.ThanhCong) Tai();
        }
    }
}
