using System;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace THNeonMirage.Manager.UI
{
    public class InGamePanelHandler : GameBehaviour, IPointerClickHandler
    {
        public GameObject titleLabel;
        public GameObject descriptionLabel;
        public GameObject tollLabel;
        public GameObject inGamePanel;
        
        public GameClient client;
        public PlayerManager player;

        private FieldTile field;
        private RectTransform rect_transform;

        private TMP_Text toll;
        private TMP_Text title;
        private TMP_Text description;
        
        private void Start()
        {
            rect_transform = GetComponent<RectTransform>();
            title = titleLabel.GetComponent<TMP_Text>();
            toll = tollLabel.GetComponent<TMP_Text>();
            description = descriptionLabel.GetComponent<TMP_Text>();

            GameMap = mapObject.GetComponent<GameMap>();
            SetTexts(player.PlayerData.Position);
        }

        public void SetInfoWhenStop(object sender, ValueEventArgs args)
        {
            field = GameMap.Fields[(int)args.Value].GetComponent<FieldTile>();
            title.text = field.Property.Name;
            description.text = field.description;
            toll.text = $"当前过路费：{field.CurrentTolls()}";
        }

        public void SetTexts(int positionIndex)
        {
            field = GameMap.Fields[positionIndex].GetComponent<FieldTile>();
            title.text = field.Property.Name;
            description.text = field.description;
            toll.text = $"当前过路费：{field.CurrentTolls()}";
        }
        
        public void SetTexts(object playerData, ValueEventArgs args) => SetTexts((int)args.Value);

        public void OnPlayerPurchase()
        {
            player.PlayerData.Balance -= field.Property.Price.Purchase;
            client.SetLabelWhenBalanceChanged(player.PlayerData, new ValueEventArgs(player.PlayerData.Balance));
            field.Owner = player.PlayerData;
        }

        public void OnPlayerBuild()
        {
            player.PlayerData.Balance -= field.Property.Price.Building;
            field.level++;
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
