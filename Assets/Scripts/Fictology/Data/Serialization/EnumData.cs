using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using THNeonMirage.Util;

namespace Fictology.Data.Serialization
{
    public class EnumData: INamedData
    {
        private readonly List<EnumData> m_entries = new();
        private EnumData m_parent;
        public string Name { get; private set; }

        private EnumData(string name)
        {
            Name = name;
        }

        private EnumData(EnumData parent, string name)
        {
            m_parent = parent;
            Name = name;
        }

        public void AddEntry(EnumData newEnumEntry)
        {
            if (newEnumEntry.m_parent != null) return;
            if (m_entries.Any(e => e.Name == newEnumEntry.Name)) return;
            newEnumEntry.m_parent = this;
            newEnumEntry.m_entries.Clear();
            newEnumEntry.m_entries.AddRange(m_entries);
            m_entries.Add(newEnumEntry);
        }

        public EnumData Get(string name) => m_entries.First(e => e.Name == name);
        public int IndexOfValue(string name) => m_entries.IndexOf(Get(name));

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            
            writer.Write(Name);
            writer.Write(IndexOfValue(Name));

            var list = new ListData();
            list.AddRange(m_entries);
            writer.Write(m_entries.Count);
            writer.Write(list.ToBytes());
            return stream.ToArray();
        }

        public void FromBytes(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            Name = reader.ReadString();
            var index = reader.ReadInt32();
            var list = new ListData();
            list.FromBytes(reader.ReadBytes(reader.ReadInt32()));
        }

        public SerializationType GetSerializedType()
        {
            return SerializationType.Object;
        }
    }
}