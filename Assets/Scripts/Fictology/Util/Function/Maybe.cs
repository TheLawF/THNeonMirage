using System;
using Fictology.Data.Serialization;

namespace Fictology.Util.Function
{
    public class Maybe<TData> : ISerializable<TData> where TData : INamedData
    {
        private TData _value;
        public Maybe(TData value) => _value = value;

        public TData Serialize()
        {
            throw new NotImplementedException();
        }

        public void Deserialize(TData data)
        {
            throw new NotImplementedException();
        }
    }

}