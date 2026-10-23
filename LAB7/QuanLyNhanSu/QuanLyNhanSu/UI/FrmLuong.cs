using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Models;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    /// <summary>
    /// Form tính / chốt bảng lương tháng.
    /// Chỉ gọi Service, không chứa công thức tính lương.
    /// </summary>
    public class FrmLuong : Form
    {
        private readonly LuongService _svc = new LuongService();
        private NumericUpDown numThang, numNam;
        private TextBox txtNguoiLap;
        private Button btnTinh, btnChot, btnTaiLai;
        private DataGridView dgvBangLuong, dgvChiTiet;
        private Label lblTrangThai, lblTong;
        private int _maBangLuong;

        public FrmLuong()
        {
            Text = "Tính / chốt bảng lương tháng";
            ClientSize = new Size(1060, 800);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(UiHelper.Lbl("Tháng:", 20, 20, 60));
            numThang = UiHelper.Num(85, 20, 12, 70);
            numThang.Minimum = 1;
            numThang.Maximum = 12;
            numThang.Value = 10;
            Controls.Add(numThang);

            Controls.Add(UiHelper.Lbl("Năm:", 175, 20, 50));
            numNam = UiHelper.Num(230, 20, 2100, 90);
            numNam.Minimum = 2000;
            numNam.Maximum = 2100;
            numNam.Value = 2026;
            Controls.Add(numNam);

            Controls.Add(UiHelper.Lbl("Người lập:", 345, 20, 80));
            txtNguoiLap = UiHelper.Txt(430, 20, 220);
            txtNguoiLap.Text = "Trưởng phòng HCNS Lê Thị Mai";
            Controls.Add(txtNguoiLap);

            btnTinh = UiHelper.Btn("Tính bảng lương", 670, 18, 150);
            btnChot = UiHelper.Btn("Chốt bảng lương", 830, 18, 150);
            btnTaiLai = UiHelper.Btn("Tải lại", 990, 18, 50);
            btnTaiLai.Width = 50;
            Controls.AddRange(new Control[] { btnTinh, btnChot, btnTaiLai });

            lblTrangThai = new Label
            {
                Left = 20, Top = 62, Width = 1020, Height = 24,
                ForeColor = Color.DimGray,
                Text = "Công thức: Lương/26×ngày công + Lương/26/8×1,5×giờ phụ trội + phụ cấp + thưởng − phạt − bảo hiểm."
            };
            Controls.Add(lblTrangThai);

            Controls.Add(new Label
            {
                Text = "Các bảng lương đã lập:", Left = 20, Top = 92,
                Width = 400, Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });
            dgvBangLuong = UiHelper.Grid(20, 114, 1020, 150);
            Controls.Add(dgvBangLuong);

            Controls.Add(new Label
            {
                Text = "Chi tiết bảng lương đang chọn:", Left = 20, Top = 274,
                Width = 400, Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });
            dgvChiTiet = UiHelper.Grid(20, 296, 1020, 420);
            Controls.Add(dgvChiTiet);

            lblTong = new Label
            {
                Left = 20, Top = 728, Width = 1020, Height = 26,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(30, 80, 140)
            };
            Controls.Add(lblTong);

            btnTinh.Click += (s, e) => Tinh();
            btnChot.Click += (s, e) => Chot();
            btnTaiLai.Click += (s, e) => { LoadBangLuong(); TaiChiTiet(); };
            dgvBangLuong.SelectionChanged += (s, e) => TaiChiTiet();
            Load += (s, e) => LoadBangLuong();
        }

        private void Tinh()
        {
            int thang = (int)numThang.Value;
            int nam = (int)numNam.Value;
            if (MessageBox.Show("Tính lại bảng lương tháng " + thang + "/" + nam + "?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            ServiceResult r = _svc.TaoBangLuong(thang, nam, txtNguoiLap.Text.Trim());
            HienTrangThai(r);
            if (r.Success)
            {
                _maBangLuong = r.Id;
                LoadBangLuong();
                ChonBangLuong(_maBangLuong);
                TaiChiTiet();
            }
        }

        private void Chot()
        {
            int ma = LayMaBangLuongDangChon();
            if (ma <= 0)
            {
                MessageBox.Show("Vui lòng chọn một bảng lương cần chốt.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Chốt bảng lương này? Sau khi chốt sẽ không tính lại được.",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            ServiceResult r = _svc.Chot(ma);
            HienTrangThai(r);
            LoadBangLuong();
            ChonBangLuong(ma);
        }

        private void HienTrangThai(ServiceResult r)
        {
            lblTrangThai.Text = r.Message;
            lblTrangThai.ForeColor = r.Success ? Color.DarkGreen : Color.Firebrick;
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK, r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void LoadBangLuong()
        {
            DataTable dt = _svc.DanhSachBangLuong();
            dgvBangLuong.DataSource = dt;
            if (dgvBangLuong.Columns["TongChiPhi"] != null)
                dgvBangLuong.Columns["TongChiPhi"].DefaultCellStyle.Format = "N0";
            if (dgvBangLuong.Columns["MaBangLuong"] != null)
                dgvBangLuong.Columns["MaBangLuong"].HeaderText = "Mã";
            if (dgvBangLuong.Columns["NgayLap"] != null)
                dgvBangLuong.Columns["NgayLap"].HeaderText = "Ngày lập";
            if (dgvBangLuong.Columns["NguoiLap"] != null)
                dgvBangLuong.Columns["NguoiLap"].HeaderText = "Người lập";
            if (dgvBangLuong.Columns["TrangThai"] != null)
                dgvBangLuong.Columns["TrangThai"].HeaderText = "Trạng thái";
            if (dgvBangLuong.Columns["TongChiPhi"] != null)
                dgvBangLuong.Columns["TongChiPhi"].HeaderText = "Tổng thực lĩnh";
            if (dt.Rows.Count > 0 && _maBangLuong <= 0)
                _maBangLuong = Convert.ToInt32(dt.Rows[0]["MaBangLuong"]);
        }

        private void ChonBangLuong(int ma)
        {
            foreach (DataGridViewRow row in dgvBangLuong.Rows)
            {
                if (row.DataBoundItem is DataRowView drv
                    && Convert.ToInt32(drv["MaBangLuong"]) == ma)
                {
                    row.Selected = true;
                    return;
                }
            }
        }

        private int LayMaBangLuongDangChon()
        {
            if (dgvBangLuong.CurrentRow == null) return 0;
            if (dgvBangLuong.CurrentRow.DataBoundItem is DataRowView drv)
                return Convert.ToInt32(drv["MaBangLuong"]);
            return 0;
        }

        private void TaiChiTiet()
        {
            int ma = LayMaBangLuongDangChon();
            if (ma <= 0)
            {
                dgvChiTiet.DataSource = null;
                lblTong.Text = "";
                return;
            }
            _maBangLuong = ma;
            DataTable dt = _svc.DanhSachChiTiet(ma);
            dgvChiTiet.DataSource = dt;
            if (dgvChiTiet.Columns["MucLuongCoBan"] != null)
                dgvChiTiet.Columns["MucLuongCoBan"].HeaderText = "Lương CB";
            if (dgvChiTiet.Columns["SoNgayCong"] != null)
                dgvChiTiet.Columns["SoNgayCong"].HeaderText = "Ngày công";
            if (dgvChiTiet.Columns["GioPhuTroi"] != null)
                dgvChiTiet.Columns["GioPhuTroi"].HeaderText = "Giờ PT";
            if (dgvChiTiet.Columns["TienPhuTroi"] != null)
                dgvChiTiet.Columns["TienPhuTroi"].HeaderText = "Tiền PT";
            if (dgvChiTiet.Columns["PhuCapChucVu"] != null)
                dgvChiTiet.Columns["PhuCapChucVu"].HeaderText = "PC chức vụ";
            if (dgvChiTiet.Columns["PhuCapLuong"] != null)
                dgvChiTiet.Columns["PhuCapLuong"].HeaderText = "PC lương";
            if (dgvChiTiet.Columns["PhuCapDocHai"] != null)
                dgvChiTiet.Columns["PhuCapDocHai"].HeaderText = "PC độc hại";
            if (dgvChiTiet.Columns["TienKhenThuong"] != null)
                dgvChiTiet.Columns["TienKhenThuong"].HeaderText = "Thưởng";
            if (dgvChiTiet.Columns["TienKyLuat"] != null)
                dgvChiTiet.Columns["TienKyLuat"].HeaderText = "Phạt";
            UiHelper.FormatMoneyGrid(dgvChiTiet);

            decimal tong = 0m;
            foreach (DataRow r in dt.Rows)
                tong += Convert.ToDecimal(r["ThucLinh"]);
            lblTong.Text = "Tổng thực lĩnh: " + tong.ToString("N0") + " đ   |   Mã bảng lương: " + ma;
        }
    }
}
