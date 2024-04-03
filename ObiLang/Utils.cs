using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang
{
    public class Utils
    {
        public static string sizeof_fmt(int len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = ((int)(len / 1024));
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
        public static string time_fmt(double seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }
    }
}
