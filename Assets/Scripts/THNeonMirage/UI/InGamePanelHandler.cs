using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace THNeonMirage.UI
{
    public class InGamePanelHandler : GameBehaviour, IPointerClickHandler
    {
        public GameObject titleLabel;
        public GameObject descriptionLabel;
        public GameObject tollLabel;
        public GameObject inGamePanel;

        public GameObject purchase;
        public GameObject cancel;
        public GameObject build;
        public GameObject mortgage;
        
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
            player.PlayerData.OnPositionChanged += UpdateUI;
        }

        private void UpdateUI(object playerData, ValueEventArgs args)
        {
            var data = (PlayerData)playerData;
            field = GameMap.fields[(int)args.Value].GetComponent<FieldTile>();
            inGamePanel.SetActive(true);
            SetTexts((int)args.Value);

            if (field.HasOwner()) purchase.GetComponent<Button>().SetEnabled(false);
            if (field.Owner.UserName.Equals(data.UserName))
            {
                purchase.SetActive(false);
                cancel.SetActive(false);
                build.SetActive(true);
                mortgage.SetActive(true);

                build.GetComponent<TMP_Text>().text = $"建造房屋<size=12>(-{field.Property.Price.Building})";
                
            }
            
            if (field.CurrentTolls() <= 0)
            {
                purchase.SetActive(false);
                cancel.SetActive(false);
            }
            else
            {
                purchase.SetActive(true);
                cancel.SetActive(true);
            }
        }

        public void SetField(int posIndex)
        {
            field = GameMap.fields[posIndex].GetComponent<FieldTile>();
            SetTexts(posIndex);
        }
        
        public void SetTexts(int positionIndex)
        {
            field = GameMap.fields[positionIndex].GetComponent<FieldTile>();
            title.text = field.Property.Name;
            description.text = field.description;
            toll.text = $"当前过路费：{field.CurrentTolls()}";

            purchase.GetComponent<TMP_Text>().text = $"购买<size=12>(-{field.Property.Price.Purchase})";
        }
        
        public void SetTexts(object playerData, ValueEventArgs args) => SetTexts((int)args.Value);

        public void OnPlayerPurchase()
        {
            player.PlayerData.Balance -= field.Property.Price.Purchase;
            client.SetLabelWhenBalanceChanged(player.PlayerData, new ValueEventArgs(player.PlayerData.Balance));
            field.Owner = player.PlayerData;
            player.PlayerData.AddField(field.id);
        }

        public void OnPlayerBuild()
        {
            player.PlayerData.Balance -= field.Property.Price.Building;
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
            field.Owner.Balance += field.level * field.Property.Price.Building + field.Property.Price.Purchase;
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
