using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fictology.Data.Serialization
{
    [Serializable]
    public class ListData
    {
        public SerializationType serializationType = SerializationType.Null;
        public string name;
        [SerializeReference] private List<INamedData> dataList;

        public ListData(string name)
        {
            this.name = name;
            dataList = new List<INamedData>();
        }

        public void Add(INamedData data)
        {
            var type = data.GetSerializedType();
            serializationType = type switch
            {
                SerializationType.Integer => SerializationType.IntList,
                SerializationType.Float => SerializationType.FloatList,
                SerializationType.Vec2 => SerializationType.Vec2List,
                SerializationType.Vec3 => SerializationType.Vec3List,
                SerializationType.StringList => SerializationType.StringList,
                SerializationType.Danmaku => SerializationType.DanmakuList,
                SerializationType.Expression => SerializationType.Null,
                _ => serializationType
            };
            dataList.Add(data);
        }

        public List<INamedData> Get() => dataList;
        
        public INamedData this[int index] => dataList[index];
    }
}