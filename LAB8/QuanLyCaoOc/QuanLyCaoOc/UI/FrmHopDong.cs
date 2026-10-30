using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Models;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    /// <summary>Lap hop dong thue nhieu van phong cung luc, tra truoc 6 thang.</summary>
    public class FrmHopDong : Form
    {
        private readonly HopDongService _svc = new HopDongService();
        private readonly PhongService _phong = new PhongService();
        private readonly ComboBox cboKH;
        private readonly CheckedListBox clbPhong;
        private readonly DateTimePicker dtKy, dtBatDau;
        private readonly NumericUpDown numThang;
        private readonly TextBox txtNoiDung;
        private readonly CheckBox chkTraTruoc;
        private readonly Label lblTien;
        private readonly DataGridView grid;

        public FrmHopDong()
        {
            Text = "Lập hợp đồng thuê văn phòng";
            ClientSize = new Size(960, 600);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            var grp = UiHelper.Group("Thông tin hợp đồng", 20, 10, 920, 270);
            grp.Controls.Add(UiHelper.Lbl("Khách hàng:", 15, 28));
            cboKH = UiHelper.Cbo(130, 28, 300);
            grp.Controls.Add(cboKH);
            var btnKH = UiHelper.Btn("+ Khách hàng mới", 440, 25, 140, 28);
            grp.Controls.Add(btnKH);

            grp.Controls.Add(UiHelper.Lbl("Ngày ký:", 15, 65));
            dtKy = UiHelper.Date(130, 65, 140);
            grp.Controls.Add(dtKy);
            grp.Controls.Add(UiHelper.Lbl("Bắt đầu thuê:", 290, 65, 90));
            dtBatDau = UiHelper.Date(385, 65, 140);
            DateTime d = DateTime.Today.AddMonths(1);
            dtBatDau.Value = new DateTime(d.Year, d.Month, 1);
            grp.Controls.Add(dtBatDau);

            grp.Controls.Add(UiHelper.Lbl("Số tháng thuê:", 15, 102));
            numThang = UiHelper.Num(130, 102, 60, 80);
            numThang.Minimum = 6;
            numThang.Value = 12;
            grp.Controls.Add(numThang);
            grp.Controls.Add(new Label { Text = "(ít nhất 6 tháng)", Left = 215, Top = 105, Width = 120, ForeColor = Color.DimGray });

            grp.Controls.Add(UiHelper.Lbl("Nội dung:", 15, 139));
            txtNoiDung = new TextBox
            {
                Left = 130, Top = 139, Width = 450, Height = 70, Multiline = true,
                Text = "Trả trước 6 tháng đầu; từ tháng 7 trả vào đầu mỗi tháng. Giá thuê chưa gồm điện, nước, internet; dịch vụ thanh toán cuối tháng."
            };
            grp.Controls.Add(txtNoiDung);
            chkTraTruoc = new CheckBox { Text = "Khách đã trả trước 6 tháng", Left = 130, Top = 218, Width = 250, Checked = true };
            grp.Controls.Add(chkTraTruoc);

            grp.Controls.Add(new Label { Text = "Chọn các văn phòng trống:", Left = 610, Top = 22, Width = 250 });
            clbPhong = new CheckedListBox { Left = 610, Top = 45, Width = 290, Height = 150, CheckOnClick = true };
            grp.Controls.Add(clbPhong);
            lblTien = new Label { Left = 610, Top = 200, Width = 300, Height = 40, ForeColor = Color.DarkBlue };
            grp.Controls.Add(lblTien);
            Controls.Add(grp);

            var btnLap = UiHelper.Btn("Lập hợp đồng", 20, 290, 160);
            Controls.Add(btnLap);
            grid = UiHelper.Grid(20, 335, 920, 250);
            Controls.Add(grid);

            clbPhong.ItemCheck += (s, e) => BeginInvoke((Action)TinhTien);
            btnLap.Click += (s, e) => Lap();
            btnKH.Click += (s, e) => ThemKhachHang();
            Load += (s, e) => TaiLai();
        }

        private void TaiLai()
        {
            UiHelper.RefreshCombo(cboKH, _svc.DanhSachKhachHang(), "MaKH", "TenCongTy");
            clbPhong.Items.Clear();
            foreach (DataRow r in _phong.PhongTrong().Rows)
                clbPhong.Items.Add(new PhongItem { MaPhong = (int)r["MaPhong"], HienThi = (string)r["HienThi"], Gia = (decimal)r["GiaThue"] });
            grid.DataSource = _svc.DanhSachHopDong();
            UiHelper.FormatMoneyGrid(grid);
            TinhTien();
        }

        private void TinhTien()
        {
            var gia = new List<decimal>();
            foreach (PhongItem p in clbPhong.CheckedItems) gia.Add(p.Gia);
            decimal thang = 0;
            foreach (var g in gia) thang += g;
            lblTien.Text = "Tiền thuê/tháng: " + thang.ToString("N0") + " đ\nTrả trước 6 tháng: "
                + HopDongService.TinhTienTraTruoc(gia).ToString("N0") + " đ";
        }

        private void Lap()
        {
            var x = new HopDongInput
            {
                MaKH = cboKH.SelectedValue == null ? 0 : (int)cboKH.SelectedValue,
                NgayKy = dtKy.Value.Date,
                NgayBatDau = dtBatDau.Value.Date,
                SoThang = (int)numThang.Value,
                NoiDung = txtNoiDung.Text.Trim(),
                DaTraTruoc = chkTraTruoc.Checked
            };
            foreach (PhongItem p in clbPhong.CheckedItems) x.DanhSachPhong.Add(p.MaPhong);
            ServiceResult r = _svc.LapHopDong(x);
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) TaiLai();
        }

        private void ThemKhachHang()
        {
            using (var f = new Form { Text = "Thêm khách hàng", ClientSize = new Size(420, 210), StartPosition = FormStartPosition.CenterParent, Font = Font })
            {
                var t1 = UiHelper.Txt(140, 15, 250); var t2 = UiHelper.Txt(140, 50, 250);
                var t3 = UiHelper.Txt(140, 85, 250); var t4 = UiHelper.Txt(140, 120, 250);
                f.Controls.AddRange(new Control[] {
                    UiHelper.Lbl("Tên công ty:", 15, 15), t1, UiHelper.Lbl("Mã số thuế:", 15, 50), t2,
                    UiHelper.Lbl("Người đại diện:", 15, 85), t3, UiHelper.Lbl("Điện thoại:", 15, 120), t4 });
                var ok = UiHelper.Btn("Lưu", 140, 158, 100);
                f.Controls.Add(ok);
                ok.Click += (s, e) =>
                {
                    var r = _svc.ThemKhachHang(t1.Text, t2.Text, t3.Text, t4.Text);
                    MessageBox.Show(r.Message);
                    if (r.Success) f.Close();
                };
                f.ShowDialog(this);
            }
            UiHelper.RefreshCombo(cboKH, _svc.DanhSachKhachHang(), "MaKH", "TenCongTy");
        }

        private class PhongItem
        {
            public int MaPhong;
            public string HienThi;
            public decimal Gia;
            public override string ToString() { return HienThi; }
        }
    }
}
