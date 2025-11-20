using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Fictology.Event
{
    public class EventKey
    {
        public readonly string EventName;

        public EventKey()
        {
            EventName = new UniqueId().ToString();
        }
        public EventKey(string eventName)
        {
            EventName = eventName;
        }
    }
}