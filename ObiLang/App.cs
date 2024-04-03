using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ObiLang.Core;

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
                Console.WriteLine(Engine.ExecuteFile(argss[0]));
            }
            
        }

    }
}
