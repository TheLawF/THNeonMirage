using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Util;
using Unity.Netcode;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [Header("连接配置")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        
        public GameObject playerPrefab;
        private NetworkVariable<PlayerManager> serverPlayer;

        private void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            
            Utils.ForAct(20, i => CreateRoom($"{i}"));
        }

        private void Update()
        {
            
        }

        public void CreateMap()
        {
            
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("服务器连接成功");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"加入到房间：{PhotonNetwork.CurrentRoom}");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var info in roomList)
            {
                if (!info.IsOpen || info.RemovedFromList) 
                    DestroyExistingRoomEntry(info);
                else
                    CreateNewRoomEntry(info);
            }
        }

        private static void CreateNewRoomEntry(RoomInfo info)
        {
            
        }

        private static void DestroyExistingRoomEntry(RoomInfo info)
        {
        }

        private void CreateRoom(string roomName) => PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4});
        
        public static void JoinRoom(string roomName) => PhotonNetwork.JoinRoom(roomName);
        
        public void CreatePlayer() => Instantiate(playerPrefab);
        
        public int StartServer() => 
            Unity.Netcode.NetworkManager.Singleton.StartServer() ? Utils.Info("服务器启动成功") : Utils.Error("服务器启动失败");

        public int StartClient() =>
            Unity.Netcode.NetworkManager.Singleton.StartClient() ? Utils.Info("客户端启动成功") : Utils.Error("客户端启动失败");

        public int StartHost()
        {
            
            return Unity.Netcode.NetworkManager.Singleton.StartHost() ? Utils.Info("主机启动成功") : Utils.Error("主机启动失败");
        }

        public void ShutDown() => Unity.Netcode.NetworkManager.Singleton.Shutdown();
    }

}