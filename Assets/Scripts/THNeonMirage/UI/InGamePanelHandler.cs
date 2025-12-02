using System;
using System.Linq;
using Fictology.Registry;
using Photon.Pun;
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
        
        public ObsoleteGameClient client;
        public PlayerManager player;
        public GameObject playerObject;

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

        public void UpdateUI(Level level, int posIndex)
        {
            field = level.GetTile<FieldTile>(posIndex);
            inGamePanel.SetActive(true);
            
            if (field.HasOwner() && field.Owner.playerData.userName.Equals(player.playerData.userName))
            {
                Registries.GetObject(UIRegistry.PurchaseButton).SetActive(false);
                cancel.SetActive(false);
                build.SetActive(true);
                mortgage.SetActive(true);
                build.GetComponent<TMP_Text>().text = $"建造房屋<size=12>(-{field.Property.Price.Building})";
                
            }
            
            if (field.CurrentTolls() <= 0)
            {
                Registries.GetObject(UIRegistry.PurchaseButton).SetActive(false);
                cancel.SetActive(false);
            }
            else
            {
                Registries.GetObject(UIRegistry.PurchaseButton).SetActive(true);
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
            field = level.GetTile<FieldTile>(positionIndex);
            title.text = field.Property.Name;
            description.text = field.description;
            toll.text = $"当前过路费：{field.CurrentTolls()}";

            purchase.text = $"购买<size=12>(-{field.Property.Price.Purchase})";
            
        }

        public void OnPlayerPurchase()
        {
            if (!CanPurchase())return;
            player.SetBalance(player.playerData.balance - field.Property.Price.Purchase);
            // player.playerData.balance -= field.Property.Price.Purchase;
            // client.SetLabelWhenBalanceChanged(player.playerData, new ValueEventArgs(player.playerData.balance));
            field.Owner = player;
            field.canPurchase = false;
            player.playerData.AddField(field.index);
            var sprite = field.GetComponent<SpriteRenderer>();
            if (PhotonNetwork.IsConnectedAndReady)
            {
                field.SetOwnerOnLocal(playerObject.GetPhotonView().ViewID);
                player.SendFieldPropertyUpdate(field.index, field.level, field.Property.AsSerializable());
                player.SendPlayerDataUpdate(player.gameObject.GetPhotonView().ViewID, player.playerData);
            }
            else
            {
                var color = Registries.GetPrefabInstances(LevelRegistry.Player).First(p =>
                        p.GetComponent<PlayerManager>().playerData.roundIndex == player.playerData.roundIndex)
                    .GetComponent<SpriteRenderer>().color;

                sprite.color = color;
            }
        }

        public void OnPlayerBuild()
        {
            player.playerData.balance -= field.Property.Price.Building;
            field.level++;
            player.SendFieldPropertyUpdate(field.index, field.level, field.Property.AsSerializable());
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

        private bool CanPurchase()
        {
            var fieldMatches = player.playerData.position == field.index;
            if (!PhotonNetwork.IsConnectedAndReady) return field.canPurchase && fieldMatches;
            return field.canPurchase && fieldMatches;
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
