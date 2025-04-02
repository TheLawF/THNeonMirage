using System;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Event;
using TMPro;
using UnityEngine;

namespace THNeonMirage.Manager.UI
{
    public class HudManager: MonoBehaviour
    {
        // public GameObject player;
        public GameObject balanceLabel;
        public TMP_InputField roomNameInput;
        private TMP_Text balanceText;
        private PlayerData data;
        private void Start()
        {
            balanceText = balanceLabel.GetComponent<TMP_Text>();
            // data = player.GetComponent<PlayerManager>().PlayerData;
            // data.OnBalanceChanged += OnPlayerBalanceChanged;
        }

        private void OnPlayerBalanceChanged(object sender, ValueEventArgs args) 
            => balanceText.text = $"月虹币余额：{data.Balance}";
        
        public void CreateRoom()
        {
            if (roomNameInput.text == null) return;
            PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = 4 });
        }

        public static void ExitRoom() => PhotonNetwork.LeaveRoom();
    }
}