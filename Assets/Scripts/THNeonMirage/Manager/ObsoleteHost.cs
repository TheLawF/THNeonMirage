using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace THNeonMirage.Manager
{
    [Obsolete]
    public class ObsoleteHost : ObsoleteGameClient
    {
        public TMP_InputField roomNameInput;
        [Header("连接配置")]
        // public string gameVersion = "1.0";
        public static int MaxPlayersPerRoom = 4;
        public List<string> rooms = new ();

        // public override void Connect()
        // {
        //     PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
        //     PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
        //     PhotonNetwork.AutomaticallySyncScene = true;
        //     PhotonNetwork.GameVersion = gameVersion;
        //     PhotonNetwork.ConnectUsingSettings();
        // }

        public override void OnJoinedLobby()
        {
            Debug.Log("已加入大厅，等待房间列表更新...");
            // StartCoroutine(CreateRooms());
        }
        
        private IEnumerator CreateRooms()
        {
            var roomCount = 20;
            for (var i = 0; i < roomCount; i++)
            {
                PhotonNetwork.CreateRoom($"{i}", 
                    new RoomOptions
                    {
                        MaxPlayers = 4,
                        EmptyRoomTtl = 100000
                    });
                yield return new WaitForSeconds(2f);
            }
            Debug.Log("房间列表更新成功，创建了20个房间");
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