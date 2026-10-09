using System;
using System.Windows.Forms;
using QuanLyCongTyDuLich.Forms;

namespace QuanLyDuLich
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
