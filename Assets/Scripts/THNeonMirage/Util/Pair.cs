using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace THNeonMirage.Util
{
    public class Pair<TK, TV>
    {
        public TK Key { get; set; }
        public TV Value { get; set; }

        public Pair(TK key, TV value)
        {
            Key = key;
            Value = value;
        }

        public static Pair<TK, TV> Of(TK key, TV value) => new (key, value);

        public override string ToString()
        {
            Debug.Log(Key is IEnumerable);
            var sK = Key is IEnumerable key ? ListString(key) : Key.ToString();
            var sV = Value is IEnumerable value ? ListString(value) : Value.ToString();
            return $"{{Key = {sK}, Value = {sV}}}";
        }
        
        public static string ListString(IEnumerable list)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var enumerable = list as object[] ?? list.Cast<object>().ToArray();
            while (enumerable.GetEnumerator().MoveNext())
            {
                sb.Append(enumerable.GetEnumerator().Current);
                sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}