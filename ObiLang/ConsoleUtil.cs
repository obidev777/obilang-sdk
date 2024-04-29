using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang
{
    public class ConsoleUtil
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void show(bool show=true)
        {
            IntPtr handle = GetConsoleWindow();

            if (show)
            {
                ShowWindow(handle, 1);
            }
            else
            {
                ShowWindow(handle, 0);
            }
        }
    }
}
