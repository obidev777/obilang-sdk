using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{

    public class FUNC
    {
        public string CMD { get; set; }
        public string[] Lines;
        public string[] Args { get; set; }
        public ObiScriptEngine engine;
        public static bool IS(string cmd)
        {
            return cmd.Contains("func");
        }

        public FUNC(string[] lines, string[] args, string cmd, ObiScriptEngine eng)
        {
            CMD = cmd;
            Lines = lines;
            Args = args;
            engine = eng;

        }

        public Delegate GetHandlerFor(EventInfo eventInfo)
        {
            MethodInfo DelegateMethodInfo = null;
            List<Type> paramsTypes = new List<Type>();
            var eventArgsType = eventInfo.EventHandlerType?.GetMethod("Invoke")?.GetParameters();
            var methods = GetType().GetMethods();
            for (int i= 0;i < methods.Length;i++)
            {
                var method = methods[i];
                if (method.Name == $"GenericHandleMethod{eventArgsType.Length}")
                {
                    DelegateMethodInfo = method;
                    break;
                }
            }
            for (int i = 0;i < eventArgsType.Length;i++)
            {
                var item = eventArgsType[i];
                paramsTypes.Add(item.ParameterType);
            }
            var handlerMethod = DelegateMethodInfo;

            if (DelegateMethodInfo.IsGenericMethod)
            {
                handlerMethod = DelegateMethodInfo?.MakeGenericMethod(paramsTypes.ToArray());
            }
            
            var target = Delegate.CreateDelegate(eventInfo.EventHandlerType,this, handlerMethod);

            return target;
        }

        public Delegate GetHandlerFor(PropertyInfo eventInfo)
        {
            MethodInfo DelegateMethodInfo = null;
            List<Type> paramsTypes = new List<Type>();
            var eventArgsType = eventInfo.GetMethod?.GetParameters();
            var methods = GetType().GetMethods();
            for (int i=0;i<methods.Length;i++)
            {
                var method = methods[i];
                if (method.Name == $"GenericHandleMethod{eventArgsType.Length}")
                {
                    DelegateMethodInfo = method;
                    break;
                }
            }
            for (int i=0;i< eventArgsType.Length;i++)
            {
                var item = eventArgsType[i];
                paramsTypes.Add(item.ParameterType);
            }

            MethodInfo handlerMethod = DelegateMethodInfo;

            if (paramsTypes.Count > 0)
            {
                handlerMethod = DelegateMethodInfo?.MakeGenericMethod(paramsTypes.ToArray());
            }
            
            var target = Delegate.CreateDelegate(eventInfo.PropertyType, this, handlerMethod);

            return target;
        }

        public Type[] GetParamsFor(EventInfo eventInfo)
        {
            List<Type> paramsTypes = new List<Type>();
            var eventArgsType = eventInfo.EventHandlerType?.GetMethod("Invoke")?.GetParameters();
            for (int i=0;i< eventArgsType.Length;i++)
            {
                var item = eventArgsType[i];
                paramsTypes.Add(item.ParameterType);
            }
            return paramsTypes.ToArray();
        }

        public void SetEventHandle(EventInfo ev, object instance)
        {
            Delegate target = GetHandlerFor(ev);
            ev.AddEventHandler(instance, target);
        }

        public void GenericHandleMethod0() => invoke(null);
        public void GenericHandleMethod1<T1>(T1 s) => invoke(s);
        public void GenericHandleMethod2<T1, T2>(T1 s, T2 e) => invoke(s, e);
        public void GenericHandleMethod3<T1, T2, T3>(T1 s, T2 e,T3 t3) => invoke(s, e, t3);
        public void GenericHandleMethod4<T1, T2, T3, T4>(T1 s, T2 e,T3 t3 , T4 t4) => invoke(s, e, t3 , t4);
        public void GenericHandleMethod5<T1, T2, T3, T4 , T5>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5) => invoke(s, e, t3 , t4,t5);
        public void GenericHandleMethod6<T1, T2, T3, T4 , T5,T6>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6) => invoke(s, e, t3 , t4,t5,t6);
        public void GenericHandleMethod7<T1, T2, T3, T4 , T5,T6,T7>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7) => invoke(s, e, t3 , t4,t5,t6,t7);
        public void GenericHandleMethod8<T1, T2, T3, T4 , T5,T6,T7,T8>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8) => invoke(s, e, t3 , t4,t5,t6,t7,t8);
        public void GenericHandleMethod9<T1, T2, T3, T4 , T5,T6,T7,T8,T9>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9);
        public void GenericHandleMethod10<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10);
        public void GenericHandleMethod11<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10,T11>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10,t11);
        public void GenericHandleMethod12<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10,T11,T12>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11,T12 t12) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10,t11,t12);
        public void GenericHandleMethod13<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10,T11,T12,T13>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11,T12 t12,T13 t13) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10,t11,t12,t13);
        public void GenericHandleMethod14<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11,T12 t12,T13 t13,T14 t14) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14);
        public void GenericHandleMethod15<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11,T12 t12,T13 t13,T14 t14,T15 t15) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15);
        public void GenericHandleMethod16<T1, T2, T3, T4 , T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(T1 s, T2 e,T3 t3 , T4 t4, T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11,T12 t12,T13 t13,T14 t14,T15 t15,T16 t16) => invoke(s, e, t3 , t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16);

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

            engine.Logic(Lines,localvars:true);
            
            for(int i=0;i<engine.LocalVars.Count;i++)
            {
                var v = engine.LocalVars[i];
                engine.RemoveVar(v);
            }
            engine.LocalVars.Clear();

            if (Args != null)
            for (int i = 0; i < Args.Length; i++)
            {
                engine.RemoveVar(Args[i]);
            }
            return null;
        }
    }
}
