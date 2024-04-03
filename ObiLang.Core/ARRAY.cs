using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObiLang.Core
{
    public class ARRAY
    {
        public List<object> List = new List<object>();


        public void append(object obj) => List.Add(obj);
        public void append(object[] obj) => List.AddRange(obj);

        public void remove(int index) => List.RemoveAt(index);
        public void clear() => List.Clear();
        public int count() => List.Count;
        public object get(long index) => List[(int)index];

    }
}
