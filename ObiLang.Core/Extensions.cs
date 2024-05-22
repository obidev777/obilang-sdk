using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public class ObiScript
    {
        public ObiScriptEngine Engine { get; internal set; }
        public string Log { get; internal set; }
    }

    public static class Extensions
    {
        public static ObiScript RunScript(this string code)
        {
            var engine = new ObiScriptEngine();
            string log = "";
            if (File.Exists(code))
                log = engine.ExecuteFile(code);
            else
                log = engine.Execute(code);
            return new ObiScript() { 
            Engine = engine

            };
        }
    }
}
