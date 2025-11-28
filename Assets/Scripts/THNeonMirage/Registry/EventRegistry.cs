using System.Collections.Generic;
using Fictology.Event;
using FlyRabbit.EventCenter;
using Photon.Pun;
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
             typeof(PlayerManager),typeof(int), typeof(int));

        public static readonly EventKey OnPositionChanged = new EventKey("OnPositionChanged",
            typeof(PlayerManager), typeof(int), typeof(int));

        public static readonly EventKey OnPositionChangedRPC = new EventKey(nameof(OnPositionChangedRPC),
            typeof(PhotonView), typeof(int), typeof(int));

        public static readonly EventKey OnBalanceChangedRPC = new EventKey(nameof(OnBalanceChangedRPC),
            typeof(PhotonView), typeof(int), typeof(int));

        public static readonly EventKey OnRoundStarted = new EventKey(nameof(OnRoundStarted));
    }
}