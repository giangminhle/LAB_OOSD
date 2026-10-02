using System;
using System.Windows.Forms;
using EShopping.UI;

namespace EShopping
{
    static class Program
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