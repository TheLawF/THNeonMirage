using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class GameClient: MonoBehaviourPunCallbacks
    {
        [Header("连接配置")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        public readonly bool IsClientSide = true;
        public List<string> rooms = new ();

        public GameObject balanceLabel;
        public GameObject buttonPrefab;
        public GameObject canvas;
        public GameObject lobbyPanel;

        public GameObject hudPanel;
        public GameObject inGamePanel;
        public GameObject progressPrefab;
        public GameObject content;

        private GameObject bar_instance;
        private Coroutine progress_coroutine;
        private RectTransform progress_transform;
        private RectTransform parent_transform;
    }
}