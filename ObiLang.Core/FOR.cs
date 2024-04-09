using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class FOR
    {
        public string CMD { get; set; }
        public string[] Lines;
        public string VName = "";
        public object Array = false;

        public static bool IS(string cmd)
        {
            return cmd.Contains("for");
        }

        public FOR(string[] lines, string vname,object array, string cmd)
        {
            CMD = cmd;
            Lines = lines;
            VName = vname;
            Array = array;
        }

        public void Logic(ObiLangEngine engine, object item)
        {
            engine.AddVar(VName, item);
            engine.Logic(Lines, this);
            engine.RemoveVar(VName);
        }

        public object Execute(ObiLangEngine engine)
        {
            if (Array.GetType().Name.Contains("List") || Array.GetType().Name.Contains("Dictionary"))
            {
                Array = Array.GetType().GetMethod("ToArray").Invoke(Array, null);
            }
            var type = Array.GetType();
            if (type.BaseType == typeof(Array))
            {
                foreach (object item in ((Array)Array))
                {
                    Logic(engine,item);
                }
            }
            if (type == typeof(ARRAY))
            {
                foreach (object item in ((ARRAY)Array).List)
                {
                    Logic(engine, item);
                }
            }
            if (type == typeof(DICT))
            {
                foreach (object item in ((DICT)Array).List)
                {
                    Logic(engine, item);
                }
            }

            return null;
        }
    }
}
