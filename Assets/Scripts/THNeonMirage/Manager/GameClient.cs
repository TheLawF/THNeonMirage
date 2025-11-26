using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fictology.UnityEditor;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class GameClient: GameBehaviourPunCallbacks
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