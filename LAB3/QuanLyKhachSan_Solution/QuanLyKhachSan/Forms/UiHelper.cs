using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKhachSan.Forms
{
    /// <summary>
    /// Các hàm dựng control lặp lại nhiều lần (nhãn + ô nhập) để các Form gọn hơn.
    /// Đây chỉ là tiện ích trình bày, không chứa logic nghiệp vụ.
    /// </summary>
    internal static class UiHelper
    {
        public static TableLayoutPanel NewFormPanel(int cols, int rows)
        {
            var p = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = cols,
                RowCount = rows,
                Padding = new Padding(8)
            };
            for (int i = 0; i < cols; i++)
                p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));
            return p;
        }

        public static Label Lbl(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(4, 8, 4, 4),
            Anchor = AnchorStyles.Left
        };

        public static TextBox Txt(string name, int width = 160) => new TextBox
        {
            Name = name,
            Width = width,
            Margin = new Padding(4)
        };

        public static ComboBox Cbo(string name, int width = 160) => new ComboBox
        {
            Name = name,
            Width = width,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(4)
        };

        public static NumericUpDown Num(string name, decimal max = 100000000, int width = 120) => new NumericUpDown
        {
            Name = name,
            Width = width,
            Maximum = max,
            Margin = new Padding(4)
        };

        public static DateTimePicker Date(string name, int width = 130) => new DateTimePicker
        {
            Name = name,
            Width = width,
            Format = DateTimePickerFormat.Short,
            Margin = new Padding(4)
        };

        public static Button Btn(string name, string text, int width = 130) => new Button
        {
            Name = name,
            Text = text,
            Width = width,
            Height = 30,
            Margin = new Padding(4)
        };

        public static DataGridView Grid(string name) => new DataGridView
        {
            Name = name,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            Margin = new Padding(4)
        };

        public static GroupBox Group(string title) => new GroupBox
        {
            Text = title,
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8)
        };
    }
}
