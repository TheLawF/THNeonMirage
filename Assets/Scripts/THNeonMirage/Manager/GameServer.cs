using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Manager.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class GameServer : MonoBehaviourPunCallbacks
    {
        [Header("连接配置")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        public readonly bool IsClientSide = false;
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

        public void Connect()
        {
            lobbyPanel.SetActive(true);
            progressPrefab.SetActive(true);
            var bar = Instantiate(progressPrefab, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
            
            bar_instance = bar;
            progress_transform = bar.GetComponent<RectTransform>();
            parent_transform = bar.transform.parent.GetComponent<RectTransform>();
            
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("服务器连接成功");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("已加入大厅，等待房间列表更新...");
            StartCoroutine(CreateRooms());
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

        private IEnumerator CreateRooms()
        {
            for (var i = 0; i <= 20; i++)
            {
                PhotonNetwork.CreateRoom($"{i}", new RoomOptions { MaxPlayers = 4 });
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1f);
            
            lobbyPanel.SetActive(true);
            bar_instance.GetComponent<ProgressBarControl>().isReady = true;

            yield return new WaitForSeconds(2f);
            Destroy(bar_instance);
            AddButtons();
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
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"房间已创建");
            switch(returnCode)
            {
                case ErrorCode.InvalidOperation:
                    Debug.LogWarning("操作无效：请检查网络连接");
                    break;
                case ErrorCode.GameFull:
                    Debug.LogWarning("房间已满");
                    break;
                default:
                    Debug.LogWarning($"创建失败: {message}");
                    break;
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning($"加入房间失败：{message}");
        }
        
        // public void CreatePlayer() => Instantiate(playerPrefab);

        #region Unity Netcode Obsolete
        public int StartServer() => 
            Unity.Netcode.NetworkManager.Singleton.StartServer() ? Utils.Info("服务器启动成功") : Utils.Error("服务器启动失败");

        public int StartClient() =>
            Unity.Netcode.NetworkManager.Singleton.StartClient() ? Utils.Info("客户端启动成功") : Utils.Error("客户端启动失败");

        public int StartHost()
        {
            return Unity.Netcode.NetworkManager.Singleton.StartHost() ? Utils.Info("主机启动成功") : Utils.Error("主机启动失败");
        }

        public void ShutDown() => Unity.Netcode.NetworkManager.Singleton.Shutdown();

        #endregion
    }

}