using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public class ENV
    {
        public string newline() => Environment.NewLine;
        public string current_dir() => Environment.CurrentDirectory;
        public int tick() => Environment.TickCount;
    }
}
