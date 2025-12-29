using System;
using Photon.Pun;
using THNeonMirage.Event;

namespace THNeonMirage.Manager
{
    [Obsolete]
    public class ObsoleteGameClient: GameBehaviourPunCallbacks
    {
        
        private void Start()
        {
            OnInstantiate += Initialize;
        }

        public void CreatePlayerIf()
        {
            PhotonView view;
        }
        
    }

    public class PlayerEventArgs : IGameEventArgs
    {
        public int Round;

        public PlayerEventArgs(int round)
        {
            Round = round;
        }
    }
}