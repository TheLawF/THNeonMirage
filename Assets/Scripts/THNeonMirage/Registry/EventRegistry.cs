using Fictology.Event;

namespace THNeonMirage.Registry
{
    public static class EventRegistry
    {
        public const string EventRootKey = "Event";
        public static readonly EventKey OnDestroyGameObject = new EventKey("OnDestroyGameObject");
        public static readonly EventKey OnBalanceChanged = new EventKey("OnBalanceChanged", 
            typeof(int), typeof(int));

        public static readonly EventKey OnPositionChanged = new EventKey("OnPositionChanged",
            typeof(int), typeof(int));
    }
}