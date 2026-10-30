using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Models;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    /// <summary>Kiem tra tinh trang phong hang nam, phat neu tu y thay doi ket cau.</summary>
    public class FrmKiemTra : Form
    {
        private readonly KiemTraPhongService _svc = new KiemTraPhongService();
        private readonly ComboBox cboPhong, cboTinhTrang;
        private readonly DateTimePicker dtNgay;
        private readonly TextBox txtMoTa, txtKeHoach, txtNguoi, txtLyDo;
        private readonly CheckBox chkKetCau;
        private readonly NumericUpDown numPhat;
        private readonly DataGridView grid;

        public FrmKiemTra()
        {
            Text = "Kiểm tra phòng hằng năm";
            ClientSize = new Size(960, 600);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            var grp = UiHelper.Group("Kết quả kiểm tra", 20, 10, 920, 230);
            grp.Controls.Add(UiHelper.Lbl("Ngày kiểm tra:", 15, 25)); dtNgay = UiHelper.Date(130, 25, 140); grp.Controls.Add(dtNgay);
            grp.Controls.Add(UiHelper.Lbl("Phòng chưa KT:", 15, 60)); cboPhong = UiHelper.Cbo(130, 60, 200); grp.Controls.Add(cboPhong);
            grp.Controls.Add(UiHelper.Lbl("Tình trạng:", 15, 95));
            cboTinhTrang = UiHelper.Cbo(130, 95, 140);
            cboTinhTrang.Items.AddRange(new object[] { "Tot", "CanSua", "HuHong" });
            cboTinhTrang.SelectedIndex = 0;
            grp.Controls.Add(cboTinhTrang);
            grp.Controls.Add(UiHelper.Lbl("Mô tả:", 15, 130)); txtMoTa = UiHelper.Txt(130, 130, 300); grp.Controls.Add(txtMoTa);
            grp.Controls.Add(UiHelper.Lbl("Kế hoạch tu sửa:", 15, 165)); txtKeHoach = UiHelper.Txt(130, 165, 300); grp.Controls.Add(txtKeHoach);
            grp.Controls.Add(UiHelper.Lbl("Người kiểm tra:", 15, 198)); txtNguoi = UiHelper.Txt(130, 198, 300);
            txtNguoi.Text = "Ban quản lý cao ốc";
            grp.Controls.Add(txtNguoi);

            chkKetCau = new CheckBox { Text = "Phòng tự ý thay đổi kết cấu → lập phiếu phạt", Left = 470, Top = 25, Width = 400 };
            grp.Controls.Add(chkKetCau);
            grp.Controls.Add(UiHelper.Lbl("Số tiền phạt:", 470, 60, 100)); numPhat = UiHelper.Num(575, 60, 1000000000, 180); grp.Controls.Add(numPhat);
            grp.Controls.Add(UiHelper.Lbl("Lý do phạt:", 470, 95, 100)); txtLyDo = UiHelper.Txt(575, 95, 320); grp.Controls.Add(txtLyDo);
            var btnLuu = UiHelper.Btn("Lưu kết quả kiểm tra", 575, 150, 200);
            grp.Controls.Add(btnLuu);
            Controls.Add(grp);

            grid = UiHelper.Grid(20, 255, 920, 330);
            Controls.Add(grid);

            chkKetCau.CheckedChanged += (s, e) => { numPhat.Enabled = txtLyDo.Enabled = chkKetCau.Checked; };
            numPhat.Enabled = txtLyDo.Enabled = false;
            dtNgay.ValueChanged += (s, e) => TaiPhong();
            btnLuu.Click += (s, e) => Luu();
            Load += (s, e) => TaiLai();
        }

        private void TaiPhong()
        {
            UiHelper.RefreshCombo(cboPhong, _svc.PhongChuaKiemTra(dtNgay.Value.Year), "MaPhong", "MaPhongCode");
        }

        private void TaiLai()
        {
            TaiPhong();
            grid.DataSource = _svc.DanhSachKiemTra();
            UiHelper.FormatMoneyGrid(grid);
        }

        private void Luu()
        {
            var r = _svc.LapKiemTra(new KiemTraInput
            {
                MaPhong = cboPhong.SelectedValue is int p ? p : 0,
                NgayKiemTra = dtNgay.Value.Date,
                TinhTrang = cboTinhTrang.Text,
                CoThayDoiKetCau = chkKetCau.Checked,
                MoTa = txtMoTa.Text,
                KeHoachTuSua = txtKeHoach.Text,
                NguoiKiemTra = txtNguoi.Text,
                SoTienPhat = numPhat.Value,
                LyDoPhat = txtLyDo.Text
            });
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) TaiLai();
        }
    }
}
