using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using THNeonMirage.Registry;

namespace Fictology.Event
{
    public class EventKey: RegistryKey
    {
        public readonly Type[] Types;
        public EventKey(string registryName, params Type[] types) : base(EventRegistry.EventRootKey, registryName)
        {
            Types = types;
        }
    }
}