using System;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Manager.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = Unity.Mathematics.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class FieldTile : MonoBehaviour
    {
        public int id;
        public int level;
        public string description;
        public Color backGroundColor;
        
        public GameClient client;
        public FieldProperty Property;
        public PlayerData Owner;
        
        [DisplayOnly] public SpriteRenderer spriteRenderer;
        [DisplayOnly] public GameObject inGamePanel;
        [DisplayOnly] public GameObject hoverPanel;
        [DisplayOnly] public GameObject hoverText;

        protected PlayerManager Player;
        protected Random Random = new();
        private string tooltipString;
        
        private void Start()
        {
            hoverPanel = GameObject.Find("Canvas/HoverPanel");
            hoverText = GameObject.Find("Canvas/HoverPanel/HoverText");
            backGroundColor = new Color(1f, 1f, 1f, 0.5f);

            description = $"土地价格：{Property.Price.Purchase}\n\n" +
                          $"空地过路费：{Property.Price.Level0}\n" +
                          $"一幢房屋：{Property.Price.Level1}\n" +
                          $"两幢房屋：{Property.Price.Level2}\n" +
                          $"三幢房屋：{Property.Price.Level3}\n" +
                          $"每幢房屋建造费用：{Property.Price.Building}";
        }

        public virtual void Init()
        {
            Start();
            InitPlayer();
        }

        protected void InitPlayer() => Player = client.playerInstance.GetComponent<PlayerManager>();
        
        public int CurrentTolls()
        {
            return level switch
            {
                0 => Property.Price.Level0,
                1 => Property.Price.Level1,
                2 => Property.Price.Level2,
                3 => Property.Price.Level3,
                4 => -2000,
                5 => -1,
                _ => throw new IndexOutOfRangeException("等级不能超过3")
            };
        }

        public int GetPurchasePrice() => Property.Price.Purchase;
        public int GetBuildingPrice() => Property.Price.Building;

        // public abstract bool IsStartTile();
        // public abstract bool HasSpecialEffect();
        
        private void OnMouseOver()
        {
            tooltipString = $"编号：{id}\n名称：{Property.Name}";
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
            
            // hoverPanel.transform.position = Input.mousePosition + new Vector3(40f, 40f);
            // hoverText.GetComponent<TMP_Text>().text = tooltipString;
            // hoverPanel.SetActive(true);
            
            inGamePanel.GetComponent<InGamePanelHandler>().OnPlayerPositionChanged(Owner, new ValueEventArgs(id));
        }

        private void OnMouseExit()
        {
            spriteRenderer.color = backGroundColor;
            // hoverPanel.SetActive(false);
        }

        public bool HasOwner() => Owner == null;
        
        public virtual void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            if (!HasOwner())return;
            if (((PlayerData)playerData).UserName == null) return;
            if (Owner.UserName == ((PlayerData)playerData).UserName)return;
            ((PlayerData)playerData).Balance -= CurrentTolls();
        }

        public virtual void OnPlayerPassBy(object playerData, object prevPosition, object currentPosition)
        {
        
        }

        protected bool NextBool() => new System.Random().Next(1) == 0;
        protected int NextInt(int min, int max) => new System.Random().Next(min, max);

        public enum Type
        {
            DreamWorld,
            MagicForest,
            YoukaiMountain,
            BambooForest,
            AncientHell,
            Village,
            Nether,
            Fairies,
            Higan,
            Other
        }
    }
}
