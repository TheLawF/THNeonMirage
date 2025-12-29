#if UNITY_EDITOR
using FlyRabbit.EventCenter.Core;
using UnityEngine.Serialization;

namespace Fictology.UnityEditor
{
    [System.Serializable]
    public class EventReference
    {
        public EventReferenceType type;
        public string className;
        public string eventKeyFieldName;
        public string fieldName;
        public string filePath;
        public int lineNumber;
        public string genericTypes;
    }

    public enum EventReferenceType
    {
        AddListener,
        RemoveListener, 
        TriggerEvent
    }
}
#endif