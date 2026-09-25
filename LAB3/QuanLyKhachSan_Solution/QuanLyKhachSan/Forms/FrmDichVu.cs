using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;
using static QuanLyKhachSan.Forms.UiHelper;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmDichVu : Form
    {
        private readonly DichVuService svc = new DichVuService();
        private readonly DanhMucService dm = new DanhMucService();

        private ComboBox cboLuot, cboDV, cboNV;
        private TextBox txtPhong;
        private DateTimePicker dtNgay;
        private NumericUpDown numSL;
        private DataGridView dgvLichSu;

        public FrmDichVu()
        {
            InitializeComponent();
            Load += Frm_Load;
        }

        private void InitializeComponent()
        {
            Text = "Sử dụng dịch vụ";
            Width = 800;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;

            var top = NewFormPanel(4, 2);
            cboLuot = Cbo("cboLuot", 220);
            cboLuot.SelectedIndexChanged += CboLuot_SelectedIndexChanged;
            txtPhong = Txt("txtPhong"); txtPhong.ReadOnly = true;
            cboDV = Cbo("cboDV");
            dtNgay = Date("dtNgay");
            numSL = Num("numSL", 1000);
            var btnGhi = Btn("btnGhi", "Ghi nhận dịch vụ", 170);
            btnGhi.Click += BtnGhi_Click;

            top.Controls.Add(Lbl("Phiếu đang ở:")); top.Controls.Add(cboLuot);
            top.Controls.Add(Lbl("Phòng:")); top.Controls.Add(txtPhong);
            top.Controls.Add(Lbl("Dịch vụ:")); top.Controls.Add(cboDV);
            top.Controls.Add(Lbl("Ngày sử dụng:")); top.Controls.Add(dtNgay);
            top.Controls.Add(Lbl("Số lượng:")); top.Controls.Add(numSL);
            top.Controls.Add(new Label()); top.Controls.Add(btnGhi);

            // cboNV cần cho tham số maNV của Service (không có trong bảng UI gốc nhưng bắt buộc theo CSDL)
            cboNV = Cbo("cboNV");
            top.Controls.Add(Lbl("Nhân viên ghi nhận:")); top.Controls.Add(cboNV);

            dgvLichSu = Grid("dgvLichSu");

            var btnDong = Btn("btnDong", "Đóng");
            btnDong.Dock = DockStyle.Bottom;
            btnDong.Click += (s, e) => Close();

            Controls.Add(dgvLichSu);
            Controls.Add(top);
            Controls.Add(btnDong);
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            cboLuot.DataSource = svc.LayPhieuDangO();
            cboLuot.DisplayMember = "SoPhieuDat"; cboLuot.ValueMember = "SoPhieuDat";

            cboDV.DataSource = svc.LayDichVu();
            cboDV.DisplayMember = "TenDV"; cboDV.ValueMember = "MaDV";

            cboNV.DataSource = dm.LayNhanVien();
            cboNV.DisplayMember = "HoTen"; cboNV.ValueMember = "MaNV";

            Tai();
        }

        private void Tai()
        {
            if (cboLuot.SelectedValue != null)
                dgvLichSu.DataSource = svc.LayLichSu(cboLuot.SelectedValue.ToString());
        }

        private void CboLuot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLuot.SelectedItem is System.Data.DataRowView r)
                txtPhong.Text = Convert.ToString(r["SoPhong"]);
            Tai();
        }

        private static string V(ComboBox c) => c.SelectedValue == null ? "" : c.SelectedValue.ToString();

        private void BtnGhi_Click(object sender, EventArgs e)
        {
            var k = svc.GhiNhan(V(cboLuot), txtPhong.Text.Trim(), dtNgay.Value, V(cboNV), V(cboDV), (int)numSL.Value);
            MessageBox.Show(k.ThongBao, k.ThanhCong ? "Thành công" : "Không thể thực hiện",
                MessageBoxButtons.OK, k.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (k.ThanhCong) Tai();
        }
    }
}
