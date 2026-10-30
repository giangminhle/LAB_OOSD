using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCaoOc.Data;
using QuanLyCaoOc.Services;

namespace QuanLyCaoOc.UI
{
    public class FrmMain : Form
    {
        private TextBox txtKetQua;

        public FrmMain()
        {
            Text = "LAB8 - Quản lý cho thuê văn phòng cao ốc";
            ClientSize = new Size(840, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Controls.Add(new Label
            {
                Text = "QUẢN LÝ CHO THUÊ VĂN PHÒNG CAO ỐC",
                Left = 20, Top = 12, Width = 800, Height = 30,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(30, 80, 140)
            });
            Controls.Add(new Label
            {
                Text = "Phòng - Hợp đồng - Tiền thuê & dịch vụ - Gia hạn - Nhân viên & thẻ xe - Kiểm tra phòng",
                Left = 20, Top = 42, Width = 800, Height = 22,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.DimGray
            });

            var grp = UiHelper.Group("Nghiệp vụ", 20, 72, 800, 290);
            Controls.Add(grp);
            AddButton(grp, "1. Tầng, phòng và giá thuê", 20, 30, (s, e) => Mo(new FrmPhong()));
            AddButton(grp, "2. Lập hợp đồng thuê văn phòng", 390, 30, (s, e) => Mo(new FrmHopDong()));
            AddButton(grp, "3. Tiền thuê hàng tháng + hóa đơn dịch vụ", 20, 82, (s, e) => Mo(new FrmThanhToan()));
            AddButton(grp, "4. Lập phiếu gia hạn", 390, 82, (s, e) => Mo(new FrmGiaHan()));
            AddButton(grp, "5. Nhân viên văn phòng + thẻ giữ xe", 20, 134, (s, e) => Mo(new FrmNhanVien()));
            AddButton(grp, "6. Kiểm tra phòng hằng năm + phạt", 390, 134, (s, e) => Mo(new FrmKiemTra()));
            AddButton(grp, "7. Kết nối CSDL", 20, 186, (s, e) => Mo(new FrmKetNoi()));
            AddButton(grp, "8. Kiểm thử quy tắc (offline)", 390, 186, (s, e) => txtKetQua.Text = RuleTests.RunAll());
            AddButton(grp, "9. Kiểm thử CSDL (tích hợp)", 20, 238, (s, e) => txtKetQua.Text = DbTests.RunAll());

            Controls.Add(new Label
            {
                Text = "Kết quả kiểm thử / trạng thái:", Left = 20, Top = 372,
                Width = 300, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });
            txtKetQua = new TextBox
            {
                Left = 20, Top = 394, Width = 800, Height = 170,
                Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true,
                Font = new Font("Consolas", 9), BackColor = Color.White
            };
            Controls.Add(txtKetQua);
            Controls.Add(new Label
            {
                Text = "Kiến trúc 3 lớp: UI → Service → Data → SQL Server | .NET Framework 4.7.2",
                Left = 20, Top = 572, Width = 800, Height = 20,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray
            });

            Load += (s, e) =>
            {
                string msg;
                bool ok = Db.TestConnection(out msg);
                txtKetQua.Text = (ok ? "Sẵn sàng. " : "Chưa kết nối CSDL. ") + msg + Environment.NewLine
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
