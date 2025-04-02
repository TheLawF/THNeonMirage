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
        public GameObject dialogueUI;
        public GameObject textInput;

        public GameObject confirm;
        private Button confirmButton;
        
        private void Start()
        {
            confirmButton.text = "加入房间";
        }

        public void CloseWindow() => dialogueUI.SetActive(false);

        public void OnCanceled()
        {
            CloseWindow();
        }

        public void OnJoinRoomConfirmed()
        {
            var roomName = textInput.GetComponent<TMP_Text>().text;
            if (roomName == null ) return;
            PhotonNetwork.JoinRoom(roomName);
        }
    }
}