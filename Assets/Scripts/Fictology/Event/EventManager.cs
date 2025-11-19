using System;
using System.Collections.Generic;

namespace Fictology.Event
{
    public class EventManager
    {
        public static readonly Dictionary<EventKey, Delegate> m_events = new();
    }
}

