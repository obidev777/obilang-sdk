using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public class OPERATOR
    {
        public static string[] Operators = new string[]{
        "++","+","--","-","**","*","//","/","<","!<",">","!>","<=",">=","=","!="
        };
        public static string[] OperatorsException = new string[]{
        "->"," ","'"
        };

        public static string GetOperator(string cmd)
        {

            return "";
        }

        public static bool Exists(string cmd)
        {
            for (int i=0;i<OperatorsException.Length;i++)
            {
                var ex = OperatorsException[i];
                if (cmd.Contains(ex))
                {
                    return false;
                }
            }
            for (int i = 0; i < Operators.Length; i++)
            {
                var item = Operators[i];
                if (cmd.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static object Execute(ObiScriptEngine engine,string op)
        {
            return null;
        }

        public static object Eval(ObiScriptEngine engine, string arg)
        {
            if (arg.Contains("="))
            {
                string[] tokens = arg.Replace("=", "#").Split('#');
                string vname = tokens[0];
                object val1 = engine.GetArgs(vname)[0];
                object val2 = null;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        val2 = engine.GetArgs(tokens[1])[0];
                    }
                }
                return val1.Equals(val2);
            }
            else if (arg.Contains("!="))
            {
                string[] tokens = arg.Replace("=", "#").Split('#');
                string vname = tokens[0];
                object val1 = engine.GetArgs(vname)[0];
                object val2 = null;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        val2 = engine.GetArgs(tokens[1])[0];
                    }
                }
                return !val1.Equals(val2);
            }
            else if (arg.Contains("<"))
            {
                string[] tokens = arg.Replace("<", "#").Split('#');
                string vname = tokens[0];
                double val1 = double.Parse(engine.GetArgs(vname)[0].ToString());
                double val2 = -999999999;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        val2 = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                return (val1 < val2);
            }
            else if (arg.Contains(">"))
            {
                string[] tokens = arg.Replace(">", "#").Split('#');
                string vname = tokens[0];
                double val1 = double.Parse(engine.GetArgs(vname)[0].ToString());
                double val2 = -999999999;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        val2 = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                return (val1 > val2);
            }
            else if (arg.Contains("<="))
            {
                string[] tokens = arg.Replace("<=", "#").Split('#');
                string vname = tokens[0];
                double val1 = double.Parse(engine.GetArgs(vname)[0].ToString());
                double val2 = -999999999;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        val2 = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                return (val1 <= val2);
            }
            else if (arg.Contains(">="))
            {
                string[] tokens = arg.Replace(">=", "#").Split('#');
                string vname = tokens[0];
                double val1 = double.Parse(engine.GetArgs(vname)[0].ToString());
                double val2 = -999999999;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        val2 = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                return (val1 >= val2);
            }
            else if (arg.Contains("++"))
            {
                string[] tokens = arg.Replace("++", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = 1;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value+=plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    engine.RemoveVar(newv);
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    engine.AddVar(newv, ret);
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] {ret});
                    }
                    return ret;
                }
            }
            else if (arg.Contains("--"))
            {
                string[] tokens = arg.Replace("--", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = 1;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value -= plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    engine.RemoveVar(newv);
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    engine.AddVar(newv, ret);
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] { ret });
                    }
                    return ret;
                }
            }
            else if (arg.Contains("**"))
            {
                string[] tokens = arg.Replace("++", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = value;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value *= plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    engine.RemoveVar(newv);
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    engine.AddVar(newv, ret);
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] { ret });
                    }
                    return ret;
                }
            }
            else if (arg.Contains("+"))
            {
                string[] tokens = arg.Replace("+", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = 1;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value += plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] { ret });
                    }
                    return ret;
                }
            }
            else if (arg.Contains("-"))
            {
                string[] tokens = arg.Replace("-", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = 1;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value -= plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] { ret });
                    }
                    return ret;
                }
            }
            else if (arg.Contains("*"))
            {
                string[] tokens = arg.Replace("*", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = 1;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value *= plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] { ret });
                    }
                    return ret;
                }
            }
            else if (arg.Contains("/"))
            {
                string[] tokens = arg.Replace("/", "#").Split('#');
                string vname = tokens[0];
                object vobj = engine.GetArgs(vname)[0];
                double value = double.Parse(vobj.ToString());
                double plus = 1;
                if (tokens.Length > 1)
                {
                    if (tokens[1] != "")
                    {
                        plus = double.Parse(engine.GetArgs(tokens[1])[0].ToString());
                    }
                }
                value /= plus;
                if (!vname.Contains("::"))
                {
                    string newv = vname.Replace("$", "");
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    return ret;
                }
                else
                {
                    object ret = Convert.ChangeType(value, vobj.GetType());
                    ICacheHandle handle = engine.GetHandleFrom(vname);
                    if (handle != null)
                    {
                        handle.Invoke(new object[] { ret });
                    }
                    return ret;
                }
            }


            return null;
        }
    }
}
