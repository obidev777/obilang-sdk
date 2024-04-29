using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class ObiLangEngine
    {
        public Dictionary<string, object> Vars = new Dictionary<string, object>();
        public List<Assembly> AsemblyList = new List<Assembly>();
        private string PATH { get; set; }

        public ObiLangEngine(object obj_funcs = null)
        {
            try
            {
                if (!Directory.Exists("libs"))
                    Directory.CreateDirectory("libs");
            }
            catch { }
            if (obj_funcs != null)
                Vars.Add("base", obj_funcs);
            else
                Vars.Add("base", new OBJFUNCS());
            Vars.Add("env", new ENV());
        }

        public void AddAssembly(Assembly asm) => AsemblyList.Add(asm);
        public void AddAssembly(string path) => AsemblyList.Add(Assembly.LoadFile(path));
        public void AddAssembly(byte[] bytes) => AsemblyList.Add(Assembly.Load(bytes));

        public Assembly[] GetAssemblies()
        {
            List<Assembly> result = new List<Assembly>();
            result.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            result.AddRange(AsemblyList.ToArray());
            return result.ToArray();
        }

        public bool VarExist(string name)
        {
            if (Vars.TryGetValue(name, out object value))
            {
                return true;
            }
            return false;
        }
        public bool AddVar(string name, object value)
        {
            if (!VarExist(name))
            {
                Vars.Add(name, value);
                return true;
            }
            return false;
        }
        public bool RemoveVar(string name)
        {
            if (VarExist(name))
            {
                Vars.Remove(name);
                return true;
            }
            return false;
        }

        public MethodInfo GetMethodFrom(string varname,string mname,int argscount=0)
        {
            
            if (Vars != null)
            {
                object vvar = GetVar(varname);
                Type t = vvar.GetType();
                if (vvar != null)
                {
                    if(vvar.GetType().Name != "RuntimeType")
                    {
                        foreach (var m in vvar.GetType().GetMethods())
                        {
                            if (m.Name == mname)
                            {
                                if (m.GetParameters().Length == argscount)
                                {
                                    return m;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var m in ((Type)vvar).GetMethods())
                        {
                            if (m.Name == mname)
                            {
                                if (m.GetParameters().Length == argscount)
                                {
                                    return m;
                                }
                            }
                        }
                    }
                        
                }
            }
            return null;
        }
        public PropertyInfo GetPropertyFrom(string varname, string mname, int argscount = 0)
        {
            if (Vars != null)
            {
                object vvar = GetVar(varname);
                if (vvar != null)
                {
                    if (vvar.GetType().Name != "RuntimeType")
                    {
                        foreach (var m in vvar.GetType().GetProperties())
                        {
                            if (m.Name == mname)
                            {
                                return m;
                            }
                        }
                    }
                    else
                    {
                        foreach (var m in ((Type)vvar).GetProperties())
                        {
                            if (m.Name == mname)
                            {
                                return m;
                            }
                        }
                    }
                    
                }
            }
            return null;
        }
        public EventInfo GetEventFrom(string varname, string mname, int argscount = 0)
        {
            if (Vars != null)
            {
                object vvar = GetVar(varname);
                if (vvar != null)
                {
                    if (vvar.GetType().Name != "RuntimeType")
                    {
                        foreach (var m in vvar.GetType().GetEvents())
                        {
                            if (m.Name == mname)
                            {
                                return m;
                            }
                        }
                    }
                    else
                    {
                        foreach (var m in ((Type)vvar).GetEvents())
                        {
                            if (m.Name == mname)
                            {
                                return m;
                            }
                        }
                    }

                }
            }
            return null;
        }
        public FieldInfo GetFieldFrom(string varname, string mname, int argscount = 0)
        {
            if (Vars != null)
            {
                object vvar = GetVar(varname);
                if (vvar != null)
                {
                    if (vvar.GetType().Name != "RuntimeType")
                    {
                        foreach (var m in vvar.GetType().GetFields())
                        {
                            if (m.Name == mname)
                            {
                                return m;
                            }
                        }
                    }
                    else
                    {
                        foreach (var m in ((Type)vvar).GetFields())
                        {
                            if (m.Name == mname)
                            {
                                return m;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private string[] GetLines(string code)
        {
            string[] lines = code.Split('\n', '\r');
            List<string> linesfix = new List<string>();
            foreach(string l in lines)
            {
                if (l == "") continue;
                if (l[0] == '#') continue;
                linesfix.Add(l);
            }
            return linesfix.ToArray();
        }

        public bool Recolect(string cmd)
        {
            string[] list = new string[] { 
            "for","if","while","func"
            };
            foreach (var item in list) if (item == cmd) return true;
            return false;
        }

        public object GetVar(string name)
        {
            if (Vars.TryGetValue(name, out object value))
            {
                return value;
            }
            return null;
        }
        public object[] GetArgs(string args, bool addengine = false,bool gettyping=false)
        {
            if (args == "") return null;
            if (args == "none") return new object[] { null };
            List<object> result = new List<object>();
            if (addengine)
                result.Add(this);

            List<string> tokens = new List<string>();

            string data = "";
            string arrray = "";
            int i = 0;
            foreach (char ch in args)
            {
                i += 1;
                if (ch == '[' || ch == ']' || ch == '<' || ch == '>')
                    arrray = ch.ToString();
                if (ch == ',' && !args.Contains("::")
                    && !args.Contains(" is ")
                    && !args.Contains(" in ")
                    && !args.Contains(" not "))
                {
                    if (arrray == "[" || arrray == "<")
                    {
                        data += ch.ToString();
                    }
                    else
                    {
                        tokens.Add(data);
                        data = "";
                    }
                }
                else
                {
                    data += ch.ToString();
                }
                if (i >= args.Length)
                {
                    tokens.Add(data);
                    data = "";
                }
            }

            foreach (var arg in tokens)
            {
                if (arg == "") continue;
                if (arg.Contains(" is "))
                {
                    string[] plags = arg.Split(new char[] { ' ' },3);
                    string comp1 = plags[0];
                    string op = plags[1];
                    string comp2 = plags[2];
                    object c1 = GetArgs(comp1)[0];
                    object c2 = GetArgs(comp2)[0];
                    if (op == "is")
                    {
                        result.Add(c1.Equals(c2));
                    }
                    if (op == "not")
                    {
                        result.Add(!c1.Equals(c2));
                    }
                }
                else if (arg.Contains("'") 
                    && arg[0]!='[' && arg[arg.Length-1] != ']' 
                    && !arg.Contains("::")
                    && !arg.Contains("->")
                    && arg[0] != '<' && arg[arg.Length - 1] != '>')
                {
                    result.Add(arg.Replace("'", ""));
                }
                else if (arg.Contains("[") && arg.Contains("]"))
                {
                    string tks = null;
                    try
                    {
                        tks = arg.Substring(1, arg.Length - 2);
                    }
                    catch { }
                    ARRAY array = new ARRAY();
                    if (tks != null)
                        array.append(GetArgs(tks));
                    result.Add(array);
                }
                else if (arg.Contains("<") && arg.Contains(">") && !arg.Contains("::") && !arg.Contains("->"))
                {
                    string tks = null;
                    try
                    {
                        tks = arg.Substring(1, arg.Length - 2);
                    }
                    catch { }
                    DICT dict = new DICT();
                    if (tks != null)
                    {
                        object[] ta = GetArgs(tks);
                        if (ta!=null)
                            dict.append(ta[0].ToString(),ta[1]);
                    }
                    result.Add(dict);
                }
                else if (arg.Contains("->"))
                {
                    bool stcobj = arg.Contains("use ");
                    string newarg = arg;
                    if (stcobj)
                    {
                        newarg = newarg.Replace("use ", "");
                    }
                    string[] tks = newarg.Replace("->", "\n").Split(new char[] { ' ','\n' },2);


                    string type = "";
                    if (tks[0]!="")
                    {
                        type = tks[0];
                    }
                    string[] taks = tks[1].Split(new char[] { ' ' }, 2);
                    string classname = taks[0];
                    string targ = "";
                    if (taks.Length > 1)
                        targ = taks[1];

                    Assembly[] assemblies = GetAssemblies();
                    List<Assembly> searchlist = new List<Assembly>();

                    foreach(var asm in assemblies)
                    {
                        if (asm.FullName.Contains(type) || type=="")
                        {
                            searchlist.Add(asm);
                        }
                    }
                    bool br = false;
                    string stt = string.Empty;
                    if (classname.Contains("::"))
                    {
                        string[] tkc = classname.Replace("::","-").Split('-');
                        classname = tkc[0];
                        stt = tkc[1];
                    }
                    
                    foreach(var asm in searchlist)
                    {
                        foreach(var typ in asm.GetTypes())
                        {
                            if (typ.Name == classname)
                            {
                                if (stt == string.Empty)
                                {
                                    try
                                    {
                                        if (stcobj)
                                            throw new Exception("STCOBJ");
                                        object[] iargs = GetArgs(targ);
                                        object instance = Activator.CreateInstance(typ, iargs);
                                        result.Add(instance);
                                    }
                                    catch (Exception ex){
                                        Vars.Add(classname, typ);
                                    }
                                }
                                else {
                                    object[] iargs = GetArgs(targ);
                                    int argcount = 0;
                                    if (iargs != null)
                                        argcount = iargs.Length;
                                    Vars.Add(classname, typ);
                                    MethodInfo method = GetMethodFrom(classname, stt, argcount);
                                    if (method != null)
                                    {
                                        object ret = method.Invoke(null, iargs);
                                        result.Add(ret);
                                    }
                                    else
                                    {
                                        PropertyInfo property = GetPropertyFrom(classname, stt, argcount);
                                        if (property != null)
                                        {
                                            object ret = property.GetValue(null);
                                            result.Add(ret);
                                        }
                                        else
                                        {
                                            FieldInfo field = GetFieldFrom(classname, stt, argcount);
                                            if (field != null)
                                            {
                                                object ret = field.GetValue(null);
                                                result.Add(ret);
                                            }
                                            else
                                            {
                                                throw new Exception($"Method Not Exist {classname}.{stt}!");
                                            }
                                        }
                                    }
                                    Vars.Remove(classname);
                                }
                                br = true;
                                break;
                            }
                        }
                        if (br)
                            break;
                    }
                }
                else if (arg.Contains("::"))
                {
                    string farg = null;
                    string fcmd = null;
                    try
                    {
                        fcmd = arg.Split(new char[] { ' ' }, 2)[0];
                        farg = arg.Split(new char[] { ' ' }, 2)[1];
                    }
                    catch { }
                    if (fcmd != null)
                    {
                        fcmd = fcmd.Replace("::", "-");
                        string[] tkes = fcmd.Split('-');
                        List<string> tkess = new List<string>();
                        tkess.AddRange(tkes);

                        if (tkess[0] == "")
                            tkess.RemoveAt(0);

                        string vname = "";
                        string mname = "";

                        if(tkess.Count==1)
                        {
                            vname = "base";
                            mname = tkess[0];
                        }
                        else
                        {
                            vname = tkess[0];
                            mname = tkess[1];
                        }

                        object[] fargs = null;
                        if (farg != null)
                        {
                            if (tkess.Count == 1)
                                fargs = GetArgs(farg,addengine);
                            else
                                fargs = GetArgs(farg);
                        }
                        int argcount = 0;
                        if (fargs != null)
                            argcount = fargs.Length;
                        object oinstance = GetVar(vname);
                        if (oinstance.GetType() == typeof(FUNC))
                        {
                            argcount = 1;
                            fargs = new object[] { fargs };
                        }
                        MethodInfo method = GetMethodFrom(vname, mname, argcount);
                        
                        if (method!=null)
                        {
                            if (method.IsStatic)
                            {
                                oinstance = null;
                            }
                            if (gettyping)
                            {
                                result.Add(method);
                                continue;
                            }
                            object ret = method.Invoke(oinstance, fargs);
                            result.Add(ret);
                        }
                        else
                        {
                            PropertyInfo property = GetPropertyFrom(vname, mname, argcount);
                            if (property != null)
                            {
                                if (property.GetMethod.IsStatic)
                                {
                                    oinstance = null;
                                }
                                if (gettyping)
                                {
                                    result.Add(property);
                                    continue;
                                }
                                object ret = property.GetValue(oinstance);
                                result.Add(ret);
                            }
                            else
                            {
                                FieldInfo field = GetFieldFrom(vname, mname, argcount);
                                if (field != null)
                                {
                                    if (field.IsStatic)
                                    {
                                        oinstance = null;
                                    }
                                    if (gettyping)
                                    {
                                        result.Add(field);
                                        continue;
                                    }
                                    object ret = field.GetValue(oinstance);
                                    result.Add(ret);
                                }
                                else
                                {
                                    EventInfo ev = GetEventFrom(vname, mname, argcount);
                                    if (ev != null)
                                    {
                                        if (gettyping)
                                        {
                                            result.Add(ev);
                                            continue;
                                        }
                                        result.Add(ev);
                                    }
                                    else
                                    {
                                        throw new Exception($"Method Not Exist {vname}.{mname}!");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"Error Parsing Func!");
                    }
                }
                else if (arg.Contains("$"))
                {
                    result.Add(GetVar(arg.Replace("$", "")));
                }
                else if (arg == "true" || arg=="false")
                {
                    if(arg == "true")
                        result.Add(true);
                    else
                        result.Add(false);
                }
                else
                {
                    object val = null;
                    if (arg.Contains("(int)"))
                    {
                        val = int.Parse(arg.Replace("(int)",""));
                    }
                    else if (arg.Contains("(float)"))
                    {
                        val = float.Parse(arg.Replace("(float)", ""));
                    }
                    else if (arg.Contains("(double)"))
                    {
                        val = double.Parse(arg.Replace("(double)", ""));
                    }
                    else if (arg.Contains("(str)"))
                    {
                        val = arg.Replace("(str)", "").ToString();
                    }
                    else if (arg.Contains("(long)"))
                    {
                        val = arg.Replace("(str)", "").ToString();
                    }
                    else
                    {
                        val = int.Parse(arg);
                    }
                    result.Add(val);
                }
            }
            return result.ToArray();
        }

        public string FixLine(string line,char fin='@')
        {
            int index = line.IndexOf(fin);
            return line.Substring(index);
        }
        internal string Logic(string[] lines,object sender=null)
        {
            string result = "";
            

                for (int i = 0; i < lines.Length; i++)
                {
                string line = lines[i];
                try
                {
                    if (line != "")
                    {
                        
                    }
                    else
                    {
                        continue;
                    }

                    // toda la otra logica

                    if (line.Contains("break"))
                        if (sender != null)
                            if (sender.GetType() == typeof(CONDITIONAL))
                            {
                                ((CONDITIONAL)sender).Condition = false;
                                return "Break";
                            }

                    //line = line.Substring(0, line.Length - 1);


                    string[] tokens = line.Split(new char[] { ' ' }, 2);
                    string arg = "";

                    if (tokens.Length > 1)
                        arg = tokens[1];

                    string cmd = tokens[0];

                    bool recolect = Recolect(cmd);
                    if (recolect)
                    {
                        List<string> reclines = new List<string>();
                        int ri = i;
                        int rindex = 1;
                        for(ri = i + 1; ri < lines.Length; ri++)
                        {
                            string ll = lines[ri];
                            if (Recolect(ll))
                            {
                                rindex++;
                            }
                            if (ll.Contains($"end{cmd}"))
                            {
                                rindex--;
                            }
                            if (rindex <= 0)
                                break;
                            reclines.Add(ll);
                        }
                        i = ri;
                        if (CONDITIONAL.IS(cmd))
                        {
                            var cond = new CONDITIONAL(reclines.ToArray(), bool.Parse(GetArgs(arg)[0].ToString()), cmd);
                            cond.Execute(this);
                        }
                        if (FOR.IS(cmd))
                        {
                            string[] tsks = arg.Replace(" in ", "#").Split('#');
                            var frd = new FOR(reclines.ToArray(),tsks[0],GetArgs(tsks[1])[0],cmd);
                            frd.Execute(this);
                        }
                        if (FUNC.IS(cmd))
                        {
                            string[] tsks = arg.Replace($"{cmd} ","").Split(new char[] { ' ' },2);
                            string[] argsname = null;
                            if (tsks.Length > 1)
                            {
                                argsname = tsks[1].Split(',');
                            }
                            var func = new FUNC(reclines.ToArray(), argsname, cmd,this);
                            AddVar(tsks[0], func);
                        }
                        continue;
                    }
                    else if (cmd == "var")
                    {
                        string[] vartokens = arg.Replace(" : ", ":").Split(new char[] { ':' }, 2);
                        string name = vartokens[0];
                        object[] args = GetArgs(vartokens[1]);
                        if (Vars.TryGetValue(name, out object value))
                        {
                            Vars.Remove(name);
                        }
                        Vars.Add(name, args[0]);
                    }
                    else if (cmd == "out")
                    {
                        string[] vartokens = arg.Replace("::","#").Replace(" : ", ":").Split(new char[] { ':' }, 2);
                        string name = vartokens[0];
                        if (name.Contains("#"))
                        {
                            name = name.Replace("#", "::");
                        }
                        bool is_delegate = false;
                        string vn = vartokens[1];
                        if (vn.Contains("(delegate)"))
                        {
                            is_delegate = true;
                            vn = vn.Replace("(delegate)","");
                        }
                        object value = GetArgs(vn)[0];
                        object property = GetArgs(name, gettyping: true)[0];
                        object varobj = GetVar(name.Replace("::", "#").Split('#')[0]);
                        try
                        {
                            ((PropertyInfo)property).SetValue(varobj, value);
                        }
                        catch
                        {
                            try
                            {
                                ((FieldInfo)property).SetValue(varobj, value);
                            }
                            catch{

                                try
                                {
                                    ((FUNC)value).SetEventHandle((EventInfo)property,varobj);
                                }
                                catch(Exception ex){ 
                                
                                }

                            }
                        }
                    }
                    else if (cmd == "use" && !line.Contains("->"))
                    {
                        string args = line.Split(new char[] { ' ' },2)[1];
                        var path = GetArgs(args)[0];
                        path = new FileInfo(PATH).Directory.FullName + "\\" + path;
                        if(!path.ToString().EndsWith(".obi"))
                        {
                            path = path.ToString() + ".obi";
                        }
                        string temppath = PATH;
                        ExecuteFile(path.ToString());
                        PATH = temppath;
                        RemoveVar("path_this");
                        AddVar("path_this", PATH);
                    }
                    else if (cmd == "lib")
                    {
                        string args = line.Split(new char[] { ' ' }, 2)[1];
                        var path = GetArgs(args)[0].ToString();

                        if (!path.Contains("http://") && !path.Contains("https://"))
                        {
                            if (!path.ToString().EndsWith(".dll"))
                            {
                                path = path + ".dll";
                            }

                            if (File.Exists(path))
                            {

                            }
                            else if (File.Exists(new FileInfo(PATH).Directory.FullName + "\\" + path))
                            {
                                path = new FileInfo(PATH).Directory.FullName + "\\" + path;
                            }
                            else
                            {
                                path = new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Directory + "\\libs\\" + path;
                            }

                            AddAssembly(path);
                        }
                        else
                        {
                            var req = HttpWebRequest.Create(path);
                            req.Method = "GET";
                            using(Stream stream = req.GetResponse().GetResponseStream())
                            {
                                List<byte> asmbytes = new List<byte>();
                                byte[] bytes = new byte[1024];
                                int read = bytes.Length;
                                while((read = stream.Read(bytes, 0, bytes.Length))!=0)
                                {
                                    byte[] nbs = new byte[read];
                                    Array.Copy(bytes, 0, nbs,0, read);
                                }
                                AddAssembly(asmbytes.ToArray());
                            }
                        }
                    }
                    else if (cmd == "del")
                    {
                        string args = line.Split(new char[] { ' ' }, 2)[1];
                        var path = GetArgs(args)[0];
                        RemoveVar(path.ToString());
                    }
                    else {
                        object[] args = GetArgs(line, true);
                    }
                }
                catch (Exception ex)
                {
                    result += $"LINE:{i+1} {line}:{ex.Message}\n";
                }
        }

            return result;
        }
        

        public string Execute(string code)
        {
            string[] lines = GetLines(code);
            return Logic(lines);
        }

        public string ExecuteFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            PATH = fi.FullName;
            RemoveVar("path_this");
            AddVar("path_this", PATH);
            string[] lines = GetLines(File.ReadAllText(path));
            return Logic(lines);
        }
    }
}
