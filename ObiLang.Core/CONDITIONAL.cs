using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public class CONDITIONAL
    {
        public string CMD { get; set; }
        public string[] Lines;
        public string[] ElseLines;
        public bool Condition = false;
        public string catchname = "catch";

        public static bool IS(string cmd)
        {
            return cmd.Contains("if") || cmd.Contains("while") || cmd.Contains("try");
        }

        public CONDITIONAL(string[] lines,string[] elselines,bool condition,string cmd)
        {
            CMD = cmd;
            Lines = lines;
            Condition = condition;
            ElseLines = elselines;
        }

        public object Execute(ObiScriptEngine engine)
        {
            if(CMD=="if")
                if (Condition)
                {
                    engine.Logic(Lines);
                }
                else
                {
                    if (ElseLines != null)
                        engine.Logic(ElseLines);
                }
            if (CMD == "while")
                while (Condition)
                {
                    engine.Logic(Lines,this);
                }
            if (CMD == "try")
            {
                try
                {
                    engine.Logic(Lines);
                }
                catch(Exception ex)
                {
                    engine.AddVar(catchname,ex);
                    engine.Logic(ElseLines);
                    engine.RemoveVar(catchname);
                }
            }
            return null;
        }
    }
}
