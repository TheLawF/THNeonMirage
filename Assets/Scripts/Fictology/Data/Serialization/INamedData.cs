using System;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;

namespace Fictology.Data.Serialization
{
    public interface INamedData: ISynchronizable
    {
        private static Dictionary<SerializationType, Func<INamedData, INamedData>> Types = new()
        {
            { SerializationType.Bool, data => (BoolData)data },
            { SerializationType.Integer, data => (IntData)data },
            { SerializationType.Float, data => (FloatData)data },
            { SerializationType.String, data => (StringData)data },
            { SerializationType.IntList, data => (ListData)data },
            { SerializationType.Object, data => (CompoundData)data },
        };
        public const INamedData Empty = null;
        SerializationType GetSerializedType();

        INamedData GetDataFromType(SerializationType type)
        {
            return Types[type].Invoke(this);
        }
    }
}