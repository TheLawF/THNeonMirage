using System.Collections.Generic;
using Fictology.Event;
using FlyRabbit.EventCenter;
using THNeonMirage.Manager;
using Unity.Mathematics;

namespace THNeonMirage.Registry
{
    public static class EventRegistry
    {
        public const string EventRootKey = "Event";

        public static readonly EventKey OnRegister = EventCenter.Register("OnRegister",
            typeof(int), typeof(List<string>), typeof(Random));
        
        public static readonly EventKey OnDestroyGameObject = new EventKey("OnDestroyGameObject");
        public static readonly EventKey OnBalanceChanged = new EventKey("OnBalanceChanged", 
            typeof(int), typeof(int));

        public static readonly EventKey OnPositionChanged = new EventKey("OnPositionChanged",
            typeof(PlayerManager), typeof(int), typeof(int));
    }
}