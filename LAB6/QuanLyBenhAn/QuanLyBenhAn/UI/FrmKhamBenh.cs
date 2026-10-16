using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Data;
using QuanLyBenhAn.Models;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.UI
{
    /// <summary>Module 2: tra cuu ho so -> tao phieu kham -> XN -> toa -> thanh toan -> hoan tat.</summary>
    public class FrmKhamBenh : Form
    {
        private TextBox txtTuKhoa, txtMaHoSo, txtTrieuChung, txtHuyetAp, txtChanDoan, txtLoaiXN, txtGhiChu;
        private NumericUpDown numMaBS, numMaKhoa, numMaPhieu, numNhiet, numTien;
        private DataGridView gridLichSu, gridToa;
        private ComboBox cboLoaiKham;
        private KhamBenhService svc = new KhamBenhService();
        private BenhNhanService bnSvc = new BenhNhanService();

        public FrmKhamBenh()
        {
            Text = "Kham benh - Xet nghiem - Toa thuoc - Thanh toan - Hoan tat";
            ClientSize = new Size(860, 620);
            StartPosition = FormStartPosition.CenterParent;

            int x = 15, y = 12;
            Controls.Add(new Label { Text = "Tu khoa (MaHS/HoTen/SDT):", Left = x, Top = y, Width = 200 });
            txtTuKhoa = new TextBox { Left = x + 205, Top = y - 3, Width = 300 };
            var btnTim = new Button { Text = "Tra cuu", Left = x + 515, Top = y - 5, Width = 90 };
            btnTim.Click += (s, e) => { gridLichSu.DataSource = null; TraCuu(); };
            Controls.AddRange(new Control[] { txtTuKhoa, btnTim }); y += 32;

            Controls.Add(new Label { Text = "MaHoSo:", Left = x, Top = y, Width = 70 });
            txtMaHoSo = new TextBox { Left = x + 75, Top = y - 3, Width = 90 };
            var btnLichSu = new Button { Text = "Xem lich su kham", Left = x + 175, Top = y - 5, Width = 140 };
            btnLichSu.Click += (s, e) => XemLichSu();
            Controls.AddRange(new Control[] { txtMaHoSo, btnLichSu }); y += 32;

            gridLichSu = new DataGridView { Left = x, Top = y, Width = 830, Height = 130, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            Controls.Add(gridLichSu); y += 140;

            // Tao phieu kham
            Controls.Add(new Label { Text = "MaBS:", Left = x, Top = y, Width = 55 });
            numMaBS = new NumericUpDown { Left = x + 60, Top = y - 3, Width = 70, Minimum = 1, Maximum = 100000, Value = 1 };
            Controls.Add(new Label { Text = "MaKhoa:", Left = x, Top = y, Width = 60 });
            numMaKhoa = new NumericUpDown { Left = x + 205, Top = y - 3, Width = 70, Minimum = 1, Maximum = 100000, Value = 1 };
            Controls.Add(new Label { Text = "Loai:", Left = x + 285, Top = y, Width = 40 });
            cboLoaiKham = new ComboBox { Left = x + 325, Top = y - 3, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoaiKham.Items.AddRange(new object[] { "1-DieuTri", "2-DinhKyQuy" }); cboLoaiKham.SelectedIndex = 0;
            Controls.Add(new Label { Text = "Nhiet do:", Left = x + 475, Top = y, Width = 60 });
            numNhiet = new NumericUpDown { Left = x + 540, Top = y - 3, Width = 80, Minimum = 34, Maximum = 42, DecimalPlaces = 1, Increment = 0.1m, Value = 37 };
            Controls.Add(new Label { Text = "HA:", Left = x + 630, Top = y, Width = 30 });
            txtHuyetAp = new TextBox { Left = x + 660, Top = y - 3, Width = 80, Text = "120/80" };
            Controls.AddRange(new Control[] { numMaBS, numMaKhoa, cboLoaiKham, numNhiet, txtHuyetAp }); y += 30;

            Controls.Add(new Label { Text = "Trieu chung:", Left = x, Top = y, Width = 90 });
            txtTrieuChung = new TextBox { Left = x + 95, Top = y - 3, Width = 735 };
            Controls.AddRange(new Control[] { txtTrieuChung }); y += 30;
            Controls.Add(new Label { Text = "Chan doan:", Left = x, Top = y, Width = 90 });
            txtChanDoan = new TextBox { Left = x + 95, Top = y - 3, Width = 735 };
            Controls.AddRange(new Control[] { txtChanDoan }); y += 32;
            var btnTaoPhieu = new Button { Text = "Tao phieu kham", Left = x, Top = y, Width = 200 };
            btnTaoPhieu.Click += (s, e) => TaoPhieu();
            Controls.Add(btnTaoPhieu); y += 36;

            // XN / Toa / Thanh toan / Hoan tat
            Controls.Add(new Label { Text = "MaPhieu:", Left = x, Top = y, Width = 65 });
            numMaPhieu = new NumericUpDown { Left = x + 70, Top = y - 3, Width = 90, Minimum = 1, Maximum = 1000000 };
            Controls.Add(new Label { Text = "Loai XN:", Left = x + 170, Top = y, Width = 60 });
            txtLoaiXN = new TextBox { Left = x + 230, Top = y - 3, Width = 150, Text = "Mau" };
            var btnXN = new Button { Text = "Chi dinh XN", Left = x + 390, Top = y - 5, Width = 110 };
            btnXN.Click += (s, e) => Bao(svc.ChiDinhXetNghiem((int)numMaPhieu.Value, txtLoaiXN.Text));
            Controls.Add(new Label { Text = "Tien VP:", Left = x + 510, Top = y, Width = 55 });
            numTien = new NumericUpDown { Left = x + 570, Top = y - 3, Width = 120, Minimum = 0, Maximum = 1000000000, ThousandsSeparator = true };
            var btnTT = new Button { Text = "Thanh toan", Left = x + 700, Top = y - 5, Width = 60 };
            btnTT.Click += (s, e) => Bao(svc.ThanhToanVienPhi((int)numMaPhieu.Value, numTien.Value));
            var btnHT = new Button { Text = "Hoan tat", Left = x + 765, Top = y - 5, Width = 65 };
            btnHT.Click += (s, e) => Bao(svc.HoanTatDieuTri((int)numMaPhieu.Value));
            Controls.AddRange(new Control[] { numMaPhieu, txtLoaiXN, btnXN, numTien, btnTT, btnHT }); y += 32;

            Controls.Add(new Label { Text = "Toa: MaThuoc | LieuLuong | SoLuong | CachDung | GhiChu:", Left = x, Top = y, Width = 420 });
            txtGhiChu = new TextBox { Left = x + 430, Top = y - 3, Width = 270 };
            var btnToa = new Button { Text = "Lap toa thuoc", Left = x + 710, Top = y - 5, Width = 120 };
            btnToa.Click += (s, e) => LapToa();
            Controls.AddRange(new Control[] { txtGhiChu, btnToa }); y += 30;

            gridToa = new DataGridView { Left = x, Top = y, Width = 830, Height = 100, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            gridToa.Columns.Add("MaThuoc", "MaThuoc"); gridToa.Columns.Add("LieuLuong", "LieuLuong");
            gridToa.Columns.Add("SoLuong", "SoLuong"); gridToa.Columns.Add("CachDung", "CachDung");
            gridToa.Rows.Add("1", "1 vien/lan, ngay 2 lan", "20", "Uong sau an");
            Controls.Add(gridToa);
        }

        private void Bao(ServiceResult r)
        {
            MessageBox.Show(r.Message, r.Success ? "Thanh cong" : "Loi", MessageBoxButtons.OK,
                r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (r.Success) XemLichSu();
        }

        private void TraCuu()
        {
            try { gridLichSu.DataSource = bnSvc.TraCuuHoSo(txtTuKhoa.Text); }
            catch (Exception ex) { MessageBox.Show("Loi DB: " + ex.Message); }
        }

        private void XemLichSu()
        {
            int hs;
            if (!int.TryParse(txtMaHoSo.Text, out hs)) { MessageBox.Show("Nhap MaHoSo truoc."); return; }
            try { gridLichSu.DataSource = bnSvc.LichSuKham(hs); }
            catch (Exception ex) { MessageBox.Show("Loi DB: " + ex.Message); }
        }

        private void TaoPhieu()
        {
            int hs;
            if (!int.TryParse(txtMaHoSo.Text, out hs)) { MessageBox.Show("Nhap MaHoSo truoc."); return; }
            var r = svc.TaoPhieuKham(new PhieuKhamInput
            {
                MaHoSo = hs, MaBS = (int)numMaBS.Value, MaKhoa = (int)numMaKhoa.Value,
                TrieuChung = txtTrieuChung.Text, ThanNhiet = numNhiet.Value,
                HuyetAp = txtHuyetAp.Text, ChanDoan = txtChanDoan.Text,
                LoaiKham = cboLoaiKham.SelectedIndex == 1 ? LoaiKham.DinhKyQuy : LoaiKham.DieuTri
            });
            if (r.Success) numMaPhieu.Value = r.Id;
            Bao(r);
        }

        private void LapToa()
        {
            var list = new List<ChiTietToaInput>();
            foreach (DataGridViewRow row in gridToa.Rows)
            {
                if (row.IsNewRow) continue;
                object a = row.Cells[0].Value, b = row.Cells[1].Value, c = row.Cells[2].Value;
                if (a == null || b == null || c == null) continue;
                int maThuoc, sl;
                if (!int.TryParse(a.ToString(), out maThuoc)) continue;
                if (!int.TryParse(c.ToString(), out sl)) continue;
                list.Add(new ChiTietToaInput
                {
                    MaThuoc = maThuoc, LieuLuong = b.ToString(), SoLuong = sl,
                    CachDung = row.Cells[3].Value == null ? null : row.Cells[3].Value.ToString()
                });
            }
            Bao(svc.LapToaThuoc((int)numMaPhieu.Value, (int)numMaBS.Value, list, txtGhiChu.Text));
        }
    }
}
