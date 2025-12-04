using System;
using System.IO;

namespace Fictology.Data.Serialization
{
    [Serializable]
    public class StringData : INamedData
    {
        public SerializationType serializationType = SerializationType.String;
        public readonly string value;

        public StringData(string value)
        {
            this.value = value;
        }

        public SerializationType GetSerializedType()
        {
            return SerializationType.String;
        }

        public byte[] Serialize(MemoryStream stream)
        {
            using var writer = new BinaryWriter(stream);
            writer.Write(value);
            return stream.ToArray();
        }

        public void Deserialize(INamedData data)
        {
            
        }
    }
}