using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Manager.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class GameClient: MonoBehaviourPunCallbacks
    {
        [Header("连接配置")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        public readonly bool IsClientSide = true;
        public List<string> rooms = new ();

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

        private GameObject bar_instance;
        private Coroutine progress_coroutine;
        private RectTransform progress_transform;
        private RectTransform parent_transform;
        
        public void Connect()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5056;
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
            AddButtons();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"加入到房间：{PhotonNetwork.CurrentRoom}");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var room in roomList.Where(room => !room.IsOpen && !room.RemovedFromList))
            {
                rooms.Add(room.Name);
                Debug.Log($"房间：{room.Name} 已创建");
            }
            Debug.Log($"当前房间列表: {string.Join(", ", rooms)}");
        }

        private void AdjustContent(int itemHeight)
        {
            var childCount = content.transform.childCount;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, childCount * itemHeight);
        }

        private void AddButton(string roomName)
        {
            var newButton = Instantiate(buttonPrefab, content.transform);
            newButton.GetComponent<Button>().onClick.AddListener(() => JoinRoom(roomName));
            newButton.GetComponentInChildren<TMP_Text>().text = $"房间号：#{roomName}";
            AdjustContent(10);
        }

        private void AddButtons()
        {
            for (var i = 1; i <= 20; i++)
            {
                AddButton($"{i}");
            }
        }
        
        public void JoinRoom(string roomName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRoom(roomName);
                SceneManager.LoadScene("GameMap");
                Debug.Log($"尝试加入房间: {roomName}");
                
                inGamePanel.SetActive(true);
                hudPanel.AddComponent<HudManager>();
                hudPanel.GetComponent<HudManager>().balanceLabel = balanceLabel;
            }
            else
            {
                Debug.LogWarning("未连接到 Photon，无法加入房间！");
            }
        }

    }
}