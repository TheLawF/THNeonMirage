using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Fictology.Data.Serialization
{
    [Serializable]
    public class CompoundData : INamedData
    {
        // public readonly ScriptExecutor Executor = new();
        public SerializationType serializationType = SerializationType.Object;
        private readonly ConcurrentDictionary<string, CompoundData> m_entries;
        public List<string> keys = new();
        [SerializeField]
        public List<INamedData> fields = new();

        public SerializationType GetSerializedType()
        {
            return serializationType;
        }

        public void Add(string key, CompoundData data)
        {
            m_entries.TryAdd(key, data);
        }
        
        public void Set(string key, CompoundData data)
        {
            fields[fields.IndexOf(Get(key))] = data;
        }
        
        public void AddFloat(string key, float value)
        {
            keys.Add(key);
            fields.Add(new FloatData(value));
            keys = keys.Distinct().ToList();
        }
        
        public void AddAll(IEnumerable<string> keyList, IEnumerable<INamedData> dataList)
        {
            keys.AddRange(keyList);
            fields.AddRange(dataList);
            keys = keys.Distinct().ToList();
        }

        public INamedData Get(string key)
        {
            return fields[keys.IndexOf(key)];
        }

        public float GetFloat(string key)
        {
            Debug.Log($"Keys: {keys[0]}");
            Debug.Log($"Fields: {fields.Count}");
            return fields[keys.IndexOf(key)] is FloatData ? ((FloatData) fields[keys.IndexOf(key)]).value : 0f;
        }

        public static CompoundData operator +(CompoundData left, CompoundData right)
        {
            var data = new CompoundData();
            data.AddAll(left.keys, left.fields);
            data.AddAll(right.keys, right.fields);
            return data;
        }
        
    }
}