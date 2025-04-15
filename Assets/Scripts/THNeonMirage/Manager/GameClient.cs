using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager.UI;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class GameClient: GameBehaviourPunCallbacks
    {
        public GameObject playerPrefab;
        public PlayerData data;
        [DisplayOnly] public GameObject playerInstance;
        [DisplayOnly] public GameObject client;

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

        [DisplayOnly] public DiceHandler dice;
        [DisplayOnly] public PlayerManager playerManager;
        private TMP_Text balance_text;
        private GameObject bar_instance;
        
        private RectTransform progress_transform;
        private RectTransform parent_transform;

        private void Start()
        {
            OnInstantiate += Initialize;
        }

        protected override GameObject Initialize<TArgs>(string prefabName, Vector3 pos, Quaternion rotation, TArgs arg5)
        {
            playerInstance = PhotonNetwork.Instantiate(prefabName, pos, rotation);
            if (arg5 is not PlayerEventArgs args) return playerInstance;
            lobbyPanel.SetActive(false);
            inGamePanel.SetActive(true);
            
            var inGame = inGamePanel.GetComponent<InGamePanelHandler>();
            inGame.player = playerManager;
            inGame.client = this;
            
            balance_text = balanceLabel.GetComponent<TMP_Text>();
            playerManager = playerInstance.GetComponent<PlayerManager>();

            playerManager.gameMap = gameMap;
            playerManager.PlayerData = data;
            playerManager.Instance = playerInstance;
            
            playerManager.PlayerData.OnBalanceChanged += SetLabelWhenBalanceChanged;
            playerManager.PlayerData.Balance += 60_000;

            return playerInstance;
        }

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

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnJoinedRoom()
        {
            Debug.Log($"加入到房间：{PhotonNetwork.CurrentRoom}");
            hudPanel.SetActive(true);
            CreatePlayer();

            gameMap.CreateMap();
            gameMap.client = this;
            playerManager.PlayerIndex = PhotonNetwork.CurrentRoom.Players.Keys.Count;
            playerManager.PlayerData.Uid(PhotonNetwork.LocalPlayer.UserId);
            playerManager.dice = dice;
            playerManager.PlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber;
            
            GameMap.Players.Add(PhotonNetwork.LocalPlayer);
            gameMap.PlayerOrder.AddRange(PhotonNetwork.PlayerList.Select(player => player.ActorNumber).ToList());
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "can_interact", "true" } });
        }
        
        public void SetLabelWhenBalanceChanged(object sender, ValueEventArgs args)
            => balance_text.text = $"月虹币余额：{(int)args.Value}";
        

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
                    $"房间名：{room.Name}  玩家：{room.PlayerCount}/{room.MaxPlayers}";
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
            }
            else Debug.LogWarning("未连接到 Photon，无法加入房间！");
        }

        
        private void AdjustContent(int itemHeight)
        {
            var childCount = content.transform.childCount;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
        }
        
        public void CreatePlayer()
        {
            Initialize("playerObject", PlayerManager.GetPlayerPosByIndex(data.Position), Quaternion.identity, new PlayerEventArgs(0));
            // playerInstance = PhotonNetwork.Instantiate("playerObject",
            //     PlayerManager.GetPlayerPosByIndex(data.Position), Quaternion.identity);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            gameMap.PlayerOrder.Add(newPlayer.ActorNumber);
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