using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.UI
{
    public class FrmMain : Form
    {
        private TextBox txtKetQua;

        public FrmMain()
        {
            Text = "LAB7 - Quản lý nhân sự - Công ty Sơn Đông Á";
            ClientSize = new Size(840, 690);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimumSize = new Size(840, 690);

            var lblTieuDe = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ NHÂN SỰ",
                Left = 20, Top = 12, Width = 800, Height = 30,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(30, 80, 140)
            };
            var lblPhu = new Label
            {
                Text = "Tuyển dụng - Thử việc - Hợp đồng - Bảo hiểm - Lương/Chức vụ - Kế hoạch tuần - Chấm công - Phụ trội - Khen thưởng/Kỷ luật - Bảng lương",
                Left = 20, Top = 42, Width = 800, Height = 22,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.DimGray
            };
            Controls.Add(lblTieuDe);
            Controls.Add(lblPhu);

            var grpNghiepVu = UiHelper.Group("Nghiệp vụ", 20, 72, 800, 390);
            Controls.Add(grpNghiepVu);

            AddButton(grpNghiepVu, "1. Tiếp nhận thử việc", 20, 30, (s, e) => Mo(new FrmTiepNhan()));
            AddButton(grpNghiepVu, "2. Ký hợp đồng chính thức + bảo hiểm", 390, 30, (s, e) => Mo(new FrmHopDong()));
            AddButton(grpNghiepVu, "3. Quyết định lương - chức vụ", 20, 82, (s, e) => Mo(new FrmQuyetDinh()));
            AddButton(grpNghiepVu, "4. Kế hoạch làm việc tuần", 390, 82, (s, e) => Mo(new FrmKeHoach()));
            AddButton(grpNghiepVu, "5. Chấm công theo kế hoạch", 20, 134, (s, e) => Mo(new FrmChamCong()));
            AddButton(grpNghiepVu, "6. Phiếu ghi nhận phụ trội", 390, 134, (s, e) => Mo(new FrmPhuTroi()));
            AddButton(grpNghiepVu, "7. Khen thưởng - Kỷ luật", 20, 186, (s, e) => Mo(new FrmKhenThuong()));
            AddButton(grpNghiepVu, "8. Tính / chốt bảng lương tháng", 390, 186, (s, e) => Mo(new FrmLuong()));
            AddButton(grpNghiepVu, "9. Kết nối CSDL", 20, 238, (s, e) => Mo(new FrmKetNoi()));
            AddButton(grpNghiepVu, "10. Kiểm thử quy tắc (offline)", 390, 238, (s, e) =>
                txtKetQua.Text = RuleTests.RunAll());
            AddButton(grpNghiepVu, "11. Kiểm thử CSDL (tích hợp)", 20, 290, (s, e) =>
                txtKetQua.Text = DbTests.RunAll());
            AddButton(grpNghiepVu, "12. Xem nhanh danh sách nhân viên", 390, 290, (s, e) => Mo(new FrmDanhSach()));

            Controls.Add(new Label
            {
                Text = "Kết quả kiểm thử / trạng thái:", Left = 20, Top = 472,
                Width = 300, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });
            txtKetQua = new TextBox
            {
                Left = 20, Top = 494, Width = 800, Height = 155,
                Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true,
                Font = new Font("Consolas", 9), BackColor = Color.White
            };
            Controls.Add(txtKetQua);

            var lblFooter = new Label
            {
                Text = "Kiến trúc 3 lớp: UI → Service → Data → SQL Server | .NET Framework 4.7.2",
                Left = 20, Top = 656, Width = 800, Height = 20,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray
            };
            Controls.Add(lblFooter);

            Load += (s, e) =>
            {
                string msg;
                bool ok = Db.TestConnection(out msg);
                txtKetQua.Text = (ok ? "Sẵn sàng. " : "Chưa kết nối CSDL. ") + msg
                    + Environment.NewLine
                    + "Gợi ý: chạy 01_CreateTables.sql và 02_SeedData.sql, sau đó kiểm tra FrmKetNoi/App.config.";
            };
        }

        private static void AddButton(GroupBox grp, string text, int left, int top, EventHandler click)
        {
            var b = new Button
            {
                Text = text, Left = left, Top = top, Width = 370, Height = 40,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };
            b.Click += click;
            grp.Controls.Add(b);
        }

        private void Mo(Form f)
        {
            try { using (f) f.ShowDialog(this); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
