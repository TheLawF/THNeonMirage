using System;
using System.Collections.Generic;
using THNeonMirage.Data;
using THNeonMirage.Util;
using Unity.Netcode;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        private NetworkVariable<PlayerManager> serverPlayer;

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        public void CreateMap()
        {
            
        }

        public void CreatePlayer() => Instantiate(playerPrefab);
        
        public int StartServer() => 
            NetworkManager.Singleton.StartServer() ? Utils.Info("服务器启动成功") : Utils.Error("服务器启动失败");

        public int StartClient() =>
            NetworkManager.Singleton.StartClient() ? Utils.Info("客户端启动成功") : Utils.Error("客户端启动失败");

        public int StartHost()
        {
            
            return NetworkManager.Singleton.StartHost() ? Utils.Info("主机启动成功") : Utils.Error("主机启动失败");
        }

        public void ShutDown() => NetworkManager.Singleton.Shutdown();
    }

}