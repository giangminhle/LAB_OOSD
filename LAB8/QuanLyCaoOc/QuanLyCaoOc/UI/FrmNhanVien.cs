using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Models;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    /// <summary>Cong ty bao cao nhan vien moi; ban quan ly cap the giu xe.</summary>
    public class FrmNhanVien : Form
    {
        private readonly NhanVienService _svc = new NhanVienService();
        private readonly HopDongService _hd = new HopDongService();
        private readonly TextBox txtHoTen, txtCCCD, txtChucVu, txtDienThoai, txtAnh, txtBienSo, txtMau;
        private readonly ComboBox cboCongTy, cboPhong, cboLoaiXe;
        private readonly DateTimePicker dtNgay;
        private readonly PictureBox pic;
        private readonly DataGridView grid;

        public FrmNhanVien()
        {
            Text = "Nhân viên văn phòng và thẻ giữ xe";
            ClientSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            var grp = UiHelper.Group("Báo cáo nhân viên mới", 20, 10, 600, 250);
            grp.Controls.Add(UiHelper.Lbl("Họ tên:", 15, 25)); txtHoTen = UiHelper.Txt(110, 25, 260); grp.Controls.Add(txtHoTen);
            grp.Controls.Add(UiHelper.Lbl("CCCD:", 15, 58)); txtCCCD = UiHelper.Txt(110, 58, 260); grp.Controls.Add(txtCCCD);
            grp.Controls.Add(UiHelper.Lbl("Công ty:", 15, 91)); cboCongTy = UiHelper.Cbo(110, 91, 260); grp.Controls.Add(cboCongTy);
            grp.Controls.Add(UiHelper.Lbl("Phòng:", 15, 124)); cboPhong = UiHelper.Cbo(110, 124, 260); grp.Controls.Add(cboPhong);
            grp.Controls.Add(UiHelper.Lbl("Chức vụ:", 15, 157)); txtChucVu = UiHelper.Txt(110, 157, 260); grp.Controls.Add(txtChucVu);
            grp.Controls.Add(UiHelper.Lbl("Điện thoại:", 15, 190)); txtDienThoai = UiHelper.Txt(110, 190, 120); grp.Controls.Add(txtDienThoai);
            grp.Controls.Add(UiHelper.Lbl("Ngày báo:", 240, 190, 65)); dtNgay = UiHelper.Date(305, 190, 105); grp.Controls.Add(dtNgay);

            pic = new PictureBox { Left = 420, Top = 25, Width = 120, Height = 150, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            grp.Controls.Add(pic);
            var btnAnh = UiHelper.Btn("Chọn ảnh...", 420, 180, 120, 28);
            grp.Controls.Add(btnAnh);
            txtAnh = new TextBox { Visible = false };
            grp.Controls.Add(txtAnh);
            var btnLuu = UiHelper.Btn("Ghi nhận", 420, 212, 120, 30);
            grp.Controls.Add(btnLuu);
            Controls.Add(grp);

            var grpXe = UiHelper.Group("Cấp thẻ giữ xe (chọn nhân viên ở bảng dưới)", 640, 10, 340, 250);
            grpXe.Controls.Add(UiHelper.Lbl("Loại xe:", 15, 30, 80));
            cboLoaiXe = UiHelper.Cbo(100, 30, 200);
            cboLoaiXe.Items.AddRange(new object[] { "XeMay", "OTo" });
            cboLoaiXe.SelectedIndex = 0;
            grpXe.Controls.Add(cboLoaiXe);
            grpXe.Controls.Add(UiHelper.Lbl("Biển số:", 15, 68, 80)); txtBienSo = UiHelper.Txt(100, 68, 200); grpXe.Controls.Add(txtBienSo);
            grpXe.Controls.Add(new Label { Text = "VD: 59X1-234.56 hoặc 51A-123.45", Left = 100, Top = 94, Width = 220, ForeColor = Color.DimGray });
            grpXe.Controls.Add(UiHelper.Lbl("Màu xe:", 15, 120, 80)); txtMau = UiHelper.Txt(100, 120, 200); grpXe.Controls.Add(txtMau);
            var btnThe = UiHelper.Btn("Cấp thẻ", 100, 165, 140);
            grpXe.Controls.Add(btnThe);
            Controls.Add(grpXe);

            grid = UiHelper.Grid(20, 275, 960, 330);
            Controls.Add(grid);

            cboCongTy.SelectedIndexChanged += (s, e) =>
            {
                if (cboCongTy.SelectedValue is int kh)
                    UiHelper.RefreshCombo(cboPhong, _svc.PhongCuaCongTy(kh), "MaPhong", "MaPhongCode");
            };
            btnAnh.Click += (s, e) =>
            {
                using (var dlg = new OpenFileDialog { Filter = "Ảnh|*.jpg;*.jpeg;*.png;*.bmp" })
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        txtAnh.Text = dlg.FileName;
                        pic.ImageLocation = dlg.FileName;
                    }
            };
            btnLuu.Click += (s, e) => Ghi();
            btnThe.Click += (s, e) => CapThe();
            Load += (s, e) =>
            {
                UiHelper.RefreshCombo(cboCongTy, _hd.DanhSachKhachHang(), "MaKH", "TenCongTy");
                TaiLai();
            };
        }

        private void TaiLai()
        {
            grid.DataSource = _svc.DanhSachNhanVien();
            if (grid.Columns.Contains("MaNV")) grid.Columns["MaNV"].Visible = false;
        }

        private void Ghi()
        {
            var r = _svc.BaoCaoNhanVienMoi(new NhanVienInput
            {
                HoTen = txtHoTen.Text, CCCD = txtCCCD.Text, HinhAnh = txtAnh.Text,
                MaKH = cboCongTy.SelectedValue is int kh ? kh : 0,
                MaPhong = cboPhong.SelectedValue is int p ? p : 0,
                ChucVu = txtChucVu.Text, DienThoai = txtDienThoai.Text, NgayBaoCao = dtNgay.Value.Date
            });
            Thong(r);
        }

        private void CapThe()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("Chưa chọn nhân viên."); return; }
            int maNV = Convert.ToInt32(grid.CurrentRow.Cells["MaNV"].Value);
            var r = _svc.CapTheGiuXe(new TheXeInput
            {
                MaNV = maNV, LoaiXe = cboLoaiXe.Text, BienSo = txtBienSo.Text, MauXe = txtMau.Text, NgayCap = DateTime.Today
            });
            Thong(r);
        }

        private void Thong(ServiceResult r)
        {
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) TaiLai();
        }
    }
}
