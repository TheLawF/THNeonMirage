namespace Fictology.Data.Serialization
{
    public interface INamedData
    {
        public const INamedData Empty = null;
        SerializationType GetSerializedType();
    }
}