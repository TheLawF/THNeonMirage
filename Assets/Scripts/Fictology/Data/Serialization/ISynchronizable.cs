using System.IO;

namespace Fictology.Data.Serialization
{
    public interface ISynchronizable
    {
        byte[] ToBytes();
        void FromBytes(byte[] bytes);
        public static byte[] Serialize(object obj) => ((ISynchronizable)obj).ToBytes();

        public static ISynchronizable Deserialize(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return INamedData.Factory.Create((SerializationType)reader.ReadInt32());
        }
    }
}