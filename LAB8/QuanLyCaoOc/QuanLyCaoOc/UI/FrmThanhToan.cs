using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Models;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    /// <summary>
    /// Tien thue hang thang (tu thang thu 7, tra dau thang), hoa don dich vu cuoi thang va thanh toan.
    /// </summary>
    public class FrmThanhToan : Form
    {
        private readonly ThanhToanService _svc = new ThanhToanService();
        private readonly HopDongService _hd = new HopDongService();
        private readonly ComboBox cboHD;
        private readonly NumericUpDown numThang, numNam;
        private readonly DataGridView gridDV, gridHoaDon;
        private readonly DateTimePicker dtNgay;
        private DataTable _dichVu;

        public FrmThanhToan()
        {
            Text = "Tiền thuê hàng tháng và hóa đơn dịch vụ";
            ClientSize = new Size(980, 640);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            Controls.Add(UiHelper.Lbl("Hợp đồng:", 20, 15, 80));
            cboHD = UiHelper.Cbo(100, 15, 420);
            Controls.Add(cboHD);
            Controls.Add(UiHelper.Lbl("Tháng:", 540, 15, 50));
            numThang = UiHelper.Num(590, 15, 12, 60);
            numThang.Minimum = 1;
            numThang.Value = DateTime.Today.Month;
            Controls.Add(numThang);
            Controls.Add(UiHelper.Lbl("Năm:", 660, 15, 40));
            numNam = UiHelper.Num(700, 15, 2100, 80);
            numNam.Minimum = 2020;
            numNam.Value = DateTime.Today.Year;
            numNam.ThousandsSeparator = false;
            Controls.Add(numNam);
            Controls.Add(UiHelper.Lbl("Ngày lập/TT:", 790, 15, 80));
            dtNgay = UiHelper.Date(870, 15, 100);
            Controls.Add(dtNgay);

            var grpThue = UiHelper.Group("1. Tiền thuê hàng tháng (từ tháng thứ 7)", 20, 50, 300, 90);
            var btnThue = UiHelper.Btn("Lập hóa đơn tiền thuê tháng", 15, 30, 260);
            grpThue.Controls.Add(btnThue);
            Controls.Add(grpThue);

            var grpDV = UiHelper.Group("2. Dịch vụ sử dụng trong tháng (nhập số lượng theo phòng)", 340, 50, 620, 220);
            gridDV = new DataGridView
            {
                Left = 15, Top = 25, Width = 590, Height = 140, AllowUserToAddRows = false,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            grpDV.Controls.Add(gridDV);
            var btnDV = UiHelper.Btn("Lập hóa đơn dịch vụ", 15, 172, 200);
            grpDV.Controls.Add(btnDV);
            Controls.Add(grpDV);

            var grpTT = UiHelper.Group("3. Thanh toán", 20, 150, 300, 120);
            grpTT.Controls.Add(new Label { Text = "Chọn hóa đơn chưa thanh toán ở bảng dưới rồi nhấn:", Left = 15, Top = 25, Width = 270, Height = 35 });
            var btnTT = UiHelper.Btn("Thanh toán hóa đơn", 15, 65, 260);
            grpTT.Controls.Add(btnTT);
            Controls.Add(grpTT);

            Controls.Add(new Label { Text = "Các hóa đơn của hợp đồng:", Left = 20, Top = 282, Width = 300, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            gridHoaDon = UiHelper.Grid(20, 305, 940, 320);
            Controls.Add(gridHoaDon);

            cboHD.SelectedIndexChanged += (s, e) => TaiHopDong();
            btnThue.Click += (s, e) => LapThue();
            btnDV.Click += (s, e) => LapDichVu();
            btnTT.Click += (s, e) => ThanhToan();
            Load += (s, e) =>
            {
                _dichVu = _svc.DanhSachDichVu();
                UiHelper.RefreshCombo(cboHD, HopDongCombo(), "MaHD", "HienThi");
            };
        }

        private DataTable HopDongCombo()
        {
            DataTable dt = _hd.DanhSachHopDong();
            dt.Columns.Add("HienThi", typeof(string));
            foreach (DataRow r in dt.Rows)
                r["HienThi"] = r["SoHD"] + " - " + r["TenCongTy"] + " (" + r["Phong"] + ")";
            return dt;
        }

        private int MaHD => cboHD.SelectedValue is int id ? id : 0;

        private void TaiHopDong()
        {
            if (MaHD <= 0) return;
            var dt = new DataTable();
            dt.Columns.Add("MaPhong", typeof(int));
            dt.Columns.Add("Phong", typeof(string));
            foreach (DataRow dv in _dichVu.Rows)
                dt.Columns.Add("DV" + dv["MaDV"], typeof(decimal)).Caption = dv["TenDV"] + " (" + dv["DonVi"] + ")";
            foreach (DataRow p in _svc.PhongCuaHopDong(MaHD).Rows)
            {
                DataRow r = dt.NewRow();
                r["MaPhong"] = p["MaPhong"];
                r["Phong"] = p["MaPhongCode"];
                foreach (DataRow dv in _dichVu.Rows) r["DV" + dv["MaDV"]] = 0m;
                dt.Rows.Add(r);
            }
            gridDV.DataSource = dt;
            gridDV.Columns["MaPhong"].Visible = false;
            gridDV.Columns["Phong"].ReadOnly = true;
            foreach (DataColumn c in dt.Columns) gridDV.Columns[c.ColumnName].HeaderText = c.Caption;
            gridHoaDon.DataSource = _svc.DanhSachHoaDon(MaHD);
            UiHelper.FormatMoneyGrid(gridHoaDon);
        }

        private void LapThue()
        {
            Show(_svc.LapHoaDonTienThueThang(MaHD, (int)numThang.Value, (int)numNam.Value));
        }

        private void LapDichVu()
        {
            gridDV.EndEdit();
            var x = new HoaDonDichVuInput { MaHD = MaHD, Thang = (int)numThang.Value, Nam = (int)numNam.Value, NgayLap = dtNgay.Value.Date };
            if (gridDV.DataSource is DataTable dt)
                foreach (DataRow r in dt.Rows)
                    foreach (DataRow dv in _dichVu.Rows)
                    {
                        decimal sl = r["DV" + dv["MaDV"]] == DBNull.Value ? 0 : (decimal)r["DV" + dv["MaDV"]];
                        if (sl > 0) x.ChiTiet.Add(new ChiTietDichVuInput { MaPhong = (int)r["MaPhong"], MaDV = (int)dv["MaDV"], SoLuong = sl });
                    }
            Show(_svc.LapHoaDonDichVu(x));
        }

        private void ThanhToan()
        {
            if (gridHoaDon.CurrentRow == null) { MessageBox.Show("Chưa chọn hóa đơn."); return; }
            var row = ((DataRowView)gridHoaDon.CurrentRow.DataBoundItem).Row;
            Show(_svc.ThanhToan((string)row["Loai"], (string)row["SoHoaDon"], dtNgay.Value.Date));
        }

        private void Show(ServiceResult r)
        {
            MessageBox.Show(r.Message, r.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) TaiHopDong();
        }
    }
}
