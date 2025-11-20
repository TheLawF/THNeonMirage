using FlyRabbit.EventCenter.Core;

namespace Fictology.UnityEditor
{
    [System.Serializable]
    public class EventReference
    {
        public EventReferenceType type;
        public EventName eventName;
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