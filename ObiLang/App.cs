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
using ObiLang.Core;
using obisoft.net.html;

namespace ObiLang
{
    class App
    {
        public static ObiLangEngine Engine;

        static void Main(string[] args)
        {
            string[] argss = args;
            //argss = new string[] {"main.obi"};

            if (argss.Length > 0)
            {
                if (argss[0] == "--version")
                {
                    Console.WriteLine("ObiLang Version 1.0 (By ObisoftDev) @Copyrigth 2024");
                    return;
                }
                Engine = new ObiLangEngine();
                Engine.Vars.Add("platform", "windows");
                Engine.Vars.Add("console", new ConsoleUtil());
                Console.WriteLine(Engine.ExecuteFile(argss[0]));
            }
            Console.WriteLine("Exit in 5 seconds...");
            Thread.Sleep(5000);
        }

    }
}
