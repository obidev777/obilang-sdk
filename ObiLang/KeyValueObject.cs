using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang
{
    public class KeyValueObject
    {
        public Dictionary<string, object> KeyValues = new Dictionary<string, object>();

        public object get(string name)
        {
            if (KeyValues.TryGetValue(name, out object value))
                return value;
            return null;
        }

        public void add(string key, object value) => KeyValues.Add(key, value);
    }
}
