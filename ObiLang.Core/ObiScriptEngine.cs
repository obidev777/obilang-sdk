using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public class ObiScriptEngine
    {
        public Dictionary<string, object> Vars = new Dictionary<string, object>();
        public List<Assembly> AsemblyList = new List<Assembly>();
        private string PATH { get; set; }

        public ObiScriptEngine(object obj_funcs = null)
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

        public ICacheHandle GetHandleFrom(string arg, object[] parms = null)
        {
            string[] tokens = arg.Replace("::","#").Split('#');
            string namelast = tokens[0];
            object v = GetVar(namelast);
            object scrap = v;
            List<string> temps = new List<string>();
            for (int i = 0; i < tokens.Length;i++)
            {
                namelast = tokens[i];
                string mname = tokens[i+1];
                if (i >= tokens.Length - 2)
                {
                    ICacheHandle handle = GetHandleFromObject(namelast, mname, parms, false);
                    handle.SetInstance(scrap);
                    for (int ic = 0; ic < temps.Count; ic++)
                        RemoveVar(temps[ic]);
                    return handle;
                }
                scrap = GetArgs(namelast+"::"+mname,false)[0];
                AddVar(mname,scrap);
                temps.Add(mname);
            }
            
            return null;
        }

        private Dictionary<string, CacheHandle> Caches = new Dictionary<string, CacheHandle>();

        public ICacheHandle GetHandleFromObject(string varname,string mname,object[] parms=null,bool usecache=true)
        {
            if (Caches.TryGetValue(varname,out CacheHandle handle))
            {
                var property = handle.GetProperty(mname);
                if (property != null)
                {
                    return ((ICacheHandle)property);
                }
                else
                {
                    var field = handle.GetField(mname);
                    if (field != null)
                    {
                        return ((ICacheHandle)field);
                    }
                    else
                    {
                        var method = handle.GetMethod(mname);
                        if (method != null)
                        {
                            return ((ICacheHandle)method);
                        }
                        else
                        {
                            var ev = handle.GetEvent(mname);
                            if (ev != null)
                            {
                                return ((ICacheHandle)ev);
                            }
                        }
                    }
                }
            }
            object ret = null;
            List<Type> parameters = new List<Type>();
            if (parms != null)
            {
                for(int i=0;i<parms.Length;i++)
                {
                   parameters.Add(parms[i].GetType());
                }
            }
            if (Vars != null)
            {
                object vvar = GetVar(varname);
                Type t = vvar.GetType();
                if (vvar != null)
                {
                    if (vvar.GetType().Name != "RuntimeType")
                    {
                        if(parms!=null)
                        {
                            if (parms.Length > 0)
                            {
                                ret = vvar.GetType().GetMethod(mname, parameters.ToArray());
                            }
                        }
                        if (ret == null)
                        {
                            ret = vvar.GetType().GetMethod(mname, parameters.ToArray());
                        }
                        if (ret == null)
                        {
                            ret = vvar.GetType().GetProperty(mname);
                        }
                        if (ret == null)
                        {
                            ret = vvar.GetType().GetField(mname);
                        }
                        if (ret == null)
                        {
                            ret = vvar.GetType().GetEvent(mname);
                        }
                    }
                    else
                    {
                        if (parms != null)
                        {
                            if (parms.Length > 0)
                            {
                                ret = ((Type)vvar).GetMethod(mname, parameters.ToArray());
                            }
                        }
                        if (ret == null)
                        {
                            ret = ((Type)vvar).GetMethod(mname, parameters.ToArray());
                        }
                        if (ret == null)
                        {
                            ret = ((Type)vvar).GetProperty(mname);
                        }
                        if (ret == null)
                        {
                            ret = ((Type)vvar).GetField(mname);
                        }
                        if (ret == null)
                        {
                            ret = ((Type)vvar).GetEvent(mname);
                        }
                    }
                        
                }
                if (ret != null)
                {
                    if (Caches.TryGetValue(varname, out CacheHandle newHandle))
                    {
                        newHandle.Instance = vvar;
                        return AddCache(newHandle, mname, ret, parms);
                    }
                    else
                    {
                        newHandle = new CacheHandle();
                        newHandle.Instance = vvar;
                        ICacheHandle cache = AddCache(newHandle, mname, ret, parms);
                        Caches.Add(varname, newHandle);
                        if (!usecache) Caches.Clear();
                        return cache;
                    }
                }
            }
            if (!usecache) Caches.Clear();
            return null;
        }

        private ICacheHandle AddCache(CacheHandle handle,string mname,object ret,object[] parms)
        {
            Type typ = ret.GetType();
            if (typ.Name.Contains("PropertyInfo"))
            {
                CacheHandleProperty prop = new CacheHandleProperty();
                prop.Name = mname;
                prop.Property = (PropertyInfo)ret;
                if (parms != null)
                {
                    prop.Args = parms.Length;
                }
                handle.Properties.Add(mname, prop);
                return (ICacheHandle)prop;
            }
            if (typ.Name.Contains("FieldInfo"))
            {
                CacheHandleField prop = new CacheHandleField();
                prop.Name = mname;
                prop.Field = (FieldInfo)ret;
                if (parms != null)
                {
                    prop.Args = parms.Length;
                }
                handle.Fields.Add(mname, prop);
                return (ICacheHandle)prop;
            }
            if (typ.Name.Contains("MethodInfo"))
            {
                CacheHandleMethod prop = new CacheHandleMethod();
                prop.Name = mname;
                prop.Method = (MethodInfo)ret;
                if (parms != null)
                {
                    prop.Args = parms.Length;
                }
                handle.Methods.Add(mname, prop);
                return (ICacheHandle)prop;
            }
            if (typ.Name.Contains("EventInfo"))
            {
                CacheHandleEvent prop = new CacheHandleEvent();
                prop.Name = mname;
                prop.Event = (EventInfo)ret;
                if (parms != null)
                {
                    prop.Args = parms.Length;
                }
                handle.Events.Add(mname, prop);
                return (ICacheHandle)prop;
            }
            return null;
        }

        private string[] GetLines(string code)
        {
            List<string> lines = new List<string>();
            string[] lins = code.Split('\n', '\r');
            for(int i=0;i< lins.Length; i++)
            {
                if (lins[i] == "" || lins[i][0]=='#') continue;
                lines.Add(lins[i].Replace("\t", ""));
            }
            return lines.ToArray();
        }

        public bool Recolect(string cmd)
        {
            string[] list = new string[] { 
            "for","if","while","func","try"
            };
            for (int i=0;i<list.Length;i++)
            {
                var item = list[i];
                if (item == cmd) return true;
            }
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
            if (args.ToLower() == "none") return new object[] { null };
            List<object> result = new List<object>();
            if (addengine)
                result.Add(this);

            List<string> tokens = new List<string>();

            string data = "";
            string arrray = "";
            int i = 0;
            for (int chi=0;chi<args.Length;chi++)
            {
                char chh = args[chi];
                i += 1;
                if (chh == '[' || chh == ']' || chh == '<' || chh == '>')
                    arrray = chh.ToString();
                if (chh == ',' && !args.Contains("::")
                    && !args.Contains(" is ")
                    && !args.Contains(" in ")
                    && !args.Contains(" not "))
                {
                    if (arrray == "[" || arrray == "<")
                    {
                        data += chh.ToString();
                    }
                    else
                    {
                        tokens.Add(data);
                        data = "";
                    }
                }
                else
                {
                    data += chh.ToString();
                }
                if (i >= args.Length)
                {
                    string arg = data;
                    if (arg == "") continue;
                    if (OPERATOR.Exists(arg) && !OPERATOR.Exists(arg[0].ToString()))
                    {
                        if (arg != "")
                        {
                            string oper = arg;
                            if (arg[0] == ':' && arg[1] == ':')
                            {
                                oper = oper.Substring(2);
                            }
                            object val = OPERATOR.Eval(this, oper);
                            result.Add(val);
                            continue;
                        }
                    }
                    if (arg.Contains(" is "))
                    {
                        string[] plags = arg.Split(new char[] { ' ' }, 3);
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
                        && arg[0] != '[' && arg[arg.Length - 1] != ']'
                        && !arg.Contains("::")
                        && !arg.Contains("->")
                        && arg[0] != '<' && arg[arg.Length - 1] != '>')
                    {
                        bool format = arg.Contains("@'");
                        string value = arg;
                        if (format)
                        {
                            value = value.Replace("@", "");
                        }
                        string lastrfm = "";
                        bool fmtsum = false;
                        List<string> Fixs = new List<string>();
                        if (format)
                            for (int ti = 0; ti < value.Length; ti++)
                            {
                                char ch = value[ti];
                                if (ch == '{')
                                {
                                    fmtsum = true;
                                    continue;
                                }
                                if (ch == '}')
                                {
                                    Fixs.Add(lastrfm.ToString());
                                    fmtsum = false;
                                    lastrfm = "";
                                    continue;
                                }
                                if (fmtsum)
                                {
                                    lastrfm += ch.ToString();
                                }
                            }
                        for(int ti = 0; ti < Fixs.Count; ti++)
                        {
                            value = value.Replace("{" + Fixs[ti] + "}", GetArgs(Fixs[ti])[0].ToString());
                        }
                        result.Add(value.Replace("'", ""));
                    }
                    else if (arg.Contains("[") && arg.Contains("]"))
                    {
                        string tks = null;
                        if(arg.Length>=2)
                            tks = arg.Substring(1, arg.Length - 2);
                        if (tks == "")
                            tks = null;
                        ARRAY array = new ARRAY();
                        if (tks != null)
                            array.append(GetArgs(tks));
                        result.Add(array);
                    }
                    else if (arg.Contains("<") && arg.Contains(">") && !arg.Contains("::") && !arg.Contains("->"))
                    {
                        string tks = null;
                        if (arg.Length >= 2)
                            tks = arg.Substring(1, arg.Length - 2);
                        if (tks == "")
                            tks = null;
                        DICT dict = new DICT();
                        if (tks != null)
                        {
                            object[] ta = GetArgs(tks);
                            if (ta != null)
                                dict.append(ta[0].ToString(), ta[1]);
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
                        string[] tks = newarg.Replace("->", "\n").Split(new char[] { ' ', '\n' }, 2);


                        string type = "";
                        if (tks[0] != "")
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

                        for (int ai=0;ai<assemblies.Length;ai++)
                        {
                            var asm = assemblies[ai];
                            if (asm.FullName.Contains(type) || type == "")
                            {
                                searchlist.Add(asm);
                            }
                        }
                        bool br = false;
                        string stt = string.Empty;
                        if (classname.Contains("::"))
                        {
                            string[] tkc = classname.Replace("::", "-").Split('-');
                            classname = tkc[0];
                            stt = tkc[1];
                        }

                        for (int asmi=0;asmi< searchlist.Count;asmi++)
                        {
                            var asm = searchlist[asmi];
                            var getTypes = asm.GetTypes();
                            for (int typi=0; typi<getTypes.Length; typi++)
                            {
                                var typ = getTypes[typi];
                                if (typ.Name == classname)
                                {
                                    if (stt == string.Empty)
                                    {
                                        if (stcobj)
                                        {
                                            Vars.Add(classname, typ);
                                        }
                                        else
                                        {
                                            object[] iargs = GetArgs(targ);
                                            object instance = Activator.CreateInstance(typ, iargs);
                                            result.Add(instance);
                                        }
                                    }
                                    else
                                    {
                                        object[] iargs = GetArgs(targ);
                                        int argcount = 0;
                                        if (iargs != null)
                                            argcount = iargs.Length;
                                        AddVar(classname, typ);
                                        ICacheHandle handle = GetHandleFromObject(classname, stt, iargs);
                                        if (handle != null)
                                        {
                                            object ret = handle.Invoke(null,iargs);
                                            result.Add(ret);
                                        }
                                        else
                                        {
                                            throw new Exception($"Not Exist {classname}.{stt}!");
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
                        string[] tkens = arg.Split(new char[] { ' ' }, 2);
                        string farg = null;
                        string fcmd = null;
                        if(tkens.Length>0)
                            fcmd = tkens[0];
                        if (tkens.Length > 1)
                            farg = tkens[1];
                        if (fcmd != null)
                        {
                            fcmd = fcmd.Replace("::", "-");
                            string[] tkes = fcmd.Split('-');
                            int count = tkes.Length;

                            if (tkes[0] == "")
                                count -= 1;

                            string vname = "";
                            string mname = "";


                            if (count <= 1)
                            {
                                vname = "base";
                                if (count >= tkes.Length)
                                    count--;
                                mname = tkes[count];
                            }
                            else
                            {
                                vname = tkes[0];
                                mname = tkes[1];
                            }

                            object[] fargs = null;

                            if (farg != null)
                            {
                                if (vname == "base")
                                    addengine = true;
                                else
                                    addengine = false;
                                List<object> fargslist = new List<object>();
                                string[] fargtoknes = farg.Split(',');
                                for (int fai=0; fai<fargtoknes.Length; fai++)
                                {
                                    var a = fargtoknes[fai];
                                    if (a != "")
                                    {
                                        fargslist.AddRange(GetArgs(a, addengine));
                                    }
                                }
                                fargs = fargslist.ToArray();
                            }

                            int argcount = 0;
                            if (fargs != null)
                                argcount = fargs.Length;
                            object oinstance = GetVar(vname);

                            if(oinstance!=null)
                            if (oinstance.GetType() == typeof(FUNC))
                            {
                                argcount = 1;
                                fargs = new object[] { fargs };
                            }

                            ICacheHandle handle = GetHandleFromObject(vname, mname, fargs);

                            if (handle != null)
                            {
                                if (handle.IsEvent())
                                {
                                    result.Add(handle.GetHandle());
                                }
                                else
                                {
                                    if (gettyping)
                                    {
                                        result.Add(handle.GetHandle());
                                        continue;
                                    }
                                    object ret = handle.Invoke(oinstance,fargs);
                                    result.Add(ret);
                                }
                            }
                            else
                            {
                                throw new Exception($"Not Exist {vname}.{mname}!");
                            }
                        }
                        else
                        {
                            throw new Exception($"Error Parsing Func!");
                        }
                    }
                    else if (arg.Contains("$"))
                    {
                        object ret = GetVar(arg.Replace("$", ""));
                        if (arg.Contains("(int)"))
                        {
                            ret = int.Parse(GetVar(arg.Replace("$", "").Replace("(int)", "")).ToString());
                        }
                        else if (arg.Contains("(float)"))
                        {
                            ret = int.Parse(GetVar(arg.Replace("$", "").Replace("(float)", "")).ToString());
                        }
                        else if (arg.Contains("(double)"))
                        {
                            ret = int.Parse(GetVar(arg.Replace("$", "").Replace("(double)", "")).ToString());
                        }
                        else if (arg.Contains("(long)"))
                        {
                            ret = long.Parse(GetVar(arg.Replace("$", "").Replace("(long)", "")).ToString());
                        }
                        result.Add(ret);
                    }
                    else if (arg == "true" || arg == "false")
                    {
                        if (arg == "true")
                            result.Add(true);
                        else
                            result.Add(false);
                    }
                    else
                    {
                        object val = null;
                        if (arg.Contains("(int)"))
                        {
                            val = int.Parse(arg.Replace("(int)", ""));
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
                            val = arg.Replace("(long)", "").ToString();
                        }
                        else
                        {
                            val = int.Parse(arg);
                        }
                        result.Add(val);
                    }
                    data = "";
                }
            }
            return result.ToArray();
        }

        public string FixLine(string line,char fin='@')
        {
            int index = line.IndexOf(fin);
            return line.Substring(index);
        }
        public List<string> LocalVars = new List<string>();
        private string line = "";
        public string GetLineExecution() => line;
        internal string Logic(string[] lines,object sender=null,bool localvars=false)
        {
            if(LocalVars.Count<=0)LocalVars.Clear();
            string result = "";
            
                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i];

                if (line == "") continue;
                if (line[0] == '#') continue;

                    // toda la otra logica

                    if (line.Contains("break"))
                        if (sender != null)
                            if (sender.GetType() == typeof(CONDITIONAL))
                            {
                                ((CONDITIONAL)sender).Condition = false;
                                return "Break";
                            }
                    
                    string[] tokens = line.Split(new char[] { ' ' }, 2);
                    string arg = "";

                    if (tokens.Length > 1)
                        arg = tokens[1];

                    string cmd = tokens[0];

                    bool recolect = Recolect(cmd);
                    if (recolect)
                    {
                        List<string> reclines = new List<string>();
                        List<string> elsereclines = new List<string>();
                        int ri = i;
                        int rindex = 1;
                        bool elselines = false;
                    string catchname = "catch";
                        for (ri = i + 1; ri < lines.Length; ri++)
                        {
                            string ll = lines[ri];
                            if (ll == "else" && CONDITIONAL.IS(cmd) 
                            || ll.Contains("catch")
                            && CONDITIONAL.IS(cmd))
                            {
                                elselines = true;
                                if (ll.Contains("catch "))
                                {
                                    catchname = ll.Replace("catch ", "");
                                }
                                continue;
                            }
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

                            if (!elselines)
                            {
                                reclines.Add(ll);
                            }
                            else
                            {
                                elsereclines.Add(ll);
                            }

                        }
                        i = ri;
                        if (CONDITIONAL.IS(cmd))
                        {
                        bool condition = false;
                        object[] argobj = GetArgs(arg);
                            if (argobj != null)
                        {
                            condition = (bool)(argobj[0]);
                        }
                            var cond = new CONDITIONAL(reclines.ToArray(),elsereclines.ToArray(), condition, cmd);
                        cond.catchname = catchname;
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
                        if (Vars.TryGetValue(name, out object value)){
                            if (localvars)
                            {
                                RemoveVar(name);
                                AddVar(name, GetArgs(vartokens[1])[0]);
                                LocalVars.Add(name);
                            }
                        }
                        else
                        {
                            AddVar(name,GetArgs(vartokens[1])[0]);
                            if (localvars)
                            {
                                LocalVars.Add(name);
                            }
                        }
                    }
                    else if (cmd == "out")
                    {
                        string[] vartokens = arg.Replace("::","#").Replace(" : ", ":").Split(new char[] { ':' }, 2);
                        string name = vartokens[0];
                        string argss = vartokens[1];
                        if (name.Contains("#"))
                        {
                            name = name.Replace("#", "::");
                        }
                        if (argss.Contains("#"))
                        {
                            argss = argss.Replace("#", "::");
                        }
                        bool is_delegate = false;
                        string vn = argss;
                        if (vn.Contains("(delegate)"))
                        {
                            is_delegate = true;
                            vn = vn.Replace("(delegate)","");
                        }
                        object value = GetArgs(vn)[0];
                        object property = GetArgs(name, gettyping: true)[0];
                        object varobj = GetVar(name.Replace("$","").Replace("::", "#").Split('#')[0]);
                        if (property.GetType().BaseType == typeof(PropertyInfo))
                            ((PropertyInfo)property).SetValue(varobj, Convert.ChangeType(value, ((PropertyInfo)property).GetValue(varobj).GetType()));
                        else if (property.GetType().BaseType == typeof(FieldInfo))
                            ((FieldInfo)property).SetValue(varobj, Convert.ChangeType(value, ((PropertyInfo)property).GetValue(varobj).GetType()));
                        else if (is_delegate)
                            ((FUNC)value).SetEventHandle((EventInfo)property, varobj);
                        else
                        {
                            string vname = name.Replace("$", "");
                            RemoveVar(vname);
                            AddVar(vname, value);
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
                            path = new FileInfo(path).FullName;
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
                        if (!line.Contains("::") && !line.Contains("use"))
                        {
                            line = "::" + line;
                        }
                        object[] args = GetArgs(line, true);
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
