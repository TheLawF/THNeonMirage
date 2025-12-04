using System;
using System.Collections.Concurrent;
using System.IO;
using Unity.VisualScripting;

namespace Fictology.Data.Serialization
{
    [Serializable]
    public class CompoundData : INamedData
    {
        // public readonly ScriptExecutor Executor = new();
        public const SerializationType SerializationType = Serialization.SerializationType.Object;
        private readonly ConcurrentDictionary<string, INamedData> m_entries;

        public SerializationType GetSerializedType()
        {
            return SerializationType;
        }

        public void Add(string key, INamedData data)
        {
            m_entries.TryAdd(key, data);
        }
        
        public void Set(string key, INamedData data)
        {
            m_entries[key] = data;
        }

        public void AddInt(string key, int value) => Add(key, IntData.Of(value));
        public void AddFloat(string key, float value)
        {
            m_entries.TryAdd(key, new FloatData(value));
        }
        
        public void AddAll(CompoundData data)
        {
            m_entries.AddRange(data.m_entries);
        }

        public INamedData Get(string key)
        {
            return m_entries[key];
        }

        public float GetFloat(string key) => m_entries[key] is FloatData ? ((FloatData)m_entries[key]).value : 0f;
        public int GetInt(string key) => m_entries[key] is IntData ? ((IntData)m_entries[key]).value : 0;
        public bool GetBool(string key) => m_entries[key] is BoolData && ((BoolData)m_entries[key]).value;

        public static CompoundData operator +(CompoundData left, CompoundData right)
        {
            var data = new CompoundData();
            data.AddAll(left);
            data.AddAll(right);
            return data;
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
    
            writer.Write(m_entries.Count);
            foreach (var kv in m_entries)
            {
                writer.Write(kv.Key);
                writer.Write((int)kv.Value.GetSerializedType());
        
                var dataBytes = kv.Value.ToBytes();
                writer.Write(dataBytes.Length); // 写入数据长度
                writer.Write(dataBytes);
            }
    
            return stream.ToArray();
        }

        public void FromBytes(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            var count = reader.ReadInt32();
            for (var i = 0; i < count - 1; i++)
            {
                var key = reader.ReadString();
                var type = (SerializationType)reader.ReadInt32();
                
                var dataLength = reader.ReadInt32();
                var dataBytes = reader.ReadBytes(dataLength);
                var value = INamedData.Factory.Create(type);
                
                value.FromBytes(dataBytes);
                m_entries.TryAdd(key, value);
            }
        }

        public static byte[] Serialize(ISynchronizable synchronizable) => synchronizable.ToBytes();

        public static ISynchronizable Deserialize(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return INamedData.Factory.Create((SerializationType)reader.ReadInt32());
        }
    }
}