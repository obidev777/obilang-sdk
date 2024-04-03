using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class CONDITIONAL
    {
        public string CMD { get; set; }
        public string[] Lines;
        public bool Condition = false;

        public static bool IS(string cmd)
        {
            return cmd.Contains("if") || cmd.Contains("while");
        }

        public CONDITIONAL(string[] lines,bool condition,string cmd)
        {
            CMD = cmd;
            Lines = lines;
            Condition = condition;
        }

        public object Execute(ObiLangEngine engine)
        {
            if(CMD=="if")
                if (Condition)
                {
                    engine.Logic(Lines);
                }
            if (CMD == "while")
                while (Condition)
                {
                    engine.Logic(Lines,this);
                }
            return null;
        }
    }
}
