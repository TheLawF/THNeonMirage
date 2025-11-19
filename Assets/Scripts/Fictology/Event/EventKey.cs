using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Fictology.Event
{
    public class EventKey
    {
        public readonly string EventName;
        public readonly List<object> Parameters;
        public readonly List<Type> ParameterTypes;
        public readonly int ParameterCount;

        public EventKey(List<object> parameters)
        {
            EventName = new UniqueId().ToString();
            Parameters = parameters;
            ParameterTypes = Parameters.ConvertAll(obj => obj.GetType()).ToList();
            ParameterCount = parameters.Count;
        }

        public EventKey(params object[] parameters)
        {
            EventName = new UniqueId().ToString();
            Parameters = parameters.ToList();
            ParameterTypes = Parameters.ConvertAll(obj => obj.GetType()).ToList();
            ParameterCount = parameters.Length;
        }
        
        public EventKey(string eventName, List<object> parameters)
        {
            EventName = eventName;
            Parameters = parameters;
            ParameterTypes = Parameters.ConvertAll(obj => obj.GetType()).ToList();
            ParameterCount = parameters.Count;
        }

        public EventKey(string eventName, params object[] parameters)
        {
            EventName = eventName;
            Parameters = parameters.ToList();
            ParameterTypes = Parameters.ConvertAll(obj => obj.GetType()).ToList();
            ParameterCount = parameters.Length;
        }

        public void Invoke(Delegate eventDelegate)
        {
            eventDelegate.DynamicInvoke(Parameters.ToArray());
        }
    }
}