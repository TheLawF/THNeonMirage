using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Fictology.Registry;
using THNeonMirage.Registry;

namespace Fictology.Event
{
    public class EventKey: RegistryKey
    {
        public readonly Type[] ParameterTypes;
        public EventKey(string registryName, params Type[] parameterTypes) : base(EventRegistry.EventRootKey, registryName)
        {
            ParameterTypes = parameterTypes;
        }
    }
}