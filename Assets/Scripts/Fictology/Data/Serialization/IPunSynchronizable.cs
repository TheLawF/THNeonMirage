namespace Fictology.Data.Serialization
{
    public interface IPunSynchronizable
    {
        byte[] ToBytes();
        void FromBytes(byte[] bytes);
    }
}