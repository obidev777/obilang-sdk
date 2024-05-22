using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using Obi.Script;
using obisoft.net.html;

namespace ObiLang
{
    class App
    {
        public static ObiScriptEngine Engine;

        static void Main(string[] args)
        {
            gui.forms.XmlForm.Load("win.xml");

            string[] argss = args;
            argss = new string[] {"main.obi"};

            if (argss.Length > 0)
            {
                if (argss[0] == "--version")
                {
                    Console.WriteLine("ObiLang Version 1.1 (By ObisoftDev) @Copyrigth 2024");
                    return;
                }
                Engine = new ObiScriptEngine();
                Engine.Vars.Add("platform", "windows");
                Engine.Vars.Add("console", new ConsoleUtil());
                try
                {
                    string output = Engine.ExecuteFile(argss[0]);
                    Console.WriteLine(output);
                }
                catch (Exception ex){
                    Console.WriteLine($"Error Line: {Engine.GetLineExecution()} - ({ex.Message})");
                }
            }
            Console.WriteLine("Exit in 5 seconds...");
            Thread.Sleep(5000);
        }

    }
}
