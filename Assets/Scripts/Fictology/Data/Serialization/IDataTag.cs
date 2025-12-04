using System.IO;

namespace Fictology.Data.Serialization
{
    public interface IDataTag
    {
        byte[] Write(MemoryStream stream);
        void Read(byte[] bytes);
    }
}