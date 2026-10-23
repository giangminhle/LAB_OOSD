using System;
using System.Windows.Forms;
using QuanLyNhanSu.UI;

namespace QuanLyNhanSu
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (s, e) =>
                MessageBox.Show("Loi khong xu ly:\n" + e.Exception.Message, "QuanLyNhanSu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Run(new FrmMain());
        }
    }
}
