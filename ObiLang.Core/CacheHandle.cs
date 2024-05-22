using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Script
{
    public interface ICacheHandle
    {
        void SetInstance(object instance);
        bool IsStatic();
        bool IsMethod();
        bool IsEvent();
        object GetHandle();
        string GetName();
        int GetArgs();
        object Invoke(object instance,object[] args);
        object Invoke(object[] args);
    }
    public class CacheHandleMethod: ICacheHandle
    {
        
        public string Name { get; set; }
        public MethodInfo Method { get; set; }
        public int Args { get; set; } = 0;

        public int GetArgs() => Args;

        public object GetHandle() => Method;

        public string GetName() => Name;

        public object Invoke(object instance, object[] args) => Method.Invoke(instance, args);

        

        public bool IsEvent() => false;

        public bool IsMethod() => true;

        public bool IsStatic() => Method.IsStatic;


        public object Instance { get; set; } = null;
        public void SetInstance(object instance) => Instance = instance;
        public object Invoke(object[] args) => Invoke(Instance, args);
    }
    public class CacheHandleProperty:ICacheHandle
    {
        public string Name { get; set; }
        public PropertyInfo Property { get; set; }
        public int Args { get; set; } = 0;

        public int GetArgs() => Args;

        public object GetHandle() => Property;

        public string GetName() => Name;
        public bool IsEvent() => false;
        public bool IsMethod() => false;

        public object Invoke(object instance, object[] args)
        {
            object ret = null;
            if (args != null)
            {
                if (args.Length > 0)
                {
                    Property.SetValue(instance, args[0]);
                }
                else
                {
                    ret = Property.GetValue(instance);
                }
            }
            else
            {
                ret = Property.GetValue(instance);
            }
            return ret;
        }

        public object Instance { get; set; } = null;
        public void SetInstance(object instance) => Instance = instance;
        public object Invoke(object[] args) => Invoke(Instance, args);

        public bool IsStatic() => false;
    }
    public class CacheHandleField : ICacheHandle
    {
        public string Name { get; set; }
        public FieldInfo Field { get; set; }
        public int Args { get; set; } = 0;

        public int GetArgs() => Args;

        public object GetHandle() => Field;

        public string GetName() => Name;
        public bool IsEvent() => false;
        public bool IsStatic() => Field.IsStatic;

        public bool IsMethod() => false;

        public object Invoke(object instance, object[] args)
        {
            object ret = null;
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (IsStatic())
                    {
                        Field.SetValue(null, args[0]);
                    }
                    else
                    {
                        Field.SetValue(instance, args[0]);
                    }
                }
                else
                {
                    if (IsStatic())
                    {
                        ret = Field.GetValue(null);
                    }
                    else
                    {
                        ret = Field.GetValue(instance);
                    }
                }
            }
            else
            {
                if (IsStatic())
                {
                    ret = Field.GetValue(null);
                }
                else
                {
                    ret = Field.GetValue(instance);
                }
            }
            return ret;
        }

        public object Instance { get; set; } = null;
        public void SetInstance(object instance) => Instance = instance;
        public object Invoke(object[] args) => Invoke(Instance, args);
    }
    public class CacheHandleEvent : ICacheHandle
    {
        public string Name { get; set; }
        public EventInfo Event { get; set; }
        public int Args { get; set; } = 0;

        public int GetArgs() => Args;

        public object GetHandle() => Event;

        public string GetName() => Name;
        public bool IsEvent() => true;
        public bool IsStatic() => false;


        public bool IsMethod() => true;

        public object Invoke(object instance, object[] args)
        {
            object ret = null;
            if (args != null)
            {
                var set = args[0];
                if (set.GetType() == typeof(FUNC))
                {
                    ((FUNC)set).SetEventHandle(Event,instance);
                }
            }
            return ret;
        }
        public object Instance { get; set; } = null;
        public void SetInstance(object instance) => Instance = instance;
        public object Invoke(object[] args) => Invoke(Instance, args);
    }
    public class CacheHandle
    {
        public string Name { get; set; }
        public Type HandleType { get; set; }
        public object Instance { get; set; }
        public Dictionary<string, CacheHandleMethod> Methods { get; private set; } = new Dictionary<string, CacheHandleMethod>();
        public Dictionary<string, CacheHandleProperty> Properties { get; private set; } = new Dictionary<string, CacheHandleProperty>();
        public Dictionary<string, CacheHandleField> Fields { get; private set; } = new Dictionary<string, CacheHandleField>();
        public Dictionary<string, CacheHandleEvent> Events { get; private set; } = new Dictionary<string, CacheHandleEvent>();

        public CacheHandleMethod GetMethod(string name)
        {
            if(Methods.TryGetValue(name,out CacheHandleMethod method))
            {
                return method;
            }
            return null;
        }
        public CacheHandleProperty GetProperty(string name)
        {
            if (Properties.TryGetValue(name, out CacheHandleProperty value))
            {
                return value;
            }
            return null;
        }
        public CacheHandleField GetField(string name)
        {
            if (Fields.TryGetValue(name, out CacheHandleField value))
            {
                return value;
            }
            return null;
        }
        public CacheHandleEvent GetEvent(string name)
        {
            if (Events.TryGetValue(name, out CacheHandleEvent value))
            {
                return value;
            }
            return null;
        }
    }
}
