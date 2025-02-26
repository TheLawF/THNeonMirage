using System;
using THNeonMirage.Data;
using THNeonMirage.Event;
using TMPro;
using UnityEngine;

namespace THNeonMirage.Manager.UI
{
    public class HudManager: MonoBehaviour
    {
        public GameObject balanceLabel;
        private TMP_Text balanceText;
        private PlayerData data;
        private void Start()
        {
            balanceText = balanceLabel.GetComponent<TMP_Text>();
            data = PlayerManager.Instance.GetComponent<PlayerManager>().PlayerData;
            data.OnBalanceChanged += OnPlayerBalanceChanged;
        }

        private void OnPlayerBalanceChanged(object sender, ValueEventArgs args) 
            => balanceText.text = $"月虹币余额：{data.Balance}";
    }
}