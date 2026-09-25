using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using QuanLyKhachSan.Services;
using static QuanLyKhachSan.Forms.UiHelper;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmDatPhong : Form
    {
        private readonly DatPhongService svc = new DatPhongService();
        private readonly DanhMucService dm = new DanhMucService();
        private readonly BindingList<PhongDatItem> chon = new BindingList<PhongDatItem>();

        // Tab Khách hàng
        private TextBox txtMaKH, txtTenKH, txtCMND, txtQT, txtSDT;
        private DataGridView dgvKhach;

        // Tab Đặt phòng
        private TextBox txtSoPhieu; private ComboBox cboKhach, cboNV, cboKenh;
        private DateTimePicker dtLap, dtNhan, dtTra; private NumericUpDown numCoc, numSoNguoi;
        private DataGridView dgvPhong, dgvChon;

        // Tab Nhận phòng / người lưu trú
        private DataGridView dgvPhieu, dgvCT, dgvNguoi;
        private TextBox txtPhieuChon, txtNguoiPhong, txtNguoiTen, txtNguoiCMND, txtNguoiQT;

        public FrmDatPhong()
        {
            InitializeComponent();
            Load += Frm_Load;
        }

        private void InitializeComponent()
        {
            Text = "Đặt / Nhận phòng";
            Width = 1050;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;

            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabKhachHang());
            tab.TabPages.Add(TabDatPhong());
            tab.TabPages.Add(TabNhanPhong());

            var btnDong = Btn("btnDong", "Đóng");
            btnDong.Dock = DockStyle.Bottom;
            btnDong.Click += (s, e) => Close();

            Controls.Add(tab);
            Controls.Add(btnDong);
        }

        private TabPage TabKhachHang()
        {
            var tp = new TabPage("Khách hàng");
            var top = NewFormPanel(4, 2);
            txtMaKH = Txt("txtMaKH"); txtTenKH = Txt("txtTenKH");
            txtCMND = Txt("txtCMND"); txtQT = Txt("txtQT"); txtSDT = Txt("txtSDT");
            var btn = Btn("btnThemKhach", "Lưu khách hàng");
            btn.Click += (s, e) => H(svc.ThemKhach(txtMaKH.Text.Trim(), txtTenKH.Text.Trim(),
                txtCMND.Text.Trim(), txtQT.Text.Trim(), txtSDT.Text.Trim()));
            top.Controls.Add(Lbl("Mã khách:")); top.Controls.Add(txtMaKH);
            top.Controls.Add(Lbl("Họ tên:")); top.Controls.Add(txtTenKH);
            top.Controls.Add(Lbl("Số CCCD:")); top.Controls.Add(txtCMND);
            top.Controls.Add(Lbl("Quốc tịch:")); top.Controls.Add(txtQT);
            top.Controls.Add(Lbl("SĐT:")); top.Controls.Add(txtSDT);
            top.Controls.Add(new Label()); top.Controls.Add(btn);

            dgvKhach = Grid("dgvKhach");
            tp.Controls.Add(dgvKhach);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabDatPhong()
        {
            var tp = new TabPage("Lập phiếu đặt phòng");

            var top = NewFormPanel(4, 2);
            txtSoPhieu = Txt("txtSoPhieu"); cboKhach = Cbo("cboKhach");
            cboNV = Cbo("cboNV"); cboKenh = Cbo("cboKenh");
            dtLap = Date("dtLap"); dtNhan = Date("dtNhan"); dtTra = Date("dtTra");
            numCoc = Num("numCoc", 1000000000);
            top.Controls.Add(Lbl("Số phiếu:")); top.Controls.Add(txtSoPhieu);
            top.Controls.Add(Lbl("Khách:")); top.Controls.Add(cboKhach);
            top.Controls.Add(Lbl("Lễ tân:")); top.Controls.Add(cboNV);
            top.Controls.Add(Lbl("Kênh đặt:")); top.Controls.Add(cboKenh);
            top.Controls.Add(Lbl("Ngày lập:")); top.Controls.Add(dtLap);
            top.Controls.Add(Lbl("Ngày nhận:")); top.Controls.Add(dtNhan);
            top.Controls.Add(Lbl("Ngày trả dự kiến:")); top.Controls.Add(dtTra);
            top.Controls.Add(Lbl("Tiền cọc:")); top.Controls.Add(numCoc);

            var mid = NewFormPanel(3, 1);
            numSoNguoi = Num("numSoNguoi", 20);
            var btnThemPhong = Btn("btnThemPhong", "Thêm phòng vào phiếu", 170);
            btnThemPhong.Click += BtnThemPhong_Click;
            var btnBoPhong = Btn("btnBoPhong", "Bỏ phòng đã chọn", 170);
            btnBoPhong.Click += (s, e) =>
            {
                if (dgvChon.CurrentRow != null && dgvChon.CurrentRow.Index >= 0 && dgvChon.CurrentRow.Index < chon.Count)
                    chon.RemoveAt(dgvChon.CurrentRow.Index);
            };
            mid.Controls.Add(Lbl("Số người ở phòng đang chọn:")); mid.Controls.Add(numSoNguoi);
            mid.Controls.Add(btnThemPhong);
            mid.Controls.Add(new Label()); mid.Controls.Add(new Label()); mid.Controls.Add(btnBoPhong);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
            dgvPhong = Grid("dgvPhong");
            dgvChon = Grid("dgvChon");
            split.Panel1.Controls.Add(dgvPhong);
            split.Panel1.Controls.Add(new Label { Text = "Danh sách phòng", Dock = DockStyle.Top });
            split.Panel2.Controls.Add(dgvChon);
            split.Panel2.Controls.Add(new Label { Text = "Phòng đã chọn cho phiếu", Dock = DockStyle.Top });

            var btnLapPhieu = Btn("btnLapPhieu", "Lập phiếu đặt phòng", 200);
            btnLapPhieu.Dock = DockStyle.Bottom;
            btnLapPhieu.Click += BtnLapPhieu_Click;

            var bottomHost = new Panel { Dock = DockStyle.Fill };
            bottomHost.Controls.Add(split);
            bottomHost.Controls.Add(mid);
            bottomHost.Controls.Add(top);

            tp.Controls.Add(bottomHost);
            tp.Controls.Add(btnLapPhieu);
            return tp;
        }

        private TabPage TabNhanPhong()
        {
            var tp = new TabPage("Nhận phòng / Người lưu trú");

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            dgvPhieu = Grid("dgvPhieu");
            dgvPhieu.SelectionChanged += DgvPhieu_SelectionChanged;
            split.Panel1.Controls.Add(dgvPhieu);
            split.Panel1.Controls.Add(new Label { Text = "Danh sách phiếu đặt phòng", Dock = DockStyle.Top });

            var bottom = new Panel { Dock = DockStyle.Fill };

            var infoPanel = NewFormPanel(4, 2);
            txtPhieuChon = Txt("txtPhieuChon"); txtPhieuChon.ReadOnly = true;
            txtNguoiPhong = Txt("txtNguoiPhong");
            txtNguoiTen = Txt("txtNguoiTen"); txtNguoiCMND = Txt("txtNguoiCMND"); txtNguoiQT = Txt("txtNguoiQT");
            var btnThemNguoi = Btn("btnThemNguoi", "Thêm người lưu trú", 170);
            btnThemNguoi.Click += (s, e) => H(svc.ThemNguoiLuuTru(txtPhieuChon.Text.Trim(),
                txtNguoiPhong.Text.Trim(), txtNguoiTen.Text.Trim(), txtNguoiCMND.Text.Trim(), txtNguoiQT.Text.Trim()));
            infoPanel.Controls.Add(Lbl("Phiếu đã chọn:")); infoPanel.Controls.Add(txtPhieuChon);
            infoPanel.Controls.Add(Lbl("Phòng:")); infoPanel.Controls.Add(txtNguoiPhong);
            infoPanel.Controls.Add(Lbl("Họ tên người ở:")); infoPanel.Controls.Add(txtNguoiTen);
            infoPanel.Controls.Add(Lbl("Số CCCD:")); infoPanel.Controls.Add(txtNguoiCMND);
            infoPanel.Controls.Add(Lbl("Quốc tịch:")); infoPanel.Controls.Add(txtNguoiQT);
            infoPanel.Controls.Add(new Label()); infoPanel.Controls.Add(btnThemNguoi);

            var actionPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            var btnNhanPhong = Btn("btnNhanPhong", "Nhận phòng");
            btnNhanPhong.Click += (s, e) => H(svc.NhanPhong(txtPhieuChon.Text.Trim(), DateTime.Now));
            var btnNoShow = Btn("btnNoShow", "Đánh dấu No-show", 160);
            btnNoShow.Click += (s, e) => H(svc.DanhDauNoShow(txtPhieuChon.Text.Trim()));
            actionPanel.Controls.Add(btnNhanPhong);
            actionPanel.Controls.Add(btnNoShow);

            var gridsSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
            dgvCT = Grid("dgvCT");
            dgvNguoi = Grid("dgvNguoi");
            gridsSplit.Panel1.Controls.Add(dgvCT);
            gridsSplit.Panel1.Controls.Add(new Label { Text = "Chi tiết phòng của phiếu", Dock = DockStyle.Top });
            gridsSplit.Panel2.Controls.Add(dgvNguoi);
            gridsSplit.Panel2.Controls.Add(new Label { Text = "Người lưu trú", Dock = DockStyle.Top });

            bottom.Controls.Add(gridsSplit);
            bottom.Controls.Add(actionPanel);
            bottom.Controls.Add(infoPanel);
            split.Panel2.Controls.Add(bottom);

            tp.Controls.Add(split);
            return tp;
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            cboKhach.DataSource = svc.LayKhach();
            cboKhach.DisplayMember = "HoTen"; cboKhach.ValueMember = "MaKhach";

            cboNV.DataSource = dm.LayNhanVien();
            cboNV.DisplayMember = "HoTen"; cboNV.ValueMember = "MaNV";

            cboKenh.Items.AddRange(new object[] { "Điện thoại", "Website", "Trực tiếp" });
            cboKenh.SelectedIndex = 0;

            dgvChon.DataSource = chon;

            Tai();
        }

        private void Tai()
        {
            dgvKhach.DataSource = svc.LayKhach();
            dgvPhong.DataSource = svc.LayPhong();
            dgvPhieu.DataSource = svc.LayPhieuDat();
        }

        private void BtnThemPhong_Click(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow == null) return;
            string p = Convert.ToString(dgvPhong.CurrentRow.Cells["SoPhong"].Value);
            foreach (var x in chon)
                if (x.SoPhong == p) { MessageBox.Show("Phòng đã có trong phiếu."); return; }

            int n = (int)numSoNguoi.Value;
            decimal g = Convert.ToDecimal(dgvPhong.CurrentRow.Cells["DonGiaNgay"].Value);
            chon.Add(new PhongDatItem { SoPhong = p, SoNguoi = n, DonGiaNgay = g });
        }

        private void BtnLapPhieu_Click(object sender, EventArgs e)
        {
            var k = svc.TaoDatPhong(txtSoPhieu.Text.Trim(), V(cboKhach), V(cboNV), dtLap.Value,
                dtNhan.Value, dtTra.Value, numCoc.Value, cboKenh.Text, new List<PhongDatItem>(chon));
            H(k);
            if (k.ThanhCong) chon.Clear();
        }

        private void DgvPhieu_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhieu.CurrentRow == null) return;
            string so = Convert.ToString(dgvPhieu.CurrentRow.Cells["SoPhieuDat"].Value);
            txtPhieuChon.Text = so;
            dgvCT.DataSource = svc.LayChiTiet(so);
            dgvNguoi.DataSource = svc.LayNguoiLuuTru(so);
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
