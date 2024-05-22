using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public class OBJFUNCS
    {
        public void printl(ObiScriptEngine engine,object text)
        {
            Console.WriteLine(text.ToString());
        }
        public void printl(ObiScriptEngine engine, string text)
        {
            Console.WriteLine(text.ToString());
        }
        public void print(ObiScriptEngine engine, object text)
        {
            Console.Write(text.ToString());
        }
        public string write(ObiScriptEngine engine, object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            return "null";
        }
        public string incrust(ObiScriptEngine engine, string path) {
            return engine.Execute(File.ReadAllText($"www/{path}"));
        }
        public void delete(ObiScriptEngine engine,string var)
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

        public object sum(ObiScriptEngine engine, string num1, object num2)
        {
            object val1 = engine.GetVar(num1);
            Type tipo1 = engine.GetVar(num1).GetType();
            Type tipo2 = num2.GetType();
            if (tipo1.IsPrimitive && tipo2.IsPrimitive)
            {
                var value = Convert.ToDouble(val1) + Convert.ToDouble(num2);
                engine.RemoveVar(num1);
                engine.AddVar(num1, value);
                return value;
            }
            return null;
        }

        public object rest(ObiScriptEngine engine, string num1, object num2)
        {
            object val1 = engine.GetVar(num1);
            Type tipo1 = engine.GetVar(num1).GetType();
            Type tipo2 = num2.GetType();
            if (tipo1.IsPrimitive && tipo2.IsPrimitive)
            {
                var value = Convert.ToDouble(val1) - Convert.ToDouble(num2);
                engine.RemoveVar(num1);
                engine.AddVar(num1, value);
                return value;
            }
            return null;
        }

        public object mult(ObiScriptEngine engine, string num1, object num2)
        {
            object val1 = engine.GetVar(num1);
            Type tipo1 = engine.GetVar(num1).GetType();
            Type tipo2 = num2.GetType();
            if (tipo1.IsPrimitive && tipo2.IsPrimitive)
            {
                var value = Convert.ToDouble(val1) * Convert.ToDouble(num2);
                engine.RemoveVar(num1);
                engine.AddVar(num1, value);
                return value;
            }
            return null;
        }

        public object div(ObiScriptEngine engine, string num1, object num2)
        {
            object val1 = engine.GetVar(num1);
            Type tipo1 = engine.GetVar(num1).GetType();
            Type tipo2 = num2.GetType();
            if (tipo1.IsPrimitive && tipo2.IsPrimitive)
            {
                var value = Convert.ToDouble(val1) / Convert.ToDouble(num2);
                engine.RemoveVar(num1);
                engine.AddVar(num1, value);
                return value;
            }
            return null;
        }
        public bool not_null(object obj) => obj != null;
        public bool is_null(object obj) => obj == null;

        public Type typof(object obj) => obj.GetType();
    }
}
