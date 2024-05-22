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

        public delegate void OnShow(bool show);

        public event OnShow onShow;

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

            if (onShow != null)
                onShow(show);
        }

        public void print(object obj) => Console.Write(obj);
        public void printl(object obj) => Console.WriteLine(obj);

        public int Black = 0;
        public int DarkBlue = 1;
        public int DarkGreen = 2;
        public int DarkCyan = 3;
        public int DarkRed = 4;
        public int DarkMagenta = 5;
        public int DarkYellow = 6;
        public int Gray = 7;
        public int DarkGray = 8;
        public int Blue = 9;
        public int Green = 10;
        public int Cyan = 11;
        public int Red = 12;
        public int Magenta = 13;
        public int Yellow = 14;
        public int White = 15;
        public void fcolor(int color) => Console.ForegroundColor = (ConsoleColor)color;
        public void bcolor(int color) => Console.BackgroundColor = (ConsoleColor)color;
        public void reset_color(int color) => Console.ResetColor();
    }
}
