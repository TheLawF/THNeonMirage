using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace THNeonMirage.Manager.UI
{
    public class DialogueHandler: MonoBehaviour
    {
        public GameObject inGamePanel;
        public GameObject dialogueUI;
        
        public TMP_Text textInput;
        public TMP_Text confirmText;
        public TMP_Text cancelText;
        
        private void Start()
        {
            confirmText.text = "加入房间";
            cancelText.text = "取消并退出";
        }

        public void CloseWindow() => dialogueUI.SetActive(false);

        public void OnCanceled()
        {
            CloseWindow();
        }

        public void OnJoinRoomConfirmed()
        {
            var roomName = textInput.text;
            if (roomName == null) return;
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRoom(roomName);
                inGamePanel.SetActive(true);
            }
            else Debug.LogWarning("未连接到 Photon，无法加入房间！");
        }
    }
}