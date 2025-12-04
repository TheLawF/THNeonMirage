using System;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;

namespace Fictology.Data.Serialization
{
    public interface INamedData: ISynchronizable
    {
        
        public const INamedData Empty = null;
        SerializationType GetSerializedType();

        INamedData GetDataFromType(SerializationType type)
        {
            return Factory.Create(type);
        }
        
        private static class Factory
        {
            private static Dictionary<SerializationType, Func<INamedData>> TypeFactories = new()
            {
                { SerializationType.Bool, () => new BoolData() },
                { SerializationType.Integer, () => new IntData() },
                { SerializationType.Float, () => new FloatData()},
                { SerializationType.String, () => new StringData()},
                { SerializationType.IntList, () => new ListData() },
                { SerializationType.FloatList, () => new ListData() },
                { SerializationType.StringList, () => new ListData() },
                { SerializationType.ObjectList, () => new ListData() },
                { SerializationType.Object, () => new CompoundData() },
            };
            public static INamedData Create(SerializationType type)
            {
                return TypeFactories[type].Invoke();
            }
        }
    }
}