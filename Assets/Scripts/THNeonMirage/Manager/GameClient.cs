using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Manager.UI;
using THNeonMirage.Map;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace THNeonMirage.Manager
{
    public class GameClient: GameBehaviourPunCallbacks
    {
        [DisplayOnly] public GameObject playerInstance;
        [Header("连接配置")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        public ObservableList<RoomInfo> rooms = new ();

        [Header("UI父组件")]
        public GameObject canvas;
        public GameObject hudPanel;
        public GameObject lobbyPanel;
        public GameObject inGamePanel;
        
        [Header("UI预制体")]
        public GameObject balanceLabel;
        public GameObject buttonPrefab;
        public GameObject progressPrefab;
        public GameObject content;
        
        private PlayerData data;
        private PlayerManager playerManager;
        
        private GameObject bar_instance;
        private Coroutine progress_coroutine;
        private RectTransform progress_transform;
        private RectTransform parent_transform;
        
        public virtual void Connect()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            
            progressPrefab.SetActive(true);
            var bar = Instantiate(progressPrefab, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
            
            bar_instance = bar;
            progress_transform = bar.GetComponent<RectTransform>();
            parent_transform = bar.transform.parent.GetComponent<RectTransform>();
            
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("客户端连接成功，进入大厅...");
            PhotonNetwork.JoinLobby();
            
            Destroy(bar_instance);
            lobbyPanel.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"加入到房间：{PhotonNetwork.CurrentRoom}");
            SceneManager.LoadScene("GameMap");
            CreatePlayer();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // 添加或更新开放房间
            foreach (var room in roomList.Where(r => r.IsOpen && !r.RemovedFromList)) {
                if (!rooms.Contains(room)) 
                    rooms.Add(room);
                else {
                    var index = rooms.IndexOf(room);
                    rooms[index] = room;
                }
            }

            // 更新UI
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            foreach (var room in rooms) {
                var newButton = Instantiate(buttonPrefab, content.transform);
                newButton.GetComponentInChildren<TMP_Text>().text = 
                    $"房间名：{room.Name} | 玩家：{room.PlayerCount}/{room.MaxPlayers}";
                newButton.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.Name));
            }
            AdjustContent(20); // 根据按钮高度调整
        }
        
        public void JoinRoom(string roomName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRoom(roomName);
                inGamePanel.SetActive(true);
                hudPanel.AddComponent<HudManager>();
                hudPanel.GetComponent<HudManager>().balanceLabel = balanceLabel;
                hudPanel.GetComponent<HudManager>().player = playerInstance;
            }
            else Debug.LogWarning("未连接到 Photon，无法加入房间！");
        }

        
        private void AdjustContent(int itemHeight)
        {
            var childCount = content.transform.childCount;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
        }
        
        private void CreatePlayer()
        {
            playerInstance = PhotonNetwork.Instantiate("playerObject",
                PlayerManager.GetPlayerPosByIndex(data.Position), Quaternion.identity);
            playerInstance.GetComponent<PlayerManager>().PlayerData.Balance = 600000;

            var playerManager = playerInstance.GetComponent<PlayerManager>();
            GameMap.players.Add(playerInstance);
            playerManager.Activity = GameMap.players.IndexOf(playerInstance);
        }
    }
}