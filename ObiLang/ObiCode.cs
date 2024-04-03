using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObiLang
{
    public class ObiCode
    {
        public string Text { get; private set; }

        public delegate string FuncAction(object[] args);

        private Dictionary<string, FuncAction> TempFuncs = new Dictionary<string, FuncAction>();
        private Dictionary<string, object> TempGlobals = new Dictionary<string, object>();


        public ObiCode(string pathcode="main.obi", Dictionary<string, object> args = null, bool coding = true) 
            => Run(pathcode,args,coding);

        public string Run(string pathcode, Dictionary<string, object> args = null, bool coding = true)
        {
            string result = "";
            Text = ReadText(pathcode);
            result = Text;

            if (args != null)
            {
                TempGlobals = args;
            }

            AddBases();

            if (result != null)
            {
                if (coding)
                {
                    result = result.Replace("\r\n", "\n");

                    string[] lines = result.Split('\n');

                    result = RunLines(lines);
                }
            }
            return result;
        }

        private string use(object[] args)
        {
            try
            {
                string use = Run(args[0].ToString());
                return use;
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }

        private string var(object[] args)
        {
            try
            {
                TempGlobals.Add(args[0].ToString(), ParseObject(args[2].ToString()));
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }

        public string GetKeyFromValue(string value)
        {
            if (value == null) return "";
            foreach (var item in TempGlobals)
            {
                if (item.Value == value)
                    return item.Key;
            }
            return "";
        }

        private string del(object[] args)
        {
            try
            {
                TempGlobals.Remove(GetKeyFromValue(args[0].ToString()));
            }
            catch (Exception ex){ return ex.Message; }
            return "";
        }

        private string write(object[] args)
        {
            string result = "";

            foreach (var a in args)
                if(a!=null)
                result += a.ToString();

            return result;
        }


        private string ReadText(string path)
        {
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"FFTPSyncWin.www.{path.Replace("/", ".")}"))
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch {
                try
                {
                    using (Stream stream = File.OpenRead(path))
                    {
                        using (TextReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        private object ParseObject(string parse)
        {
            if (parse.Contains("'"))
                return parse.Replace("'", "");
            try
            {
                return double.Parse(parse);
            }
            catch {
                string[] parseargs = null;
                if (parse.Contains("."))
                {
                    parseargs = parse.Split('.');
                    parse = parseargs[0];
                }
                if(TempGlobals.TryGetValue(parse,out object value))
                {
                    if (parseargs != null)
                    {
                        if (parseargs.Length>1)
                        if (value.GetType() == typeof(KeyValueObject))
                        {
                            KeyValueObject obj = value as KeyValueObject;
                            var result = obj.get(parseargs[1]);
                            return result;
                        }
                        try
                        {

                            object[] array = value as object[];
                            if (parseargs[1] == "len")
                                return array.Length;
                        }
                        catch { }
                    }
                    return value;
                }
            }
            return parse;
        }

        enum ExecuteType
        {
            Line,
            Conditional
        }
        private ExecuteType GetExecuteType(string name)
        {
            if (name.Contains("for"))
                return ExecuteType.Conditional;
            if (name.Contains("if"))
                return ExecuteType.Conditional;
            return ExecuteType.Line;
        }


        public string ExecuteLine(string func,string[] executes)
        {
            string htmlresult = "";
            List<object> parms = new List<object>();
            if (executes.Length > 1)
            {
                string item = null;
                bool recolect = false;
                for (int ii = 1; ii < executes.Length; ii++)
                {
                    string linp = executes[ii];
                    if (recolect)
                    {
                        object result = ParseObject(linp);
                        if (linp.Contains("'"))
                            item += " " + linp;
                        else
                            item += " " + result;
                        if (linp.Contains("'"))
                        {
                            recolect = false;
                            parms.Add(ParseObject(item));
                        }
                        continue;
                    }
                    if (linp.Contains("'"))
                    {
                        item = linp;
                        recolect = true;
                        if(executes.Length<=2)
                            parms.Add(ParseObject(item));
                        continue;
                    }
                    parms.Add(ParseObject(linp));
                }
            }
            FuncAction linecall = null;
            if (TempFuncs.TryGetValue(func, out linecall))
            {
                htmlresult += linecall(parms.ToArray());
            }
            return htmlresult;
        }

        public string[] RecolectLines(string func,string[] lines,ref int index)
        {
            List<string> recolect = new List<string>();
            for (int i = index + 1; i < lines.Length; i++)
            {
                string forlin = lines[i];
                if (forlin == "") continue;
                if (forlin.Contains($"@end{func}"))
                {
                    index = i + 1;
                    break;
                }
                recolect.Add(forlin);
            }
            return recolect.ToArray();
        }

        public string RunLines(string[] lines)
        {
            string htmlresult = "";
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line == "") continue;

                string execregex = "@(.+?)";
                if (Regex.IsMatch(line, execregex))
                {
                    string[] executes = line.Substring(line.IndexOf('@') + 1).Split(' ');
                    string func = executes[0];

                    var type = GetExecuteType(func);

                    if (type == ExecuteType.Line)
                    {
                        htmlresult += ExecuteLine(func, executes) + "\n";
                        continue;
                    }
                    else
                    {
                        if (func == "for")
                        {
                            try
                            {
                                string[] executeforlines = RecolectLines(func, lines,ref i);
                                object item = null;
                                object[] foriter = ParseObject(executes[3]) as object[];
                                if(foriter!=null)
                                for (int fi = 0; fi < foriter.Length; fi++)
                                {
                                    item = foriter[fi];
                                    TempGlobals.Add(executes[1], item);
                                    htmlresult += RunLines(executeforlines) + "\n";
                                    TempGlobals.Remove(executes[1]);
                                }
                            }
                            catch (Exception ex)
                            {
                                htmlresult += ex.Message + "\n";
                            }
                        }
                        if(func == "if")
                        {
                            string[] executeforif = RecolectLines(func, lines, ref i);
                            string conditional = "";
                            for(int ci=1; ci<executes.Length;ci++)
                            {
                                conditional += executes[ci];
                            }
                            if (ContitionTrue(conditional))
                            {
                                htmlresult += RunLines(executeforif) + "\n";
                            }
                        }
                        if (func == "func")
                        {
                            string[] executeforif = RecolectLines(func, lines, ref i);
                            string conditional = "";
                            for (int ci = 1; ci < executes.Length; ci++)
                            {
                                conditional += executes[ci];
                            }
                            if (ContitionTrue(conditional))
                            {
                                htmlresult += RunLines(executeforif) + "\n";
                            }
                        }
                        continue;
                    }
                }
                htmlresult += line + "\n";
            }
            return htmlresult;
        }

        private bool ContitionTrue(string conditional)
        {
            if (conditional.Contains("<"))
            {
                string[] condtoknes = conditional.Split('<');
                double arg1 = double.Parse(ParseObject(condtoknes[0]).ToString());
                double arg2 = double.Parse(ParseObject(condtoknes[1]).ToString());
                if (arg1 < arg2)
                    return true;
            }
            if (conditional.Contains(">"))
            {
                string[] condtoknes = conditional.Split('<');
                double arg1 = double.Parse(ParseObject(condtoknes[0]).ToString());
                double arg2 = double.Parse(ParseObject(condtoknes[1]).ToString());
                if (arg1 > arg2)
                    return true;
            }
            if (conditional.Contains("<="))
            {
                string[] condtoknes = conditional.Split('<');
                double arg1 = double.Parse(ParseObject(condtoknes[0]).ToString());
                double arg2 = double.Parse(ParseObject(condtoknes[1]).ToString());
                if (arg1 <= arg2)
                    return true;
            }
            if (conditional.Contains(">="))
            {
                string[] condtoknes = conditional.Split('<');
                double arg1 = double.Parse(ParseObject(condtoknes[0]).ToString());
                double arg2 = double.Parse(ParseObject(condtoknes[1]).ToString());
                if (arg1 < arg2)
                    return true;
            }
            bool inverse = false;
            if (conditional.Contains("!"))
            {
                conditional = conditional.Replace("!","");
                inverse = true;
            }
            if (TempGlobals.TryGetValue(conditional,out object value))
            {
                if (!inverse)
                {
                    if (value != null)
                    {
                        if (value.GetType() == typeof(bool))
                        {
                            if ((bool)value) return true;
                        }
                        return true;
                    }
                }
                else
                {
                    if (value != null)
                    {
                        if (value.GetType() != typeof(bool))
                        {
                            if (!(bool)value) return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        

        private void AddBases()
        {
            try
            {
                TempFuncs.Add("pragma", pragma);

                TempFuncs.Add("write", write);
                TempFuncs.Add("printl", printl);
                TempFuncs.Add("del", del);
                TempFuncs.Add("var", var);
                TempFuncs.Add("set", set);
                TempFuncs.Add("use", use);

                TempFuncs.Add("size_fmt", size_fmt);
                TempFuncs.Add("time_fmt", time_fmt);

                TempGlobals.Add("true", true);
                TempGlobals.Add("false", false);
            }
            catch { }
        }

        private string printl(object[] args)
        {
            try
            {
                Console.WriteLine(args[0].ToString());
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }

        private string pragma(object[] args)
        {
            try
            {
                string type = args[0].ToString();
                if (type.Equals("console"))
                {

                }
                if (type.Equals("winapp"))
                {

                }
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }

        private string size_fmt(object[] args)
        {
            return Utils.sizeof_fmt((int)args[0]);
        }

        private string time_fmt(object[] args)
        {
            return Utils.time_fmt((int)args[0]);
        }

        private string set(object[] args)
        {
            try
            {
                string varname = args[0].ToString();
                object value = ParseObject(args[2].ToString());
                if (TempGlobals.TryGetValue(varname,out object result))
                {
                    TempGlobals.Remove(varname);
                    TempGlobals.Add(varname,value);
                }
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }
    }
}
