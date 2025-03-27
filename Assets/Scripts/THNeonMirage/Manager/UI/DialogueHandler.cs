using System;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace THNeonMirage.Manager.UI
{
    public class DialogueHandler : GameBehaviour, IPointerClickHandler
    {
        public GameObject titleLabel;
        public GameObject descriptionLabel;
        public GameObject tollLabel;
        public GameObject inGamePanel;

        public Action<int> OnMouseOver;
        private FieldTile field;
        private PlayerManager player;
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
            player = PlayerManager.Instance.GetComponent<PlayerManager>();
            player.PlayerData.OnPositionChanged += OnPlayerPositionChanged;

            SetTexts(player.PlayerData.Position);
            OnMouseOver += SetTexts;
        }

        private void OnPlayerPositionChanged(object sender, ValueEventArgs args) => SetTexts((int)args.Value);
        
        private void SetTexts(int positionIndex)
        {
            field = GameMap.fields[positionIndex].GetComponent<FieldTile>();
            title.text = field.Property.Name;
            description.text = field.description;
            toll.text = $"当前过路费：{field.CurrentTolls()}";
        }

        public void OnPlayerPurchase()
        {
            player.PlayerData.Balance -= field.Property.Price.Purchase;
            field.Owner = player.PlayerData;
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
