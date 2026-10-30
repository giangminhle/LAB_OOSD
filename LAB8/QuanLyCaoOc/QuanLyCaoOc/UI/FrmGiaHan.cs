using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Models;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    /// <summary>Lap phieu gia han: chi khi khong no tien thue va da tra du dich vu.</summary>
    public class FrmGiaHan : Form
    {
        private readonly GiaHanService _svc = new GiaHanService();
        private readonly ComboBox cboPhong;
        private readonly NumericUpDown numThang;
        private readonly DateTimePicker dtNgay;
        private readonly TextBox txtNguoiLap;
        private readonly Label lblThongTin;
        private readonly DataGridView grid;

        public FrmGiaHan()
        {
            Text = "Lập phiếu gia hạn thuê văn phòng";
            ClientSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            Controls.Add(UiHelper.Lbl("Phòng đang thuê:", 20, 15));
            cboPhong = UiHelper.Cbo(135, 15, 450);
            Controls.Add(cboPhong);
            Controls.Add(UiHelper.Lbl("Ngày lập:", 20, 52));
            dtNgay = UiHelper.Date(135, 52, 140);
            Controls.Add(dtNgay);
            Controls.Add(UiHelper.Lbl("Số tháng gia hạn:", 300, 52));
            numThang = UiHelper.Num(415, 52, 60, 70);
            numThang.Minimum = 1;
            numThang.Value = 3;
            Controls.Add(numThang);
            Controls.Add(UiHelper.Lbl("Người lập:", 20, 89));
            txtNguoiLap = UiHelper.Txt(135, 89, 250);
            txtNguoiLap.Text = "Ban quản lý cao ốc";
            Controls.Add(txtNguoiLap);

            var btnKiemTra = UiHelper.Btn("Kiểm tra công nợ", 600, 12, 160);
            var btnLap = UiHelper.Btn("Lập phiếu gia hạn", 600, 52, 160);
            Controls.Add(btnKiemTra);
            Controls.Add(btnLap);

            lblThongTin = new Label
            {
                Left = 20, Top = 125, Width = 860, Height = 80, BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(6), Text = "Chọn phòng rồi nhấn 'Kiểm tra công nợ'."
            };
            Controls.Add(lblThongTin);
            Controls.Add(new Label { Text = "Các phiếu gia hạn đã lập:", Left = 20, Top = 215, Width = 300, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            grid = UiHelper.Grid(20, 238, 860, 305);
            Controls.Add(grid);

            btnKiemTra.Click += (s, e) => KiemTraCongNo();
            btnLap.Click += (s, e) => Lap();
            cboPhong.SelectedIndexChanged += (s, e) => KiemTraCongNo();
            Load += (s, e) => TaiLai();
        }

        private int MaCTHD => cboPhong.SelectedValue is int id ? id : 0;

        private void TaiLai()
        {
            UiHelper.RefreshCombo(cboPhong, _svc.DanhSachPhongDangThue(), "MaCTHD", "HienThi");
            grid.DataSource = _svc.DanhSachPhieu();
        }

        private void KiemTraCongNo()
        {
            if (MaCTHD <= 0) return;
            ThongTinGiaHan t = _svc.LayThongTin(MaCTHD, dtNgay.Value.Date);
            if (t == null) return;
            string loi = GiaHanService.KiemTraDieuKien(t, (int)numThang.Value, dtNgay.Value.Date);
            lblThongTin.Text = "Phòng " + t.MaPhongCode + " - thuê " + t.SoThangThue + " tháng, hết hạn "
                + t.NgayKetThuc.ToString("dd/MM/yyyy") + ", được gia hạn tối đa " + GiaHanService.SoThangGiaHanToiDa(t.SoThangThue) + " tháng.\n"
                + "Nợ tiền thuê: " + t.SoHoaDonThueNo + " hóa đơn (" + t.TienThueNo.ToString("N0") + " đ). "
                + "Dịch vụ chưa trả: " + t.SoHoaDonDichVuNo + " hóa đơn (" + t.TienDichVuNo.ToString("N0") + " đ).\n"
                + (loi == null ? "→ Đủ điều kiện gia hạn." : "→ Không gia hạn được: " + loi);
            lblThongTin.ForeColor = loi == null ? Color.DarkGreen : Color.Firebrick;
        }

        private void Lap()
        {
            ServiceResult r = _svc.LapPhieuGiaHan(new GiaHanInput
            {
                MaCTHD = MaCTHD, SoThangGiaHan = (int)numThang.Value,
                NgayLap = dtNgay.Value.Date, NguoiLap = txtNguoiLap.Text
            });
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Không gia hạn được", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) { TaiLai(); KiemTraCongNo(); }
        }
    }
}
