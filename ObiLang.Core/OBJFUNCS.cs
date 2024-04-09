using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class OBJFUNCS
    {
        public void printl(ObiLangEngine engine,object text)
        {
            Console.WriteLine(text.ToString());
        }
        public void print(ObiLangEngine engine, object text)
        {
            Console.Write(text.ToString());
        }
        public string write(ObiLangEngine engine, object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            return "null";
        }
        public string incrust(ObiLangEngine engine, string path) {
            return engine.Execute(File.ReadAllText($"www/{path}"));
        }
        public void delete(ObiLangEngine engine,string var)
        {
            engine.Vars.Remove(var);
        }

        public List<string> GetList()
        {
            List<string> ret = new List<string>();
            ret.Add("jih");
            return ret;
        }
        public Dictionary<string,string> GetDict()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            ret.Add("jih","LLL");
            return ret;
        }
    }
}
