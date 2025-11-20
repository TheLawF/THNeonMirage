using Fictology.Event;

namespace THNeonMirage.Registry
{
    public class EventRegistry
    {
        public const string EventRootKey = "Event";
        public static readonly EventKey OnDestroyGameObject = new EventKey();
    }
}