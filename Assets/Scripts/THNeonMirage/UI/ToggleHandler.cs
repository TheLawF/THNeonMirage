using System.Collections.Generic;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.UI
{
    public class ToggleHandler : MonoBehaviour
    {
        public PlayerManager player;
        public FieldTile currentField;
        public List<GameObject> toggleList;

        public GameObject screen;
        public GameObject house;
        public GameObject land;
        public GameObject confirmSale;
        public GameObject doubleConfirm;
        public TMP_Text countLabel;

        private Toggle houseToggle;
        private Toggle landToggle;
        private void Start()
        {
            houseToggle = house.GetComponent<Toggle>();
            landToggle = land.GetComponent<Toggle>();
        }

        public void OnAdd()
        {
            if (player.playerData.position != currentField.index) return;
            var i = int.Parse(countLabel.text);
            if (i > currentField.level) return;
            countLabel.text = $"{i + 1}";
            SetText();
        }
        
        public void OnSub()
        {
            if (player.playerData.position != currentField.index) return;
            var i = int.Parse(countLabel.text);
            if (i <= 0) return;
            countLabel.text = $"{i - 1}";
            SetText();
        }

        public void OnHouseSelected()
        {
            landToggle.isOn = false;
            SetText();
        }

        public void OnLandSelected()
        {
            houseToggle.isOn = false;
            SetText();
        }

        public void OnConfirm() => doubleConfirm.SetActive(true);
        public void OnCanceled() => doubleConfirm.SetActive(false);

        public void OnDoubleConfirmSale()
        {
            player.playerData.balance += int.Parse(countLabel.text) * currentField.Property.Price.Building +
                                         (landToggle.isOn ? currentField.Property.Price.Purchase : 0);
            screen.SetActive(false);
        }
        
        public void SetText()
        {
            var price = int.Parse(countLabel.text) * currentField.Property.Price.Building +
                        (landToggle.isOn ? currentField.Property.Price.Purchase : 0);
            confirmSale.GetComponent<TMP_Text>().text = $"确认出售<size=12>+({price})";
        }
    }
}