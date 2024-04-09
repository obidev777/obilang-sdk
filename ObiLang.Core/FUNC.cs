using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class FUNC
    {
        public string CMD { get; set; }
        public string[] Lines;
        public string[] Args { get; set; }
        public ObiLangEngine engine;

        public static bool IS(string cmd)
        {
            return cmd.Contains("func");
        }

        public FUNC(string[] lines, string[] args, string cmd,ObiLangEngine eng)
        {
            CMD = cmd;
            Lines = lines;
            Args = args;
            engine = eng;
        }

        public object invoke(params object[] args)
        {
            if(Args!=null)
            if (args.Length != Args.Length)
                throw new Exception($"This Func Contain ({Args.Length}) Params Error Invoke!");

            if (Args != null)
            for (int i=0;i<Args.Length;i++)
            {
                engine.AddVar(Args[i], args[i]);
            }

            engine.Logic(Lines);

            if (Args != null)
            for (int i = 0; i < Args.Length; i++)
            {
                engine.RemoveVar(Args[i]);
            }
            return null;
        }
    }
}
