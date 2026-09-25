using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using QuanLyKhachSan.Services;
using static QuanLyKhachSan.Forms.UiHelper;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmTraPhong : Form
    {
        private readonly TraPhongService svc = new TraPhongService();
        private readonly DanhMucService dm = new DanhMucService();
        private readonly BindingList<DenBuItem> db = new BindingList<DenBuItem>();

        private ComboBox cboDat, cboNV, cboNV2, cboHT;
        private DataGridView dgvPhong, dgvTN, dgvDBChon, dgvHD;
        private TextBox txtPhong, txtSoDB, txtMucDo, txtSoHD, txtMaTT, txtHDChon;
        private NumericUpDown numDenBu, numSoNgay, numTienTT;

        public FrmTraPhong()
        {
            InitializeComponent();
            Load += Frm_Load;
        }

        private void InitializeComponent()
        {
            Text = "Trả phòng - Thanh toán";
            Width = 1050;
            Height = 750;
            StartPosition = FormStartPosition.CenterParent;

            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabPhongTienNghi());
            tab.TabPages.Add(TabDenBu());
            tab.TabPages.Add(TabHoaDonThanhToan());

            var btnDong = Btn("btnDong", "Đóng");
            btnDong.Dock = DockStyle.Bottom;
            btnDong.Click += (s, e) => Close();

            Controls.Add(tab);
            Controls.Add(btnDong);
        }

        private TabPage TabPhongTienNghi()
        {
            var tp = new TabPage("Phiếu / Phòng / Tiện nghi");
            var top = NewFormPanel(2, 1);
            cboDat = Cbo("cboDat", 260);
            cboDat.SelectedIndexChanged += (s, e) => Tai();
            top.Controls.Add(Lbl("Phiếu đang ở:")); top.Controls.Add(cboDat);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
            dgvPhong = Grid("dgvPhong");
            dgvPhong.SelectionChanged += DgvPhong_SelectionChanged;
            dgvTN = Grid("dgvTN");
            split.Panel1.Controls.Add(dgvPhong);
            split.Panel1.Controls.Add(new Label { Text = "Phòng theo phiếu", Dock = DockStyle.Top });
            split.Panel2.Controls.Add(dgvTN);
            split.Panel2.Controls.Add(new Label { Text = "Tiện nghi đã lắp cho phòng đang chọn", Dock = DockStyle.Top });

            txtPhong = Txt("txtPhong");

            tp.Controls.Add(split);
            tp.Controls.Add(top);
            return tp;
        }

        private TabPage TabDenBu()
        {
            var tp = new TabPage("Đền bù");
            var top = NewFormPanel(4, 2);
            txtSoDB = Txt("txtSoDB");
            txtMucDo = Txt("txtMucDo");
            numDenBu = Num("numDenBu", 100000000);
            var btnThemDB = Btn("btnThemDB", "Thêm vào phiếu", 150);
            btnThemDB.Click += BtnThemDB_Click;
            var btnLapDB = Btn("btnLapDB", "Lập phiếu đền bù", 170);
            btnLapDB.Click += BtnLapDB_Click;

            top.Controls.Add(Lbl("Số phiếu đền bù:")); top.Controls.Add(txtSoDB);
            top.Controls.Add(Lbl("Mức độ thiệt hại:")); top.Controls.Add(txtMucDo);
            top.Controls.Add(Lbl("Số tiền đền bù:")); top.Controls.Add(numDenBu);
            top.Controls.Add(new Label()); top.Controls.Add(btnThemDB);
            top.Controls.Add(new Label()); top.Controls.Add(btnLapDB);

            var noteLbl = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Text = "Chọn tiện nghi hư hỏng/mất ở tab trước (dòng đang chọn trong lưới Tiện nghi) rồi bấm Thêm vào phiếu."
            };

            dgvDBChon = Grid("dgvDBChon");
            tp.Controls.Add(dgvDBChon);
            tp.Controls.Add(top);
            tp.Controls.Add(noteLbl);
            return tp;
        }

        private TabPage TabHoaDonThanhToan()
        {
            var tp = new TabPage("Hóa đơn - Thanh toán - Trả phòng");

            var top = NewFormPanel(4, 3);
            txtSoHD = Txt("txtSoHD"); numSoNgay = Num("numSoNgay", 365);
            cboNV2 = Cbo("cboNV2");
            var btnLapHD = Btn("btnLapHD", "Lập hóa đơn", 150);
            btnLapHD.Click += BtnLapHD_Click;

            txtMaTT = Txt("txtMaTT"); cboHT = Cbo("cboHT");
            numTienTT = Num("numTienTT", 1000000000);
            var btnThanhToan = Btn("btnThanhToan", "Ghi thanh toán", 150);
            btnThanhToan.Click += BtnThanhToan_Click;

            var btnTraPhong = Btn("btnTraPhong", "Hoàn tất trả phòng", 170);
            btnTraPhong.Click += BtnTraPhong_Click;

            cboNV2 = Cbo("cboNV2");
            cboNV = Cbo("cboNV");

            top.Controls.Add(Lbl("Số hóa đơn:")); top.Controls.Add(txtSoHD);
            top.Controls.Add(Lbl("Số ngày tính tiền:")); top.Controls.Add(numSoNgay);
            top.Controls.Add(Lbl("Nhân viên lập:")); top.Controls.Add(cboNV2);
            top.Controls.Add(new Label()); top.Controls.Add(btnLapHD);

            top.Controls.Add(Lbl("Mã thanh toán:")); top.Controls.Add(txtMaTT);
            top.Controls.Add(Lbl("Hình thức:")); top.Controls.Add(cboHT);
            top.Controls.Add(Lbl("Số tiền:")); top.Controls.Add(numTienTT);
            top.Controls.Add(new Label()); top.Controls.Add(btnThanhToan);



            txtHDChon = Txt("txtHDChon"); txtHDChon.ReadOnly = true;
            top.Controls.Add(Lbl("Hóa đơn đang chọn:")); top.Controls.Add(txtHDChon);
            top.Controls.Add(new Label()); top.Controls.Add(btnTraPhong);

            dgvHD = Grid("dgvHD");
            dgvHD.SelectionChanged += DgvHD_SelectionChanged;

            tp.Controls.Add(dgvHD);
            tp.Controls.Add(top);
            return tp;
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            cboDat.DataSource = svc.LayPhieuDangO();
            cboDat.DisplayMember = "SoPhieuDat"; cboDat.ValueMember = "SoPhieuDat";

            cboNV.DataSource = dm.LayNhanVien();
            cboNV.DisplayMember = "HoTen"; cboNV.ValueMember = "MaNV";

            cboNV2.DataSource = dm.LayNhanVien();
            cboNV2.DisplayMember = "HoTen"; cboNV2.ValueMember = "MaNV";

            cboHT.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ", "Ví điện tử" });
            cboHT.SelectedIndex = 0;

            dgvDBChon.DataSource = db;

            Tai();
        }

        private static string V(ComboBox c) => c.SelectedValue == null ? "" : c.SelectedValue.ToString();

        private void Tai()
        {
            if (cboDat.SelectedValue != null)
                dgvPhong.DataSource = svc.LayPhongTheoPhieu(V(cboDat));
            dgvHD.DataSource = svc.LayHoaDon();
        }

        private void DgvPhong_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow == null) return;
            txtPhong.Text = Convert.ToString(dgvPhong.CurrentRow.Cells["SoPhong"].Value);
            dgvTN.DataSource = svc.LayTienNghiPhong(txtPhong.Text);
        }

        private void BtnThemDB_Click(object sender, EventArgs e)
        {
            if (dgvTN.CurrentRow == null) return;
            string ma = Convert.ToString(dgvTN.CurrentRow.Cells["MaTienNghi"].Value);
            string ten = Convert.ToString(dgvTN.CurrentRow.Cells["TenLoaiTN"].Value);
            foreach (var x in db)
                if (x.MaTienNghi == ma) { MessageBox.Show("Tiện nghi đã có trong phiếu đền bù."); return; }

            db.Add(new DenBuItem
            {
                MaTienNghi = ma,
                TenLoaiTN = ten,
                MucDoThietHai = txtMucDo.Text.Trim(),
                SoTien = numDenBu.Value
            });
        }

        private void BtnLapDB_Click(object sender, EventArgs e)
        {
            var k = svc.LapPhieuDenBu(txtSoDB.Text.Trim(), V(cboDat), txtPhong.Text.Trim(), DateTime.Now,
                V(cboNV), new List<DenBuItem>(db));
            H(k);
            if (k.ThanhCong) db.Clear();
        }

        private void BtnLapHD_Click(object sender, EventArgs e)
        {
            var k = svc.LapHoaDon(txtSoHD.Text.Trim(), V(cboDat), DateTime.Now, V(cboNV2), (int)numSoNgay.Value);
            H(k);
        }

        private void DgvHD_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHD.CurrentRow != null)
                txtHDChon.Text = Convert.ToString(dgvHD.CurrentRow.Cells["SoHoaDon"].Value);
        }

        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            var k = svc.ThanhToan(txtMaTT.Text.Trim(), txtHDChon.Text.Trim(), DateTime.Now, cboHT.Text, numTienTT.Value);
            H(k);
        }

        private void BtnTraPhong_Click(object sender, EventArgs e)
        {
            var k = svc.TraPhong(V(cboDat), DateTime.Now);
            H(k);
        }

        private void H(KetQuaXuLy k)
        {
            MessageBox.Show(k.ThongBao, k.ThanhCong ? "Thành công" : "Không thể thực hiện",
                MessageBoxButtons.OK, k.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (k.ThanhCong) Tai();
        }
    }
}
