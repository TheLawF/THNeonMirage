using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Manager.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public void Connect()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
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
        
        private IEnumerator CreateRooms()
        {
            for (var i = 0; i <= 20; i++)
            {
                PhotonNetwork.CreateRoom($"{i}", new RoomOptions { MaxPlayers = 4 });
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("房间列表更新成功，创建了20个房间");
            yield return new WaitForSeconds(1f);
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
    }

}