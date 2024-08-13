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
        public List<GameObject> players;
        public List<GameObject> fields;
        private NetworkVariable<Player> serverPlayer;

        private void Start()
        {
            
        }

        public void CreateMap()
        {
            
        }

        public void CreatePlayer() => Instantiate(playerPrefab);
        
        public int StartServer() => 
            NetworkManager.Singleton.StartServer() ? LogUtil.Info("服务器启动成功") : LogUtil.Error("服务器启动失败");

        public int StartClient() =>
            NetworkManager.Singleton.StartClient() ? LogUtil.Info("客户端启动成功") : LogUtil.Error("客户端启动失败");

        public int StartHost()
        {
            
            return NetworkManager.Singleton.StartHost() ? LogUtil.Info("主机启动成功") : LogUtil.Error("主机启动失败");
        }

        public void ShutDown() => NetworkManager.Singleton.Shutdown();
    }

}