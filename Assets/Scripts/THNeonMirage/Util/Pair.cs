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
            var sK = Key is ICollection key ? ListString(key) : Key.ToString();
            var sV = Value is ICollection value ? ListString(value) : Value.ToString();
            return $"{{Key = {sK}, Value = {sV}}}";
        }

        public string ToJsonString(string keyName, string valueName)
        {
            var kStr = $"{Key}";
            var vStr = $"{Value}";
            var sK = Key switch
            {
                int => kStr,
                long => kStr,
                short => kStr,
                float => kStr,
                double => kStr,
                string => $"\"{Key}\"",
                ICollection key => ListString(key),
                _ => ""
            }; 
            
            var sV = Value switch
            {
                int => vStr,
                long => vStr,
                short => vStr,
                float => vStr,
                double => vStr,
                string => $"\"{Value}\"",
                ICollection key => ListString(key),
                _ => ""
            }; 
            return $"{{\"{keyName}\":{sK},\"{valueName}\":{sV}}}";
        }
        
        public static string ListString(ICollection list)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var each in list)
            {
                sb.Append(each);
                sb.Append(list.GetEnumerator().MoveNext() ? "," : "");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
        
        public static string ObjString(object obj)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
    }
}