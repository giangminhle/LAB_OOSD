using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyNhanSu.UI
{
    /// <summary>Tien ich dung chung cho cac Form (chi trinh bay, khong chua nghiep vu).</summary>
    internal static class UiHelper
    {
        public static Label Lbl(string text, int left, int top, int width = 110)
        {
            return new Label { Text = text, Left = left, Top = top + 3, Width = width, TextAlign = ContentAlignment.MiddleLeft };
        }

        public static TextBox Txt(int left, int top, int width = 220)
        {
            return new TextBox { Left = left, Top = top, Width = width };
        }

        public static ComboBox Cbo(int left, int top, int width = 220)
        {
            return new ComboBox { Left = left, Top = top, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
        }

        public static NumericUpDown Num(int left, int top, decimal max = 1000000000m, int width = 220)
        {
            return new NumericUpDown
            {
                Left = left, Top = top, Width = width, Maximum = max, Minimum = 0,
                DecimalPlaces = 0, ThousandsSeparator = true
            };
        }

        public static DateTimePicker Date(int left, int top, int width = 220)
        {
            return new DateTimePicker { Left = left, Top = top, Width = width, Format = DateTimePickerFormat.Short };
        }

        public static Button Btn(string text, int left, int top, int width = 150, int height = 34)
        {
            return new Button { Text = text, Left = left, Top = top, Width = width, Height = height };
        }

        public static DataGridView Grid(int left, int top, int width, int height)
        {
            return new DataGridView
            {
                Left = left, Top = top, Width = width, Height = height,
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, RowHeadersVisible = false,
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle
            };
        }

        public static GroupBox Group(string text, int left, int top, int width, int height)
        {
            return new GroupBox { Text = text, Left = left, Top = top, Width = width, Height = height };
        }

        public static void RefreshCombo(ComboBox cbo, System.Data.DataTable dt,
            string valueMember, string displayMember, int? selected = null)
        {
            cbo.DataSource = null;
            cbo.DataSource = dt;
            cbo.ValueMember = valueMember;
            cbo.DisplayMember = displayMember;
            if (selected.HasValue)
            {
                try { cbo.SelectedValue = selected.Value; } catch { }
            }
        }

        public static void FormatMoneyGrid(DataGridView grid)
        {
            string[] tien = { "MucLuongCoBan", "TienPhuTroi", "PhuCapChucVu", "PhuCapLuong",
                "PhuCapDocHai", "TienKhenThuong", "TienKyLuat", "BHXH", "BHYT", "BHTN", "ThucLinh",
                "TongChiPhi", "SoTien", "SoTienPhat" };
            foreach (DataGridViewColumn c in grid.Columns)
                if (Array.IndexOf(tien, c.DataPropertyName) >= 0)
                    c.DefaultCellStyle.Format = "N0";
        }
    }
}
