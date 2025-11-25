using System;
using Fictology.Registry;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace THNeonMirage.UI
{
    public class InGamePanelHandler : GameBehaviour, IPointerClickHandler
    {
        public GameObject inGamePanel;
        public GameObject cancel;
        public GameObject build;
        public GameObject mortgage;
        
        public GameClient client;
        public PlayerManager player;

        private FieldTile field;
        private RectTransform rect_transform;

        public TextMeshProUGUI toll;
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        public TextMeshProUGUI purchase;
        
        private void Start()
        {
            title = Registries.GetComponent<TextMeshProUGUI>(UIRegistry.TileName);
            toll = Registries.GetComponent<TextMeshProUGUI>(UIRegistry.TollText);
            purchase = Registries.GetComponent<TextMeshProUGUI>(UIRegistry.PurchaseText);
            description = Registries.GetComponent<TextMeshProUGUI>(UIRegistry.DescriptionText);
        }

        private void UpdateUI(object playerData, ValueEventArgs args)
        {
            var data = (PlayerData)playerData;
            field = Level.fields[(int)args.Value].GetComponent<FieldTile>();
            inGamePanel.SetActive(true);

            if (field.HasOwner()) purchase.GetComponent<Button>().SetEnabled(false);
            if (field.Owner.playerData.userName.Equals(data.userName))
            {
                purchase.enabled = false;
                cancel.SetActive(false);
                build.SetActive(true);
                mortgage.SetActive(true);

                build.GetComponent<TMP_Text>().text = $"建造房屋<size=12>(-{field.Property.Price.Building})";
                
            }
            
            if (field.CurrentTolls() <= 0)
            {
                purchase.enabled = false;
                cancel.SetActive(false);
            }
            else
            {
                purchase.enabled = false;
                cancel.SetActive(true);
            }
        }

        public void SetTile(Level level, int posIndex)
        {
            field = level.GetTile<FieldTile>(posIndex);
            TrySetTexts(level, posIndex);
        }
        
        public void TrySetTexts(Level level, int positionIndex)
        {
            if (!Registries.GetObject(UIRegistry.InGamePanel).activeInHierarchy) return;
            Debug.Log(positionIndex);
            field = level.GetTile<FieldTile>(positionIndex);
            title.text = field.Property.Name;
            description.text = field.description;
            toll.text = $"当前过路费：{field.CurrentTolls()}";

            purchase.text = $"购买<size=12>(-{field.Property.Price.Purchase})";
            
        }

        public void OnPlayerPurchase()
        {
            if (!field.canPurchased)return;
            player.SetBalance(player.playerData.balance - field.Property.Price.Purchase);
            // player.playerData.balance -= field.Property.Price.Purchase;
            // client.SetLabelWhenBalanceChanged(player.playerData, new ValueEventArgs(player.playerData.balance));
            field.Owner = player;
            player.playerData.AddField(field.index);
        }

        public void OnPlayerBuild()
        {
            player.playerData.balance -= field.Property.Price.Building;
            field.level++;
        }
        
        public void OnCanceled()
        {
            inGamePanel.SetActive(false);
        }

        public void OnHouseBuild()
        {
            field.level++;
        }

        public void OnFieldSold()
        {
            field.level = 0;
            field.Owner.SetBalance(field.level * field.Property.Price.Building + field.Property.Price.Purchase);
            field.Owner = null;
        }
        
// #if UNITY_EDITOR || UNITY_STANDALONE
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(rect_transform, eventData.position))
                inGamePanel.SetActive(false);
        }
// #endif
    }

    public enum DialogueType
    {
        Info,
        Warn,
        Error,
        Fatal
    }
}
