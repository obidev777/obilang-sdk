using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class DICT
    {
        public Dictionary<string,object> List = new Dictionary<string,object>();


        public void append(string key,object obj) => List.Add(key,obj);

        public void remove(string key) => List.Remove(key);
        public void clear() => List.Clear();
        public int count() => List.Count;
        public object get(string key) {
            if (List.TryGetValue(key, out object val))
                return val;
            return null;
        }

    }
}
