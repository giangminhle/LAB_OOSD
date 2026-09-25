using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;
using static QuanLyKhachSan.Forms.UiHelper;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmThongKe : Form
    {
        private readonly ThongKeService svc = new ThongKeService();
        private DateTimePicker dtTu, dtDen;
        private DataGridView dgvTongHop, dgvDV;

        public FrmThongKe()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Thống kê";
            Width = 850;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            var top = NewFormPanel(4, 1);
            dtTu = Date("dtTu"); dtTu.Value = DateTime.Today.AddMonths(-1);
            dtDen = Date("dtDen"); dtDen.Value = DateTime.Today;
            var btnTK = Btn("btnTK", "Thống kê");
            btnTK.Click += BtnTK_Click;

            top.Controls.Add(Lbl("Từ ngày:")); top.Controls.Add(dtTu);
            top.Controls.Add(Lbl("Đến ngày:")); top.Controls.Add(dtDen);
            top.Controls.Add(new Label()); top.Controls.Add(btnTK);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            dgvTongHop = Grid("dgvTongHop");
            dgvDV = Grid("dgvDV");
            split.Panel1.Controls.Add(dgvTongHop);
            split.Panel1.Controls.Add(new Label { Text = "Tổng hợp (đặt phòng, hóa đơn, doanh thu, đền bù)", Dock = DockStyle.Top });
            split.Panel2.Controls.Add(dgvDV);
            split.Panel2.Controls.Add(new Label { Text = "Dịch vụ sử dụng theo khoảng thời gian", Dock = DockStyle.Top });

            var btnDong = Btn("btnDong", "Đóng");
            btnDong.Dock = DockStyle.Bottom;
            btnDong.Click += (s, e) => Close();

            Controls.Add(split);
            Controls.Add(top);
            Controls.Add(btnDong);
        }

        private void BtnTK_Click(object sender, EventArgs e)
        {
            if (dtDen.Value.Date < dtTu.Value.Date)
            {
                MessageBox.Show("Đến ngày không được trước từ ngày.");
                return;
            }
            dgvTongHop.DataSource = svc.TongHop(dtTu.Value, dtDen.Value);
            dgvDV.DataSource = svc.DichVu(dtTu.Value, dtDen.Value);
        }
    }
}
