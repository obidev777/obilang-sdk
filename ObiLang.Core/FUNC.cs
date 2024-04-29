using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{

    public class FUNC
    {
        public string CMD { get; set; }
        public string[] Lines;
        public string[] Args { get; set; }
        public ObiLangEngine engine;

        public  MethodInfo DelegateMethodInfo => this.GetType().GetMethod("GenericHandleMethod");

        public static bool IS(string cmd)
        {
            return cmd.Contains("func");
        }

        public FUNC(string[] lines, string[] args, string cmd, ObiLangEngine eng)
        {
            CMD = cmd;
            Lines = lines;
            Args = args;
            engine = eng;

        }

        private Delegate GetHandlerFor(EventInfo eventInfo)
        {
            List<Type> paramsTypes = new List<Type>();
            var eventArgsType = eventInfo.EventHandlerType?.GetMethod("Invoke")?.GetParameters();
            foreach (var item in eventArgsType)
            {
                paramsTypes.Add(item.ParameterType);
            }
            var handlerMethod = DelegateMethodInfo?.MakeGenericMethod(paramsTypes.ToArray());
            
            var target = Delegate.CreateDelegate(eventInfo.EventHandlerType,this, handlerMethod);

            return target;
        }
        private Type[] GetParamsFor(EventInfo eventInfo)
        {
            List<Type> paramsTypes = new List<Type>();
            var eventArgsType = eventInfo.EventHandlerType?.GetMethod("Invoke")?.GetParameters();
            foreach (var item in eventArgsType)
            {
                paramsTypes.Add(item.ParameterType);
            }
            return paramsTypes.ToArray();
        }

        public void SetEventHandle(EventInfo ev, object instance)
        {
            Delegate target = GetHandlerFor(ev);
            ev.AddEventHandler(instance, target);
        }

        public void GenericHandleMethod<T1, T2>(T1 s, T2 e) => invoke(s, e);

        public object invoke(params object[] args)
        {
            if(Args!=null)
            if (args.Length != Args.Length)
                throw new Exception($"This Func Contain ({Args.Length}) Params Error Invoke!");

            if (Args != null)
            for (int i=0;i<Args.Length;i++)
            {
                engine.AddVar(Args[i], args[i]);
            }

            engine.Logic(Lines);

            if (Args != null)
            for (int i = 0; i < Args.Length; i++)
            {
                engine.RemoveVar(Args[i]);
            }
            return null;
        }
    }
}
