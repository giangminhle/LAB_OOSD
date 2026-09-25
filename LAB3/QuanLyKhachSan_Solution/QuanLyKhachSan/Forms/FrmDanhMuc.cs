using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;
using static QuanLyKhachSan.Forms.UiHelper;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmDanhMuc : Form
    {
        private readonly DanhMucService svc = new DanhMucService();

        // Khu vực
        private TextBox txtKhuMa, txtKhuTen;
        private DataGridView dgvKhu;

        // Nhân viên
        private TextBox txtNVMa, txtNVTen, txtNVVaiTro, txtNVSDT;
        private DataGridView dgvNV;

        // Loại tiện nghi
        private TextBox txtLoaiMa, txtLoaiTen;
        private DataGridView dgvLoaiTN;

        // Dịch vụ
        private TextBox txtDVMa, txtDVTen, txtDVDVT;
        private NumericUpDown numDVGia;
        private DataGridView dgvDV;

        // Quy định đền bù
        private TextBox txtQDMa, txtQDMucDo;
        private ComboBox cboQDLoai;
        private NumericUpDown numQDTien;
        private DataGridView dgvQD;

        public FrmDanhMuc()
        {
            InitializeComponent();
            Load += (s, e) => Tai();
        }

        private void InitializeComponent()
        {
            Text = "Danh mục";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabKhuVuc());
            tab.TabPages.Add(TabNhanVien());
            tab.TabPages.Add(TabLoaiTN());
            tab.TabPages.Add(TabDichVu());
            tab.TabPages.Add(TabQuyDinh());

            var btnDong = Btn("btnDong", "Đóng");
            btnDong.Dock = DockStyle.Bottom;
            btnDong.Click += (s, e) => Close();

            Controls.Add(tab);
            Controls.Add(btnDong);
        }

        private TabPage TabKhuVuc()
        {
            var tp = new TabPage("Khu vực");
            var top = NewFormPanel(4, 1);
            txtKhuMa = Txt("txtKhuMa");
            txtKhuTen = Txt("txtKhuTen");
            var btn = Btn("btnThemKhu", "Thêm khu vực");
            btn.Click += (s, e) => H(svc.ThemKhu(txtKhuMa.Text.Trim(), txtKhuTen.Text.Trim()));
            top.Controls.Add(Lbl("Mã khu vực:")); top.Controls.Add(txtKhuMa);
            top.Controls.Add(Lbl("Tên khu vực:")); top.Controls.Add(txtKhuTen);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvKhu = Grid("dgvKhu");
            tp.Controls.Add(dgvKhu);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabNhanVien()
        {
            var tp = new TabPage("Nhân viên");
            var top = NewFormPanel(4, 2);
            txtNVMa = Txt("txtNVMa"); txtNVTen = Txt("txtNVTen");
            txtNVVaiTro = Txt("txtNVVaiTro"); txtNVSDT = Txt("txtNVSDT");
            var btn = Btn("btnThemNV", "Thêm nhân viên");
            btn.Click += (s, e) => H(svc.ThemNhanVien(txtNVMa.Text.Trim(), txtNVTen.Text.Trim(),
                txtNVVaiTro.Text.Trim(), txtNVSDT.Text.Trim()));
            top.Controls.Add(Lbl("Mã NV:")); top.Controls.Add(txtNVMa);
            top.Controls.Add(Lbl("Họ tên:")); top.Controls.Add(txtNVTen);
            top.Controls.Add(Lbl("Vai trò:")); top.Controls.Add(txtNVVaiTro);
            top.Controls.Add(Lbl("SĐT:")); top.Controls.Add(txtNVSDT);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvNV = Grid("dgvNV");
            tp.Controls.Add(dgvNV);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabLoaiTN()
        {
            var tp = new TabPage("Loại tiện nghi");
            var top = NewFormPanel(4, 1);
            txtLoaiMa = Txt("txtLoaiMa"); txtLoaiTen = Txt("txtLoaiTen");
            var btn = Btn("btnThemLoaiTN", "Thêm loại TN");
            btn.Click += (s, e) => H(svc.ThemLoaiTN(txtLoaiMa.Text.Trim(), txtLoaiTen.Text.Trim()));
            top.Controls.Add(Lbl("Mã loại TN:")); top.Controls.Add(txtLoaiMa);
            top.Controls.Add(Lbl("Tên loại TN:")); top.Controls.Add(txtLoaiTen);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvLoaiTN = Grid("dgvLoaiTN");
            tp.Controls.Add(dgvLoaiTN);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabDichVu()
        {
            var tp = new TabPage("Dịch vụ");
            var top = NewFormPanel(4, 2);
            txtDVMa = Txt("txtDVMa"); txtDVTen = Txt("txtDVTen");
            txtDVDVT = Txt("txtDVDVT"); numDVGia = Num("numDVGia");
            var btn = Btn("btnThemDV", "Thêm dịch vụ");
            btn.Click += (s, e) => H(svc.ThemDichVu(txtDVMa.Text.Trim(), txtDVTen.Text.Trim(),
                txtDVDVT.Text.Trim(), numDVGia.Value));
            top.Controls.Add(Lbl("Mã DV:")); top.Controls.Add(txtDVMa);
            top.Controls.Add(Lbl("Tên DV:")); top.Controls.Add(txtDVTen);
            top.Controls.Add(Lbl("Đơn vị tính:")); top.Controls.Add(txtDVDVT);
            top.Controls.Add(Lbl("Đơn giá:")); top.Controls.Add(numDVGia);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvDV = Grid("dgvDV");
            tp.Controls.Add(dgvDV);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabQuyDinh()
        {
            var tp = new TabPage("Quy định đền bù");
            var top = NewFormPanel(4, 2);
            txtQDMa = Txt("txtQDMa"); cboQDLoai = Cbo("cboQDLoai");
            txtQDMucDo = Txt("txtQDMucDo"); numQDTien = Num("numQDTien");
            var btn = Btn("btnThemQD", "Thêm quy định");
            btn.Click += (s, e) => H(svc.ThemQuyDinh(txtQDMa.Text.Trim(),
                cboQDLoai.SelectedValue == null ? "" : cboQDLoai.SelectedValue.ToString(),
                txtQDMucDo.Text.Trim(), numQDTien.Value));
            top.Controls.Add(Lbl("Mã quy định:")); top.Controls.Add(txtQDMa);
            top.Controls.Add(Lbl("Loại tiện nghi:")); top.Controls.Add(cboQDLoai);
            top.Controls.Add(Lbl("Mức độ thiệt hại:")); top.Controls.Add(txtQDMucDo);
            top.Controls.Add(Lbl("Mức đền bù:")); top.Controls.Add(numQDTien);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvQD = Grid("dgvQD");
            tp.Controls.Add(dgvQD);
            tp.Controls.Add(top);
            return tp;
        }

        private void Tai()
        {
            dgvKhu.DataSource = svc.LayKhuVuc();
            dgvNV.DataSource = svc.LayNhanVien();
            dgvLoaiTN.DataSource = svc.LayLoaiTienNghi();
            dgvDV.DataSource = svc.LayDichVu();
            dgvQD.DataSource = svc.LayQuyDinhDenBu();

            cboQDLoai.DataSource = svc.LayLoaiTienNghi();
            cboQDLoai.DisplayMember = "TenLoaiTN";
            cboQDLoai.ValueMember = "MaLoaiTN";
        }

        private void H(KetQuaXuLy k)
        {
            MessageBox.Show(k.ThongBao, k.ThanhCong ? "Thành công" : "Không thể thực hiện",
                MessageBoxButtons.OK, k.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (k.ThanhCong) Tai();
        }
    }
}
