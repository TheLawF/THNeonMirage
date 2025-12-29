using System.Text;
using Fictology.Data.Serialization;

namespace THNeonMirage.Util
{
    public class Pair<TKey, TValue>: ISerializable<CompoundData>where TKey : class, INamedData where TValue : class, INamedData
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public Pair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public static Pair<TKey, TValue> Of(TKey key, TValue value) => new (key, value);

        public override string ToString()
        {
            var sK = Key is ListData key ? ListString(key) : Key.ToString();
            var sV = Value is ListData value ? ListString(value) : Value.ToString();
            return $"{{Key = {sK}, Value = {sV}}}";
        }


        public string ToJsonString(string keyName, string valueName)
        {
            var kStr = $"{Key}";
            var vStr = $"{Value}";
            var sK = Key switch
            {
                ListData key => ListString(key),
                _ => ""
            }; 
            
            var sV = Value switch
            {
                ListData key => ListString(key),
                _ => ""
            }; 
            return $"{{\"{keyName}\":{sK},\"{valueName}\":{sV}}}";
        }
        
        public static string ListString(ListData list)
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

        public CompoundData Serialize()
        {
            var data = new CompoundData();
            data.Add("kay", Key);
            data.Add("value", Value);

            return data;
        }

        public void Deserialize(CompoundData data)
        {
            Key = data["key"] as TKey;
            Value = data["value"] as TValue;
        }
    }
}